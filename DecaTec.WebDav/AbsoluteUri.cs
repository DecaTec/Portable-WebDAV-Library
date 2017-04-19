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
        /// <param name="absoluteUri">The <see cref="Uri"/> to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="absoluteUri"/> is null.</exception>
        /// <remarks>Use the <see cref="TryParse"/> factory method to correctly construct an <see cref="AbsoluteUri"/>.</remarks>
        internal AbsoluteUri(Uri absoluteUri)
        {
            if (absoluteUri == null || !absoluteUri.IsAbsoluteUri)
                throw new ArgumentException($"Cannot create AbsoluteUri from Uri '{absoluteUri}'");

            this.absoluteUri = absoluteUri;
        }

        /// <summary>
        /// Gets the string representation of this AbsoluteUri.
        /// </summary>
        /// <returns>The string representation of this AbsoluteUri.</returns>
        public override string ToString() => absoluteUri.ToString();

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
