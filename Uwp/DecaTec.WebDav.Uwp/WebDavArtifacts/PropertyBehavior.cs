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
    [XmlType(TypeName = "propertybehavior", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class PropertyBehavior
    {
        private object itemField;

        /// <summary>
        /// Gets or sets the Item.
        /// </summary>
        [XmlElement(ElementName = "keepalive", Type = typeof(KeepAlive))]
        [XmlElement(ElementName = "omit", Type = typeof(Omit))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }
}
