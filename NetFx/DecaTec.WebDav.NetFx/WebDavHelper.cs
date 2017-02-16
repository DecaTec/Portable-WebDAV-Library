using System;
using System.Collections.Generic;
using System.Linq;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Helper functions for WebDAV.
    /// </summary>
    public static partial class WebDavHelper
    {
        /// <summary>
        /// Gets a <see cref="LockToken"/> from a <see cref="WebDavResponseMessage"/>.
        /// </summary>
        /// <param name="responseMessage">The <see cref="WebDavResponseMessage"/> whose <see cref="LockToken"/> should be retrieved.</param>
        /// <returns>The <see cref="LockToken"/> of the <see cref="WebDavResponseMessage"/> or null if the WebDavResponseMessage does not contain a lock token.</returns>
        public static LockToken GetLockTokenFromWebDavResponseMessage(WebDavResponseMessage responseMessage)
        {
            // Try to get lock token from response header.
            IEnumerable<string> lockTokenHeaderValues;
            if (responseMessage.Headers.TryGetValues(WebDavRequestHeader.LockTocken, out lockTokenHeaderValues))
            {
                var lockTokenHeaderValue = lockTokenHeaderValues.FirstOrDefault();
                if (lockTokenHeaderValue != null)
                    return new LockToken(lockTokenHeaderValue);
            }

            // If lock token was not submitted by response header, it should be found in the response content.
            try
            {
                var prop = WebDavResponseContentParser.ParsePropResponseContentAsync(responseMessage.Content).Result;
                return new LockToken(prop.LockDiscovery.ActiveLock[0].LockToken.Href);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
