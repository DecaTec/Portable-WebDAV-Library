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
        /// <param name="expectFile">True, if the function should expect a file at the end of the URI. False if these should be no distinction between files and folders containing a dot in their name.</param>
        /// <returns>The <see cref="Uri"/> with a trailing slash (only if needed).</returns>
        public static Uri AddTrailingSlash(Uri uri, bool expectFile = false)
        {
            return new Uri(AddTrailingSlash(uri.ToString(), expectFile), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        ///  Adds a trailing slash to a URL (only if needed).
        /// </summary>
        /// <param name="url">The URL to add the trailing slash when needed.</param>
        /// <param name="expectFile">True, if the function should expect a file at the end of the URL. False if these should be no distinction between files and folders containing a dot in their name.</param>
        /// <returns>The URL with a trailing slash (only if needed).</returns>
        public static string AddTrailingSlash(string url, bool expectFile = false)
        {
            var startsWithSlash = url.StartsWith("/");
            var slashSplit = url.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
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

            if (expectFile && slashSplit.Last().Contains("."))
                return url; // It's a file.
            else
                return url + "/"; // Trailing slash not present, add it.
        }

        /// <summary>
        /// Gets a combined <see cref="Uri"/> from two URIs.
        /// </summary>
        /// <param name="uri1">The first <see cref="Uri"/>.</param>
        /// <param name="uri2">The second <see cref="Uri"/>.</param>
        /// <returns>The combined <see cref="Uri"/> from the two URIs specified.</returns>
        /// <remarks>This method does not simply combine the URIs when they are partially the same. E.g. combining the URIs https://myserver.com/webdav and /webdav/myfile.txt will result in https://myserver.com/webdav/myfile.txt.</remarks>
        public static Uri CombineUri(Uri uri1, Uri uri2)
        {
            if (uri1 == null)
                return uri2;
            else
            {
                if (uri1.IsAbsoluteUri && uri2.IsAbsoluteUri
                    && (uri1.Scheme != uri2.Scheme || uri1.Host != uri2.Host))
                    throw new ArgumentException("The absolute URIs provided do not have the same host/scheme");

                if (!uri1.IsAbsoluteUri && uri2.IsAbsoluteUri)
                    throw new ArgumentException("Cannot combine URIs because uri1 is relative URI and uri2 is absolute URI");

                if (uri1.IsAbsoluteUri && uri2.IsAbsoluteUri)
                    return new Uri(uri1, uri2);
                else if (!uri1.IsAbsoluteUri && !uri2.IsAbsoluteUri)
                {
                    return new Uri(uri1.ToString().TrimEnd('/') + "/" + uri2.ToString().TrimStart('/'), UriKind.Relative);
                }
                else
                {
                    var absolutePath1 = uri1.AbsolutePath.TrimEnd('/');
                    var absolutePath2 = uri2.ToString().TrimEnd('/');

                    if (uri2.IsAbsoluteUri)
                        absolutePath2 = uri2.AbsolutePath;

                    if (!string.IsNullOrEmpty(absolutePath2) && absolutePath1.EndsWith(absolutePath2))
                        return uri1;
                    else
                    {
                        var uri2Str = uri2.ToString().Trim('/');
                        var splitUri2 = absolutePath2.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                        if (splitUri2.Length > 0)
                        {
                            var tmp = string.Empty;
                            var absolutePath2WithoutLastPath = string.Empty;

                            for (int i = splitUri2.Length - 1; i > 0; i--)
                            {
                                tmp = splitUri2[i] + (string.IsNullOrEmpty(tmp) ? tmp : "/" + tmp);
                                var index = absolutePath2.LastIndexOf(tmp);
                                absolutePath2WithoutLastPath = absolutePath2.Remove(index);

                                if (!string.IsNullOrEmpty(absolutePath2WithoutLastPath) && absolutePath1.TrimEnd('/').EndsWith(absolutePath2WithoutLastPath.TrimEnd('/')))
                                {
                                    uri2Str = tmp;
                                }
                            }
                        }

                        UriBuilder uriBuilder = new UriBuilder(uri1);
                        var pathUri1 = uriBuilder.Path.Trim('/');
                        var pathUri2 = uri2Str;

                        if (!string.IsNullOrEmpty(pathUri2))
                            pathUri1 = pathUri1.Replace(pathUri2, string.Empty);

                        var trailingSlash = uri2.ToString().EndsWith("/") ? "/" : string.Empty;
                        pathUri2 = !string.IsNullOrEmpty(pathUri2) ? "/" + pathUri2.Trim('/') : string.Empty;
                        uriBuilder.Path = pathUri1.Trim('/') + pathUri2 + trailingSlash;
                        return uriBuilder.Uri;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a combined URL from two URLs.
        /// </summary>
        /// <param name="url1">The first URL.</param>
        /// <param name="url2">The second URL.</param>
        /// <returns>The combined URL as string.</returns>
        /// /// <remarks>This method does not simply combine the URLs when they are partially the same. E.g. combining the URLs https://myserver.com/webdav and /webdav/myfile.txt will result in https://myserver.com/webdav/myfile.txt.</remarks>
        public static string CombineUrl(string url1, string url2)
        {
            var uri1 = new Uri(url1, UriKind.RelativeOrAbsolute);
            var uri2 = new Uri(url2, UriKind.RelativeOrAbsolute);
            return CombineUri(uri1, uri2).ToString();
        }

        /// <summary>
        /// Gets a combined <see cref="Uri"/> from two URIs (absolute or relative) with a trailing slash added at the end when needed.
        /// </summary>
        /// <param name="uri1">The first <see cref="Uri"/>.</param>
        /// <param name="uri2">The second <see cref="Uri"/>.</param>
        /// <returns>A combined <see cref="Uri"/> from the two URIs specified with a trailing slash added at the end when needed.</returns>
        /// <remarks>This is a combination of the methods CombineUri and AddTrailingSlash in this class.</remarks>
        public static Uri GetCombinedUriWithTrailingSlash(Uri uri1, Uri uri2)
        {
            return AddTrailingSlash(CombineUri(uri1, uri2));
        }
    }
}
