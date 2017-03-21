using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'lockroot' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = WebDavConstants.LockRoot, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class LockRoot
    {
        /// <summary>
        /// Gets or sets the Href.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.Href)]
        public string Href
        {
            get;
            set;
        }
    }
}
