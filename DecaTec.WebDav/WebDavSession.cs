using DecaTec.WebDav.Exceptions;
using DecaTec.WebDav.Headers;
using DecaTec.WebDav.Tools;
using DecaTec.WebDav.WebDavArtifacts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
    /// <para>The WebDavSession uses HTTP/2 by default. To use other HTTP versions, specify the HTTP version to use in an overloaded constructor of WebDavSession or set the HTTP version with the property <see cref="HttpVersion"/>.</para>
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
        /// <summary>
        /// The <see cref="WebDavClient"/> to use for this WebDavSession.
        /// </summary>
        protected readonly WebDavClient webDavClient;

        private readonly ConcurrentDictionary<Uri, PermanentLock> permanentLocks;

        #region Constructor

        /// <summary>
        /// Creates a new instance of WebDavSession.
        /// </summary>
        protected WebDavSession()
        {

        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with a default <see cref="HttpClientHandler"/>.
        /// </summary>
        /// <param name="credentials">The <see cref="ICredentials"/> to use.</param>
        public WebDavSession(ICredentials credentials)
            : this(new HttpClientHandler() { PreAuthenticate = true, Credentials = credentials })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with a default <see cref="HttpClientHandler"/> and the HTTP version specified.
        /// </summary>
        /// <param name="credentials">The <see cref="ICredentials"/> to use.</param>
        /// <param name="httpVersion">The HTTP <see cref="Version"/> to use for requests.</param>
        public WebDavSession(ICredentials credentials, Version httpVersion)
            : this(credentials)
        {
            this.webDavClient.HttpVersion = httpVersion;
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URL and a default <see cref="HttpClientHandler"/>.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="credentials">The <see cref="ICredentials"/> to use.</param>
        public WebDavSession(string baseUrl, ICredentials credentials)
            : this(new Uri(baseUrl), new HttpClientHandler() { PreAuthenticate = true, Credentials = credentials })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URL and a default <see cref="HttpClientHandler"/> and the HTTP version specified.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="credentials">The <see cref="ICredentials"/> to use.</param>
        /// <param name="httpVersion">The HTTP <see cref="Version"/> to use for requests.</param>
        public WebDavSession(string baseUrl, ICredentials credentials, Version httpVersion)
            : this(baseUrl, credentials)
        {
            this.webDavClient.HttpVersion = httpVersion;
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base <see cref="Uri"/> and a default <see cref="HttpClientHandler"/>.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> to use for this WebDavSession.</param>
        /// <param name="credentials">The <see cref="ICredentials"/> to use.</param>
        public WebDavSession(Uri baseUri, ICredentials credentials)
            : this(baseUri, new HttpClientHandler() { PreAuthenticate = true, Credentials = credentials })
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base <see cref="Uri"/> and a default <see cref="HttpClientHandler"/> and the HTTP version specified.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> to use for this WebDavSession.</param>
        /// <param name="credentials">The <see cref="ICredentials"/> to use.</param>
        /// <param name="httpVersion">The HTTP <see cref="Version"/> to use for requests.</param>
        public WebDavSession(Uri baseUri, ICredentials credentials, Version httpVersion)
            : this(baseUri, credentials)
        {
            this.webDavClient.HttpVersion = httpVersion;
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
        /// Initializes a new instance of WebDavSession with the <see cref="HttpMessageHandler"/> specified and the HTTP version specified.
        /// </summary>
        /// <param name="httpMessageHandler">The <see cref="HttpMessageHandler"/> to use with this WebDavSession.</param>
        /// <param name="httpVersion">The HTTP <see cref="Version"/> to use for requests.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(HttpMessageHandler httpMessageHandler, Version httpVersion)
            : this(httpMessageHandler)
        {
            this.webDavClient.HttpVersion = httpVersion;
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
        /// Initializes a new instance of WebDavSession with the given base URL and the <see cref="HttpMessageHandler"/> specified and the HTTP version specified.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="httpMessageHandler">The <see cref="HttpMessageHandler"/> to use with this WebDavSession.</param>
        /// <param name="httpVersion">The HTTP <see cref="Version"/> to use for requests.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(string baseUrl, HttpMessageHandler httpMessageHandler, Version httpVersion)
            : this(baseUrl, httpMessageHandler)
        {
            this.webDavClient.HttpVersion = httpVersion;
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base <see cref="Uri"/> and the <see cref="HttpMessageHandler"/> specified.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> to use for this WebDavSession.</param>
        /// <param name="httpMessageHandler">The <see cref="HttpMessageHandler"/> to use with this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(Uri baseUri, HttpMessageHandler httpMessageHandler)
            : this(baseUri, httpMessageHandler, WebDavClient.DefaultHttpVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base <see cref="Uri"/> and the <see cref="HttpMessageHandler"/> specified and the HTTP version specified.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> to use for this WebDavSession.</param>
        /// <param name="httpMessageHandler">The <see cref="HttpMessageHandler"/> to use with this WebDavSession.</param>
        /// <param name="httpVersion">The HTTP <see cref="Version"/> to use for requests.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        public WebDavSession(Uri baseUri, HttpMessageHandler httpMessageHandler, Version httpVersion)
        {
            this.permanentLocks = new ConcurrentDictionary<Uri, PermanentLock>();
            this.webDavClient = CreateWebDavClient(httpMessageHandler);
            this.webDavClient.HttpVersion = httpVersion;
            this.BaseUri = baseUri;
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URL and <see cref="WebDavClient"/> specified.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="webDavClient">The base <see cref="WebDavClient"/> to use for this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        protected WebDavSession(string baseUrl, WebDavClient webDavClient)
            : this(string.IsNullOrEmpty(baseUrl) ? null : new Uri(baseUrl), webDavClient, WebDavClient.DefaultHttpVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base URL and <see cref="WebDavClient"/> specified and the HTTP version specified.
        /// </summary>
        /// <param name="baseUrl">The base URL to use for this WebDavSession.</param>
        /// <param name="webDavClient">The base <see cref="WebDavClient"/> to use for this WebDavSession.</param>
        /// <param name="httpVersion">The HTTP <see cref="Version"/> to use for requests.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        protected WebDavSession(string baseUrl, WebDavClient webDavClient, Version httpVersion)
            : this(string.IsNullOrEmpty(baseUrl) ? null : new Uri(baseUrl), webDavClient, httpVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base <see cref="Uri"/> and <see cref="WebDavClient"/> specified.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> to use for this WebDavSession.</param>
        /// <param name="webDavClient">The base <see cref="WebDavClient"/> to use for this WebDavSession.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        protected WebDavSession(Uri baseUri, WebDavClient webDavClient)
            : this(baseUri, webDavClient, WebDavClient.DefaultHttpVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavSession with the given base <see cref="Uri"/> and <see cref="WebDavClient"/> specified and the HTTP version specified.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/> to use for this WebDavSession.</param>
        /// <param name="webDavClient">The base <see cref="WebDavClient"/> to use for this WebDavSession.</param>
        /// <param name="httpVersion">The HTTP <see cref="Version"/> to use for requests.</param>
        /// <remarks>If credentials are needed, these are part of the <see cref="HttpMessageHandler"/> and are specified with it.</remarks>
        protected WebDavSession(Uri baseUri, WebDavClient webDavClient, Version httpVersion)
        {
            this.webDavClient = webDavClient;
            this.webDavClient.HttpVersion = httpVersion;
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
        /// Gets or sets the BaseUri of this WebDavSession by a URL string.
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return this.BaseUri == null ? string.Empty : this.BaseUri.ToString();
            }
            set
            {
                this.BaseUri = new Uri(value);
            }
        }

        /// <summary>
        /// Gets or sets the timespan to wait before the request times out.
        /// </summary>
        /// <remarks>The default value is 100,000 milliseconds (100 seconds).
        /// To set an infinite timeout, set the property value to <see cref="Timeout.InfiniteTimeSpan"/>.</remarks>
        public TimeSpan Timeout
        {
            get
            {
                return this.webDavClient.Timeout;
            }
            set
            {
                this.webDavClient.Timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the HTTP version the WebDavClient should use for its requests. Defaults to HTTP/2.
        /// </summary>
        /// <remarks>Even when HTTP/2 is specified as HTTP version, there will be a fall back to HTTP/1.1 when the server does not support HTTP/2.</remarks>
        public Version HttpVersion
        {
            get
            {
                return this.webDavClient.HttpVersion;
            }
            set
            {
                this.webDavClient.HttpVersion = value;
            }
        }

        /// <summary>
        /// Gets the headers which should be sent with every request of this WebDavSession.
        /// </summary>
        public HttpRequestHeaders DefaultRequestHeaders
        {
            get
            {
                return this.webDavClient.DefaultRequestHeaders;
            }
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
            return await CopyAsync(UriHelper.CreateUriFromUrl(sourceUrl), UriHelper.CreateUriFromUrl(destinationUrl), false);
        }

        /// <summary>
        /// Copies a resource specified as <see cref="WebDavSessionItem"/> to the destination URL (without overwriting).
        /// </summary>
        /// <param name="itemToCopy">The <see cref="WebDavSessionItem"/> to copy.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CopyAsync(WebDavSessionItem itemToCopy, string destinationUrl)
        {
            return await CopyAsync(itemToCopy, UriHelper.CreateUriFromUrl(destinationUrl), false);
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
        /// Copies a resource specified as <see cref="WebDavSessionItem"/> to the destination URL (without overwriting).
        /// </summary>
        /// <param name="itemToCopy">The <see cref="WebDavSessionItem"/> to copy.</param>
        /// <param name="destinationUri">The destination <see cref="Uri"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CopyAsync(WebDavSessionItem itemToCopy, Uri destinationUri)
        {
            return await CopyAsync(itemToCopy, destinationUri, false);
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
            return await CopyAsync(UriHelper.CreateUriFromUrl(sourceUrl), UriHelper.CreateUriFromUrl(destinationUrl), overwrite);
        }

        /// <summary>
        /// Copies a resource specified as <see cref="WebDavSessionItem"/> to the destination URL.
        /// </summary>
        /// <param name="itemToCopy">The <see cref="WebDavSessionItem"/> to copy.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CopyAsync(WebDavSessionItem itemToCopy, string destinationUrl, bool overwrite)
        {
            return await CopyAsync(itemToCopy, UriHelper.CreateUriFromUrl(destinationUrl), overwrite);
        }

        /// <summary>
        /// Copies a resource specified as <see cref="WebDavSessionItem"/> to the destination <see cref="Uri"/>.
        /// </summary>
        /// <param name="itemToCopy">The <see cref="WebDavSessionItem"/> to copy.</param>
        /// <param name="destinationUri">The destination <see cref="Uri"/>.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CopyAsync(WebDavSessionItem itemToCopy, Uri destinationUri, bool overwrite)
        {
            return await CopyAsync(UriHelper.CombineUri(this.BaseUri, itemToCopy.Uri, true), destinationUri, overwrite);
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
            sourceUri = UriHelper.CombineUri(this.BaseUri, sourceUri, true);
            destinationUri = UriHelper.CombineUri(this.BaseUri, destinationUri, true);
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
            return await CreateDirectoryAsync(UriHelper.CreateUriFromUrl(url));
        }

        /// <summary>
        /// Creates a directory at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the directory to create.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CreateDirectoryAsync(Uri uri)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
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
            return await DeleteAsync(UriHelper.CreateUriFromUrl(url));
        }

        /// <summary>
        /// Deletes a directory or file specified by a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="itemToDelete">The <see cref="WebDavSessionItem"/> to delete.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DeleteAsync(WebDavSessionItem itemToDelete)
        {
            return await DeleteAsync(UriHelper.CombineUri(this.BaseUri, itemToDelete.Uri, true));
        }

        /// <summary>
        /// Deletes a directory or file at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the directory or file to delete.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DeleteAsync(Uri uri)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
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
            return await DownloadFileAsync(UriHelper.CreateUriFromUrl(url), localStream);
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
        ///  Downloads a file specified by a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="itemToDownload">The <see cref="WebDavSessionItem"/> to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the file to.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileAsync(WebDavSessionItem itemToDownload, Stream localStream)
        {
            return await DownloadFileAsync(itemToDownload, localStream, CancellationToken.None);
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
            return await DownloadFileAsync(UriHelper.CreateUriFromUrl(url), localStream, ct);
        }

        /// <summary>
        ///  Downloads a file specified by a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="itemToDownload">The <see cref="WebDavSessionItem"/> to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the file to.</param>
        /// <param name="ct">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileAsync(WebDavSessionItem itemToDownload, Stream localStream, CancellationToken ct)
        {
            return await DownloadFileAsync(UriHelper.CombineUri(this.BaseUri, itemToDownload.Uri, true), localStream, ct);
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
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
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
        /// <param name="localStream">The <see cref="Stream"/> to save the downloaded file to.</param> 
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileWithProgressAsync(string url, Stream localStream, IProgress<WebDavProgress> progress)
        {
            return await DownloadFileWithProgressAsync(url, localStream, progress, CancellationToken.None);
        }

        /// <summary>
        /// Downloads a file (with progress) specified by a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="itemToDownload">The <see cref="WebDavSessionItem"/> to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the downloaded file to.</param> 
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileWithProgressAsync(WebDavSessionItem itemToDownload, Stream localStream, IProgress<WebDavProgress> progress)
        {
            return await DownloadFileWithProgressAsync(UriHelper.CombineUri(this.BaseUri, itemToDownload.Uri, true), localStream, progress, CancellationToken.None);
        }

        /// <summary>
        /// Downloads a file (with progress) from the given URL.
        /// </summary>
        /// <param name="url">Te URL of the file to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the downloaded file to.</param> 
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileWithProgressAsync(string url, Stream localStream, IProgress<WebDavProgress> progress, CancellationToken cancellationToken)
        {
            return await DownloadFileWithProgressAsync(UriHelper.CreateUriFromUrl(url), localStream, progress, cancellationToken);
        }

        /// <summary>
        /// Downloads a file (with progress) specified by a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="itemToDownload">The <see cref="WebDavSessionItem"/> to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the downloaded file to.</param> 
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileWithProgressAsync(WebDavSessionItem itemToDownload, Stream localStream, IProgress<WebDavProgress> progress, CancellationToken cancellationToken)
        {
            return await DownloadFileWithProgressAsync(UriHelper.CombineUri(this.BaseUri, itemToDownload.Uri, true), localStream, progress, cancellationToken);
        }

        /// <summary>
        /// Downloads a file (with progress) from the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">Te <see cref="Uri"/> of the file to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the downloaded file to.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileWithProgressAsync(Uri uri, Stream localStream, IProgress<WebDavProgress> progress)
        {
            return await DownloadFileWithProgressAsync(uri, localStream, progress, CancellationToken.None);
        }

        /// <summary>
        /// Downloads a file (with progress) from the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">Te <see cref="Uri"/> of the file to download.</param>
        /// <param name="localStream">The <see cref="Stream"/> to save the downloaded file to.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> DownloadFileWithProgressAsync(Uri uri, Stream localStream, IProgress<WebDavProgress> progress, CancellationToken cancellationToken)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
            var response = await this.webDavClient.DownloadFileWithProgressAsync(uri, localStream, cancellationToken, progress);
            return response.IsSuccessStatusCode;
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
            return await ExistsAsync(UriHelper.CreateUriFromUrl(url));
        }

        /// <summary>
        /// Checks if a file or directory exists at the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to check.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> ExistsAsync(Uri uri)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
            var response = await this.webDavClient.HeadAsync(uri);
            return response.IsSuccessStatusCode;
        }

        #endregion Exists

        #region GetSupportedPropNames

        /// <summary>
        /// Gets the WebDAV property names which are supported by the WebDAV server for the given <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="WebDavSessionItem"/> to get the supported property names for.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>Not all WebDAV servers return all known property names upon such a request.</remarks>
        public async Task<string[]> GetSupportedPropertyNamesAsync(WebDavSessionItem item)
        {
            return await GetSupportedPropertyNamesAsync(UriHelper.CombineUri(this.BaseUri, item.Uri, true));
        }

        /// <summary>
        /// Gets the WebDAV property names which are supported by the WebDAV server for the element at the given URL.
        /// </summary>
        /// <param name="url">The URL to get the supported property names for.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>Not all WebDAV servers return all known property names upon such a request.</remarks>
        public async Task<string[]> GetSupportedPropertyNamesAsync(string url)
        {
            return await GetSupportedPropertyNamesAsync(UriHelper.CreateUriFromUrl(url));
        }

        /// <summary>
        /// Gets the WebDAV property names which are supported by the WebDAV server for the element at the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to get the supported property names for.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>Not all WebDAV servers return all known property names upon such a request.</remarks>
        public async Task<string[]> GetSupportedPropertyNamesAsync(Uri uri)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
            var propFind = PropFind.CreatePropFindWithPropName();
            var response = await this.webDavClient.PropFindAsync(uri, WebDavDepthHeaderValue.Zero, propFind);
            var propertyNames = await WebDavHelper.GetPropertyNamesKnownAndUnknownFromMultiStatusContentAsync(response.Content);

            return propertyNames;
        }

        #endregion GetSupportedPropNames

        #region List

        /// <summary>
        /// Retrieves a list of files and directories of the directory at the <see cref="Uri"/> specified (using 'allprop').
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the directory which content should be listed. Has to be an absolute URI (including the base URI) or a relative URI (relative to base URI).</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>This method uses a so called 'allprop'. A server should return all known properties to the server.
        /// If not all of the expected properties are return by the server, use an overload of this method specifying a <see cref="PropFind"/> explicitly.</remarks>
        public async Task<IList<WebDavSessionItem>> ListAsync(Uri uri)
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
        public async Task<IList<WebDavSessionItem>> ListAsync(string url)
        {
            return await ListAsync(UriHelper.CreateUriFromUrl(url));
        }

        /// <summary>
        /// Retrieves a list of files and directories of the directory from the <see cref="WebDavSessionItem"/> specified
        /// </summary>
        /// <param name="item">The <see cref="WebDavSessionItem"/> which content should be listed.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>This method uses a so called 'allprop'. A server should return all known properties to the server.
        /// If not all of the expected properties are return by the server, use an overload of this method specifying a <see cref="PropFind"/> explicitly.</remarks>
        public async Task<IList<WebDavSessionItem>> ListAsync(WebDavSessionItem item)
        {
            return await ListAsync(UriHelper.CombineUri(this.BaseUri, item.Uri, true));
        }

        /// <summary>
        /// Retrieves a list of files and directories of the directory at the URL specified.
        /// </summary>
        /// <param name="url">The URL of the directory which content should be listed. Has to be an absolute URL (including the base URL) or a relative URL (relative to base URL).</param>
        /// <param name="propFind">The <see cref="PropFind"/> to use. Different PropFind  types can be created using the static methods of the class <see cref="PropFind"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IList<WebDavSessionItem>> ListAsync(string url, PropFind propFind)
        {
            return await ListAsync(UriHelper.CreateUriFromUrl(url), propFind);
        }

        /// <summary>
        /// Retrieves a list of files and directories of the directory from the <see cref="WebDavSessionItem"/> specified using the <see cref="PropFind"/> specified.
        /// </summary>
        /// <param name="item">The <see cref="WebDavSessionItem"/> which content should be listed. Has to be an absolute URI (including the base URI) or a relative URI (relative to base URI).</param>
        /// <param name="propFind">The <see cref="PropFind"/> to use. Different PropFind  types can be created using the static methods of the class <see cref="PropFind"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IList<WebDavSessionItem>> ListAsync(WebDavSessionItem item, PropFind propFind)
        {
            return await ListAsync(UriHelper.CombineUri(this.BaseUri, item.Uri, true), propFind);
        }

        /// <summary>
        /// Retrieves a list of files and directories of the directory at the <see cref="Uri"/> specified using the <see cref="PropFind"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the directory which content should be listed. Has to be an absolute URI (including the base URI) or a relative URI (relative to base URI).</param>
        /// <param name="propFind">The <see cref="PropFind"/> to use. Different PropFind  types can be created using the static methods of the class <see cref="PropFind"/>.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IList<WebDavSessionItem>> ListAsync(Uri uri, PropFind propFind)
        {
            if (propFind == null)
                throw new ArgumentException("Argument propFind must not be null.");

            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
            var response = await this.webDavClient.PropFindAsync(uri, WebDavDepthHeaderValue.One, propFind);

            // Remember the original port to include it in the hrefs later.
            var port = UriHelper.GetPort(uri);

            if (response.StatusCode != WebDavStatusCode.MultiStatus)
                throw new WebDavException($"Error while executing ListAsync (wrong response status code). Expected status code: 207 (MultiStatus); actual status code: {(int)response.StatusCode} ({response.StatusCode})");

            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

            var itemList = new List<WebDavSessionItem>();

            foreach (var responseItem in multistatus.Response)
            {
                Uri webDavSessionItemUri = null;
                string webDavSessionItemName = string.Empty;
                string webDavSessionItemDisplayName = string.Empty;
                DateTime? webDavSessionItemCreationDate = null;
                string webDavSessionItemContentLanguage = string.Empty;
                long? webDavSessionItemContentLength = null;
                string webDavSessionItemContentType = string.Empty;
                string webDavSessionItemETag = string.Empty;
                DateTime? webDavSessionItemLastModified = null;
                long? webDavSessionItemQuotaAvailableBytes = null;
                long? webDavSessionItemQuotaUsedBytes = null;
                long? webDavSessionItemChildCount = null;
                string webDavSessionItemDefaultDocument = string.Empty;
                string webDavSessionItemId = string.Empty;
                bool? webDavSessionItemIsStructuredDocument = null;
                bool? webDavSessionItemHasSubDirectories = null;
                bool? webDavSessionItemNoSubDirectoriesAllowed = null;
                long? webDavSessionItemFileCount = null;
                bool? webDavSessionItemIsReserved = null;
                long? webDavSessionItemVisibleFiles = null;
                string webDavSessionItemContentClass = string.Empty;
                bool? webDavSessionItemIsReadonly = null;
                bool? webDavSessionItemIsRoot = null;
                DateTime? webDavSessionItemLastAccessed = null;
                string webDavSessionItemParentName = string.Empty;
                bool? webDavSessionItemIsFolder = null;
                XElement[] webDavSessionItemAdditionalProperties = null;

                Uri href = null;

                if (!string.IsNullOrEmpty(responseItem.Href))
                {
                    if (UriHelper.TryCreateUriFromUrl(responseItem.Href, out href))
                    {
                        var fullQualifiedUri = UriHelper.CombineUri(uri, href, true);
                        fullQualifiedUri = UriHelper.SetPort(fullQualifiedUri, port);
                        webDavSessionItemUri = fullQualifiedUri;
                    }
                }

                // Skip the folder which contents were requested, only add children.
                if (href != null && WebUtility.UrlDecode(UriHelper.RemovePort(uri).ToString().Trim('/')).EndsWith(WebUtility.UrlDecode(UriHelper.RemovePort(href).ToString().Trim('/')), StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (var item in responseItem.Items)
                {
                    var propStat = item as Propstat;

                    // Do not items where no properties could be found.
                    if (propStat == null || !PropstatHelper.IsSuccessStatusCode(propStat.Status))
                        continue;

                    var prop = propStat.Prop;

                    // Do not add hidden items.
                    if (prop.IsHidden ?? false)
                        continue;

                    webDavSessionItemDisplayName = prop.DisplayName;
                    webDavSessionItemCreationDate = prop.CreationDate;
                    webDavSessionItemContentLanguage = prop.GetContentLanguage;
                    webDavSessionItemContentLength = prop.GetContentLength;
                    webDavSessionItemContentType = prop.GetContentType;
                    webDavSessionItemETag = prop.GetEtag;
                    webDavSessionItemLastModified = prop.GetLastModified;

                    // RFC 4331
                    webDavSessionItemQuotaAvailableBytes = prop.QuotaAvailableBytes;
                    webDavSessionItemQuotaUsedBytes = prop.QuotaUsedBytes;

                    // Additional WebDAV Collection Properties
                    webDavSessionItemChildCount = prop.ChildCount;
                    webDavSessionItemDefaultDocument = prop.DefaultDocument;
                    webDavSessionItemId = prop.Id;
                    webDavSessionItemIsStructuredDocument = prop.IsStructuredDocument;
                    webDavSessionItemHasSubDirectories = prop.HasSubs;
                    webDavSessionItemNoSubDirectoriesAllowed = prop.NoSubs;
                    webDavSessionItemFileCount = prop.ObjectCount;
                    webDavSessionItemIsReserved = prop.Reserved;
                    webDavSessionItemVisibleFiles = prop.VisibleCount;

                    // IIS specific properties
                    webDavSessionItemContentClass = prop.ContentClass;
                    webDavSessionItemIsReadonly = prop.IsReadonly;
                    webDavSessionItemIsRoot = prop.IsRoot;
                    webDavSessionItemLastAccessed = prop.LastAccessed;
                    webDavSessionItemParentName = prop.ParentName;

                    // Additional/unknown properties.
                    webDavSessionItemAdditionalProperties = prop.AdditionalProperties;

                    // Make sure that the IsDirectory property is set if it's a directory.
                    if (prop.IsFolder.HasValue && prop.IsFolder.Value)
                        webDavSessionItemIsFolder = prop.IsFolder.Value;
                    else if (prop.ResourceType != null && prop.ResourceType.Collection != null)
                        webDavSessionItemIsFolder = true;

                    // Make sure that the name property is set.
                    // Naming priority:
                    // 1. displayname (only if it doesn't contain raw unicode, otherwise there are problems with non western characters)
                    // 2. name
                    // 3. (part of) URI.
                    if (!TextHelper.StringContainsRawUnicode(prop.DisplayName))
                        webDavSessionItemName = prop.DisplayName;

                    if (string.IsNullOrEmpty(webDavSessionItemName))
                        webDavSessionItemName = prop.Name;

                    if (string.IsNullOrEmpty(webDavSessionItemName) && href != null)
                        webDavSessionItemName = WebUtility.UrlDecode(href.ToString().Split('/').Last(x => !string.IsNullOrEmpty(x)));
                }

                var webDavSessionItem = new WebDavSessionItem(webDavSessionItemUri, webDavSessionItemCreationDate, webDavSessionItemDisplayName, webDavSessionItemContentLanguage, webDavSessionItemContentLength,
                    webDavSessionItemContentType, webDavSessionItemETag, webDavSessionItemLastModified, webDavSessionItemQuotaAvailableBytes, webDavSessionItemQuotaUsedBytes, webDavSessionItemChildCount,
                    webDavSessionItemDefaultDocument, webDavSessionItemId, webDavSessionItemIsFolder, webDavSessionItemIsStructuredDocument, webDavSessionItemHasSubDirectories, webDavSessionItemNoSubDirectoriesAllowed,
                    webDavSessionItemFileCount, webDavSessionItemIsReserved, webDavSessionItemVisibleFiles, webDavSessionItemContentClass, webDavSessionItemIsReadonly, webDavSessionItemIsRoot,
                    webDavSessionItemLastAccessed, webDavSessionItemName, webDavSessionItemParentName, webDavSessionItemAdditionalProperties);

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
            return await LockAsync(UriHelper.CreateUriFromUrl(url));
        }

        /// <summary>
        ///  Locks a file or directory specified as <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="itemToLock">The <see cref="WebDavSessionItem"/> to lock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> LockAsync(WebDavSessionItem itemToLock)
        {
            return await LockAsync(UriHelper.CombineUri(this.BaseUri, itemToLock.Uri, true));
        }

        /// <summary>
        ///  Locks a file or directory at the URL specified.
        /// </summary>
        /// <param name="uri">The URI of the file or directory to lock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> LockAsync(Uri uri)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);

            if (this.permanentLocks.ContainsKey(uri))
                return true; // Lock already set.

            var lockInfo = new LockInfo()
            {
                LockScope = LockScope.CreateExclusiveLockScope(),
                LockType = LockType.CreateWriteLockType()
            };

            var response = await this.webDavClient.LockAsync(uri, WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(), WebDavDepthHeaderValue.Infinity, lockInfo);

            if (!response.IsSuccessStatusCode)
                return false; // Lock already exists.

            // Save the content stream as string as it will be consumed while getting the lock token.
            var responseContentString = await response.Content.ReadAsStringAsync();
            var lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);

            var prop = WebDavResponseContentParser.ParsePropResponseContentString(responseContentString);
            var lockDiscovery = prop.LockDiscovery;

            if (lockDiscovery == null)
                return false;

            var url = uri.ToString();
            var lockGranted = lockDiscovery.ActiveLock.FirstOrDefault(x => UriHelper.AddTrailingSlash(UriHelper.RemovePort(url), false).EndsWith(UriHelper.AddTrailingSlash(x.LockRoot.Href, false), StringComparison.OrdinalIgnoreCase));

            if (lockGranted == null)
            {
                // Try with file expected.
                lockGranted = lockDiscovery.ActiveLock.FirstOrDefault(x => UriHelper.AddTrailingSlash(url, true).EndsWith(UriHelper.AddTrailingSlash(x.LockRoot.Href, true), StringComparison.OrdinalIgnoreCase));
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
            return await MoveAsync(UriHelper.CreateUriFromUrl(sourceUrl), UriHelper.CreateUriFromUrl(destinationUrl), false);
        }

        /// <summary>
        /// Moves a file or directory specified by a <see cref="WebDavSessionItem"/> to another URI (without overwrite).
        /// </summary>
        /// <param name="itemToMove">The <see cref="WebDavSessionItem"/> to move.</param>
        /// <param name="destinationUrl">The URL of the destination.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> MoveAsync(WebDavSessionItem itemToMove, string destinationUrl)
        {
            return await MoveAsync(itemToMove, destinationUrl);
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
        /// Moves a file or directory specified by a <see cref="WebDavSessionItem"/> to another URI (without overwrite).
        /// </summary>
        /// <param name="itemToMove">The <see cref="WebDavSessionItem"/> to move.</param>
        /// <param name="destinationUri">The URL of the destination.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> MoveAsync(WebDavSessionItem itemToMove, Uri destinationUri)
        {
            return await MoveAsync(itemToMove, destinationUri, false);
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
            return await MoveAsync(UriHelper.CreateUriFromUrl(sourceUrl), UriHelper.CreateUriFromUrl(destinationUrl), overwrite);
        }

        /// <summary>
        /// Moves a file or directory specified by a <see cref="WebDavSessionItem"/> to another URI.
        /// </summary>
        /// <param name="itemToMove">The <see cref="WebDavSessionItem"/> to move.</param>
        /// <param name="destinationUrl">The URL of the destination.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> MoveAsync(WebDavSessionItem itemToMove, string destinationUrl, bool overwrite)
        {
            return await MoveAsync(itemToMove, UriHelper.CreateUriFromUrl(destinationUrl), overwrite);
        }

        /// <summary>
        /// Moves a file or directory specified by a <see cref="WebDavSessionItem"/> to another <see cref="Uri"/>.
        /// </summary>
        /// <param name="itemToMove">The <see cref="WebDavSessionItem"/> to move.</param>
        /// <param name="destinationUri">The <see cref="Uri"/> of the destination.</param>
        /// <param name="overwrite">True, if an already existing resource should be overwritten, otherwise false.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> MoveAsync(WebDavSessionItem itemToMove, Uri destinationUri, bool overwrite)
        {
            return await MoveAsync(UriHelper.CombineUri(this.BaseUri, itemToMove.Uri, true), destinationUri, overwrite);
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
            sourceUri = UriHelper.CombineUri(this.BaseUri, sourceUri, true);
            destinationUri = UriHelper.CombineUri(this.BaseUri, destinationUri, true);
            var lockTokenSource = GetAffectedLockToken(sourceUri);
            var lockTokenDestination = GetAffectedLockToken(destinationUri);
            var response = await this.webDavClient.MoveAsync(sourceUri, destinationUri, overwrite, lockTokenSource, lockTokenDestination);
            return response.IsSuccessStatusCode;
        }

        #endregion Move

        #region UpdateItem

        /// <summary>
        /// Updates a WebDAV element using a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="WebDavSessionItem"/> representing the changes which should be updated.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks><para>This method is the PROPPATCH equivalent for <see cref="WebDavSession"/>.</para>
        /// <para>The workflow for updating a WebDAV element is as follows:</para>
        /// <list type="bullet">
        /// <item><description>Use the ListAsync methods of WebDavSession to retrieve a list of <see cref="WebDavSessionItem"/>s.</description></item>
        /// <item><description>Update the properties of the WebDavSessionItems: You can either set new values (this will update the properties on the WebDAV server later on 
        /// or set values to null or <see cref="String.Empty"/> (this will remove the properties on the WebDAV server later on).</description></item>
        /// <item><description>Now you can use the UpdateItem or UpdateItems methods of WebDavSession in order to write the updates elements back to the server.</description></item>
        /// </list>
        /// <para>Note that you cannot change all of <see cref="WebDavSessionItem"/>'s properties as some of them are readonly properties. You can only change the values for 
        /// properties which support an update/remove in terms of a PROPPATCH (as defined in RFC 4918).</para>
        /// <para>Also note that not all WebDAV servers support the same set of properties to be changed by the client. So, even if you can change a property of <see cref="WebDavSessionItem"/>, 
        /// the server may not be able to process the update or remove of that property.</para></remarks>
        public async Task<bool> UpdateItemAsync(WebDavSessionItem item)
        {
            var uri = UriHelper.CombineUri(this.BaseUri, item.Uri, true);
            var lockToken = GetAffectedLockToken(uri);
            var response = await this.webDavClient.PropPatchAsync(uri, item.ToPropertyUpdate(), lockToken);            
            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);
            var success = true;

            foreach (var msResponse in multistatus.Response)
            {                
                foreach (var msResponseItem in msResponse.Items)
                {
                    if (msResponseItem is Propstat propStat)
                    {
                        success &= PropstatHelper.IsSuccessStatusCode(propStat.Status);
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Updates multiple WebDAV elements using an array of <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="items">An array of <see cref="WebDavSessionItem"/> representing the changes which should be updated.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks><para>This method is the PROPPATCH equivalent for <see cref="WebDavSession"/>.</para>
        /// <para>The workflow for updating a WebDAV element is as follows:</para>
        /// <list type="bullet">
        /// <item><description>Use the ListAsync methods of WebDavSession to retrieve a list of <see cref="WebDavSessionItem"/>s.</description></item>
        /// <item><description>Update the properties of the WebDavSessionItems: You can either set new values (this will update the properties on the WebDAV server later on 
        /// or set values to null or <see cref="String.Empty"/> (this will remove the properties on the WebDAV server later on).</description></item>
        /// <item><description>Now you can use the UpdateItem or UpdateItems methods of WebDavSession in order to write the updates elements back to the server.</description></item>
        /// </list>
        /// <para>Note that you cannot change all of <see cref="WebDavSessionItem"/>'s properties as some of them are readonly properties. You can only change the values for 
        /// properties which support an update/remove in terms of a PROPPATCH (as defined in RFC 4918).</para>
        /// <para>Also note that not all WebDAV servers support the same set of properties to be changed by the client. So, even if you can change a property of <see cref="WebDavSessionItem"/>, 
        /// the server may not be able to process the update or remove of that property.</para></remarks>
        public async Task<bool> UpdateItemsAsync(params WebDavSessionItem[] items)
        {
            var success = false;

            foreach (var item in items)
            {
                success &= await UpdateItemAsync(item);
            }

            return success;
        }

        #endregion UpdateItem

        #region Upload file

        /// <summary>
        /// Uploads a file to the URL specified.
        /// </summary>
        /// <param name="url">The URL of the file to upload.</param>
        /// <param name="localStream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileAsync(string url, Stream localStream)
        {
            return await UploadFileAsync(UriHelper.CreateUriFromUrl(url), localStream);
        }

        /// <summary>
        /// Uploads a file to the folder specified by a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="folderToUploadTo">The folder as <see cref="WebDavSessionItem"/> to upload to.</param>
        /// <param name="fileName">The file name of the uploaded file.</param>
        /// <param name="localStream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileAsync(WebDavSessionItem folderToUploadTo, string fileName, Stream localStream)
        {
            if (!(folderToUploadTo.IsFolder ?? false))
                throw new WebDavException("The upload target is no folder.");

            var uploadUrl = UriHelper.CombineUriAndUrl(folderToUploadTo.Uri, fileName, true);
            return await UploadFileAsync(uploadUrl, localStream);
        }

        /// <summary>
        /// Uploads a file to the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file to upload.</param>
        /// <param name="localStream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileAsync(Uri uri, Stream localStream)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
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
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileWithProgressAsync(string url, Stream stream, string contentType, IProgress<WebDavProgress> progress)
        {
            return await UploadFileWithProgressAsync(url, stream, contentType, progress, CancellationToken.None);
        }

        /// <summary>
        /// Uploads a file (with progress) to the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file to upload.</param>
        /// <param name="stream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileWithProgressAsync(Uri uri, Stream stream, string contentType, IProgress<WebDavProgress> progress)
        {
            return await UploadFileWithProgressAsync(uri, stream, contentType, progress, CancellationToken.None);
        }

        /// <summary>
        /// Uploads a file (with progress) to the folder specified by a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="folderToUploadTo">The folder as <see cref="WebDavSessionItem"/> to upload to.</param>
        /// <param name="fileName">The file name of the uploaded file.</param>
        /// <param name="stream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileWithProgressAsync(WebDavSessionItem folderToUploadTo, string fileName, Stream stream, string contentType, IProgress<WebDavProgress> progress)
        {
            return await UploadFileWithProgressAsync(folderToUploadTo, fileName, stream, contentType, progress, CancellationToken.None);
        }

        /// <summary>
        /// Uploads a file (with progress) to the URL specified.
        /// </summary>
        /// <param name="url">The URL of the file to upload.</param>
        /// <param name="stream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileWithProgressAsync(string url, Stream stream, string contentType, IProgress<WebDavProgress> progress, CancellationToken cancellationToken)
        {
            return await UploadFileWithProgressAsync(UriHelper.CreateUriFromUrl(url), stream, contentType, progress, cancellationToken);
        }

        /// <summary>
        /// Uploads a file (with progress) to the folder specified by a <see cref="WebDavSessionItem"/>.
        /// </summary>
        /// <param name="folderToUploadTo">The folder as <see cref="WebDavSessionItem"/> to upload to.</param>
        /// <param name="fileName">The file name of the uploaded file.</param>
        /// <param name="stream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileWithProgressAsync(WebDavSessionItem folderToUploadTo, string fileName, Stream stream, string contentType, IProgress<WebDavProgress> progress, CancellationToken cancellationToken)
        {
            if (!(folderToUploadTo.IsFolder ?? false))
                throw new WebDavException("The upload target is no folder.");

            var uploadUrl = UriHelper.CombineUrl(folderToUploadTo.Uri.ToString(), fileName, true);
            return await UploadFileWithProgressAsync(uploadUrl, stream, contentType, progress, cancellationToken);
        }

        /// <summary>
        /// Uploads a file (with progress) to the <see cref="Uri"/> specified.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file to upload.</param>
        /// <param name="stream">The <see cref="Stream"/> containing the file to upload.</param>
        /// <param name="contentType">The content type of the file to upload.</param>
        /// <param name="progress">An object representing the progress of the operation.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UploadFileWithProgressAsync(Uri uri, Stream stream, string contentType, IProgress<WebDavProgress> progress, CancellationToken cancellationToken)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);
            var lockToken = GetAffectedLockToken(uri);
            var response = await this.webDavClient.UploadFileWithProgressAsync(uri, stream, contentType, progress, cancellationToken, lockToken);
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
            return await UnlockAsync(UriHelper.CreateUriFromUrl(url));
        }

        /// <summary>
        /// Unlocks a file or directory specified by a <see cref="WebDavSessionItem"/>. 
        /// </summary>
        /// <param name="itemToUnlock">The <see cref="WebDavSessionItem"/> to unlock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UnlockAsync(WebDavSessionItem itemToUnlock)
        {
            return await UnlockAsync(UriHelper.CombineUri(this.BaseUri, itemToUnlock.Uri, true));
        }

        /// <summary>
        /// Unlocks a file or directory at the <see cref="Uri"/> specified. 
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the file or directory to unlock.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> UnlockAsync(Uri uri)
        {
            uri = UriHelper.CombineUri(this.BaseUri, uri, true);

            if (!this.permanentLocks.TryRemove(uri, out PermanentLock permanentLock))
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
            if (this.permanentLocks == null)
                return null;

            uri = UriHelper.CombineUri(this.BaseUri, uri, true);

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