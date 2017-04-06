using System;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing a WebDAV lock token. <para/>
    /// See <see href="https://tools.ietf.org/html/rfc4918#section-6.5"/> for the definition.
    /// </summary>
    public class LockToken
    {
        /// <summary>
        /// Constructs a <see cref="LockToken"/> based on the <paramref name="absoluteUri"/>.
        /// </summary>
        /// <param name="absoluteUri">The lock token in absolute-URI format as defined in https://tools.ietf.org/html/rfc3986#section-4.3. </param>
        /// <remarks>Use the strong-typed constructors to create a new <see cref="LockToken"/>.</remarks>
        /// <exception cref="WebDavException">Thrown when <paramref name="absoluteUri"/> is null.</exception>
        public LockToken(AbsoluteUri absoluteUri)
        {
            AbsoluteUri = absoluteUri ?? throw new WebDavException($"The {nameof(absoluteUri)} cannot be null.");

            var codedUrl = new CodedUrl(absoluteUri);
            LockTokenHeaderFormat = codedUrl;
            IfHeaderNoTagListFormat = new NoTagList(codedUrl);
        }

        /// <summary>
        /// Gets the absolute-URI representation of the lock token for serialization purposes. <para/>
        /// See <see href="https://tools.ietf.org/html/rfc3986#section-4.3"/> for the absolute-URI definition. <para/>
        /// </summary>
        public AbsoluteUri AbsoluteUri { get; }

        /// <summary>
        /// The Coded-URL Lock-Token header formatted version of this <see cref="LockToken"/>.
        /// </summary>
        public CodedUrl LockTokenHeaderFormat { get; }

        /// <summary>
        /// The No-Tag If header formatted version of this <see cref="LockToken"/>.
        /// </summary>
        public NoTagList IfHeaderNoTagListFormat { get; }
    }
}
