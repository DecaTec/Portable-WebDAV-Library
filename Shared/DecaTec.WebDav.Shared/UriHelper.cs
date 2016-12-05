using System;
using System.Linq;
using System.Text;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Helper class for handling URIs/URLs.
    /// </summary>
    public static class UriHelper
    {
        /// <summary>
        /// Adds a trailing slash to a URI (only if needed).
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to add the trailing slash when needed.</param>
        /// <returns>The <see cref="Uri"/> with a trailing slash (only if needed).</returns>
        public static Uri AddTrailingSlash(Uri uri)
        {
            return new Uri(AddTrailingSlash(uri.ToString()), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        ///  Adds a trailing slash to a URL (only if needed).
        /// </summary>
        /// <param name="url">The URL to add the trailing slash when needed.</param>
        /// <returns>The URL with a trailing slash (only if needed).</returns>
        public static string AddTrailingSlash(string url)
        {
            var startsWithSlash = url.StartsWith("/");
            var slashSplit = url.Split(new string[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();

            for (int i = 0; i < slashSplit.Length; i++)
            {
                sb.Append(slashSplit[i]);

                if (i == 0 && slashSplit[i].Contains(":"))
                    sb.Append("//"); // First has to be a double slash (http://...).
                else if (i != slashSplit.Length - 1)
                    sb.Append("/"); // Do not add a slash at the very end, this is handled further below.
            }

            url = sb.ToString();

            if (startsWithSlash)
                url = "/" + url;

            if (slashSplit.Last().Contains("."))
                return url; // It's a file.
            else
                return url + "/"; // Trailing slash not present, add it.
        }

        /// <summary>
        /// Gets an absolute <see cref="Uri"/> from a base URI and a relative URI.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/>.</param>
        /// <param name="relativeUri">The relative <see cref="Uri"/>.</param>
        /// <returns>The combined <see cref="Uri"/> from the base and relative URI.</returns>
        public static Uri GetAbsoluteUri(Uri baseUri, Uri relativeUri)
        {
            if (baseUri == null)
                return relativeUri;
            else
            {
                if (baseUri.IsAbsoluteUri && relativeUri.IsAbsoluteUri
                    && (baseUri.Scheme != relativeUri.Scheme || baseUri.Host != relativeUri.Host))
                    throw new ArgumentException("The absolute URIs provided do not have the same host/scheme");

                return new Uri(baseUri, relativeUri);
            }
        }

        /// <summary>
        /// Gets an absolute <see cref="Uri"/> from a base URI and a relative URI with a trailing slash when needed.
        /// </summary>
        /// <param name="baseUri">The base <see cref="Uri"/>.</param>
        /// <param name="relativeUri">The relative <see cref="Uri"/>.</param>
        /// <returns>An absolute <see cref="Uri"/> from a base URI and a relative URI with a trailing slash when needed.</returns>
        /// <remarks>This is a combination of the methods GetAbsoluteUri and AddTrailingSlash in this class.</remarks>
        public static Uri GetAbsoluteUriWithTrailingSlash(Uri baseUri, Uri relativeUri)
        {
            return AddTrailingSlash(GetAbsoluteUri(baseUri, relativeUri));
        }
    }
}
