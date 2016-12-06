using DecaTec.WebDav.WebDavArtifacts;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Provides a class for sending WebDAV requests and receiving WebDAV responses from a resource identified by URI.
    /// </summary>
    /// <remarks>
    /// <para>WebDavClient uses a <see cref="Windows.Web.Http.HttpClient"/> to provide WebDAV specific methods.</para>
    /// <para>It implements the <see href="http://tools.ietf.org/html/rfc4918">RFC 4918</see> specification and can be used to communicate directly with a WebDAV server.</para>
    /// <para>For most use cases regarding WebDAV communication, the <see cref="DecaTec.WebDav.WebDavSession"/> is sufficient because it hides most of the WebDAV specific communication 
    /// and provides an easy access to WebDAV servers.</para>
    /// <example>To send a propfind request you can use following code:
    /// <code>
    /// // You have to add a reference to DecaTec.WebDav.Uwp.dll.
    /// //
    /// // The base URL of the WebDAV server.
    /// var webDavServerUrl = "http://www.myserver.com/webdav/";
    ///
    /// // Specify the user credentials and pass it to a HttpBaseProtocolFilter.
    /// var credentials = new PasswordCredential(webDavServerUrl, "MyUserName", "MyPassword");
    /// var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
    /// httpBaseProtocolFilter.ServerCredential = credentials;
    ///
    /// // Use the HttpBaseProtocolFilter (with credentials) to create a new WebDavClient.
    ///var webDavClient = new WebDavClient(httpBaseProtocolFilter);
    ///
    /// // Create a PropFind object with represents a so called 'allprop' request.
    /// var pf = PropFind.CreatePropFindAllProp();
    /// var response = await webDavClient.PropFindAsync("http://www.myserver.com/webdav/MyFolder/", WebDavDepthHeaderValue.Infinity, pf);
    ///
    /// // You could also use an XML string directly for use with the WebDavClient.
    /// //var xmlString = "&lt;?xml version=\&quot;1.0\&quot; encoding=\&quot;utf-8\&quot;?&gt;&lt;D:propfind xmlns:D=\&quot;DAV:\&quot;&gt;&lt;D:allprop /&gt;&lt;/D:propfind&gt;";
    /// //var response = await webDavClient.PropFindAsync(@"http://www.myserver.com/webdav/MyFolder/", WebDavDepthHeaderValue.Infinity, xmlString);
    ///
    /// // Use the WebDavResponseContentParser to parse the response message and get a MultiStatus instance (this is also an async method).
    /// var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);
    ///
    /// // Now you can use the MultiStatus object to get access to the items properties.
    /// foreach (var responseItem in multistatus.Response)
    /// {
    ///     // Handle propfind multistatus response, e.g responseItem.Href is the URL of an item (folder or file).
    /// }
    /// 
    /// // Dispose the WebDavClient when it is not longer needed.
    /// webDavClient.Dispose();
    /// </code>
    /// <para></para>
    /// See the following code which demonstrates locking using a WebDavClient:
    /// <code>
    /// // You have to add references to DecaTec.WebDav.Uwp.dll.
    /// //
    /// // The base URL of the WebDAV server.
    /// var webDavServerUrl = "http://www.myserver.com/webdav/";
    ///
    /// // Specify the user credentials and pass it to a HttpBaseProtocolFilter.
    /// var credentials = new PasswordCredential(webDavServerUrl, "MyUserName", "MyPassword");
    /// var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
    /// httpBaseProtocolFilter.ServerCredential = credentials;
    ///
    /// // Use the HttpBaseProtocolFilter (with credentials) to create a new WebDavClient.
    /// var webDavClient = new WebDavClient(httpBaseProtocolFilter);
    ///
    /// // Create a LockInfo object with is needed for locking.
    /// // We are using the class LockInfo (from DecaTec.WebDav.WebDavArtifacts) to avoid building a XML string directly.
    /// var lockInfo = new LockInfo();
    /// lockInfo.LockScope = LockScope.CreateExclusiveLockScope();
    /// lockInfo.LockType = LockType.CreateWriteLockType();
    /// lockInfo.Owner = new OwnerHref("test@test.com");
    ///
    /// // Lock the desired folder by specifying a WebDavTimeOutHeaderValue (in this example, the timeout should be infinite), a value for depth and the LockInfo.
    /// var lockResult = await webDavClient.LockAsync(@"http://www.myserver.com/webdav/MyFolder/", WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(), WebDavDepthHeaderValue.Infinity, lockInfo);
    ///
    /// // On successful locking, a lock token will be returned by the WebDAV server.
    /// // We have to save this lock token in order to use it on operations which affect a locked folder.
    /// LockToken lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(lockResult);
    ///
    /// // Now create a new folder in the locked location.
    /// // Notice that the LockToken has to be specified as a locked folder is affected by this operation.
    /// // If the LockToken would not be specified, the operation will fail!
    /// await webDavClient.MkcolAsync(@"http://www.myserver.com/webdav/myfolder/NewFolder/", lockToken);
    ///
    /// // Delete the folder again.
    /// await webDavClient.DeleteAsync(@"http://www.myserver.com/webdav/myfolder/NewFolder/", lockToken);
    ///
    /// // Unlock the locked folder.
    /// // Notice that the URL is the same as used with the lock method (see above).
    /// await webDavClient.UnlockAsync(@"http://www.myserver.com/webdav/MyFolder/", lockToken);
    /// 
    /// // Dispose the WebDavClient when it is not longer needed.
    /// webDavClient.Dispose();
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="DecaTec.WebDav.WebDavSession"/>
    ///
    public class WebDavClient : IDisposable
    {
        private HttpClient httpClient;

        private static readonly XmlSerializer MultistatusSerializer = new XmlSerializer(typeof(Multistatus));
        private static readonly XmlSerializer PropFindSerializer = new XmlSerializer(typeof(PropFind));
        private static readonly XmlSerializer PropertyUpdateSerializer = new XmlSerializer(typeof(PropertyUpdate));
        private static readonly XmlSerializer LockInfoSerializer = new XmlSerializer(typeof(LockInfo));

        private const string MediaTypeXml = "application/xml";

        #region Constructor

        /// <summary>
        /// Initializes a new instance of WebDavClient.
        /// </summary>
        public WebDavClient()
        {
            this.httpClient = new HttpClient();
        }

        /// <summary>
        ///  Initializes a new instance of WebDavClient with a specific <see cref="IHttpFilter"/> for handling HTTP response messages.
        /// </summary>
        /// <param name="httpFilter">The <see cref="IHttpFilter"/> to use for handling response messages.</param>
        public WebDavClient(IHttpFilter httpFilter)
        {
            this.httpClient = new HttpClient(httpFilter);
        }

        #endregion Constructor

        #region Copy

        /// <summary>
        /// Copies a resource from the source URL to the destination URL (Depth = 'infinity', Overwrite = false).
        /// </summary>
        /// <param name="sourceUrl">The source URL.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> CopyAsync(string sourceUrl, string destinationUrl)
        {
            return await CopyAsync(new Uri(sourceUrl), new Uri(destinationUrl), false, WebDavDepthHeaderValue.Infinity, null);
        }

        /// <summary>
        /// Copies a resource from the source <see cref="Uri"/> to the destination <see cref="Uri"/> (Depth = 'infinity', Overwrite = false).
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destinationUri">The destination <see cref="Uri"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> CopyAsync(Uri sourceUri, Uri destinationUri)
        {
            return await CopyAsync(sourceUri, destinationUri, false, WebDavDepthHeaderValue.Infinity, null);
        }

        /// <summary>
        /// Copies a resource from the source URL to the destination URL (Overwrite = false).
        /// </summary>
        /// <param name="sourceUrl">The source URL.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> CopyAsync(string sourceUrl, string destinationUrl, bool overwrite)
        {
            return await CopyAsync(new Uri(sourceUrl), new Uri(destinationUrl), overwrite, WebDavDepthHeaderValue.Infinity, null);
        }

        /// <summary>
        /// Copies a resource from the source <see cref="Uri"/> to the destination <see cref="Uri"/> (Overwrite = false).
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destinationUri">The destination <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> CopyAsync(Uri sourceUri, Uri destinationUri, bool overwrite)
        {
            return await CopyAsync(sourceUri, destinationUri, overwrite, WebDavDepthHeaderValue.Infinity, null);
        }

        /// <summary>
        /// Copies a resource from the source URL to the destination URL.
        /// </summary>
        /// <param name="sourceUrl">The source URL.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> of the copy command. On collections, depth must be '0' or 'infinity'.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> CopyAsync(string sourceUrl, string destinationUrl, bool overwrite, WebDavDepthHeaderValue depth)
        {
            return await CopyAsync(new Uri(sourceUrl), new Uri(destinationUrl), overwrite, depth, null);
        }

        /// <summary>
        /// Copies a resource from the source <see cref="Uri"/> to the destination <see cref="Uri"/>.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destinationUri">The destination <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> of the copy command. On collections, depth must be '0' or 'infinity'.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> CopyAsync(Uri sourceUri, Uri destinationUri, bool overwrite, WebDavDepthHeaderValue depth)
        {
            return await CopyAsync(sourceUri, destinationUri, overwrite, depth, null);
        }

        /// <summary>
        /// Copies a resource from the source URL to the destination URL.
        /// </summary>
        /// <param name="sourceUrl">The source URL.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> of the copy command. On collections, depth must be '0' or 'infinity'.</param>
        /// <param name="lockTokenDestination">The <see cref="LockToken"/> of the destination or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> CopyAsync(string sourceUrl, string destinationUrl, bool overwrite, WebDavDepthHeaderValue depth, LockToken lockTokenDestination)
        {
            return await CopyAsync(new Uri(sourceUrl), new Uri(destinationUrl), overwrite, depth, lockTokenDestination);
        }

        /// <summary>
        /// Copies a resource from the source <see cref="Uri"/> to the destination <see cref="Uri"/>.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destinationUri">The destination <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> of the copy command. On collections, depth must be '0' or 'infinity'.</param>
        /// <param name="lockTokenDestination">The <see cref="LockToken"/> of the destination or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> CopyAsync(Uri sourceUri, Uri destinationUri, bool overwrite, WebDavDepthHeaderValue depth, LockToken lockTokenDestination)
        {
            var requestMethod = new HttpRequestMessage(new HttpMethod(WebDavMethod.Copy), sourceUri);
            // Destination header must be present on copy commands.
            requestMethod.Headers.Add(WebDavRequestHeader.Destination, destinationUri.ToString());

            if (depth != null)
            {
                // On collections: Depth must be '0' or 'infinity'.
                requestMethod.Headers.Add(WebDavRequestHeader.Depth, depth.ToString());
            }

            if (overwrite)
                requestMethod.Headers.Add(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.Overwrite);
            else
                requestMethod.Headers.Add(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.NoOverwrite);

            if (lockTokenDestination != null)
                requestMethod.Headers.Add(WebDavRequestHeader.If, lockTokenDestination.ToString(LockTokenFormat.IfHeader));

            var taskHttpResponseMessage = await this.SendAsync(requestMethod);
            return taskHttpResponseMessage;
        }

        #endregion Copy

        #region Delete

        /// <summary>
        /// Sends a DELETE request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> DeleteAsync(string requestUrl)
        {
            return await DeleteAsync(new Uri(requestUrl), null);
        }

        /// <summary>
        /// Sends a DELETE request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
        {
            return await DeleteAsync(requestUri, null);
        }

        /// <summary>
        /// Sends a DELETE request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> DeleteAsync(string requestUrl, LockToken lockToken)
        {
            return await DeleteAsync(new Uri(requestUrl), lockToken);
        }

        /// <summary>
        /// Sends a DELETE request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> DeleteAsync(Uri requestUri, LockToken lockToken)
        {
            // On collections: Clients must not use any other value for the Depth header but 'infinity'.
            // A DELETE command without depth header will be treated by the server as if Depth = 'infinity' was used.
            // Thus, no Depth header is explicitly specified.
            var requestMethod = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            if (lockToken != null)
                requestMethod.Headers.Add(WebDavRequestHeader.If, lockToken.ToString(LockTokenFormat.IfHeader));

            var httpResponseMessage = await this.httpClient.SendRequestAsync(requestMethod, HttpCompletionOption.ResponseContentRead);
            return httpResponseMessage;
        }

        #endregion Delete

        #region Get

        /// <summary>
        /// Send a GET request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> GetAsync(string requestUrl)
        {
            return await GetAsync(new Uri(requestUrl), HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a GET request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            return await GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a GET request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> GetAsync(string requestUrl, HttpCompletionOption completionOption)
        {
            return await GetAsync(new Uri(requestUrl), completionOption);
        }

        /// <summary>
        /// Send a GET request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="completionOption">An<see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
        {
            var httpResponseMessage = await this.httpClient.GetAsync(requestUri, completionOption);
            return httpResponseMessage;
        }

        #endregion Get

        #region Head

        /// <summary>
        /// Send a HEAD request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> HeadAsync(string requestUrl)
        {
            return await HeadAsync(new Uri(requestUrl), HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a HEAD request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> HeadAsync(Uri requestUri)
        {
            return await HeadAsync(requestUri, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a HEAD request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> HeadAsync(string requestUrl, HttpCompletionOption completionOption)
        {
            return await HeadAsync(new Uri(requestUrl), completionOption);
        }

        /// <summary>
        /// Send a HEAD request to the specified <see cref="Uri"/>
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The<see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> HeadAsync(Uri requestUri, HttpCompletionOption completionOption)
        {
            var requestMethod = new HttpRequestMessage(HttpMethod.Head, requestUri);
            var httpResponseMessage = await this.httpClient.SendRequestAsync(requestMethod, completionOption);
            return httpResponseMessage;
        }

        #endregion Head

        #region Lock

        #region Set lock

        /// <summary>
        /// Send a LOCK request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="lockInfo">The <see cref="LockInfo"/> object specifying the lock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> LockAsync(string requestUrl, WebDavTimeoutHeaderValue timeout, WebDavDepthHeaderValue depth, LockInfo lockInfo)
        {
            return await LockAsync(requestUrl, timeout, depth, lockInfo, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a LOCK request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="lockInfoXmlString">The XML string specifying which item should be locked.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> LockAsync(string requestUrl, WebDavTimeoutHeaderValue timeout, WebDavDepthHeaderValue depth, string lockInfoXmlString)
        {
            return await LockAsync(requestUrl, timeout, depth, lockInfoXmlString, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a LOCK request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="lockInfo">The <see cref="LockInfo"/> object specifying the lock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> LockAsync(Uri requestUri, WebDavTimeoutHeaderValue timeout, WebDavDepthHeaderValue depth, LockInfo lockInfo)
        {
            return await LockAsync(requestUri, timeout, depth, lockInfo, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a LOCK request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="lockinfoXmlString">The XML string specifying which item should be locked.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> LockAsync(Uri requestUri, WebDavTimeoutHeaderValue timeout, WebDavDepthHeaderValue depth, string lockinfoXmlString)
        {
            return await LockAsync(requestUri, timeout, depth, lockinfoXmlString, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a LOCK request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="lockInfoXmlString">The XML string specifying which item should be locked.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> LockAsync(string requestUrl, WebDavTimeoutHeaderValue timeout, WebDavDepthHeaderValue depth, string lockInfoXmlString, HttpCompletionOption completionOption)
        {
            return await LockAsync(requestUrl, timeout, depth, lockInfoXmlString, completionOption);
        }

        /// <summary>
        /// Send a LOCK request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="lockInfo">The <see cref="LockInfo"/> object specifying the lock.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> LockAsync(string requestUrl, WebDavTimeoutHeaderValue timeout, WebDavDepthHeaderValue depth, LockInfo lockInfo, HttpCompletionOption completionOption)
        {
            return await LockAsync(new Uri(requestUrl), timeout, depth, lockInfo, completionOption);
        }

        /// <summary>
        /// Send a LOCK request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="lockInfo">The <see cref="LockInfo"/> object specifying the lock.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> LockAsync(Uri requestUri, WebDavTimeoutHeaderValue timeout, WebDavDepthHeaderValue depth, LockInfo lockInfo, HttpCompletionOption completionOption)
        {
            string requestContentString = string.Empty;

            if (lockInfo != null)
                requestContentString = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(LockInfoSerializer, lockInfo);

            return await LockAsync(requestUri, timeout, depth, requestContentString, completionOption);
        }

        /// <summary>
        /// Send a LOCK request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="lockinfoXmlString">The XML string specifying which item should be locked.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> LockAsync(Uri requestUri, WebDavTimeoutHeaderValue timeout, WebDavDepthHeaderValue depth, string lockinfoXmlString, HttpCompletionOption completionOption)
        {
            if (depth == WebDavDepthHeaderValue.One)
                throw new WebDavException("Values other than '0' or 'infinity' must not be used on a LOCK command.");

            var requestMethod = new HttpRequestMessage(new HttpMethod(WebDavMethod.Lock), requestUri);

            if (depth != null)
                requestMethod.Headers.Add(WebDavRequestHeader.Depth, depth.ToString());

            if (timeout != null)
                requestMethod.Headers.Add(WebDavRequestHeader.Timeout, timeout.ToString());

            if (!String.IsNullOrEmpty(lockinfoXmlString))
            {
                var httpContent = new HttpStringContent(lockinfoXmlString, Windows.Storage.Streams.UnicodeEncoding.Utf8, MediaTypeXml);
                requestMethod.Content = httpContent;
            }

            var httpResponseMessage = await this.httpClient.SendRequestAsync(requestMethod, completionOption);
            return httpResponseMessage;
        }

        #endregion Set lock

        #region Refresh lock

        /// <summary>
        /// Send a LOCK request to the specified URI in order to refresh an already existing lock.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> of the lock which should be refreshed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> RefreshLockAsync(string requestUrl, WebDavTimeoutHeaderValue timeout, LockToken lockToken)
        {
            return await RefreshLockAsync(new Uri(requestUrl), timeout, lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a LOCK request to the specified <see cref="Uri"/> in order to refresh an already existing lock.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> of the lock which should be refreshed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> RefreshLockAsync(Uri requestUri, WebDavTimeoutHeaderValue timeout, LockToken lockToken)
        {
            return await RefreshLockAsync(requestUri, timeout, lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a LOCK request to the specified <see cref="Uri"/> in order to refresh an already existing lock.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> of the lock which should be refreshed.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> RefreshLockAsync(string requestUrl, WebDavTimeoutHeaderValue timeout, LockToken lockToken, HttpCompletionOption completionOption)
        {
            return await RefreshLockAsync(new Uri(requestUrl), timeout, lockToken, completionOption);
        }

        /// <summary>
        /// Send a LOCK request to the specified <see cref="Uri"/> in order to refresh an already existing lock.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="timeout">The <see cref="WebDavTimeoutHeaderValue"/> to use for the lock. The server might ignore this timeout.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> of the lock which should be refreshed.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> RefreshLockAsync(Uri requestUri, WebDavTimeoutHeaderValue timeout, LockToken lockToken, HttpCompletionOption completionOption)
        {
            if (lockToken == null)
                throw new WebDavException("No lock token specified. A lock token is required to refresh a lock.");

            var requestMethod = new HttpRequestMessage(new HttpMethod(WebDavMethod.Lock), requestUri);

            if (timeout != null)
                requestMethod.Headers.Add(WebDavRequestHeader.Timeout, timeout.ToString());

            requestMethod.Headers.Add(WebDavRequestHeader.If, lockToken.ToString(LockTokenFormat.IfHeader));

            var httpResponseMessage = await this.httpClient.SendRequestAsync(requestMethod, completionOption);
            return httpResponseMessage;
        }

        #endregion Refresh lock

        #endregion Lock

        #region Mkcol

        /// <summary>
        /// Creates a collection at the URL specified.
        /// </summary>
        /// <param name="requestUrl">The URL of the collection to create.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MkcolAsync(string requestUrl)
        {
            return await MkcolAsync(new Uri(requestUrl), null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Creates a collection at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> of the collection to create.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MkcolAsync(Uri requestUri)
        {
            return await MkcolAsync(requestUri, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Creates a collection at the URL specified.
        /// </summary>
        /// <param name="requestUrl">The URL of the collection to create.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MkcolAsync(string requestUrl, LockToken lockToken)
        {
            return await MkcolAsync(new Uri(requestUrl), lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Creates a collection at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> of the collection to create.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MkcolAsync(Uri requestUri, LockToken lockToken)
        {
            return await MkcolAsync(requestUri, lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Creates a collection at the URL specified.
        /// </summary>
        /// <param name="requestUrl">The URL of the collection to create.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MkcolAsync(string requestUrl, HttpCompletionOption completionOption)
        {
            return await MkcolAsync(new Uri(requestUrl), null, completionOption);
        }

        /// <summary>
        /// Creates a collection at the URL specified.
        /// </summary>
        /// <param name="requestUrl">The URL of the collection to create.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MkcolAsync(string requestUrl, LockToken lockToken, HttpCompletionOption completionOption)
        {
            return await MkcolAsync(new Uri(requestUrl), lockToken, completionOption);
        }

        /// <summary>
        ///  Creates a collection at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> of the collection to create.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MkcolAsync(Uri requestUri, HttpCompletionOption completionOption)
        {
            return await MkcolAsync(requestUri, null, completionOption);
        }

        /// <summary>
        ///  Creates a collection at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> of the collection to create.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MkcolAsync(Uri requestUri, LockToken lockToken, HttpCompletionOption completionOption)
        {
            var requestMethod = new HttpRequestMessage(new HttpMethod(WebDavMethod.Mkcol), requestUri);

            if (lockToken != null)
                requestMethod.Headers.Add(WebDavRequestHeader.If, lockToken.ToString(LockTokenFormat.IfHeader));

            var httpResponseMessage = await this.httpClient.SendRequestAsync(requestMethod, completionOption);
            return httpResponseMessage;
        }

        #endregion Mkcol

        #region Move

        /// <summary>
        /// Moves a resource to another URL (Overwrite = false).
        /// </summary>
        /// <param name="sourceUrl">The URL of the resource which should be moved.</param>
        /// <param name="destinationUrl">The target URL.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(string sourceUrl, string destinationUrl)
        {
            return await MoveAsync(new Uri(sourceUrl), new Uri(destinationUrl), false, null, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Moves a resource to another <see cref="Uri"/> (Overwrite = false).
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the resource which should be moved.</param>
        /// <param name="destinationUri">The target <see cref="Uri"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(Uri sourceUri, Uri destinationUri)
        {
            return await MoveAsync(sourceUri, destinationUri, false, null, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Moves a resource to another URL (Overwrite = false).
        /// </summary>
        /// <param name="sourceUrl">The URL of the resource which should be moved.</param>
        /// <param name="destinationUrl">The target URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(string sourceUrl, string destinationUrl, bool overwrite)
        {
            return await MoveAsync(new Uri(sourceUrl), new Uri(destinationUrl), overwrite, null, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Moves a resource to another <see cref="Uri"/> (Overwrite = false).
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the resource which should be moved.</param>
        /// <param name="destinationUri">The target <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(Uri sourceUri, Uri destinationUri, bool overwrite)
        {
            return await MoveAsync(sourceUri, destinationUri, overwrite, null, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Moves a resource to another URL.
        /// </summary>
        /// <param name="sourceUrl">The URL of the resource which should be moved.</param>
        /// <param name="destinationUrl">The target URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="lockTokenSource">The <see cref="LockToken"/> of the source. Specify null if the source is not locked.</param>
        /// <param name="lockTokenDestination">The <see cref="LockToken"/> of the destination. Specify null if the destination is not locked.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(string sourceUrl, string destinationUrl, bool overwrite, LockToken lockTokenSource, LockToken lockTokenDestination)
        {
            return await MoveAsync(new Uri(sourceUrl), new Uri(destinationUrl), overwrite, lockTokenSource, lockTokenDestination, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Moves a resource to another <see cref="Uri"/>.
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the resource which should be moved.</param>
        /// <param name="destinationUri">The target <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="lockTokenSource">The <see cref="LockToken"/> of the source. Specify null if the source is not locked.</param>
        /// <param name="lockTokenDestination">The <see cref="LockToken"/> of the destination. Specify null if the destination is not locked.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(Uri sourceUri, Uri destinationUri, bool overwrite, LockToken lockTokenSource, LockToken lockTokenDestination)
        {
            return await MoveAsync(sourceUri, destinationUri, overwrite, lockTokenSource, lockTokenDestination, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Moves a resource to another URL.
        /// </summary>
        /// <param name="sourceUrl">The URL of the resource which should be moved.</param>
        /// <param name="destinationUrl">The target URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(string sourceUrl, string destinationUrl, bool overwrite, HttpCompletionOption completionOption)
        {
            return await MoveAsync(new Uri(sourceUrl), new Uri(destinationUrl), overwrite, null, null, completionOption);
        }

        /// <summary>
        /// Moves a resource to another URL.
        /// </summary>
        /// <param name="sourceUrl">The URL of the resource which should be moved.</param>
        /// <param name="destinationUrl">The target URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="lockTokenSource">The <see cref="LockToken"/> of the source. Specify null if the source is not locked.</param>
        /// <param name="lockTokenDestination">The <see cref="LockToken"/> of the destination. Specify null if the destination is not locked.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(string sourceUrl, string destinationUrl, bool overwrite, LockToken lockTokenSource, LockToken lockTokenDestination, HttpCompletionOption completionOption)
        {
            return await MoveAsync(new Uri(sourceUrl), new Uri(destinationUrl), overwrite, lockTokenSource, lockTokenDestination, completionOption);
        }

        /// <summary>
        /// Moves a resource to another <see cref="Uri"/>.
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the resource which should be moved.</param>
        /// <param name="destinationUri">The target <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(Uri sourceUri, Uri destinationUri, bool overwrite, HttpCompletionOption completionOption)
        {
            return await MoveAsync(sourceUri, destinationUri, overwrite, null, null, completionOption);
        }

        /// <summary>
        /// Moves a resource to another <see cref="Uri"/>.
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the resource which should be moved.</param>
        /// <param name="destinationUri">The target <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <param name="lockTokenSource">The <see cref="LockToken"/> of the source. Specify null if the source is not locked.</param>
        /// <param name="lockTokenDestination">The <see cref="LockToken"/> of the destination. Specify null if the destination is not locked.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> MoveAsync(Uri sourceUri, Uri destinationUri, bool overwrite, LockToken lockTokenSource, LockToken lockTokenDestination, HttpCompletionOption completionOption)
        {
            var requestMethod = new HttpRequestMessage(new HttpMethod(WebDavMethod.Move), sourceUri);
            // Destination header must be present on MOVE commands.
            requestMethod.Headers.Add(WebDavRequestHeader.Destination, destinationUri.ToString());

            // On Collections: Clients must not submit other Depth header than 'infinity', any other depth header does not make sense on non-collections.
            // So set the depth header always to 'infinity'.
            requestMethod.Headers.Add(WebDavRequestHeader.Depth, WebDavDepthHeaderValue.Infinity.ToString());

            if (lockTokenSource != null || lockTokenSource != null)
            {
                var sb = new StringBuilder();

                if (lockTokenSource != null)
                    sb.Append(lockTokenSource.ToString(LockTokenFormat.IfHeader));

                if (lockTokenDestination != null)
                    sb.Append(lockTokenDestination.ToString(LockTokenFormat.IfHeader));

                if (lockTokenSource != null)
                    requestMethod.Headers.Add(WebDavRequestHeader.If, sb.ToString());
            }

            if (overwrite)
                requestMethod.Headers.Add(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.Overwrite);
            else
                requestMethod.Headers.Add(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.NoOverwrite);

            var httpResponseMessage = await this.SendAsync(requestMethod, completionOption);
            return httpResponseMessage;
        }

        #endregion Move

        #region Post

        /// <summary>
        /// Send a POST request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> to send to the server.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PostAsync(string requestUrl, IHttpContent content)
        {
            return await PostAsync(new Uri(requestUrl), content, null);
        }

        /// <summary>
        /// Send a POST request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> to send to the server.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PostAsync(Uri requestUri, IHttpContent content)
        {
            return await PostAsync(requestUri, content, null);
        }

        /// <summary>
        /// Send a POST request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PostAsync(string requestUrl, IHttpContent content, LockToken lockToken)
        {
            return await PostAsync(new Uri(requestUrl), content, lockToken);
        }

        /// <summary>
        /// Send a POST request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PostAsync(Uri requestUri, IHttpContent content, LockToken lockToken)
        {
            var requestMethod = new HttpRequestMessage(HttpMethod.Post, requestUri);
            requestMethod.Content = content;

            if (lockToken != null)
                requestMethod.Headers.Add(WebDavRequestHeader.If, lockToken.ToString(LockTokenFormat.IfHeader));

            var httpResponseMessage = await this.SendAsync(requestMethod, HttpCompletionOption.ResponseContentRead);
            return httpResponseMessage;
        }

        #endregion Post

        #region Propfind

        /// <summary>
        /// Send a PROPFIND request to the specified URL (Depth header = '1' and 'allprop').
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>       
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(string requestUrl)
        {
            return await PropFindAsync(new Uri(requestUrl), WebDavDepthHeaderValue.One, string.Empty, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified <see cref="Uri"/> (Depth header = '1' and 'allprop').
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>       
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(Uri requestUri)
        {
            return await PropFindAsync(requestUri, WebDavDepthHeaderValue.One, string.Empty, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified URL ('allprop').
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>       
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(string requestUrl, WebDavDepthHeaderValue depth)
        {
            return await PropFindAsync(new Uri(requestUrl), depth, string.Empty, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified <see cref="Uri"/> ('allprop')
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>       
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(Uri requestUri, WebDavDepthHeaderValue depth)
        {
            return await PropFindAsync(requestUri, depth, string.Empty, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="propfind">The <see cref="PropFind"/> object specifying which properties should be searched for. If null is specified here, a so called 'allprop' request will be sent.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(string requestUrl, WebDavDepthHeaderValue depth, PropFind propfind, HttpCompletionOption completionOption)
        {
            return await PropFindAsync(new Uri(requestUrl), depth, propfind, completionOption);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="propfindXmlString">The XML string specifying which items should be returned by the response. If an empty string is specified here, a so called 'allprop' request will be sent.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(string requestUrl, WebDavDepthHeaderValue depth, string propfindXmlString)
        {
            return await PropFindAsync(new Uri(requestUrl), depth, propfindXmlString, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="propfindXmlString">The XML string specifying which items should be returned by the response. If an empty string is specified here, a so called 'allprop' request will be sent.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(Uri requestUri, WebDavDepthHeaderValue depth, string propfindXmlString)
        {
            return await PropFindAsync(requestUri, depth, propfindXmlString, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="propfind">The <see cref="PropFind"/> object specifying which properties should be searched for. If null is specified here, a so called 'allprop' request will be sent.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(string requestUrl, WebDavDepthHeaderValue depth, PropFind propfind)
        {
            return await PropFindAsync(new Uri(requestUrl), depth, propfind, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="propfind">The <see cref="PropFind"/> object specifying which properties should be searched for. If null is specified here, a so called 'allprop' request will be sent.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(Uri requestUri, WebDavDepthHeaderValue depth, PropFind propfind)
        {
            return await PropFindAsync(requestUri, depth, propfind, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="propfind">The <see cref="PropFind"/> object specifying which properties should be searched for. If null is specified here, a so called 'allprop' request will be sent.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(Uri requestUri, WebDavDepthHeaderValue depth, PropFind propfind, HttpCompletionOption completionOption)
        {
            string requestContentString = string.Empty;

            if (propfind != null)
                requestContentString = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(PropFindSerializer, propfind);

            return await PropFindAsync(requestUri, depth, requestContentString, completionOption);
        }

        /// <summary>
        /// Send a PROPFIND request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="depth">The <see cref="WebDavDepthHeaderValue"/> value to use for the operation.</param>
        /// <param name="propfindXmlString">The XML string specifying which items should be returned by the response. If an empty string is specified here, a so called 'allprop' request will be sent.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropFindAsync(Uri requestUri, WebDavDepthHeaderValue depth, string propfindXmlString, HttpCompletionOption completionOption)
        {
            // Client must submit a depth header.
            if (depth == null)
                throw new WebDavException("A Depth header must be present on a PROPFIND command.");

            var requestMethod = new HttpRequestMessage(new HttpMethod(WebDavMethod.PropFind), requestUri);

            // If Depth = 'infinity', the server could response with 403 (Forbidden) when Depth = 'infinity' is not supported.
            requestMethod.Headers.Add(WebDavRequestHeader.Depth, depth.ToString());

            if (!String.IsNullOrEmpty(propfindXmlString))
            {
                var httpContent = new HttpStringContent(propfindXmlString, Windows.Storage.Streams.UnicodeEncoding.Utf8, MediaTypeXml);
                requestMethod.Content = httpContent;
            }

            var httpResponseMessage = await this.SendAsync(requestMethod, completionOption);
            return httpResponseMessage;
        }

        #endregion Propfind

        #region Proppatch

        /// <summary>
        /// Sends a PROPPATCH request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="propPatchXmlString">The XML string specifying which changes to which properties should be sent with this request.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(string requestUrl, string propPatchXmlString)
        {
            return await PropPatchAsync(new Uri(requestUrl), propPatchXmlString, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="propertyUpdate">A <see cref="PropertyUpdate"/> which specifies the properties and values which should be updated.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(string requestUrl, PropertyUpdate propertyUpdate)
        {
            return await PropPatchAsync(new Uri(requestUrl), propertyUpdate, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="propPatchXmlString">The XML string specifying which changes to which properties should be sent with this request.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(Uri requestUri, string propPatchXmlString)
        {
            return await PropPatchAsync(requestUri, propPatchXmlString, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="propertyUpdate">A <see cref="PropertyUpdate"/> which specifies the properties and values which should be updated.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(Uri requestUri, PropertyUpdate propertyUpdate)
        {
            return await PropPatchAsync(requestUri, propertyUpdate, null, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="propPatchXmlString">The XML string specifying which changes to which properties should be sent with this request.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(string requestUrl, string propPatchXmlString, LockToken lockToken)
        {
            return await PropPatchAsync(new Uri(requestUrl), propPatchXmlString, lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="propertyUpdate">A <see cref="PropertyUpdate"/> which specifies the properties and values which should be updated.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(string requestUrl, PropertyUpdate propertyUpdate, LockToken lockToken)
        {
            return await PropPatchAsync(new Uri(requestUrl), propertyUpdate, lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="propPatchXmlString">The XML string specifying which changes to which properties should be sent with this request.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(Uri requestUri, string propPatchXmlString, LockToken lockToken)
        {
            return await PropPatchAsync(requestUri, propPatchXmlString, lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="propertyUpdate">A <see cref="PropertyUpdate"/> which specifies the properties and values which should be updated.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(Uri requestUri, PropertyUpdate propertyUpdate, LockToken lockToken)
        {
            return await PropPatchAsync(requestUri, propertyUpdate, lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="propPatchXmlString">The XML string specifying which changes to which properties should be sent with this request.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(string requestUrl, string propPatchXmlString, HttpCompletionOption completionOption)
        {
            return await PropPatchAsync(new Uri(requestUrl), propPatchXmlString, null, completionOption);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="propPatchXmlString">The XML string specifying which changes to which properties should be sent with this request.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(string requestUrl, string propPatchXmlString, LockToken lockToken, HttpCompletionOption completionOption)
        {
            return await PropPatchAsync(new Uri(requestUrl), propPatchXmlString, lockToken, completionOption);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="propertyUpdate">A <see cref="PropertyUpdate"/> which specifies the properties and values which should be updated.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(string requestUrl, PropertyUpdate propertyUpdate, HttpCompletionOption completionOption)
        {
            return await PropPatchAsync(new Uri(requestUrl), propertyUpdate, null, completionOption);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="propertyUpdate">A <see cref="PropertyUpdate"/> which specifies the properties and values which should be updated.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(string requestUrl, PropertyUpdate propertyUpdate, LockToken lockToken, HttpCompletionOption completionOption)
        {
            return await PropPatchAsync(new Uri(requestUrl), propertyUpdate, lockToken, completionOption);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="propertyUpdate">A <see cref="PropertyUpdate"/> which specifies the properties and values which should be updated.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(Uri requestUri, PropertyUpdate propertyUpdate, HttpCompletionOption completionOption)
        {
            string requestContentString = string.Empty;

            if (propertyUpdate != null)
                requestContentString = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(PropertyUpdateSerializer, propertyUpdate);

            return await PropPatchAsync(requestUri, requestContentString, null, completionOption);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="propertyUpdate">A <see cref="PropertyUpdate"/> which specifies the properties and values which should be updated.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(Uri requestUri, PropertyUpdate propertyUpdate, LockToken lockToken, HttpCompletionOption completionOption)
        {
            string requestContentString = string.Empty;

            if (propertyUpdate != null)
                requestContentString = WebDavHelper.GetUtf8EncodedXmlWebDavRequestString(PropertyUpdateSerializer, propertyUpdate);

            return await PropPatchAsync(requestUri, requestContentString, lockToken, completionOption);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="propPatchXmlString">The XML string specifying which changes to which properties should be sent with this request.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(Uri requestUri, string propPatchXmlString, HttpCompletionOption completionOption)
        {
            return await PropPatchAsync(requestUri, propPatchXmlString, null, completionOption);
        }

        /// <summary>
        /// Sends a PROPPATCH request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="propPatchXmlString">The XML string specifying which changes to which properties should be sent with this request.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PropPatchAsync(Uri requestUri, string propPatchXmlString, LockToken lockToken, HttpCompletionOption completionOption)
        {
            var requestMethod = new HttpRequestMessage(new HttpMethod(WebDavMethod.PropPatch), requestUri);

            if (lockToken != null)
                requestMethod.Headers.Add(WebDavRequestHeader.If, lockToken.ToString(LockTokenFormat.IfHeader));

            if (!String.IsNullOrEmpty(propPatchXmlString))
            {
                var httpContent = new HttpStringContent(propPatchXmlString, Windows.Storage.Streams.UnicodeEncoding.Utf8, MediaTypeXml);
                requestMethod.Content = httpContent;
            }

            var httpResponseMessage = await this.SendAsync(requestMethod, completionOption);
            return httpResponseMessage;
        }

        #endregion Proppatch

        #region Put

        /// <summary>
        /// Send a PUT request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PutAsync(string requestUrl, IHttpContent content)
        {
            return await PutAsync(new Uri(requestUrl), content, null);
        }

        /// <summary>
        /// Send a PUT request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PutAsync(Uri requestUri, IHttpContent content)
        {
            return await PutAsync(requestUri, content, null);
        }

        /// <summary>
        /// Send a PUT request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PutAsync(string requestUrl, IHttpContent content, LockToken lockToken)
        {
            return await PutAsync(new Uri(requestUrl), content, lockToken);
        }

        /// <summary>
        /// Send a PUT request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PutAsync(Uri requestUri, IHttpContent content, LockToken lockToken)
        {
            var requestMethod = new HttpRequestMessage(HttpMethod.Put, requestUri);
            requestMethod.Content = content;

            if (lockToken != null)
                requestMethod.Headers.Add(WebDavRequestHeader.If, lockToken.ToString(LockTokenFormat.IfHeader));

            var httpResponseMessage = await this.SendAsync(requestMethod, HttpCompletionOption.ResponseContentRead);
            return httpResponseMessage;
        }

        /// <summary>
        /// Sends a PUT request to the specified URL.
        /// </summary>
        /// <param name="url">The URL the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PutAsync(string url, IHttpContent content, CancellationTokenSource cts, IProgress<HttpProgress> progress)
        {
            return await this.PutAsync(url, content, cts, progress, null);
        }

        /// <summary>
        /// Sends a PUT request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PutAsync(Uri uri, IHttpContent content, CancellationTokenSource cts, IProgress<HttpProgress> progress)
        {
            return await this.PutAsync(uri, content, cts, progress, null);
        }

        /// <summary>
        /// Sends a PUT request to the specified URL.
        /// </summary>
        /// <param name="url">The URL the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PutAsync(string url, IHttpContent content, CancellationTokenSource cts, IProgress<HttpProgress> progress, LockToken lockToken)
        {
            return await this.PutAsync(new Uri(url), content, cts, progress, lockToken);
        }

        /// <summary>
        /// Sends a PUT request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="content">The <see cref="IHttpContent"/> sent to the server.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> PutAsync(Uri uri, IHttpContent content, CancellationTokenSource cts, IProgress<HttpProgress> progress, LockToken lockToken)
        {
            if (lockToken != null)
                content.Headers.Add(WebDavRequestHeader.If, lockToken.ToString(LockTokenFormat.IfHeader));

            return await this.httpClient.PutAsync(uri, content).AsTask(cts.Token, progress);
        }

        #endregion Put

        #region Upload file

        /// <summary>
        /// Uploads a file from a given <see cref="IRandomAccessStream"/> to the given URL.
        /// </summary>
        /// <param name="url">The URL the request is sent to.</param>
        /// <param name="stream">The file's content as <see cref="IRandomAccessStream"/>.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> UploadFileAsync(string url, IRandomAccessStream stream, string contentType, CancellationTokenSource cts, IProgress<HttpProgress> progress)
        {
            return await this.UploadFileAsync(new Uri(url), stream, contentType, cts, progress, null);
        }

        /// <summary>
        /// Uploads a file from a given <see cref="IRandomAccessStream"/> to the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="stream">The file's content as <see cref="IRandomAccessStream"/>.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> UploadFileAsync(Uri uri, IRandomAccessStream stream, string contentType, CancellationTokenSource cts, IProgress<HttpProgress> progress)
        {
            return await this.UploadFileAsync(uri, stream, contentType, cts, progress, null);
        }

        /// <summary>
        /// Uploads a file from a given <see cref="IRandomAccessStream"/> to the given URL.
        /// </summary>
        /// <param name="url">The URL the request is sent to.</param>
        /// <param name="stream">The file's content as <see cref="IRandomAccessStream"/>.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> UploadFileAsync(string url, IRandomAccessStream stream, string contentType, CancellationTokenSource cts, IProgress<HttpProgress> progress, LockToken lockToken)
        {
            return await this.UploadFileAsync(new Uri(url), stream, contentType, cts, progress, lockToken);
        }

        /// <summary>
        /// Uploads a file from a given <see cref="IRandomAccessStream"/> to the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="stream">The file's content as <see cref="IRandomAccessStream"/>.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> to use or null if no lock token should be used.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> UploadFileAsync(Uri uri, IRandomAccessStream stream, string contentType, CancellationTokenSource cts, IProgress<HttpProgress> progress, LockToken lockToken)
        {
            var inputStream = stream.GetInputStreamAt(0);
            var streamContent = new HttpStreamContent(inputStream);
            
            if(!string.IsNullOrEmpty(contentType))
                streamContent.Headers.Add("Content-Type", contentType);

            streamContent.Headers.Add("Content-Length", stream.Size.ToString());

            if (lockToken != null)
                streamContent.Headers.Add(WebDavRequestHeader.If, lockToken.ToString(LockTokenFormat.IfHeader));

            return await this.PutAsync(uri, streamContent, cts, progress, lockToken);
        }

        #endregion Upload file

        #region Download file

        /// <summary>
        /// Downloads a file from the given URL.
        /// </summary>
        /// <param name="url">Te URL of the file to download.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IBuffer> DownloadFileAsync(string url, CancellationTokenSource cts, IProgress<HttpProgress> progress)
        {
            return await this.httpClient.GetBufferAsync(new Uri(url)).AsTask(cts.Token, progress);
        }

        /// <summary>
        /// Downloads a file from the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file to download.</param>
        /// <param name="cts">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IBuffer> DownloadFileAsync(Uri uri, CancellationTokenSource cts, IProgress<HttpProgress> progress)
        {
            return await this.httpClient.GetBufferAsync(uri).AsTask(cts.Token, progress);
        }

        #endregion Download file

        #region Send

        /// <summary>
        /// Send an HTTP request.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to send.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await SendAsync(request, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send an HTTP request.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to send.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            var httpResponseMessage = await this.httpClient.SendRequestAsync(request, completionOption);
            return httpResponseMessage;
        }

        #endregion Send

        #region Unlock

        /// <summary>
        /// Send a UNLOCK request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> of a locked resource which should be unlocked.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> UnlockAsync(string requestUrl, LockToken lockToken)
        {
            return await UnlockAsync(new Uri(requestUrl), lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a UNLOCK request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> of a locked resource which should be unlocked.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> UnlockAsync(Uri requestUri, LockToken lockToken)
        {
            return await UnlockAsync(requestUri, lockToken, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Send a UNLOCK request to the specified URL.
        /// </summary>
        /// <param name="requestUrl">The URL the request is sent to.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> of a locked resource which should be unlocked.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> UnlockAsync(string requestUrl, LockToken lockToken, HttpCompletionOption completionOption)
        {
            return await UnlockAsync(new Uri(requestUrl), lockToken, completionOption);
        }

        /// <summary>
        /// Send a UNLOCK request to the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="requestUri">The <see cref="Uri"/> the request is sent to.</param>
        /// <param name="lockToken">The <see cref="LockToken"/> of a locked resource which should be unlocked.</param>
        /// <param name="completionOption">An <see cref="HttpCompletionOption"/> value that indicates when the operation should be considered completed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<HttpResponseMessage> UnlockAsync(Uri requestUri, LockToken lockToken, HttpCompletionOption completionOption)
        {
            if (lockToken == null)
                throw new WebDavException("No lock token specified. A lock token is required for unlocking.");

            var requestMethod = new HttpRequestMessage(new HttpMethod(WebDavMethod.Unlock), requestUri);
            requestMethod.Headers.Add(WebDavRequestHeader.LockTocken, lockToken.ToString(LockTokenFormat.LockTokenHeader));

            var httpResponseMessage = await this.SendAsync(requestMethod, completionOption);
            return httpResponseMessage;
        }

        #endregion Unlock

        #region Private methods

        private async static Task<Multistatus> GetMultistatusRequestResult(HttpResponseMessage responseMessage)
        {
            var responseString = await responseMessage.Content.ReadAsStringAsync();
            Multistatus multiStatus;

            using (XmlReader xmlReader = XmlReader.Create(responseString))
            {
                multiStatus = (Multistatus)MultistatusSerializer.Deserialize(xmlReader);
            }

            return multiStatus;
        }

        #endregion Private methods

        #region IDisposable members

        bool disposed = false;

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.

                if (this.httpClient != null)
                {
                    this.httpClient.Dispose();
                    this.httpClient = null;
                }
            }

            // Free any unmanaged objects here.

            disposed = true;
        }

        #endregion IDisposable members
    }
}
