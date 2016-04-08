namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing WebDAV methods.
    /// </summary>
    public class WebDavMethod
    {
        /// <summary>
        /// WebDavMethod for PROPFIND.
        /// </summary>
        public static readonly string PropFind = "PROPFIND";

        /// <summary>
        /// WebDavMethod for PROPPATCH.
        /// </summary>
        public static readonly string PropPatch = "PROPPATCH";

        /// <summary>
        ///  WebDavMethod for MKCOL.
        /// </summary>
        public static readonly string Mkcol = "MKCOL";

        /// <summary>
        ///  WebDavMethod for COPY.
        /// </summary>
        public static readonly string Copy = "COPY";

        /// <summary>
        ///  WebDavMethod for MOVE.
        /// </summary>
        public static readonly string Move = "MOVE";

        /// <summary>
        ///  WebDavMethod for LOCK.
        /// </summary>
        public static readonly string Lock = "LOCK";

        /// <summary>
        ///  WebDavMethod for UNLOCK.
        /// </summary>
        public static readonly string Unlock = "UNLOCK";
    }
}
