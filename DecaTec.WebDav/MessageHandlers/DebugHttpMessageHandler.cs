using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DecaTec.WebDav.MessageHandlers
{
    /// <summary>
    /// <see cref="DelegatingHandler"/> providing debug output for requests/responses (and their content) on the debug console.
    /// </summary>
    /// <remarks>This class is only for debug purposes and should not be used in a productive environment.</remarks>
    public class DebugHttpMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// Initializes a new instance of the DebugHttpMessageHandler class.
        /// </summary>
        public DebugHttpMessageHandler()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the DebugHttpMessageHandler class with a specific inner handler.
        /// </summary>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        public DebugHttpMessageHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {

        }

        /// <summary>
        /// This member overrides <see cref="HttpMessageHandler.SendAsync(HttpRequestMessage, CancellationToken)"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>to use.</param>
        /// <returns>The <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await DebugWriteRequestContent(request);
            var response = await base.SendAsync(request, cancellationToken);
            await DebugWriteResponseContent(response);
            return response;
        }

        private async Task DebugWriteRequestContent(HttpRequestMessage request)
        {
            var sb = new StringBuilder();
            sb.Append("========");
            sb.Append(Environment.NewLine);
            sb.Append("REQUEST:");
            sb.Append(Environment.NewLine);
            sb.Append(request.ToString());

            if (request.Content != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append("REQUEST CONTENT:");
                sb.Append(Environment.NewLine);
                sb.Append(await request.Content.ReadAsStringAsync());
            }

            sb.Append(Environment.NewLine);
            sb.Append("========");

            Debug.WriteLine(sb.ToString());
            sb.Clear();
        }

        private async Task DebugWriteResponseContent(HttpResponseMessage responseMessage)
        {
            if (responseMessage.Content == null)
            {
                Debug.WriteLine(GetEmptyContentString());
                return;
            }

            var contentString = await responseMessage.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(contentString))
            {
                Debug.WriteLine(GetEmptyContentString());
                return;
            }

            var sb = new StringBuilder();
            sb.Append("========");
            sb.Append(Environment.NewLine);
            sb.Append("RESPONSE CONTENT:");
            sb.Append(Environment.NewLine);
            sb.Append(contentString);
            sb.Append(Environment.NewLine);
            sb.Append("========");

            Debug.WriteLine(sb.ToString());
            sb.Clear();
        }

        private string GetEmptyContentString()
        {
            var sb = new StringBuilder();
            sb.Append("========");
            sb.Append(Environment.NewLine);
            sb.Append("RESPONSE CONTENT:");
            sb.Append(Environment.NewLine);
            sb.Append("NONE");
            sb.Append(Environment.NewLine);
            sb.Append("========");

            return sb.ToString();
        }
    }
}
