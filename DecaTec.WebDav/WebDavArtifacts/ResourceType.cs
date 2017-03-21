using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'resourcetype' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = WebDavConstants.ResourceType, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class ResourceType
    {
        /// <summary>
        /// Gets or sets the <see cref="DecaTec.WebDav.WebDavArtifacts.Collection"/>.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.Collection)]
        public Collection Collection
        {
            get;
            set;
        }
    }
}
