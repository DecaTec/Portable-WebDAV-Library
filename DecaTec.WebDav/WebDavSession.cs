using DecaTec.WebDav.WebDavArtifacts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class for WebDAV sessions.
    /// </summary>
    /// <remarks>
    /// <para>This class acts as an abstraction layer between the application and the <see cref="DecaTec.WebDav.WebDavClient"/>, which is used to communicate with the WebDAV server.</para>
    /// <para>If you want to communicate with the WebDAV server directly, you should use the <see cref="DecaTec.WebDav.WebDavClient"/>.</para>
    /// <para>The WebDavSession can be used with a base URL/<see cref="System.Uri"/>. If such a base URL/<see cref="System.Uri"/> is specified, all subsequent operations involving an 
    /// URL/<see cref="System.Uri"/> will be relative on this base URL/<see cref="System.Uri"/>.
    /// If no base URL/<see cref="System.Uri"/> is specified, all operations has the be called with an absolute URL/<see cref="System.Uri"/>.</para>
    /// </remarks>
    /// <example>See the following code to list the content of a directory with the WebDavSession:
    /// <code>
    /// // You have to add a reference to DecaTec.WebDav.dll.
    /// //
    /// // Specify the user credentials and use it to create a WebDavSession instance.
    /// var credentials = new NetworkCredential("MyUserName", "MyPassword");
    /// var webDavSession = new WebDavSession(@"http://www.myserver.com/webdav/", credentials);
    /// var items = await webDavSession.ListAsync(@"MyFolder/");
    ///
    /// foreach (var item in items)
    /// {
    ///     Console.WriteLine(item.Name);
    /// }
    /// 
    /// // Dispose the WebDavSession when it is not longer needed.
    /// webDavSession.Dispose();
    /// </code>
    /// <para></para>
    /// See the following code which uses locking with a WebDavSession:
    /// <code>
    /// // Specify the user credentials and use it to create a WebDavSession instance.
    /// var credentials = new NetworkDavCredential("MyUserName", "MyPassword");
    /// var webDavSession = new WebDavSession(@"http://www.myserver.com/webdav/", credentials);
    /// await webDavSession.LockAsync(@"Test/");
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
    public class WebDavSession : IDisposable
    {
        private readonly WebDavClient webDavClient;
        private readonly ConcurrentDictionary<Uri, PermanentLock> permanentLocks;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of WebDavSession with a default <see cref="HttpClientHandler"/>.
        /// </summary>
        /// <param name="networkCredential">The <see cref="NetworkCredential"/> to use.</param>
        public WebDavSession(NetworkCredential networkCredential)
            : this(new HttpClientHandler() { PreAuthenticate = true, Credentials = networkCredential })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URL and a default <see cref="HttpClientHandler"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="networkCredential">The <see cref="NetworkCredential"/> to use.</param>
        public WebDavSession(string baseUrl, NetworkCredential networkCredential)
            : this(new Uri(baseUrl), new HttpClientHandler() { PreAuthenticate = true, Credentials = networkCredential })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base <see cref="Uri"/> and a default <see cref="HttpClientHandler"/>.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> to use for this WebDavSession.</param>
        /// <param name="networkCredential">The <see cref="NetworkCredential"/> to use.</param>
        public WebDavSession(Uri baseUri, NetworkCredential networkCredential)
            : this(baseUri, new HttpClientHandler() { PreAuthenticate = true, Credentials = networkCredential })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the <see cref="HttpMessageHandler"/> specified.
        /// </summary>
        /// <param name="httpMessageHandler">The <see cref="HttpMessageHandler"/> to use with this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(HttpMessageHandler httpMessageHandler)
            : this(string.Empty, httpMessageHandler)
        {            
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URL and the <see cref="HttpMessageHandler"/> specified.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="httpMessageHandler">The <see cref="HttpMessageHandler"/> to use with this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(string baseUrl, HttpMessageHandler httpMessageHandler)
            : this(string.IsNullOrEmpty(baseUrl) ? null : new Uri(baseUrl), httpMessageHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base <see cref="Uri"/> and the <see cref="HttpMessageHandler"/> specified.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> to use for this WebDavSession.</param>
        /// <param name="httpMessageHandler">The <see cref="HttpMessageHandler"/> to use with this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(Uri baseUri, HttpMessageHandler httpMessageHandler)
        {
            this.permanentLocks = new ConcurrentDictionary<Uri, PermanentLock>();
            this.webDavClient = CreateWebDavClient(httpMessageHandler);
            this.BaseUri = baseUri;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the base <see cref="Uri"/> of this WebDavSession.
        /// </summary>
        public Uri BaseUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IWebProxy"/> to use with this WebDavSession.
        /// </summary>
        public IWebProxy WebProxy
        {
            get;
            set;
        }

        #endregion Properties

        #region Public methods

        #region Copy

        /// <summary>
        /// Copies a resource from the source URL to the destination URL (without overwriting).
        /// </summary>
        /// <param name="sourceUrl">The source URL.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CopyAsync(string sourceUrl, string destinationUrl)
        {
            return await CopyAsync(new Uri(sourceUrl, UriKind.RelativeOrAbsolute), new Uri(destinationUrl, UriKind.RelativeOrAbsolute), false);
        }

        /// <summary>
        /// Copies a resource from the source <see cref="Uri"/> to the destination <see cref="Uri"/> (without overwriting).
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destinationUri">The destination <see cref="Uri"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CopyAsync(Uri sourceUri, Uri destinationUri)
        {
            return await CopyAsync(sourceUri, destinationUri, false);
        }

        /// <summary>
        /// Copies a resource from the source URL to the destination URL.
        /// </summary>
        /// <param name="sourceUrl">The source URL.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CopyAsync(string sourceUrl, string destinationUrl, bool overwrite)
        {
            return await CopyAsync(new Uri(sourceUrl, UriKind.RelativeOrAbsolute), new Uri(destinationUrl, UriKind.RelativeOrAbsolute), overwrite);
        }

        /// <summary>
        /// Copies a resource from the source <see cref="Uri"/> to the destination <see cref="Uri"/>.
        /// </summary>
        /// <param name="sourceUri">The source <see cref="Uri"/>.</param>
        /// <param name="destinationUri">The destination <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CopyAsync(Uri sourceUri, Uri destinationUri, bool overwrite)
        {
            sourceUri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, sourceUri, true, false);
            destinationUri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, destinationUri, true, false);
            var lockToken = GetAffectedLockToken(destinationUri);
            var response = await this.webDavClient.CopyAsync(sourceUri, destinationUri, overwrite, WebDavDepthHeaderValue.Infinity, lockToken);
            return response.IsSuccessStatusCode;
        }

        #endregion Copy

        #region Create directory

        /// <summary>
        /// Creates a directory at the URL specified.
        /// </summary>
        /// <param name="url">The URL of the directory to create.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CreateDirectoryAsync(string url)
        {
            return await CreateDirectoryAsync(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Creates a directory at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the directory to create.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CreateDirectoryAsync(Uri uri)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            var lockToken = GetAffectedLockToken(uri);
            var response = await this.webDavClient.MkcolAsync(uri, lockToken);
            return response.IsSuccessStatusCode;
        }

        #endregion Create directory

        #region Delete

        /// <summary>
        /// Deletes a directory or file at the URL specified.
        /// </summary>
        /// <param name="url">The URL of the directory or file to delete.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DeleteAsync(string url)
        {
            return await DeleteAsync(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Deletes a directory or file at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the directory or file to delete.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DeleteAsync(Uri uri)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            var lockToken = GetAffectedLockToken(uri);
            var response = await this.webDavClient.DeleteAsync(uri, lockToken);
            return response.IsSuccessStatusCode;
        }

        #endregion Delete

        #region Download file

        /// <summary>
        /// Downloads a file from the URL specified.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the file to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileAsync(string url, Stream localStream)
        {
            return await DownloadFileAsync(new Uri(url, UriKind.RelativeOrAbsolute), localStream);
        }

        /// <summary>
        ///  Downloads a file from the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the file to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileAsync(Uri uri, Stream localStream)
        {
            return await DownloadFileAsync(uri, localStream, CancellationToken.None);
        }

        /// <summary>
        ///  Downloads a file from the URL specified.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the file to.</param>
        /// <param name="ct">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileAsync(string url, Stream localStream, CancellationToken ct)
        {
            return await DownloadFileAsync(new Uri(url, UriKind.RelativeOrAbsolute), localStream, ct);
        }

        /// <summary>
        ///  Downloads a file from the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the file to.</param>
        /// <param name="ct">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileAsync(Uri uri, Stream localStream, CancellationToken ct)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            var response = await this.webDavClient.GetAsync(uri, ct);

            if (response.Content != null)
            {
                try
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    await contentStream.CopyToAsync(localStream);
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
        /// Downloads a file (with progress) from the given URL.
        /// </summary>
        /// <param name="url">Te URL of the file to download.</param>
        /// <param name="targetStream">The <see cref="Stream"/> to save the downloaded file to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<WebDavResponseMessage> DownloadFileWithProgressAsync(string url, Stream targetStream, CancellationToken cancellationToken, IProgress<WebDavProgress> progress)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            return await DownloadFileWithProgressAsync(uri, targetStream, cancellationToken, progress);
        }

        /// <summary>
        /// Downloads a file (with progress) from the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">Te <see cref="Uri"/> of the file to download.</param>
        /// <param name="targetStream">The <see cref="Stream"/> to save the downloaded file to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationTokenSource"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<WebDavResponseMessage> DownloadFileWithProgressAsync(Uri uri, Stream targetStream, CancellationToken cancellationToken, IProgress<WebDavProgress> progress)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            return await this.webDavClient.DownloadFileWithProgressAsync(uri, targetStream, cancellationToken, progress);
        }

        #endregion Download file

        #region Exists

        /// <summary>
        /// Checks if a file or directory exists at the URL specified.
        /// </summary>
        /// <param name="url">The URL to check.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> ExistsAsync(string url)
        {
            return await ExistsAsync(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Checks if a file or directory exists at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to check.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> ExistsAsync(Uri uri)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            var response = await this.webDavClient.HeadAsync(uri);
            return response.IsSuccessStatusCode;
        }

        #endregion Exists

        #region List

        /// <summary>
        /// Retrieves a list of files and directories of the directory at the <see cref="Uri"/> specified (using 'allprop').
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the directory which content should be listed. Has to be an absolute URI (including the base URI) or a relative URI (relative to base URI).</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>This method uses a so called 'allprop'. A server should return all known properties to the server.
        /// If not all of the expected properties are return by the server, use an overload of this method specifying a <see cref="PropFind"/> explicitly.</remarks>
        public async Task<IList<WebDavSessionListItem>> ListAsync(Uri uri)
        {
            return await ListAsync(uri, PropFind.CreatePropFindAllProp());
        }

        /// <summary>
        /// Retrieves a list of files and directories of the directory at the URL specified.
        /// </summary>
        /// <param name="url">The URL of the directory which content should be listed. Has to be an absolute URL (including the base URL) or a relative URL (relative to base URL).</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>This method uses a so called 'allprop'. A server should return all known properties to the server.
        /// If not all of the expected properties are return by the server, use an overload of this method specifying a <see cref="PropFind"/> explicitly.</remarks>
        public async Task<IList<WebDavSessionListItem>> ListAsync(string url)
        {
            return await ListAsync(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Retrieves a list of files and directories of the directory at the URL specified.
        /// </summary>
        /// <param name="url">The URL of the directory which content should be listed. Has to be an absolute URL (including the base URL) or a relative URL (relative to base URL).</param>
        /// <param name="propFind">The <see cref="PropFind"/> to use. Different PropFind  types can be created using the static methods of the class <see cref="PropFind"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IList<WebDavSessionListItem>> ListAsync(string url, PropFind propFind)
        {
            return await ListAsync(new Uri(url, UriKind.RelativeOrAbsolute), propFind);
        }

        /// <summary>
        /// Retrieves a list of files and directories of the directory at the <see cref="Uri"/> specified using the <see cref="PropFind"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the directory which content should be listed. Has to be an absolute URI (including the base URI) or a relative URI (relative to base URI).</param>
        /// <param name="propFind">The <see cref="PropFind"/> to use. Different PropFind  types can be created using the static methods of the class <see cref="PropFind"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IList<WebDavSessionListItem>> ListAsync(Uri uri, PropFind propFind)
        {
            if (propFind == null)
                throw new ArgumentException("Argument propFind must not be null.");

            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            var response = await this.webDavClient.PropFindAsync(uri, WebDavDepthHeaderValue.One, propFind);

            // Remember the original port to include it in the hrefs later.
            var port = UriHelper.GetPort(uri);

            if (response.StatusCode != WebDavStatusCode.MultiStatus)
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
                        var fullQualifiedUri = UriHelper.CombineUri(uri, href, true);
                        fullQualifiedUri = UriHelper.SetPort(fullQualifiedUri, port);
                        webDavSessionItem.Uri = fullQualifiedUri;
                    }
                }

                // Skip the folder which contents were requested, only add children.
                if (href != null && WebUtility.UrlDecode(UriHelper.RemovePort(uri).ToString().Trim('/')).EndsWith(WebUtility.UrlDecode(UriHelper.RemovePort(href).ToString().Trim('/')), StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (var item in responseItem.Items)
                {
                    var propStat = item as Propstat;

                    // Do not items where no properties could be found.
                    if (propStat == null || propStat.Status.ToLower().Contains("404 not found"))
                        continue;

                    // Do not add hidden items.
                    if (!string.IsNullOrEmpty(propStat.Prop.IsHidden) && propStat.Prop.IsHidden.Equals("1"))
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
                        webDavSessionItem.IsReadonly = propStat.Prop.IsReadonly.Equals("1");

                    if (!string.IsNullOrEmpty(propStat.Prop.IsRoot))
                        webDavSessionItem.IsRoot = propStat.Prop.IsRoot.Equals("1");

                    if (!string.IsNullOrEmpty(propStat.Prop.IsStructuredDocument))
                        webDavSessionItem.IsStructuredDocument = propStat.Prop.IsStructuredDocument.Equals("1");

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
                        webDavSessionItem.IsCollection = propStat.Prop.IsCollection.Equals("1");
                    else if (propStat.Prop.ResourceType != null && propStat.Prop.ResourceType.Collection != null)
                        webDavSessionItem.IsCollection = true;

                    // Make sure that the name property is set.
                    // Naming priority:
                    // 1. displayname (only if it doesn't contain raw unicode, otherwise there are problems with non western characters)
                    // 2. name
                    // 3. (part of) URI.
                    if (!TextHelper.StringContainsRawUnicode(propStat.Prop.DisplayName))
                        webDavSessionItem.Name = propStat.Prop.DisplayName;

                    if (string.IsNullOrEmpty(webDavSessionItem.Name))
                        webDavSessionItem.Name = propStat.Prop.Name;

                    if (string.IsNullOrEmpty(webDavSessionItem.Name) && href != null)
                        webDavSessionItem.Name = WebUtility.UrlDecode(href.ToString().Split('/').Last(x => !string.IsNullOrEmpty(x)));
                }

                itemList.Add(webDavSessionItem);
            }

            return itemList;
        }

        #endregion List

        #region Lock

        /// <summary>
        /// Locks a file or directory at the URL specified.
        /// </summary>
        /// <param name="url">The URL of the file or directory to lock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> LockAsync(string url)
        {
            return await LockAsync(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        ///  Locks a file or directory at the URL specified.
        /// </summary>
        /// <param name="uri">The URI of the file or directory to lock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> LockAsync(Uri uri)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);

            if (this.permanentLocks.ContainsKey(uri))
                return true; // Lock already set.

            var lockInfo = new LockInfo();
            lockInfo.LockScope = LockScope.CreateExclusiveLockScope();
            lockInfo.LockType = LockType.CreateWriteLockType();
            var response = await this.webDavClient.LockAsync(uri, WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(), WebDavDepthHeaderValue.Infinity, lockInfo);

            if (!response.IsSuccessStatusCode)
                return false; // Lock already exists.

            var lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);

            var prop = await WebDavResponseContentParser.ParsePropResponseContentAsync(response.Content);
            var lockDiscovery = prop.LockDiscovery;

            if (lockDiscovery == null)
                return false;

            var url = uri.ToString();
            var lockGranted = lockDiscovery.ActiveLock.FirstOrDefault(x => url.EndsWith(UriHelper.AddTrailingSlash(x.LockRoot.Href, false), StringComparison.OrdinalIgnoreCase));

            if (lockGranted == null)
            {
                // Try with file expected.
                lockGranted = lockDiscovery.ActiveLock.FirstOrDefault(x => url.EndsWith(UriHelper.AddTrailingSlash(x.LockRoot.Href, true), StringComparison.OrdinalIgnoreCase));
            }

            if (lockGranted == null)
                return false;

            var permanentLock = new PermanentLock(this.webDavClient, lockToken, uri, lockGranted.Timeout);

            if (!this.permanentLocks.TryAdd(uri, permanentLock))
                throw new WebDavException("Lock with lock root " + uri.ToString() + " already exists.");

            return response.IsSuccessStatusCode;
        }

        #endregion Lock

        #region Move

        /// <summary>
        /// Moves a file or directory with the specified URL to another URL (without overwrite)
        /// </summary>
        /// <param name="sourceUrl">The URL of the source.</param>
        /// <param name="destinationUrl">The URL of the destination.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> MoveAsync(string sourceUrl, string destinationUrl)
        {
            return await MoveAsync(new Uri(sourceUrl, UriKind.RelativeOrAbsolute), new Uri(destinationUrl, UriKind.RelativeOrAbsolute), false);
        }

        /// <summary>
        /// Moves a file or directory with the specified URI to another URI (without overwrite).
        /// </summary>
        /// <param name="sourceUri">The URI of the source.</param>
        /// <param name="destinationUri">The URL of the destination.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> MoveAsync(Uri sourceUri, Uri destinationUri)
        {
            return await MoveAsync(sourceUri, destinationUri, false);
        }

        /// <summary>
        /// Moves a file or directory with the specified URL to another URL.
        /// </summary>
        /// <param name="sourceUrl">The URL of the source.</param>
        /// <param name="destinationUrl">The URL of the destination.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> MoveAsync(string sourceUrl, string destinationUrl, bool overwrite)
        {
            return await MoveAsync(new Uri(sourceUrl, UriKind.RelativeOrAbsolute), new Uri(destinationUrl, UriKind.RelativeOrAbsolute), overwrite);
        }

        /// <summary>
        /// Moves a file or directory with the specified <see cref="Uri"/> to another <see cref="Uri"/>.
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source.</param>
        /// <param name="destinationUri">The <see cref="Uri"/> of the destination.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> MoveAsync(Uri sourceUri, Uri destinationUri, bool overwrite)
        {
            sourceUri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, sourceUri, true, false);
            destinationUri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, destinationUri, true, false);
            var lockTokenSource = GetAffectedLockToken(sourceUri);
            var lockTokenDestination = GetAffectedLockToken(destinationUri);
            var response = await this.webDavClient.MoveAsync(sourceUri, destinationUri, overwrite, lockTokenSource, lockTokenDestination);
            return response.IsSuccessStatusCode;
        }

        #endregion Move

        #region Upload file

        /// <summary>
        /// Uploads a file to the URL specified.
        /// </summary>
        /// <param name="url">The URL of the file to upload.</param>
        /// <param name="localStream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileAsync(string url, Stream localStream)
        {
            return await UploadFileAsync(new Uri(url, UriKind.RelativeOrAbsolute), localStream);
        }

        /// <summary>
        /// Uploads a file to the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file to upload.</param>
        /// <param name="localStream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileAsync(Uri uri, Stream localStream)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            var lockToken = GetAffectedLockToken(uri);
            var content = new StreamContent(localStream);
            var response = await this.webDavClient.PutAsync(uri, content, lockToken);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Uploads a file (with progress) to the URL specified.
        /// </summary>
        /// <param name="url">The URL of the file to upload.</param>
        /// <param name="stream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileWithProgressAsync(string url, Stream stream, string contentType, CancellationToken cancellationToken, IProgress<WebDavProgress> progress)
        {
            return await UploadFileWithProgressAsync(new Uri(url, UriKind.RelativeOrAbsolute), stream, contentType, cancellationToken, progress);
        }

        /// <summary>
        /// Uploads a file (with progress) to the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file to upload.</param>
        /// <param name="stream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileWithProgressAsync(Uri uri, Stream stream, string contentType, CancellationToken cancellationToken, IProgress<WebDavProgress> progress)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            var lockToken = GetAffectedLockToken(uri);
            var response = await this.webDavClient.UploadFileWithProgressAsync(uri, stream, contentType, cancellationToken, progress, lockToken);
            return response.IsSuccessStatusCode;
        }


        #endregion Upload file

        #region Unlock

        /// <summary>
        /// Unlocks a file or directory at the URL specified. 
        /// </summary>
        /// <param name="url">The URL of the file or directory to unlock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UnlockAsync(string url)
        {
            return await UnlockAsync(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Unlocks a file or directory at the <see cref="Uri"/> specified. 
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file or directory to unlock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UnlockAsync(Uri uri)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);
            PermanentLock permanentLock;

            if (!this.permanentLocks.TryRemove(uri, out permanentLock))
                return false;

            var result = await permanentLock.UnlockAsync();
            var success = result.IsSuccessStatusCode;

            if (!success)
            {
                // Lock couldn't be removed on WebDav server, add it again in the permanent locks.
                if (!this.permanentLocks.TryAdd(uri, permanentLock))
                    throw new WebDavException("Failed to unlock resource " + uri.ToString()); ;
            }

            return success;
        }

        #endregion Unlock

        #endregion Public methods

        #region Private methods

        private static WebDavClient CreateWebDavClient(HttpMessageHandler messageHandler)
        {
            return new WebDavClient(messageHandler, false);
        }

        private LockToken GetAffectedLockToken(Uri uri)
        {
            uri = UriHelper.GetCombinedUriWithTrailingSlash(this.BaseUri, uri, true, false);

            foreach (var lockItem in this.permanentLocks)
            {
                var lockUrl = lockItem.Key.ToString();
                var testUrl = uri.ToString();

                if (testUrl.StartsWith(lockUrl))
                    return lockItem.Value.LockToken;
            }

            return null;
        }

        #endregion Private methods

        #region Dispose

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

                // Unlock all active locks.
                if (this.permanentLocks != null)
                {
                    foreach (var pLock in this.permanentLocks)
                    {
                        pLock.Value.Dispose();
                    }
                }

                if (this.webDavClient != null)
                {
                    this.webDavClient.Dispose();
                }
            }

            // Free any unmanaged objects here.

            disposed = true;
        }

        #endregion Dispose
    }
}
