using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'lockdiscovery' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = "lockdiscovery", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class LockDiscovery
    {
        private ActiveLock[] activelockField;

        /// <summary>
        /// Gets or sets the ActiveLock.
        /// </summary>
        [XmlElement(ElementName = "activelock")]
        public ActiveLock[] ActiveLock
        {
            get
            {
                return this.activelockField;
            }
            set
            {
                this.activelockField = value;
            }
        }
    }
}
