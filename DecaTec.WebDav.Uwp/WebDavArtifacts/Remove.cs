using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'remove' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = "remove", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class Remove
    {
        private Prop propField;

        /// <summary>
        /// Gets or sets the Prop.
        /// </summary>
        [XmlElement(ElementName = "prop")]
        public Prop Prop
        {
            get
            {
                return this.propField;
            }
            set
            {
                this.propField = value;
            }
        }
    }
}
