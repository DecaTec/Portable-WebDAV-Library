namespace DecaTec.WebDav
{
    /// <summary>
    /// Class defining all the Prop names defined in RFC 4918.
    /// </summary>
    public static class PropNameConstants
    {
        #region RFC 4918

        /// <summary>
        /// Prop name for 'creationdate'.
        /// </summary>
        public const string CreationDate = "creationdate";

        /// <summary>
        /// Prop name for 'getcontentlanguage'.
        /// </summary>
        public const string GetContentLanguage = "getcontentlanguage";

        /// <summary>
        /// Prop name for 'displayname'.
        /// </summary>
        public const string DisplayName = "displayname";

        /// <summary>
        /// Prop name for 'getcontentlength'.
        /// </summary>
        public const string GetContentLength = "getcontentlength";

        /// <summary>
        /// Prop name for 'getcontenttype'.
        /// </summary>
        public const string GetContentType = "getcontenttype";

        /// <summary>
        /// Prop name for 'getlastmodified'.
        /// </summary>
        public const string GetLastModified = "getlastmodified";

        /// <summary>
        /// Prop name for 'getetag'.
        /// </summary>
        public const string GetEtag = "getetag";

        /// <summary>
        /// Prop name for 'resourcetype'.
        /// </summary>
        public const string ResourceType = "resourcetype";

        /// <summary>
        /// Prop name for 'href'.
        /// </summary>
        public const string Href = "href";

        /// <summary>
        /// Prop name for 'lockdiscovery'.
        /// </summary>
        public const string LockDiscovery = "lockdiscovery";

        /// <summary>
        /// Prop name for 'supportedlock'.
        /// </summary>
        public const string SupportedLock = "supportedlock";

        #endregion RFC 4918

        #region RFC 4331

        // Properties as defined in https://tools.ietf.org/html/rfc4331

        /// <summary>
        /// Constant for 'quota-used-bytes'.
        /// </summary>
        public const string QuotaUsedBytes = "quota-used-bytes";

        /// <summary>
        /// Constant for 'quota-available-bytes'.
        /// </summary>
        public const string QuotaAvailableBytes = "quota-available-bytes";

        #endregion RFC 4331

        #region  Additional WebDAV Collection Properties

        // Properties as defined in http://www.ics.uci.edu/~ejw/authoring/props/draft-hopmann-collection-props-00.txt

        /// <summary>
        /// Constant for 'cildcount'.
        /// </summary>
        public const string ChildCount = "childcount";

        /// <summary>
        /// Prop name for 'defaultdocument'.
        /// </summary>
        public const string DefaultDocument = "defaultdocument";

        /// <summary>
        /// Prop name for 'id'.
        /// </summary>
        public const string Id = "id";

        /// <summary>
        /// Prop name for 'isfolder'.
        /// </summary>
        public const string IsFolder = "isfolder";

        /// <summary>
        /// Prop name for 'ishidden'.
        /// </summary>
        public const string IsHidden = "ishidden";

        /// <summary>
        /// Prop name for 'isstructureddocument'.
        /// </summary>
        public const string IsStructuredDocument = "isstructureddocument";

        /// <summary>
        /// Prop name for 'hassubs'.
        /// </summary>
        public const string HasSubs = "hassubs";

        /// <summary>
        /// Prop name for 'nosubs'.
        /// </summary>
        public const string NoSubs = "nosubs";

        /// <summary>
        /// Prop name for 'objectcount'.
        /// </summary>
        public const string ObjectCount = "objectcount";

        /// <summary>
        /// Prop name for 'reserved'.
        /// </summary>
        public const string Reserved = "reserved";

        /// <summary>
        /// Prop name for 'visiblecount'.
        /// </summary>
        public const string VisibleCount = "visiblecount";

        #endregion Additional WebDAV Collection Properties 

        #region IIS specific properties

        /// <summary>
        /// Prop name for 'contentclass'.
        /// </summary>
        public const string ContentClass = "contentclass";

        /// <summary>
        /// Prop name for 'isreadonly'.
        /// </summary>
        public const string IsReadonly = "isreadonly";

        /// <summary>
        /// Prop name for 'isroot'.
        /// </summary>
        public const string IsRoot = "isroot";

        /// <summary>
        /// Prop name for 'lastaccessed'.
        /// </summary>
        public const string LastAccessed = "lastaccessed";

        /// <summary>
        /// Prop name for 'name'.
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// Prop name for 'parentname'.
        /// </summary>
        public const string ParentName = "parentname";

        #endregion IIS specific properties
    }
}