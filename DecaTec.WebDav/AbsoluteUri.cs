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

        private AbsoluteUri(Uri absoluteUri)
        {
            if (absoluteUri == null)
                throw new ArgumentNullException(nameof(absoluteUri));

            this.absoluteUri = absoluteUri;
        }

        /// <inheritdoc />
        public override string ToString() =>
            absoluteUri.ToString();

        /// <summary>
        /// Tries to parse the given <paramref name="rawAbsoluteUri"/> into an <see cref="AbsoluteUri"/>.
        /// </summary>
        /// <param name="rawAbsoluteUri">The absolute-URI to parse.</param>
        /// <param name="absoluteUri">The <see cref="AbsoluteUri"/>.</param>
        /// <returns>True if the parsing succeeded, false if it did not.</returns>
        public static bool TryParse(string rawAbsoluteUri, out AbsoluteUri absoluteUri)
        {
            try
            {
                if (Uri.TryCreate(rawAbsoluteUri, UriKind.Absolute, out var parsedUri))
                {
                    absoluteUri = new AbsoluteUri(parsedUri);
                    return true;
                }
                absoluteUri = null;
                return false;
            }
            catch (UriFormatException)
            {
                absoluteUri = null;
                return false;
            }
        }
    }
}
