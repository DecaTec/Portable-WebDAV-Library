using System;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Represents the Absolute-URI object as specified in <see href="https://tools.ietf.org/html/rfc3986#section-4.3"/>.
    /// </summary>
    public class AbsoluteUri
    {
        /// <summary>
        /// The URI of this <see cref="AbsoluteUri"/>.
        /// </summary>
        private readonly Uri absoluteUri;

        /// <summary>
        /// Constructs an <see cref="AbsoluteUri"/>.
        /// </summary>
        /// <param name="absoluteUrl">The URL to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="absoluteUrl"/> is null.</exception>
        public AbsoluteUri(string absoluteUrl)
        {
            if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out Uri absoluteUri))
                throw new ArgumentException($"Cannot create AbsoluteUri from URL '{absoluteUrl}'");

            this.absoluteUri = absoluteUri;
        }

        /// <summary>
        /// Constructs an <see cref="AbsoluteUri"/>.
        /// </summary>
        /// <param name="absoluteUri">The <see cref="Uri"/> to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="absoluteUri"/> is null.</exception>
        public AbsoluteUri(Uri absoluteUri)
        {
            if (absoluteUri == null || !absoluteUri.IsAbsoluteUri)
                throw new ArgumentException($"Cannot create AbsoluteUri from Uri '{absoluteUri}'");

            this.absoluteUri = absoluteUri;
        }

        /// <summary>
        /// Gets the string representation of this <see cref="AbsoluteUri"/>.
        /// </summary>
        /// <returns>The string representation of this <see cref="AbsoluteUri"/>.</returns>
        /// <remarks>This ToString method returns the original string used to create this <see cref="AbsoluteUri"/>. 
        /// This is according to specification, which states "Clients must not attempt to interpret lock tokens in any way.": http://www.webdav.org/specs/rfc4918.html#lock-tokens. </remarks>
        public override string ToString() => absoluteUri.OriginalString;

        /// <summary>
        /// Tries to parse the given <paramref name="rawAbsoluteUri"/> into an <see cref="AbsoluteUri"/>.
        /// </summary>
        /// <param name="rawAbsoluteUri">The absolute-URI to parse.</param>
        /// <param name="absoluteUri">The <see cref="AbsoluteUri"/>.</param>
        /// <returns>True if the parsing succeeded, false if it did not.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rawAbsoluteUri"/> is null.</exception>
        public static bool TryParse(string rawAbsoluteUri, out AbsoluteUri absoluteUri)
        {
            if (rawAbsoluteUri == null)
                throw new ArgumentNullException(nameof(rawAbsoluteUri));

            if (Uri.TryCreate(rawAbsoluteUri, UriKind.Absolute, out var parsedUri))
            {
                absoluteUri = new AbsoluteUri(parsedUri);
                return true;
            }

            absoluteUri = null;
            return false;
        }
    }
}
