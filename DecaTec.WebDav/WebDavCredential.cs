using System;
using System.Net;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class for credentials of a WebDAV request.
    /// </summary>
    public class WebDavCredential : ICredentials
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of WebDavCredential with basic authentication.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        public WebDavCredential(string userName, string password)
            : this(userName, password, string.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of WebDavCredential with basic authentication.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="domain">The domain.</param>
        public WebDavCredential(string userName, string password, string domain)
            : this(userName, password, domain, null)
        {
        }

        /// <summary>
        /// Creates a new instance of WebDavCredential.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="webDavAuthenticationType">The authentication type.</param>
        public WebDavCredential(string userName, string password, string domain, WebDavAuthenticationType webDavAuthenticationType)
        {
            this.UserName = userName;
            this.Password = password;
            this.Domain = domain;
            this.WebDavAuthenticationType = webDavAuthenticationType;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public string Domain
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the WebDavAuthenticationType.
        /// </summary>
        public WebDavAuthenticationType WebDavAuthenticationType
        {
            get;
            set;
        }

        #endregion Properties

        #region Public methods

        /// <summary>
        /// Gets the NetworkCredential for a URI and authenticationType specified.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="authType">The authentication type.</param>
        /// <returns>The NetworkCredential.</returns>
        public NetworkCredential GetCredential(Uri uri, string authType)
        {
            var authenticationType = this.WebDavAuthenticationType == null ? authType : this.WebDavAuthenticationType.ToString();
            var credentialCache = new CredentialCache();
            credentialCache.Add(uri, authenticationType, new NetworkCredential(this.UserName, this.Password, this.Domain));
            return credentialCache.GetCredential(uri, authType);
        }

        #endregion Public methods
    }
}
