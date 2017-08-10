using DecaTec.WebDav.WebDavArtifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing a WebDAV element to be used for methods of WebDavSession.
    /// </summary>
    /// <remarks>Note that not all properties defined in this class are always used. Some WebDAV servers use only a subset of these properties or even provide additional properties.
    /// So, if (strongly typed) properties defined by this class are null, these values may be not supported by the specific WebDAV server.</remarks>
    public class WebDavSessionItem
    {
        /// <summary>
        /// Creates a new instance of WebDavSessionitem with the properties specified.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <param name="creationDate">The creation date.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="contentLanguage">The content language.</param>
        /// <param name="contentLength">The content length.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="eTag">The ETag.</param>
        /// <param name="lastModified">The last modified date.</param>
        /// <param name="quotaAvailableBytes">The QuotaAvailableBytes.</param>
        /// <param name="quotaUsedBytes">The QuotaUsedBytes.</param>
        /// <param name="childCount">The child count.</param>
        /// <param name="defaultDocument">The default document.</param>
        /// <param name="id">The ID.</param>
        /// <param name="isFolder">The IsFolder.</param>
        /// <param name="isStructuredDocument">The IsStructuredDocument.</param>
        /// <param name="hasSubDirectories">The HasSubdirectories.</param>
        /// <param name="noSubDirectoriesAllowed">The NoSubDirectoriesAllowed.</param>
        /// <param name="fileCount">The file count.</param>
        /// <param name="isReserved">The IsReserved.</param>
        /// <param name="visibleFiles">The visible files count.</param>
        /// <param name="contentClass">The content class.</param>
        /// <param name="isReadonly">The IsReadonly.</param>
        /// <param name="isRoot">The IsRoot.</param>
        /// <param name="lastAccessed">The last accessed date.</param>
        /// <param name="name">The name.</param>
        /// <param name="parentName">The parent name.</param>
        /// <param name="additionalProperties">The addition/unknown properties.</param>
        public WebDavSessionItem(Uri uri, DateTime? creationDate, string displayName, string contentLanguage, long? contentLength, string contentType, string eTag, DateTime? lastModified,
            long? quotaAvailableBytes, long? quotaUsedBytes, long? childCount, string defaultDocument, string id, bool? isFolder, bool? isStructuredDocument, bool? hasSubDirectories,
            bool? noSubDirectoriesAllowed, long? fileCount, bool? isReserved, long? visibleFiles, string contentClass, bool? isReadonly, bool? isRoot, DateTime? lastAccessed, string name, string parentName,
            XElement[] additionalProperties = null)
        {
            this.uri = uri;
            this.creationDate = creationDate;
            this.displayName = displayName;
            this.contentLanguage = contentLanguage;
            this.contentLength = contentLength;
            this.contentType = contentType;
            this.eTag = eTag;
            this.lastModified = lastModified;
            this.quotaAvailableBytes = quotaAvailableBytes;
            this.quotaUsedBytes = quotaUsedBytes;
            this.childCount = childCount;
            this.defaultDocument = defaultDocument;
            this.id = id;
            this.isFolder = isFolder;
            this.isStructuredDocument = isStructuredDocument;
            this.hasSubDirectories = hasSubDirectories;
            this.noSubDirectoriesAllowed = noSubDirectoriesAllowed;
            this.fileCount = fileCount;
            this.isReserved = isReserved;
            this.visibleFiles = visibleFiles;
            this.contentClass = contentClass;
            this.isReadonly = isReadonly;
            this.isRoot = isRoot;
            this.lastAccessed = lastAccessed;
            this.name = name;
            this.parentName = parentName;

            this.additionalProperties = new AdditionalWebDavProperties(additionalProperties);
        }

        private Uri uri;

        /// <summary>
        /// Gets the <see cref="Uri"/> of the item.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.uri;
            }
            private set
            {
                this.uri = value;
            }
        }

        private DateTime? creationDate;
        private bool creationDateChanged;

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> when the item was created.
        /// </summary>
        public DateTime? CreationDate
        {
            get
            {
                return this.creationDate;
            }
            set
            {
                if (value != this.creationDate)
                {
                    this.creationDate = value;
                    this.creationDateChanged = true;
                }
            }
        }

        private string displayName;
        private bool displayNameChanged;

        /// <summary>
        /// Gets or sets the DisplayName of the item.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                if (String.CompareOrdinal(value, this.displayName) != 0)
                {
                    this.displayName = value;
                    this.displayNameChanged = true;
                }
            }
        }

        private string contentLanguage;
        private bool contentLanguageChanged;

        /// <summary>
        /// Gets or sets the language of the content.
        /// </summary>
        public string ContentLanguage
        {
            get
            {
                return this.contentLanguage;
            }
            set
            {
                if (String.CompareOrdinal(value, this.contentLanguage) != 0)
                {
                    this.contentLanguage = value;
                    this.contentLanguageChanged = true;
                }
            }
        }

        private long? contentLength;

        /// <summary>
        /// Gets the length of the content in bytes.
        /// </summary>
        public long? ContentLength
        {
            get
            {
                return this.contentLength;
            }
            private set
            {
                this.contentLength = value;
            }
        }

        private string contentType;
        private bool contentTypeChanged;

        /// <summary>
        /// Gets or sets the MIME type of the content.
        /// </summary>
        public string ContentType
        {
            get
            {
                return this.contentType;
            }
            set
            {
                if (String.CompareOrdinal(value, this.contentType) != 0)
                {
                    this.contentType = value;
                    this.contentTypeChanged = true;
                }
            }
        }

        private string eTag;

        /// <summary>
        /// Gets the ETag.
        /// </summary>
        public string ETag
        {
            get
            {
                return this.eTag;
            }
            private set
            {
                this.eTag = value;
            }
        }

        private DateTime? lastModified;
        private bool lastModifiedChanged;

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> when the item was last modified.
        /// </summary>
        public DateTime? LastModified
        {
            get
            {
                return this.lastModified;
            }
            set
            {
                if (value != this.lastModified)
                {
                    this.lastModified = value;
                    this.lastModifiedChanged = true;
                }
            }
        }

        private string resourceType;

        /// <summary>
        /// Gets or sets the ResourceType of the item.
        /// </summary>
        public string ResourceType
        {
            get
            {
                return this.resourceType;
            }
            private set
            {
                this.resourceType = value;
            }
        }

        #region RFC 4331

        private long? quotaAvailableBytes;

        /// <summary>
        /// Gets the quota (available) in bytes.
        /// </summary>
        public long? QuotaAvailableBytes
        {
            get
            {
                return this.quotaAvailableBytes;
            }
            private set
            {
                this.quotaAvailableBytes = value;
            }
        }

        private long? quotaUsedBytes;

        /// <summary>
        /// Gets the quota (used) in bytes.
        /// </summary>
        public long? QuotaUsedBytes
        {
            get
            {
                return this.quotaUsedBytes;
            }
            private set
            {
                this.quotaUsedBytes = value;
            }
        }

        #endregion RFC 431

        #region  Additional WebDAV Collection Properties

        private long? childCount;

        /// <summary>
        /// Gets the count of contained resources (files and folders) of the item.
        /// </summary>
        public long? ChildCount
        {
            get
            {
                return this.childCount;
            }
            private set
            {
                this.childCount = value;
            }
        }

        private string defaultDocument;
        private bool defaultDocumentChanged;

        /// <summary>
        /// Gets or sets the default document for a collection.
        /// </summary>
        public string DefaultDocument
        {
            get
            {
                return this.defaultDocument;
            }
            set
            {
                if (String.CompareOrdinal(value, this.defaultDocument) != 0)
                {
                    this.defaultDocument = value;
                    this.defaultDocumentChanged = true;
                }
            }
        }

        private string id;

        /// <summary>
        /// Gets the globally unique identifier for this resource.
        /// </summary>
        public string Id
        {
            get
            {
                return this.id;
            }
            private set
            {
                this.id = value;
            }
        }

        private bool? isFolder;

        /// <summary>
        /// Gets a value indicating if the item is a folder.
        /// </summary>
        public bool? IsFolder
        {
            get
            {
                return this.isFolder;
            }
            private set
            {
                this.isFolder = value;
            }
        }

        private bool? isHidden;

        /// <summary>
        /// Gets or sets a value indicating if the item is hidden.
        /// </summary>
        public bool? IsHidden
        {
            get
            {
                return this.isHidden;
            }
            private set
            {
                this.isHidden = value;
            }
        }

        private bool? isStructuredDocument;

        /// <summary>
        /// Gets a value indicating whether the resource is a structured document.
        /// </summary>
        public bool? IsStructuredDocument
        {
            get
            {
                return this.isStructuredDocument;
            }
            private set
            {
                this.isStructuredDocument = value;
            }
        }

        private bool? hasSubDirectories;

        /// <summary>
        /// Gets a value indicating if the item contains subfolders.
        /// </summary>
        public bool? HasSubDirectories
        {
            get
            {
                return this.hasSubDirectories;
            }
            private set
            {
                this.hasSubDirectories = value;
            }
        }

        private bool? noSubDirectoriesAllowed;

        /// <summary>
        /// Gets a value indicating if the item allows the creating of subfolders.
        /// </summary>
        public bool? NoSubDirectoriesAllowed
        {
            get
            {
                return this.noSubDirectoriesAllowed;
            }
            private set
            {
                this.noSubDirectoriesAllowed = value;
            }
        }

        private long? fileCount;

        /// <summary>
        /// Gets the count of files contained in this item.
        /// </summary>
        public long? FileCount
        {
            get
            {
                return this.fileCount;
            }
            private set
            {
                this.fileCount = value;
            }
        }

        private bool? isReserved;

        /// <summary>
        /// Gets a value indicating if the item is reserved (i.e. system controlled). A Reserved item usually not be deleted, renamed or moved.
        /// </summary>
        public bool? IsReserved
        {
            get
            {
                return this.isReserved;
            }
            private set
            {
                this.isReserved = value;
            }
        }

        private long? visibleFiles;

        /// <summary>
        /// Gets the count of visible files contained in this item.
        /// </summary>
        public long? VisibleFiles
        {
            get
            {
                return this.visibleFiles;
            }
            private set
            {
                this.visibleFiles = value;
            }
        }


        #endregion  Additional WebDAV Collection Properties

        #region IIS specific properties

        private string contentClass;

        /// <summary>
        /// Gets or sets the ContentClass.
        /// </summary>
        public string ContentClass
        {
            get
            {
                return this.contentClass;
            }
            private set
            {
                this.contentClass = value;
            }
        }

        private bool? isReadonly;
        private bool isReadonlyChanged;

        /// <summary>
        /// Gets or sets a value indicating if the item is read only.
        /// </summary>
        public bool? IsReadonly
        {
            get
            {
                return this.isReadonly;
            }
            set
            {
                if (value != this.isReadonly)
                {
                    this.isReadonly = value;
                    this.isReadonlyChanged = true;
                }
            }
        }

        private bool? isRoot;

        /// <summary>
        /// Gets a value indicating if the item is the root.
        /// </summary>
        public bool? IsRoot
        {
            get
            {
                return this.isRoot;
            }
            private set
            {
                this.isRoot = value;
            }
        }

        private DateTime? lastAccessed;
        private bool lastAccessedChanged;

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> when the item was last accessed.
        /// </summary>
        public DateTime? LastAccessed
        {
            get
            {
                return this.lastAccessed;
            }
            set
            {
                if (value != this.lastAccessed)
                {
                    this.lastAccessed = value;
                    this.lastAccessedChanged = true;
                }
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            private set
            {
                this.name = value;
            }
        }

        private string parentName;

        /// <summary>
        /// Gets the name of the parent item.
        /// </summary>
        public string ParentName
        {
            get
            {
                return this.parentName;
            }
            private set
            {
                this.parentName = value;
            }
        }

        #endregion IIS specific properties

        #region Additional/unknown properties

        private AdditionalWebDavProperties additionalProperties;

        /// <summary>
        /// Gets the <see cref="AdditionalWebDavProperties"/> representing the additional WebDAV properties not defined in <see href="https://www.ietf.org/rfc/rfc4918.txt">RFC 4918</see>, <see href="https://tools.ietf.org/html/rfc4331">RFC 4331</see>, <see href="https://tools.ietf.org/html/draft-hopmann-collection-props-00">Additional WebDAV Collection Properties</see> or the IIS WebDAV specification.
        /// </summary>
        /// <remarks>Important: Additional WebDAV properties are currently not supported on Xamarin. See <see href="https://github.com/DecaTec/Portable-WebDAV-Library/wiki/Xamarin">the project's wiki</see> for more information about the Portable WebDAV Library used on Xamarin.</remarks>
        public AdditionalWebDavProperties AdditionalProperties
        {
            get
            {
                return this.additionalProperties;
            }
        }

        #endregion Additional/unknown properties

        #region Public methods

        /// <summary>
        /// Creates a <see cref="PropertyUpdate"/> from this WebDavSessionItem.
        /// </summary>
        /// <returns>A <see cref="PropertyUpdate"/> representing this WebDavSessionItem.</returns>
        /// <remarks>Use this method then you have changed some properties on the WebDavSessionItem and want to update the item's properties on the server using <see cref="WebDavSession.UpdateItemAsync(WebDavSessionItem)"/>.</remarks>
        public PropertyUpdate ToPropertyUpdate()
        {
            return GetPropertyUpdate();
        }

        #endregion Public methods

        #region Protected methods

        /// <summary>
        /// Gets a value indicating if this WebDavSessionItem was changed after it was retrieved with the ListAsync methods of <see cref="WebDavSession"/>.
        /// </summary>
        protected virtual bool HasChanged => this.creationDateChanged || this.displayNameChanged || this.contentLanguageChanged ||
                                   this.contentTypeChanged || this.lastModifiedChanged || this.defaultDocumentChanged ||
                                   isReadonlyChanged || this.lastAccessedChanged || this.additionalProperties.HasChanged;

        #endregion Protected methods

        #region Internal methods

        /// <summary>
        /// Gets a PropertyUpdate from the properties changed for a WebDavSessionItem.
        /// </summary>
        /// <returns></returns>
        internal PropertyUpdate GetPropertyUpdate()
        {
            if (!HasChanged)
                return null;

            var setProp = new Prop();
            var removeProp = new Prop();
            var setRequested = false;
            var removeRequested = false;

            // If property has changed and is not null/has a value now, it's a set operation.
            // DateTime values (as string) should have the format of ISO8601.
            if (this.creationDateChanged && this.CreationDate.HasValue)
            {
                setProp.CreationDateString = this.CreationDate.Value.ToString("o");
                setRequested = true;
            }

            if (this.displayNameChanged && !string.IsNullOrEmpty(this.DisplayName))
            {
                setProp.DisplayName = this.DisplayName;
                setRequested = true;
            }

            if (this.contentLanguageChanged && !string.IsNullOrEmpty(this.ContentLanguage))
            {
                setProp.GetContentLanguage = this.ContentLanguage;
                setRequested = true;
            }

            if (this.contentTypeChanged && !string.IsNullOrEmpty(this.ContentType))
            {
                setProp.GetContentType = this.ContentType;
                setRequested = true;
            }

            if (this.lastModifiedChanged && this.LastModified.HasValue)
            {
                setProp.GetLastModifiedString = this.LastModified.Value.ToString("o");
                setRequested = true;
            }

            if (this.defaultDocumentChanged && !string.IsNullOrEmpty(this.DefaultDocument))
            {
                setProp.DefaultDocument = this.DefaultDocument;
                setRequested = true;
            }

            if (this.isReadonlyChanged && this.IsReadonly.HasValue)
            {
                setProp.IsReadonlyString = this.IsReadonly.Value ? "1" : "0";
                setRequested = true;
            }

            if (this.lastAccessedChanged && this.lastAccessed.HasValue)
            {
                setProp.LastAccessedString = this.LastAccessed.Value.ToString("o");
                setRequested = true;
            }

            if (this.additionalProperties.HasChanged)
            {
                var xElementList = additionalProperties.GetChangedAndAddedProperties();

                if (xElementList.Count > 0)
                {
                    setProp.AdditionalProperties = xElementList.ToArray();
                    setRequested = true;
                }
            }

            // If a property has changed and is null/has no value now, it's a remove operation.			
            var removePropertyNames = new List<string>();

            if (this.creationDateChanged && !this.CreationDate.HasValue)
            {
                removePropertyNames.Add(PropNameConstants.CreationDate);
                removeRequested = true;
            }

            if (this.displayNameChanged && string.IsNullOrEmpty(this.DisplayName))
            {
                removePropertyNames.Add(PropNameConstants.DisplayName);
                removeRequested = true;
            }

            if (this.contentLanguageChanged && string.IsNullOrEmpty(this.ContentLanguage))
            {
                removePropertyNames.Add(PropNameConstants.GetContentLanguage);
                removeRequested = true;
            }

            if (this.contentTypeChanged && string.IsNullOrEmpty(this.ContentType))
            {
                removePropertyNames.Add(PropNameConstants.GetContentType);
                removeRequested = true;
            }

            if (this.lastModifiedChanged && !this.LastModified.HasValue)
            {
                removePropertyNames.Add(PropNameConstants.GetLastModified);
                removeRequested = true;
            }

            if (this.defaultDocumentChanged && string.IsNullOrEmpty(this.DefaultDocument))
            {
                removePropertyNames.Add(PropNameConstants.DefaultDocument);
                removeRequested = true;
            }

            if (this.isReadonlyChanged && !this.IsReadonly.HasValue)
            {
                removePropertyNames.Add(PropNameConstants.IsReadonly);
                removeRequested = true;
            }

            if (this.lastAccessedChanged && !this.lastAccessed.HasValue)
            {
                removePropertyNames.Add(PropNameConstants.LastAccessed);
                removeRequested = true;
            }

            removeProp = Prop.CreatePropWithEmptyProperties(removePropertyNames.ToArray());

            if (this.additionalProperties.HasChanged)
            {
                var xElementList = this.additionalProperties.GetRemovedProperties();

                if (xElementList.Count > 0)
                {
                    removeProp.AdditionalProperties = xElementList.ToArray();
                    removeRequested = true;
                }
            }

            // Build up PropertyUpdate.
            var propertyUpdate = new PropertyUpdate();
            var propertyUpdateItems = new List<object>();

            if (setRequested)
            {
                var set = new Set()
                {
                    Prop = setProp
                };

                propertyUpdateItems.Add(set);
            }

            if (removeRequested)
            {
                var remove = new Remove()
                {
                    Prop = removeProp
                };

                propertyUpdateItems.Add(remove);
            }

            propertyUpdate.Items = propertyUpdateItems.ToArray();
            return propertyUpdate;
        }

        #endregion Internal methods
    }
}