using System;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class representing a list item from a WebDavSession's list method.
    /// </summary>
    public class WebDavSessionListItem
    {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URI of the item.
        /// </summary>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the item in bytes.
        /// </summary>
        public long Size
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item's creation date.
        /// </summary>
        public DateTime Created
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item's last modified date.
        /// </summary>
        public DateTime Modified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating if the item represents a directory.
        /// </summary>
        public bool IsDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item's content type.
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }
    }
}
