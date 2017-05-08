using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'prop' XML element for WebDAV communication. It offers strongly typed access to these WebDAV properties.
    /// This class contains all properties defined in <see href="https://www.ietf.org/rfc/rfc4918.txt">RFC 4918</see>, <see href="https://tools.ietf.org/html/rfc4331">RFC 4331</see>, <see href="https://tools.ietf.org/html/draft-hopmann-collection-props-00">Additional WebDAV Collection Properties</see> and some IIS specific properties, but property names are not limited to these properties.
    /// </summary>
    /// <remarks>When the (strongly typed) properties returned by a WebDAV server are null, the server probably does not support these properties.
    /// In order to find out which properties are supported by the server, you can use a 'propname' request. Every (string) property, which is <see cref="string.Empty"/> then is supported by the WebDAV server 
    /// (see <see cref="PropFind.CreatePropFindWithPropName"/>).</remarks>
    [DataContract]
    [XmlType(TypeName = WebDavConstants.Prop, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class Prop
    {
        /// <summary>
        /// Creates a Prop with empty properties. This is especially useful for PROPFIND commands where only specific properties should be requested.
        /// </summary>
        /// <param name="emptyPropertyNames">The name of the properties of the Prop which should be empty.</param>
        /// <returns>A Prop with the empty properties which where requested.</returns>
        /// <remarks>As an example, if a PROPFIND command should be sent returning only the 'getlastmodified' property, you can use the following code:
        /// 
        /// <code>
        /// PropFind pf = new PropFind();
        /// Prop p = Prop.CreatePropWithEmptyProperties(PropNameConstants.GetLastModified);
        /// pf.Item = p;
        ///  </code>
        ///  
        /// This PropFind object can then be used in a PROPFIND command on the WebDavClient.
        /// 
        /// This implementation of Prop contains all properties defined in <see href="https://www.ietf.org/rfc/rfc4918.txt">RFC 4918</see>, <see href="https://tools.ietf.org/html/rfc4331">RFC 4331</see>, <see href="https://tools.ietf.org/html/draft-hopmann-collection-props-00">Additional WebDAV Collection Properties</see> and some IIS specific properties. 
        /// If you need to request other properties from a WebDav server, you will need 
        /// to implement your own classes which can be (de-)serialized for communication with the server.
        /// </remarks>
        public static Prop CreatePropWithEmptyProperties(params string[] emptyPropertyNames)
        {
            Prop prop = new Prop();

            foreach (var emptyPropertyName in emptyPropertyNames)
            {
                switch (emptyPropertyName.ToLower())
                {
                    case PropNameConstants.Href:
                        prop.HrefString = string.Empty;
                        break;
                    case PropNameConstants.CreationDate:
                        prop.CreationDateString = string.Empty;
                        break;
                    case PropNameConstants.GetContentLanguage:
                        prop.GetContentLanguage = string.Empty;
                        break;
                    case PropNameConstants.DisplayName:
                        prop.DisplayName = string.Empty;
                        break;
                    case PropNameConstants.GetContentLength:
                        prop.GetContentLengthString = string.Empty;
                        break;
                    case PropNameConstants.GetContentType:
                        prop.GetContentType = string.Empty;
                        break;
                    case PropNameConstants.GetLastModified:
                        prop.GetLastModifiedString = string.Empty;
                        break;
                    case PropNameConstants.GetEtag:
                        prop.GetEtag = string.Empty;
                        break;
                    case PropNameConstants.ResourceType:
                        prop.ResourceType = new ResourceType();
                        break;
                    case PropNameConstants.SupportedLock:
                        prop.SupportedLock = new SupportedLock();
                        break;
                    case PropNameConstants.LockDiscovery:
                        prop.LockDiscovery = new LockDiscovery();
                        break;
                    case PropNameConstants.QuotaAvailableBytes:
                        prop.QuotaAvailableBytesString = string.Empty;
                        break;
                    case PropNameConstants.QuotaUsedBytes:
                        prop.QuotaUsedBytesString = string.Empty;
                        break;
                    case PropNameConstants.ChildCount:
                        prop.ChildCountString = string.Empty;
                        break;
                    case PropNameConstants.DefaultDocument:
                        prop.DefaultDocument = string.Empty;
                        break;
                    case PropNameConstants.Id:
                        prop.Id = string.Empty;
                        break;
                    case PropNameConstants.IsFolder:
                        prop.IsFolderString = string.Empty;
                        break;
                    case PropNameConstants.IsHidden:
                        prop.IsHiddenString = string.Empty;
                        break;
                    case PropNameConstants.IsStructuredDocument:
                        prop.IsStructuredDocumentString = string.Empty;
                        break;
                    case PropNameConstants.HasSubs:
                        prop.HasSubsString = string.Empty;
                        break;
                    case PropNameConstants.NoSubs:
                        prop.NoSubsString = string.Empty;
                        break;
                    case PropNameConstants.ObjectCount:
                        prop.ObjectCountString = string.Empty;
                        break;
                    case PropNameConstants.Reserved:
                        prop.ReservedString = string.Empty;
                        break;
                    case PropNameConstants.VisibleCount:
                        prop.VisibleCountString = string.Empty;
                        break;
                    case PropNameConstants.ContentClass:
                        prop.ContentClass = string.Empty;
                        break;
                    case PropNameConstants.IsReadonly:
                        prop.IsReadonlyString = string.Empty;
                        break;
                    case PropNameConstants.IsRoot:
                        prop.IsRootString = string.Empty;
                        break;
                    case PropNameConstants.LastAccessed:
                        prop.LastAccessedString = string.Empty;
                        break;
                    case PropNameConstants.Name:
                        prop.Name = string.Empty;
                        break;
                    case PropNameConstants.ParentName:
                        prop.ParentName = string.Empty;
                        break;
                    default:
                        break;
                }
            }

            return prop;
        }

        /// <summary>
        /// Creates a Prop with all empty properties which are defined in <see href="https://www.ietf.org/rfc/rfc4918.txt">RFC 4918</see>, <see href="https://tools.ietf.org/html/rfc4331">RFC 4331</see>, <see href="https://tools.ietf.org/html/draft-hopmann-collection-props-00">Additional WebDAV Collection Properties</see> and some IIS specific properties. 
        /// This is especially useful for PROPFIND commands when the so called 'allprop' cannot be used because the WebDAV server does not return all properties.
        /// </summary>
        /// <returns>A Prop with all empty properties defined in <see href="https://www.ietf.org/rfc/rfc4918.txt">RFC 4918</see>, <see href="https://tools.ietf.org/html/rfc4331">RFC 4331</see>, <see href="https://tools.ietf.org/html/draft-hopmann-collection-props-00">Additional WebDAV Collection Properties</see> and some IIS specific properties.</returns>
        public static Prop CreatePropWithEmptyPropertiesAll()
        {
            Prop prop = new Prop()
            {
                CreationDateString = string.Empty,
                GetContentLanguage = string.Empty,
                DisplayName = string.Empty,
                GetContentLengthString = string.Empty,
                GetContentType = string.Empty,
                GetLastModifiedString = string.Empty,
                GetEtag = string.Empty,
                ResourceType = new ResourceType(),
                SupportedLock = new SupportedLock(),
                LockDiscovery = new LockDiscovery(),
                QuotaAvailableBytesString = string.Empty,
                QuotaUsedBytesString = string.Empty,
                ChildCountString = string.Empty,
                DefaultDocument = string.Empty,
                Id = string.Empty,
                IsFolderString = string.Empty,
                IsHiddenString = string.Empty,
                IsStructuredDocumentString = string.Empty,
                HasSubsString = string.Empty,
                NoSubsString = string.Empty,
                ObjectCountString = string.Empty,
                ReservedString = string.Empty,
                VisibleCountString = string.Empty,
                ContentClass = string.Empty,
                IsReadonlyString = string.Empty,
                IsRootString = string.Empty,
                LastAccessedString = string.Empty,
                Name = string.Empty,
                ParentName = string.Empty
            };

            return prop;
        }

        /// <summary>
        /// Gets or sets the Href as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.Href)]
        public string HrefString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Href or null when there is no Href available.
        /// </summary>
        [XmlIgnore]
        public Uri Href
        {
            get
            {
                if (!string.IsNullOrEmpty(this.HrefString))
                    return UriHelper.CreateUriFromUrl(this.HrefString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the CreationDate as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.CreationDate)]
        public string CreationDateString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the CreationDate or null if there is no CreationDate available.
        /// </summary>
        [XmlIgnore]
        public DateTime? CreationDate
        {
            get
            {
                if (!string.IsNullOrEmpty(this.CreationDateString))
                    return DateTime.Parse(this.CreationDateString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the DisplayName.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.DisplayName)]
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the GetContentLanguage.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.GetContentLanguage)]
        public string GetContentLanguage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the GetContentLength as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.GetContentLength)]
        public string GetContentLengthString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the GetContentLength or null when there is no GetContentLength available.
        /// </summary>
        [XmlIgnore]
        public long? GetContentLength
        {
            get
            {
                if (!string.IsNullOrEmpty(this.GetContentLengthString))
                    return long.Parse(this.GetContentLengthString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the GetContentType.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.GetContentType)]
        public string GetContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the GetEtag.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.GetEtag)]
        public string GetEtag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the GetLastModified as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.GetLastModified)]
        public string GetLastModifiedString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the GetLastModified or null if there is no GetLastModified available.
        /// </summary>
        [XmlIgnore]
        public DateTime? GetLastModified
        {
            get
            {
                if (!string.IsNullOrEmpty(this.GetLastModifiedString))
                    return DateTime.Parse(this.GetLastModifiedString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DecaTec.WebDav.WebDavArtifacts.LockDiscovery"/>.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.LockDiscovery)]
        public LockDiscovery LockDiscovery
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="DecaTec.WebDav.WebDavArtifacts.ResourceType"/>.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.ResourceType)]
        public ResourceType ResourceType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="DecaTec.WebDav.WebDavArtifacts.SupportedLock"/>.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.SupportedLock)]
        public SupportedLock SupportedLock
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the xml:lang attribute.
        /// </summary>
        [XmlAttribute(WebDavConstants.XmlLangAttribute)]
        public string Language
        {
            get;
            set;
        }

        #region RFC 4331

        // Properties as defined in https://tools.ietf.org/html/rfc4331

        /// <summary>
        /// Gets or sets the QuotaAvailableBytes as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.QuotaAvailableBytes)]
        public string QuotaAvailableBytesString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the QuotaAvailableBytes or null when there is no QuotaAvailableBytes available.
        /// </summary>
        [XmlIgnore]
        public long? QuotaAvailableBytes
        {
            get
            {
                if (!string.IsNullOrEmpty(this.QuotaAvailableBytesString))
                    return long.Parse(this.QuotaAvailableBytesString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the QuotaUsedBytes as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.QuotaUsedBytes)]
        public string QuotaUsedBytesString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the QuotaUsedBytes or null if there is no QuotaUsedBytes available.
        /// </summary>
        [XmlIgnore]
        public long? QuotaUsedBytes
        {
            get
            {
                if (!string.IsNullOrEmpty(this.QuotaUsedBytesString))
                    return long.Parse(this.QuotaUsedBytesString);
                else
                    return null;
            }
        }

        #endregion RFC 4331

        #region  Additional WebDAV Collection Properties

        // Properties as defined in http://www.ics.uci.edu/~ejw/authoring/props/draft-hopmann-collection-props-00.txt

        /// <summary>
        /// Gets or sets the ChildCount as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.ChildCount)]
        public string ChildCountString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ChildCount or null if there is no ChildCount available.
        /// </summary>
        [XmlIgnore]
        public long? ChildCount
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ChildCountString))
                    return long.Parse(this.ChildCountString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the DefaultDocument.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.DefaultDocument)]
        public string DefaultDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.Id)]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the IsFolder as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.IsFolder)]
        public string IsFolderString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the IsFolder or null if there is no IsFolder available.
        /// </summary>
        [XmlIgnore]
        public bool? IsFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(this.IsFolderString))
                {
                    if (this.IsFolderString.Equals("1"))
                        return true;
                    else
                        return false;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the IsHidden as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.IsHidden)]
        public string IsHiddenString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the IsHidden or null if there is no IsHidden available.
        /// </summary>
        [XmlIgnore]
        public bool? IsHidden
        {
            get
            {
                if (!string.IsNullOrEmpty(this.IsHiddenString))
                {
                    if (this.IsHiddenString.Equals("1"))
                        return true;
                    else
                        return false;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the IsStructuredDocument as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.IsStructuredDocument)]
        public string IsStructuredDocumentString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the IsStructuredDocument or null if there is no IsStructuredDocument available.
        /// </summary>
        [XmlIgnore]
        public bool? IsStructuredDocument
        {
            get
            {
                if (!string.IsNullOrEmpty(this.IsStructuredDocumentString))
                {
                    if (this.IsStructuredDocumentString.Equals("1"))
                        return true;
                    else
                        return false;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the HasSubs as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.HasSubs)]
        public string HasSubsString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the HasSubs or null if there is no HasSubs available.
        /// </summary>
        [XmlIgnore]
        public bool? HasSubs
        {
            get
            {
                if (!string.IsNullOrEmpty(this.HasSubsString))
                {
                    if (this.HasSubsString.Equals("1"))
                        return true;
                    else
                        return false;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the NoSubs as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.NoSubs)]
        public string NoSubsString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the NoSubs or null if there is no NoSubs available.
        /// </summary>
        [XmlIgnore]
        public bool? NoSubs
        {
            get
            {
                if (!string.IsNullOrEmpty(this.NoSubsString))
                {
                    if (this.NoSubsString.Equals("1"))
                        return true;
                    else
                        return false;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the ObjectCount as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.ObjectCount)]
        public string ObjectCountString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ObjectCount or null if there is no ObjectCount available.
        /// </summary>
        [XmlIgnore]
        public long? ObjectCount
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ObjectCountString))
                    return long.Parse(this.ObjectCountString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the NoSubs as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.Reserved)]
        public string ReservedString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the NoSubs or null if there is no NoSubs available.
        /// </summary>
        [XmlIgnore]
        public bool? Reserved
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ReservedString))
                {
                    if (this.ReservedString.Equals("1"))
                        return true;
                    else
                        return false;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the VisibleCount as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.VisibleCount)]
        public string VisibleCountString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the VisibleCount or null if there is no VisibleCount available.
        /// </summary>
        [XmlIgnore]
        public long? VisibleCount
        {
            get
            {
                if (!string.IsNullOrEmpty(this.VisibleCountString))
                    return long.Parse(this.VisibleCountString);
                else
                    return null;
            }
        }


        #endregion Additional WebDAV Collection Properties 

        #region IIS specific properties

        /// <summary>
        /// Gets or sets the GetContentClass.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.ContentClass)]
        public string ContentClass
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the IsReadOnly as string.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.IsReadonly)]
        public string IsReadonlyString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the IsReadonly or null if there is no IsReadonly available.
        /// </summary>
        [XmlIgnore]
        public bool? IsReadonly
        {
            get
            {
                if (!string.IsNullOrEmpty(this.IsReadonlyString))
                {
                    if (this.IsReadonlyString.Equals("1"))
                        return true;
                    else
                        return false;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the IsRoot.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.IsRoot)]
        public string IsRootString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the IsRoot or null if there is no IsRoot available.
        /// </summary>
        [XmlIgnore]
        public bool? IsRoot
        {
            get
            {
                if (!string.IsNullOrEmpty(this.IsRootString))
                {
                    if (this.IsRootString.Equals("1"))
                        return true;
                    else
                        return false;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the LastAccessed.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.LastAccessed)]
        public string LastAccessedString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the LastAccessed or null if there is no LastAccessed available.
        /// </summary>
        [XmlIgnore]
        public DateTime? LastAccessed
        {
            get
            {
                if (!string.IsNullOrEmpty(this.LastAccessedString))
                    return DateTime.Parse(this.LastAccessedString);
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.Name)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ParentName.
        /// </summary>
        [XmlElement(ElementName = PropNameConstants.ParentName)]
        public string ParentName
        {
            get;
            set;
        }

        #endregion IIS specific properties
    }
}
