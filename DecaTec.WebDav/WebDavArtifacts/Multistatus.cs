using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'multistatus' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [DebuggerStepThrough]
    [XmlType(TypeName = "multistatus", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class Multistatus
    {
        private Response[] responseField;
        private string responsedescriptionField;

        /// <summary>
        /// Gets or sets the Response.
        /// </summary>
        [XmlElement(ElementName = "response")]
        public Response[] Response
        {
            get
            {
                return this.responseField;
            }
            set
            {
                this.responseField = value;
            }
        }

        /// <summary>
        /// Gets or sets the ResponseDescription.
        /// </summary>
        [XmlElement(ElementName = "responsedescription")]
        public string ResponseDescription
        {
            get
            {
                return this.responsedescriptionField;
            }
            set
            {
                this.responsedescriptionField = value;
            }
        }
    }
}
