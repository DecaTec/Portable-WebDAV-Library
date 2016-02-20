using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'prop' XML element for WebDAV communication.
    /// This class contains all properties defined in RFC4918, but property names are not limited to these properties.
    /// </summary>
    [DataContract]
    [XmlType(TypeName = "prop", Namespace = "DAV:")]
    [XmlRoot(Namespace = "DAV:", IsNullable = false)]
    public class Prop
    {
        /// <summary>
        /// Creates a Prop with empty properties. This is especially useful for PROPFIND commands where only specific properties should be requested.
        /// </summary>
        /// <param name="emptyPropertyNames">The name of the properties of the Prop which should be empty.</param>
        /// <returns>A Prop with the empty properties which where requested.</returns>
        /// <remarks>As an example, if a PROPFIND command should be sent returning only the getlastaccess property, you can use the following code:
        /// 
        /// <code>
        /// PropFind pf = new PropFind();
        /// Prop p = Prop.CreatePropWithEmptyProperties("getlastmodified");
        /// pf.Item = p;
        ///  </code>
        ///  
        /// This PropFind object can then be used in a PROPFIND command on the WebDavClient.
        /// 
        /// This implementation of Prop contains all properties defined in RFC4918. If you need to request other properties from a WebDav server, you will need 
        /// to implement your own classes which can be (de-)serialized for communication with the server.
        /// </remarks>
        public static Prop CreatePropWithEmptyProperties(params string[] emptyPropertyNames)
        {
            Prop prop = new Prop();

            foreach (var emptyPropertyName in emptyPropertyNames)
            {
                switch (emptyPropertyName.ToLower())
                {
                    case "creationdate":
                        prop.CreationDate = string.Empty;
                        break;        
                    case "getcontentlanguage":
                        prop.GetContentLanguage = string.Empty;
                        break;
                    case "displayname":
                        prop.DisplayName = string.Empty;
                        break;
                    case "getcontentlength":
                        prop.GetContentLength = string.Empty;
                        break;
                    case "getcontenttype":
                        prop.GetContentType = string.Empty;
                        break;
                    case "getlastmodified":
                        prop.GetLastModified = string.Empty;
                        break;
                    case "getetag":
                        prop.GetEtag = string.Empty;
                        break;
                    case "source":
                        prop.Source = new Source();
                        break;
                    case "resourcetype":
                        prop.ResourceType = new ResourceType();
                        break;
                    case "contentclass":
                        prop.ContentClass = string.Empty;
                        break;
                    case "defaultdocument":
                        prop.DefaultDocument = string.Empty;
                        break;
                    case "href":
                        prop.Href = string.Empty;
                        break;
                    case "iscollection":
                        prop.IsCollection = string.Empty;
                        break;
                    case "ishidden":
                        prop.IsHidden = string.Empty;
                        break;
                    case "isreadonly":
                        prop.IsReadonly = string.Empty;
                        break;
                    case "isroot":
                        prop.IsRoot = string.Empty;
                        break;
                    case "isstructureddocument":
                        prop.IsStructuredDocument = string.Empty;
                        break;
                    case "lastaccessed":
                        prop.LastAccessed = string.Empty;
                        break;
                    case "name":
                        prop.Name = string.Empty;
                        break;
                    case "parentname":
                        prop.ParentName = string.Empty;
                        break;
                    default:
                        break;
                }
            }

            return prop;
        }

        private string creationdateField;
        private bool creationdateFieldSpecified;
        private string getcontentlanguageField;
        private string displaynameField;
        private string getcontentlengthField;
        private string getcontenttypeField;
        private string getlastmodifiedField;
        private string getetagField;
        private Source sourceField;
        private ResourceType resourcetypeField;
        private string contentclassField;
        private string defaultdocumentField;
        private string hrefField;
        private string iscollectionField;
        private string ishiddenField;
        private string isreadonlyField;
        private string isrootField;
        private string isstructureddocumentField;
        private string lastaccessedField;
        private string nameField;
        private string parentnameField;
        private LockDiscovery lockDiscoveryField;

        /// <summary>
        /// Gets or sets the CreationDate.
        /// </summary>
        [XmlElement(ElementName = "creationdate")]
        public string CreationDate
        {
            get
            {
                return this.creationdateField;
            }
            set
            {
                this.creationdateField = value;
            }
        }

        /// <summary>
        /// gets or sets the CreationDateSpecified.
        /// </summary>
        [XmlIgnore]
        public bool CreationDateSpecified
        {
            get
            {
                return this.creationdateFieldSpecified;
            }
            set
            {
                this.creationdateFieldSpecified = value;
            }
        }

        /// <summary>
        /// Gets or sets the GetContentLanguage.
        /// </summary>
        [XmlElement(ElementName = "getcontentlanguage")]
        public string GetContentLanguage
        {
            get
            {
                return this.getcontentlanguageField;
            }
            set
            {
                this.getcontentlanguageField = value;
            }
        }

        /// <summary>
        /// Gets or sets the DisplayName.
        /// </summary>
        [XmlElement(ElementName = "displayname")]
        public string DisplayName
        {
            get
            {
                return this.displaynameField;
            }
            set
            {
                this.displaynameField = value;
            }
        }

        /// <summary>
        /// Gets or sets the GetContentLength.
        /// </summary>
        [XmlElement(ElementName = "getcontentlength")]
        public string GetContentLength
        {
            get
            {
                return this.getcontentlengthField;
            }
            set
            {
                this.getcontentlengthField = value;
            }
        }

        /// <summary>
        /// Gets or sets the GetContentType.
        /// </summary>
        [XmlElement(ElementName = "getcontenttype")]
        public string GetContentType
        {
            get
            {
                return this.getcontenttypeField;
            }
            set
            {
                this.getcontenttypeField = value;
            }
        }

        /// <summary>
        /// Gets or sets the GetLastModified.
        /// </summary>
        [XmlElement(ElementName = "getlastmodified")]
        public string GetLastModified
        {
            get
            {
                return this.getlastmodifiedField;
            }
            set
            {
                this.getlastmodifiedField = value;
            }
        }

        /// <summary>
        /// Gets or sets the GetEtag.
        /// </summary>
        [XmlElement(ElementName = "getetag")]
        public string GetEtag
        {
            get
            {
                return this.getetagField;
            }
            set
            {
                this.getetagField = value;
            }
        }

        /// <summary>
        /// Gets or sets the Source.
        /// </summary>
        [XmlElement(ElementName = "source")]
        public Source Source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <summary>
        /// Gets or sets the ResourceType.
        /// </summary>
        [XmlElement(ElementName = "resourcetype")]
        public ResourceType ResourceType
        {
            get
            {
                return this.resourcetypeField;
            }
            set
            {
                this.resourcetypeField = value;
            }
        }

        /// <summary>
        /// Gets or sets the GetContentClass.
        /// </summary>
        [XmlElement(ElementName = "contentclass")]
        public string ContentClass
        {
            get
            {
                return this.contentclassField;
            }
            set
            {
                this.contentclassField = value;
            }
        }

        /// <summary>
        /// Gets or sets the DefaultDocument.
        /// </summary>
        [XmlElement(ElementName = "defaultdocument")]
        public string DefaultDocument
        {
            get
            {
                return this.defaultdocumentField;
            }
            set
            {
                this.defaultdocumentField = value;
            }
        }

        /// <summary>
        /// Gets or sets the Href.
        /// </summary>
        [XmlElement(ElementName = "href")]
        public string Href
        {
            get
            {
                return this.hrefField;
            }
            set
            {
                this.hrefField = value;
            }
        }

        /// <summary>
        /// Gets or sets the IsCollection.
        /// </summary>
        [XmlElement(ElementName = "iscollection")]
        public string IsCollection
        {
            get
            {
                return this.iscollectionField;
            }
            set
            {
                this.iscollectionField = value;
            }
        }

        /// <summary>
        /// Gets or sets the IsHidden.
        /// </summary>
        [XmlElement(ElementName = "ishidden")]
        public string IsHidden
        {
            get
            {
                return this.ishiddenField;
            }
            set
            {
                this.ishiddenField = value;
            }
        }

        /// <summary>
        /// Gets or sets the IsReadOnly.
        /// </summary>
        [XmlElement(ElementName = "isreadonly")]
        public string IsReadonly
        {
            get
            {
                return this.isreadonlyField;
            }
            set
            {
                this.isreadonlyField = value;
            }
        }

        /// <summary>
        /// Gets or sets the IsRoot.
        /// </summary>
        [XmlElement(ElementName = "isroot")]
        public string IsRoot
        {
            get
            {
                return this.isrootField;
            }
            set
            {
                this.isrootField = value;
            }
        }

        /// <summary>
        /// Gets or sets the IsStructuredDocument.
        /// </summary>
        [XmlElement(ElementName = "isstructureddocument")]
        public string IsStructuredDocument
        {
            get
            {
                return this.isstructureddocumentField;
            }
            set
            {
                this.isstructureddocumentField = value;
            }
        }

        /// <summary>
        /// Gets or sets the LastAccessed.
        /// </summary>
        [XmlElement(ElementName = "lastaccessed")]
        public string LastAccessed
        {
            get
            {
                return this.lastaccessedField;
            }
            set
            {
                this.lastaccessedField = value;
            }
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [XmlElement(ElementName = "name")]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <summary>
        /// Gets or sets the ParentName.
        /// </summary>
        [XmlElement(ElementName = "parentname")]
        public string ParentName
        {
            get
            {
                return this.parentnameField;
            }
            set
            {
                this.parentnameField = value;
            }
        }

        /// <summary>
        /// Gets or sets the LockDiscovery.
        /// </summary>
        [XmlElement(ElementName = "lockdiscovery")]
        public LockDiscovery LockDiscovery
        {
            get
            {
                return this.lockDiscoveryField;
            }
            set
            {
                this.lockDiscoveryField = value;
            }
        }
    }
}
