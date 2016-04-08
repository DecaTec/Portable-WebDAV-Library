using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'propfind' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [XmlType(TypeName = "propfind", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class PropFind
    {
        /// <summary>
        /// Creates a PropFind instance representing an AllProp-Propfind.
        /// </summary>
        /// <returns>A PropFind instance containing an AllProp element.</returns>
        public static PropFind CreatePropFindAllProp()
        {
            var propFind = new PropFind();
            propFind.Item = new AllProp();
            return propFind;
        }

        /// <summary>
        /// Creates a PropFind instance containg empty property items with the specified names.
        /// </summary>
        /// <param name="propertyNames">The property names which should be contained in the PropFind instance.</param>
        /// <returns>A PropFind instance containing the empty properties specified.</returns>
        public static PropFind CreatePropFindWithEmptyProperties(params string[] propertyNames)
        {
            var propFind = new PropFind();
            var prop = Prop.CreatePropWithEmptyProperties(propertyNames);
            propFind.Item = prop;
            return propFind;
        }

        /// <summary>
        /// Creates a PropFind instance containg a PropertyName item.
        /// </summary>
        /// <returns>A PropFind instance containing a PropertyName item.</returns>
        public static PropFind CreatePropFindWithPropName()
        {
            var propFind = new PropFind();
            propFind.Item = new PropName();
            return propFind;
        }

        private object itemField;

        /// <summary>
        /// Gets or sets the Item.
        /// </summary>
        [XmlElement(ElementName = "allprop", Type = typeof(AllProp))]
        [XmlElement(ElementName = "prop", Type = typeof(Prop))]
        [XmlElement(ElementName = "propname", Type =  typeof(PropName))]
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
