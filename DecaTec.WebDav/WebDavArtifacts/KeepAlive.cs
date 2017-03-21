using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'keepalive' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = WebDavConstants.KeepAlive, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class KeepAlive
    {
        /// <summary>
        /// Gets or sets the Href.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.Href)]
        public string[] Href
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Text.
        /// </summary>
        [XmlText]
        [XmlElement(ElementName = WebDavConstants.Text)]
        public string[] Text
        {
            get;
            set;
        }
    }
}
