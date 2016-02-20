using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'propertyupdate' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [XmlType(TypeName = "propertyupdate", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class PropertyUpdate
    {
        private object[] itemsField;

        /// <summary>
        /// Gets or sets the Items.
        /// </summary>
        [XmlElement(ElementName = "remove", Type = typeof(Remove))]
        [XmlElement(ElementName = "set", Type = typeof(Set))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }
}
