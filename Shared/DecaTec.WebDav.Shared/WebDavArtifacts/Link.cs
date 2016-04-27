using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'link' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = "link", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class Link
    {
        private string[] srcField;
        private string[] dstField;

        /// <summary>
        /// Gets or sets the Src.
        /// </summary>
        [XmlElement(ElementName = "src")]
        public string[] Src
        {
            get
            {
                return this.srcField;
            }
            set
            {
                this.srcField = value;
            }
        }

        /// <summary>
        /// Gets or sets the Dst.
        /// </summary>
        [XmlElement(ElementName = "dst")]
        public string[] Dst
        {
            get
            {
                return this.dstField;
            }
            set
            {
                this.dstField = value;
            }
        }
    }
}
