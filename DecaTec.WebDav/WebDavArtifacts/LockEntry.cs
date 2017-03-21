using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'lockentry' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = WebDavConstants.LockEntry, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class LockEntry
    {
        /// <summary>
        /// Gets or sets the <see cref="DecaTec.WebDav.WebDavArtifacts.LockScope"/>.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.LockScope)]
        public LockScope LockScope
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="DecaTec.WebDav.WebDavArtifacts.LockType"/>.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.LockType)]
        public LockType LockType
        {
            get;
            set;
        }
    }
}
