using System;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing a list item from a WebDavSession's list method.
    /// </summary>
    /// <remarks>Note that not all properties defined in this class are always used. Some WebDAV servers use only a subset of these properties or even provide additional properties.
    /// So, if properties provided by this class contain default values (0, false, etc.), these values maybe wrong just because the WebDAV server does not support these properties.</remarks>
    public class WebDavSessionListItem
    {
        /// <summary>
        /// Gets or sets the URI (Href) of the item.
        /// </summary>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> when the item was created.
        /// </summary>
        public DateTime CreationDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the language of the content.
        /// </summary>
        public string ContentLanguage
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the length of the content in bytes.
        /// </summary>
        public long ContentLength
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the MIME type of the content.
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ETag.
        /// </summary>
        public string ETag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> when the item was last modified.
        /// </summary>
        public DateTime LastModified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ResourceType of the item.
        /// </summary>
        public string ResourceType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DisplayName of the item.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        #region RFC4331

        /// <summary>
        /// Gets or sets the quota (available) in bytes.
        /// </summary>
        public long QuotaAvailableBytes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the quota (used) in bytes.
        /// </summary>
        public long QuotaUsedBytes
        {
            get;
            set;
        }

        #endregion RFC431

        #region  Additional WebDAV Collection Properties

        /// <summary>
        /// Gets or sets the count of contained resources (files and folders) of the item.
        /// </summary>
        public long ChildCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default document for a collection.
        /// </summary>
        public string DefaultDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the globally unique identifier for this resource.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the item is a folder.
        /// </summary>
        public bool IsFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the item is hidden.
        /// </summary>
        public bool IsHidden
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the resource is a structured document.
        /// </summary>
        public bool IsStructuredDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the item contains subfolders.
        /// </summary>
        public bool HasSubDirectories
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a value indicating if the item allows the creating of subfolders.
        /// </summary>
        public bool NoSubDirectoriesAllowed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the count of files contained in this item.
        /// </summary>
        public long FileCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the item is reserved (i.e. system controlled). A Reserved item usually not be deleted, renamed or moved.
        /// </summary>
        public bool IsReserved
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the count of visible files contained in this item.
        /// </summary>
        public long VisibleFiles
        {
            get;
            set;
        }


        #endregion  Additional WebDAV Collection Properties

        #region IIS specific properties

        /// <summary>
        /// Gets or sets the ContentClass.
        /// </summary>
        public string ContentClass
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the item is read only.
        /// </summary>
        public bool IsReadonly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the item is the root.
        /// </summary>
        public bool IsRoot
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> when the item was last accessed.
        /// </summary>
        public DateTime LastAccessed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the parent item.
        /// </summary>
        public string ParentName
        {
            get;
            set;
        }

        #endregion IIS specific properties       
    }
}
