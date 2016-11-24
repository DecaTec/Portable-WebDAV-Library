using System;
using System.Linq;
using System.Text;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Helper class for handling URLs/URIs.
    /// </summary>
    public static class UrlHelper
    {
        /// <summary>
        /// Adds a trailing slash to a URI (only if needed).
        /// </summary>
        /// <param name="uri">The URI to add the trailing slash when needed.</param>
        /// <returns>The URI with a trailing slash (only if needed).</returns>
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
        /// Gets an absolute URI from a base URI and a relative URI.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <returns>The combined URI from the base and relative URI.</returns>
        public static Uri GetAbsoluteUri(Uri baseUri, Uri relativeUri)
        {
            if (baseUri == null)
                return relativeUri;
            else
            {
                return new Uri(baseUri, relativeUri) ;
                //var t = Uri.TryCreate($"{baseUri.ToString()}{relativeUri.ToString()}", UriKind.Absolute, out er);
                ////return new Uri(string.Concat(baseUri.Scheme, baseUri.)

                //var url1 = baseUri.ToString();
                //url1 = url1.TrimEnd('/');
                //var url2 = relativeUri.ToString();
                //url2 = url2.TrimStart('/');

                //if(url2.StartsWith(url1))
                //    url2 = url2.Substring(url1.Length).TrimStart('/');              

                //return new Uri(string.Concat(url1, @"/", url2));

            }
        }

        /// <summary>
        /// Gets an absolute URI from a base URI and a relative URI with a trailing slash when needed.
        /// </summary>
        /// <param name="baseUri">The base URI.</param>
        /// <param name="relativeUri">The relative URI.</param>
        /// <returns>An absolute URI from a base URI and a relative URI with a trailing slash when needed.</returns>
        /// <remarks>This is a combination of the methods GetAbsoluteUri and AddTrailingSlash in this class.</remarks>
        public static Uri GetAbsoluteUriWithTrailingSlash(Uri baseUri, Uri relativeUri)
        {
            return AddTrailingSlash(GetAbsoluteUri(baseUri, relativeUri));
        }
    }
}
