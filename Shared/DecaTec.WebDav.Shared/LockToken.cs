namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing a WebDAV lock token.
    /// </summary>
    public class LockToken
    {
        /// <summary>
        /// The prefix for a "List" syntax. <para/>
        /// http://www.webdav.org/specs/rfc4918.html#if.header.syntax
        /// </summary>
        private const char ListPrefix = '(';

        /// <summary>
        /// The postfix for a "List" syntax. <para/>
        /// http://www.webdav.org/specs/rfc4918.html#if.header.syntax
        /// </summary>
        private const char ListPostfix = ')';

        /// <summary>
        /// The prefix for a "Coded-URL" syntax. <para/>
        /// http://www.webdav.org/specs/rfc4918.html#HEADER_DAV
        /// </summary>
        private const char CodedUrlPrefix = '<';

        /// <summary>
        /// The postfix for a "Coded-URL" syntax. <para/>
        /// http://www.webdav.org/specs/rfc4918.html#HEADER_DAV
        /// </summary>
        private const char CodedUrlPostfix = '>';

        /// <summary>
        /// Initializes a new instance of LockToken.
        /// </summary>
        /// <param name="lockToken">A lock token string.</param>
        public LockToken(string lockToken)
        {
            if (string.IsNullOrEmpty(lockToken))
                throw new WebDavException("A lock token cannot be null or empty.");

            this.RawLockToken = lockToken;
        }

        /// <summary>
        /// Gets the raw representation of the lock token for serialization purposes.        /// 
        /// Use <see cref="ToString"/> to get the formatted representation for use in headers.
        /// </summary>
        public string RawLockToken { get; }

        /// <summary>
        /// Gets the string representation of a lock token as used in an If or Lock-Token header.
        /// </summary>
        /// <param name="format">The desired <see cref="LockTokenFormat"/>.</param>
        /// <returns>A lock token string with the desired format.</returns>
        /// <remarks>
        /// If header according to http://www.webdav.org/specs/rfc4918.html#HEADER_If <para/>
        /// Lock-Token header according to http://www.webdav.org/specs/rfc4918.html#HEADER_Lock-Token
        /// </remarks>
        public string ToString(LockTokenFormat format)
        {
            var cleanLockToken = RawLockToken.Trim(ListPrefix, ListPostfix, CodedUrlPrefix, CodedUrlPostfix);
            var codedUrl = $"{CodedUrlPrefix}{cleanLockToken}{CodedUrlPostfix}";

            switch (format)
            {
                case LockTokenFormat.IfHeader:
                    var list = $"{ListPrefix}{codedUrl}{ListPostfix}";
                    return list;
                case LockTokenFormat.LockTokenHeader:
                    return codedUrl;
                default:
                    return RawLockToken;
            }
        }
    }
}
