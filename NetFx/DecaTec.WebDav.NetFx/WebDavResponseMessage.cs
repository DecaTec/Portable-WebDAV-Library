using System.Net;
using System.Net.Http;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing a WebDAV response.
    /// </summary>
    public class WebDavResponseMessage : HttpResponseMessage
    {
        /// <summary>
        /// Initializes a new instance of WebDavResponseMessage.
        /// </summary>
        public WebDavResponseMessage()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of WebDavResponseMessage.
        /// </summary>
        /// <param name="statusCode">The request's HttpStatusCode.</param>
        public WebDavResponseMessage(HttpStatusCode statusCode)
            : base(statusCode)
        {

        }

        /// <summary>
        /// Initializes a new instance of WebDavResponseMessage.
        /// </summary>
        /// <param name="statusCode">The request's WebDavStatusCode.</param>
        public WebDavResponseMessage(WebDavStatusCode statusCode)
            : base((HttpStatusCode)statusCode)
        {
        }

        /// <summary>
        /// Initializes a new instance of WebDavResponseMessage.
        /// </summary>
        /// <param name="httpResponseMessage">The HttpResponseMessage the WebDavResponseMessage should be based on.</param>
        public WebDavResponseMessage(HttpResponseMessage httpResponseMessage)
            : base()
        {
            this.Content = httpResponseMessage.Content;
            this.ReasonPhrase = httpResponseMessage.ReasonPhrase;
            this.RequestMessage = httpResponseMessage.RequestMessage;
            this.StatusCode = (WebDavStatusCode)httpResponseMessage.StatusCode;
            this.Version = httpResponseMessage.Version;

            // Transfer headers.
            foreach (var header in httpResponseMessage.Headers)
            {
                this.Headers.Add(header.Key, header.Value);
            }
        }

        /// <summary>
        /// Gets or sets the WebDavStatusCode of this WebDavRespnseMessage.
        /// </summary>
        public new WebDavStatusCode StatusCode
        {
            get
            {
                return (WebDavStatusCode)base.StatusCode;
            }
            set
            {
                base.StatusCode = (HttpStatusCode)value;
            }
        }
    }
}
