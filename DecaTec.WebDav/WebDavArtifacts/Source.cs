using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'source' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = WebDavConstants.Source, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class Source
    {
        /// <summary>
        /// Gets or sets the <see cref="DecaTec.WebDav.WebDavArtifacts.Link"/> array.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.Link)]
        public Link[] Link
        {
            get;
            set;
        }
    }
}
