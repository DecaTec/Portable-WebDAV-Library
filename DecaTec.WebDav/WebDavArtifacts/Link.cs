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
    [XmlType(TypeName = WebDavConstants.Link, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class Link
    {
        /// <summary>
        /// Gets or sets the Src.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.Src)]
        public string[] Src
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Dst.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.Dst)]
        public string[] Dst
        {
            get;
            set;
        }
    }
}
