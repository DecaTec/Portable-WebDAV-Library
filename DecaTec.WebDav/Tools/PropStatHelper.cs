using DecaTec.WebDav.WebDavArtifacts;

namespace DecaTec.WebDav.Tools
{
    /// <summary>
    /// Helper class for Propstat operations.
    /// </summary>
    public static class PropstatHelper
    {
        /// <summary>
        /// Gets the <see cref="WebDavStatusCode"/> from a <see cref="Propstat"/>'s status code.
        /// </summary>
        /// <param name="propstatStatus">The PropStat's status code as string.</param>
        /// <returns>The PropStat's status code as WebDavStatusCode.</returns>
        public static WebDavStatusCode GetWebDavStatusCodeFromPropStatStatus(string propstatStatus)
        {
            // Example for a PorpStat Status: HTTP/1.1 403 Forbidden
            var splitted = propstatStatus.Split(new char[] { ' ' });
            return (WebDavStatusCode)int.Parse(splitted[1]);
        }

        /// <summary>
        /// Determines if the Status of a <see cref="Propstat"/> signals a successful status code.
        /// </summary>
        /// <param name="propStatStatusCode">The PropStat's status code as string.</param>
        /// <returns>True, if the PropStat's Status signals a successful status code. Otherwise false.</returns>
        public static bool IsSuccessStatusCode(string propStatStatusCode)
        {
            var statusCode = GetWebDavStatusCodeFromPropStatStatus(propStatStatusCode);
            return ((int)statusCode >= 200) && ((int)statusCode <= 299);
        }
    }
}
