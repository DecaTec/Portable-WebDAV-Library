using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'supportedlock' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = "supportedlock", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class SupportedLock
    {
        private LockEntry[] lockentryField;

        /// <summary>
        /// Gets or sets the LockEntry.
        /// </summary>
        [XmlElement(ElementName = "lockentry")]
        public LockEntry[] LockEntry
        {
            get
            {
                return this.lockentryField;
            }
            set
            {
                this.lockentryField = value;
            }
        }
    }
}
