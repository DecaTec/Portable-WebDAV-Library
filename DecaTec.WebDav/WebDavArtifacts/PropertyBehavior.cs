using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'propertybehavior' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = WebDavConstants.PropertyBehavior, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class PropertyBehavior
    {
        /// <summary>
        /// Gets or sets the Item.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.KeepAlive, Type = typeof(KeepAlive))]
        [XmlElement(ElementName = WebDavConstants.Omit, Type = typeof(Omit))]
        public object Item
        {
            get;
            set;
        }
    }
}
