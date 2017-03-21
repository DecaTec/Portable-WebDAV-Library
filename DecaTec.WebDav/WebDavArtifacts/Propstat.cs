using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'propstat' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = WebDavConstants.PropStat, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class Propstat
    {
        /// <summary>
        /// Gets or sets the <see cref="DecaTec.WebDav.WebDavArtifacts.Prop"/>.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.Prop)]
        public Prop Prop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.Status)]
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ResponseDescription.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.ResponseDescription)]
        public string ResponseDescription
        {
            get;
            set;
        }
    }
}
