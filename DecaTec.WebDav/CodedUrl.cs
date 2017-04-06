using System;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Represents the Coded-URL object as specified in <see href="http://www.webdav.org/specs/rfc4918.html#rfc.section.10.1"/>.
    /// </summary>
    public class CodedUrl
    {
        /// <summary>
        /// The absolute-URI of this <see cref="CodedUrl"/>.
        /// </summary>
        public readonly AbsoluteUri AbsoluteUri;

        /// <summary>
        /// The Coded-URL prefix.
        /// </summary>
        private const char CodedUrlPrefix = '<';

        /// <summary>
        /// The Coded-URL postfix.
        /// </summary>
        private const char CodedUrlPostfix = '>';

        /// <summary>
        /// Constructs a Coded-URL based on the <paramref name="absoluteUri"/>. <para/>
        /// See <see href="http://www.webdav.org/specs/rfc4918.html#rfc.section.10.1"/> for the Coded-URL definition.
        /// </summary>
        /// <param name="absoluteUri">The lock token in absolute-URI format.</param>
        internal CodedUrl(AbsoluteUri absoluteUri)
        {
            AbsoluteUri = absoluteUri ?? throw new ArgumentNullException(nameof(absoluteUri));
        }

        /// <inheritdoc />
        public override string ToString() => $"{CodedUrlPrefix}{AbsoluteUri}{CodedUrlPostfix}";

        /// <summary>
        /// Tries to parse the given <paramref name="rawCodedUrl"/> to a <see cref="CodedUrl"/>. <para/>
        /// See <see href="http://www.webdav.org/specs/rfc4918.html#rfc.section.10.1"/> for the Coded-URL definition.
        /// </summary>
        /// <param name="rawCodedUrl">The raw coded URL to parse into the <see cref="CodedUrl"/>.</param>
        /// <param name="codedUrl">The <see cref="CodedUrl"/>.</param>
        /// <returns>The parsed <see cref="CodedUrl"/>.</returns>
        public static bool TryParse(string rawCodedUrl, out CodedUrl codedUrl)
        {
            if (rawCodedUrl == null)
                throw new ArgumentNullException(nameof(rawCodedUrl));

            if (!rawCodedUrl.StartsWith(CodedUrlPrefix.ToString()) || !rawCodedUrl.EndsWith(CodedUrlPostfix.ToString()))
            {
                codedUrl = null;
                return false;
            }

            try
            {
                if (AbsoluteUri.TryParse(rawCodedUrl.Trim(CodedUrlPrefix, CodedUrlPostfix), out var absoluteUri))
                {
                    codedUrl = new CodedUrl(absoluteUri);
                    return true;
                }
                codedUrl = null;
                return false;
            }
            catch (FormatException)
            {
                codedUrl = null;
                return false;
            }
        }
    }
}
