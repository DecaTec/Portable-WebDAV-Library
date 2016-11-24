using DecaTec.WebDav.WebDavArtifacts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class for WebDAV sessions.
    /// </summary>
    /// <remarks>
    /// <para>This class acts as an abstraction layer between the application and the <see cref="DecaTec.WebDav.WebDavClient"/>, which is used to communicate with the WebDAV server.</para>
    /// <para>If you want to communicate with the WebDAV server directly, you should use the <see cref="DecaTec.WebDav.WebDavClient"/>.</para>
    /// <para>The WebDavSession can be used with a base URL/<see cref="System.Uri"/>. If such a base URL/<see cref="System.Uri"/> is specified, all subsequent operations involving an 
    /// URL/<see cref="System.Uri"/> will be relative to this base URL/<see cref="System.Uri"/>.
    /// If no base URL/<see cref="System.Uri"/> is specified, all operations has the be called with an absolute URL/<see cref="System.Uri"/>.</para>
    /// </remarks>
    /// <example>See the following code to list the content of a directory with the WebDavSession:
    /// <code>
    /// // You have to add a reference to DecaTec.WebDav.Uwp.dll.
    /// //
    /// // The base URL of the WebDAV server.
    /// var webDavServerUrl = "http://www.myserver.com/webdav/";
    ///
    /// // Specify the user credentials and use it to create a WebDavSession instance.
    /// var credentials = new PasswordCredential(webDavServerUrl, "MyUsername", "MyPassword");
    /// var webDavSession = new WebDavSession(@"http://www.myserver.com/webdav/", credentials);
    /// var items = await webDavSession.ListAsync(@"MyFolder/");
    ///
    /// foreach (var item in items)
    /// {
    ///     // Handle the response (list of WebDavSessionListItems), e.g item.Uri is the URL of an item (folder or file).
    /// }
    /// </code>
    /// <para></para>
    /// See the following code which uses locking with a WebDavSession:
    /// <code>
    /// // The base URL of the WebDAV server.
    /// var webDavServerUrl = "http://www.myserver.com/webdav/";
    /// 
    /// // Specify the user credentials and use it to create a WebDavSession instance.
    /// var credentials = new PasswordCredential(webDavServerUrl, "MyUsername", "MyPassword");
    /// var webDavSession = new WebDavSession(@"http://www.myserver.com/webdav/", credentials);
    /// await webDavSession.LockAsync(@"MyFolder/");
    ///
    /// // Create new folder and delete it.
    /// // You DO NOT have to care about that the folder is locked (i.e. you do not have to submit a lock token).
    /// // This is all handled by the WebDavSession itself.
    /// await webDavSession.CreateDirectoryAsync("MyFolder/NewFolder");
    /// await webDavSession.DeleteAsync("MyFolder/NewFolder");
    ///
    /// // Unlock the folder again.
    /// await webDavSession.UnlockAsync(@"MyFolder/");
    ///
    /// // You should always call Dispose on the WebDavSession when it is not longer needed.
    /// // During Dispose, all locks held by the WebDavSession will be automatically unlocked.
    /// webDavSession.Dispose();
    /// </code>
    /// </example>
    /// <seealso cref="DecaTec.WebDav.WebDavClient"/>
    public partial class WebDavSession : IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of WebDavSession with a default <see cref="System.Net.Http.HttpClientHandler"/>.
        /// </summary>
        /// <param name="passwordCredential">The <see cref="Windows.Security.Credentials.PasswordCredential"/> to use.</param>
        public WebDavSession(PasswordCredential passwordCredential)
            : this(string.Empty, new HttpBaseProtocolFilter() { ServerCredential = passwordCredential })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URL and a default <see cref="System.Net.Http.HttpClientHandler"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="passwordCredential">The <see cref="Windows.Security.Credentials.PasswordCredential"/> to use.</param>
        public WebDavSession(string baseUrl, PasswordCredential passwordCredential)
            : this(new Uri(baseUrl), new HttpBaseProtocolFilter() { ServerCredential = passwordCredential })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URI and a default <see cref="System.Net.Http.HttpClientHandler"/>.
        /// </summary>
        /// <param name="baseUri">The base URI to use for this WebDavSession.</param>
        /// <param name="passwordCredential">The <see cref="Windows.Security.Credentials.PasswordCredential"/> to use.</param>
        public WebDavSession(Uri baseUri, PasswordCredential passwordCredential)
            : this(baseUri, new HttpBaseProtocolFilter() { ServerCredential = passwordCredential })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the <see cref="System.Net.Http.HttpMessageHandler"/> specified.
        /// </summary>
        /// <param name="httpFilter">The <see cref="Windows.Web.Http.Filters.IHttpFilter"/> to use with this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="System.Net.Http.HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(IHttpFilter httpFilter)
            : this(string.Empty, httpFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the <see cref="System.Net.Http.HttpMessageHandler"/> specified.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="httpFilter">The <see cref="Windows.Web.Http.Filters.IHttpFilter"/> to use with this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="System.Net.Http.HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(string baseUrl, IHttpFilter httpFilter)
            : this(string.IsNullOrEmpty(baseUrl) ? null : new Uri(baseUrl), httpFilter)
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URI and the <see cref="System.Net.Http.HttpMessageHandler"/> specified.
        /// </summary>
        /// <param name="baseUri">The base URI to use for this WebDavSession.</param>
        /// <param name="httpFilter">The <see cref="Windows.Web.Http.Filters.IHttpFilter"/> to use with this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="System.Net.Http.HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(Uri baseUri, IHttpFilter httpFilter)
        {
            this.permanentLocks = new ConcurrentDictionary<Uri, PermanentLock>();
            this.webDavClient = CreateWebDavClient(httpFilter);
            this.BaseUri = baseUri;
        }

        #endregion Constructor

        #region Public methods

        #region Download file

        /// <summary>
        ///  Downloads a file from the URI specified.
        /// </summary>
        /// <param name="uri">The URI of the file to download.</param>
        /// <param name="localStream">The stream to save the file to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileAsync(Uri uri, Stream localStream)
        {
            uri = UrlHelper.GetAbsoluteUriWithTrailingSlash(this.BaseUri, uri);
            var response = await this.webDavClient.GetAsync(uri);

            if (response.Content != null)
            {
                try
                {
                    var contentBuffer = await response.Content.ReadAsBufferAsync();
                    var bArr = contentBuffer.ToArray();
                    await localStream.WriteAsync(bArr, 0, (int)contentBuffer.Length);
                    return true;
                }
                catch (IOException)
                {
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Downloads a file from the given URI.
        /// </summary>
        /// <param name="uri">Te URI of the file to download.</param>
        /// <param name="cts">The CancellationTokenSource to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<IBuffer> DownloadFileAsync(Uri uri, CancellationTokenSource cts, IProgress<HttpProgress> progress)
        {
            uri = UrlHelper.GetAbsoluteUriWithTrailingSlash(this.BaseUri, uri);
            return await this.webDavClient.DownloadFileAsync(uri, cts, progress);
        }

        #endregion Download file

        #region List

        /// <summary>
        /// Retrieves a list of files and directories of the directory at the URI specified.
        /// </summary>
        /// <param name="uri">The URI of the directory which content should be listed.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<IList<WebDavSessionListItem>> ListAsync(Uri uri)
        {
            uri = UrlHelper.GetAbsoluteUriWithTrailingSlash(this.BaseUri, uri);

            // Do not use an allprop here because some WebDav servers will not return the expected results when using allprop.
            var propFind = PropFind.CreatePropFindWithEmptyPropertiesAll();
            var response = await this.webDavClient.PropFindAsync(uri, WebDavDepthHeaderValue.One, propFind);

            if (response.StatusCode.ToString() != WebDavStatusCode.MultiStatus.ToString())
                throw new WebDavException(string.Format("Error while executing ListAsync (wrong response status code). Expected status code: 207 (MultiStatus); actual status code: {0} ({1})", (int)response.StatusCode, response.StatusCode));

            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

            var itemList = new List<WebDavSessionListItem>();

            foreach (var responseItem in multistatus.Response)
            {
                var webDavSessionItem = new WebDavSessionListItem();

                Uri href = null;

                if (!string.IsNullOrEmpty(responseItem.Href))
                {
                    if (Uri.TryCreate(responseItem.Href, UriKind.RelativeOrAbsolute, out href))
                    {
                        var fullQualifiedUri = UrlHelper.GetAbsoluteUri(uri, href);
                        webDavSessionItem.Uri = fullQualifiedUri;
                    }
                }

                // Skip the folder which contents were requested, only add children.
                if (href != null && uri.ToString().EndsWith(href.ToString(), StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (var item in responseItem.Items)
                {
                    var propStat = item as Propstat;

                    // Do not items where no properties could be found.
                    if (propStat == null || propStat.Status.ToLower().Contains("404 not found"))
                        continue;

                    // Do not add hidden items.
                    if (propStat.Prop.IsHidden == "1")
                        continue;

                    webDavSessionItem.ContentClass = propStat.Prop.ContentClass;
                    webDavSessionItem.ContentLanguage = propStat.Prop.GetContentLanguage;

                    if (!string.IsNullOrEmpty(propStat.Prop.GetContentLength))
                        webDavSessionItem.ContentLength = long.Parse(propStat.Prop.GetContentLength, CultureInfo.InvariantCulture);
                   
                    webDavSessionItem.ContentType = propStat.Prop.GetContentType;

                    if (propStat.Prop.CreationDateSpecified && !string.IsNullOrEmpty(propStat.Prop.CreationDate))
                        webDavSessionItem.CreationDate = DateTime.Parse(propStat.Prop.CreationDate, CultureInfo.InvariantCulture);

                    webDavSessionItem.DefaultDocument = propStat.Prop.DefaultDocument;
                    webDavSessionItem.DisplayName = propStat.Prop.DisplayName;
                    webDavSessionItem.ETag = propStat.Prop.GetEtag;

                    if (!string.IsNullOrEmpty(propStat.Prop.GetLastModified))
                        webDavSessionItem.LastModified = DateTime.Parse(propStat.Prop.GetLastModified, CultureInfo.InvariantCulture);

                    if (!string.IsNullOrEmpty(propStat.Prop.IsReadonly))
                        webDavSessionItem.IsReadonly = bool.Parse(propStat.Prop.IsReadonly);

                    if (!string.IsNullOrEmpty(propStat.Prop.IsRoot))
                        webDavSessionItem.IsRoot = bool.Parse(propStat.Prop.IsRoot);

                    if (!string.IsNullOrEmpty(propStat.Prop.IsStructuredDocument))
                        webDavSessionItem.IsStructuredDocument = bool.Parse(propStat.Prop.IsStructuredDocument);

                    if (!string.IsNullOrEmpty(propStat.Prop.LastAccessed))
                        webDavSessionItem.LastAccessed = DateTime.Parse(propStat.Prop.LastAccessed, CultureInfo.InvariantCulture);

                    webDavSessionItem.Name = propStat.Prop.Name;
                    webDavSessionItem.ParentName = propStat.Prop.ParentName;

                    if (!string.IsNullOrEmpty(propStat.Prop.QuotaAvailableBytes))
                        webDavSessionItem.QuotaAvailableBytes = long.Parse(propStat.Prop.QuotaAvailableBytes, CultureInfo.InvariantCulture);

                    if (!string.IsNullOrEmpty(propStat.Prop.QuotaUsedBytes))
                        webDavSessionItem.QuotaUsedBytes = long.Parse(propStat.Prop.QuotaUsedBytes, CultureInfo.InvariantCulture);

                    // Make sure that the IsDirectory property is set if it's a directory.
                    if (!string.IsNullOrEmpty(propStat.Prop.IsCollection))
                        webDavSessionItem.IsCollection = bool.Parse(propStat.Prop.IsCollection);
                    else if (propStat.Prop.ResourceType != null && propStat.Prop.ResourceType.Collection != null)
                    {
                        webDavSessionItem.IsCollection = true;
                    }

                    // Make sure that the name property is set.
                    // Naming priority:
                    // 1. displayname
                    // 2. name
                    // 3. (part of) URI.
                    webDavSessionItem.Name = propStat.Prop.DisplayName;

                    if (string.IsNullOrEmpty(webDavSessionItem.Name))
                        webDavSessionItem.Name = propStat.Prop.Name;

                    if (string.IsNullOrEmpty(webDavSessionItem.Name) && href != null)
                        webDavSessionItem.Name = href.ToString().Split('/').Last(x => !string.IsNullOrEmpty(x));
                }

                itemList.Add(webDavSessionItem);
            }

            return itemList;
        }

        #endregion List

        #region Upload file

        /// <summary>
        /// Uploads a file to the URI specified.
        /// </summary>
        /// <param name="uri">The URI of the file to upload.</param>
        /// <param name="localStream">The stream containing the file to upload.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileAsync(Uri uri, Stream localStream)
        {
            uri = UrlHelper.GetAbsoluteUriWithTrailingSlash(this.BaseUri, uri);
            var lockToken = GetAffectedLockToken(uri);
            var content = new HttpStreamContent(localStream.AsInputStream());
            var response = await this.webDavClient.PutAsync(uri, content, lockToken);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Uploads a file to the URI specified.
        /// </summary>
        /// <param name="uri">The URI of the file to upload.</param>
        /// <param name="stream">The stream containing the file to upload.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="cts">The CancellationTokenSource to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileAsync(Uri uri, IRandomAccessStream stream, string contentType, CancellationTokenSource cts, IProgress<HttpProgress> progress)
        {
            uri = UrlHelper.GetAbsoluteUriWithTrailingSlash(this.BaseUri, uri);
            var lockToken = GetAffectedLockToken(uri);
            var response = await this.webDavClient.UploadFileAsync(uri, stream, contentType, cts, progress, lockToken);
            return response.IsSuccessStatusCode;
        }

        #endregion Upload file

        #endregion Public methods

        #region Private methods

        private static WebDavClient CreateWebDavClient(IHttpFilter httpFilter)
        {
            return new WebDavClient(httpFilter);
        }

        #endregion Private methods
    }
}
