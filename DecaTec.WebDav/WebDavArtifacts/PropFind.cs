using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DecaTec.WebDav.WebDavArtifacts
{
    /// <summary>
    /// Class representing an 'propfind' XML element for WebDAV communication.
    /// </summary>
    [DataContract]
    [XmlType(TypeName = WebDavConstants.PropFind, Namespace = WebDavConstants.DAV)]
    [XmlRoot(Namespace = WebDavConstants.DAV, IsNullable = false)]
    public class PropFind
    {
        /// <summary>
        /// Creates a PropFind instance representing an 'allprop'-Propfind.
        /// </summary>
        /// <returns>A PropFind instance containing an <see cref="DecaTec.WebDav.WebDavArtifacts.AllProp"/> element.</returns>
        /// <remarks>The number and types of properties returned by a WebDAV server upon a 'allprop-Profind' depends on the WebDAV server implementation.
        /// Not all WebDAV servers return the same set of properties upon a 'allprop-Propfind'.
        /// In order to find out which properties are supported by the server, you can use a 'propname' request. Every (string) property, which is <see cref="string.Empty"/> then is supported by the WebDAV server 
        /// (see <see cref="PropFind.CreatePropFindWithPropName"/>).</remarks>
        public static PropFind CreatePropFindAllProp()
        {
            var propFind = new PropFind()
            {
                Item = new AllProp()
            };

            return propFind;
        }

        /// <summary>
        /// Creates an empty PropFind instance. The server should return all known properties (for the server) by that empty PropFind.
        /// </summary>
        /// <returns>An empty PropFind instance.</returns>
        /// <remarks>The number and types of properties returned by a WebDAV server upon such a Propfind depends on the WebDAV server implementation.
        /// Not all WebDAV servers return the same set of properties upon a such a request. 
        /// In order to find out which properties are supported by the server, you can use a 'propname' request. Every (string) property, which is <see cref="string.Empty"/> then is supported by the WebDAV server 
        /// (see <see cref="PropFind.CreatePropFindWithPropName"/>).</remarks>
        public static PropFind CreatePropFind()
        {
            return new PropFind();
        }

        /// <summary>
        /// Creates a PropFind instance containing empty property items with the specified names. Useful for obtaining only a few properties from the server.
        /// </summary>
        /// <param name="propertyNames">The property names which should be contained in the PropFind instance.</param>
        /// <returns>A PropFind instance containing the empty <see cref="DecaTec.WebDav.WebDavArtifacts.Prop"/> items specified.</returns>
        /// <remarks>Not all WebDAV servers implement the same set of WebDAV properties. So it depends on the server implementation which requested properties are returned.
        /// If the returned <see cref="Prop"/>'s properties are null, the server probably does not support these properties.
        /// In order to find out which properties are supported by the server, you can use a 'propname' request. Every (string) property, which is <see cref="string.Empty"/> then is supported by the WebDAV server 
        /// (see <see cref="PropFind.CreatePropFindWithPropName"/>).</remarks>
        public static PropFind CreatePropFindWithEmptyProperties(params string[] propertyNames)
        {
            var propFind = new PropFind();
            var prop = Prop.CreatePropWithEmptyProperties(propertyNames);
            propFind.Item = prop;
            return propFind;
        }

        /// <summary>
        /// Creates a PropFind instance containing empty property items for all the Props defined in <see href="https://www.ietf.org/rfc/rfc4918.txt">RFC 4918</see>, <see href="https://tools.ietf.org/html/rfc4331">RFC 4331</see>, <see href="https://tools.ietf.org/html/draft-hopmann-collection-props-00">Additional WebDAV Collection Properties</see> and some IIS specific properties.
        /// </summary>
        /// <returns>A PropFind instance containing the empty <see cref="DecaTec.WebDav.WebDavArtifacts.Prop"/> items of all Props defined in <see href="https://www.ietf.org/rfc/rfc4918.txt">RFC 4918</see>, <see href="https://tools.ietf.org/html/rfc4331">RFC 4331</see>, <see href="https://tools.ietf.org/html/draft-hopmann-collection-props-00">Additional WebDAV Collection Properties</see> and some IIS specific properties.</returns>
        /// <remarks>Not all WebDAV servers implement the same set of WebDAV properties. So it depends on the server implementation which requested properties are returned.
        /// If the returned <see cref="Prop"/>'s properties are null, the server probably does not support these properties.
        /// In order to find out which properties are supported by the server, you can use a 'propname' request. Every (string) property, which is <see cref="string.Empty"/> then is supported by the WebDAV server 
        /// (see <see cref="PropFind.CreatePropFindWithPropName"/>).</remarks>
        public static PropFind CreatePropFindWithEmptyPropertiesAll()
        {
            var propFind = new PropFind();
            var prop = Prop.CreatePropWithEmptyPropertiesAll();
            propFind.Item = prop;
            return propFind;
        }

        /// <summary>
        /// Creates a PropFind instance containing a PropertyName item. This method can be used to inspect the server for supported WebDAV properties.
        /// </summary>
        /// <returns>A PropFind instance containing a <see cref="DecaTec.WebDav.WebDavArtifacts.PropName"/> item.</returns>
        /// <remarks>Upon a 'propname' request the server returns <see cref="Prop"/> items containing <see cref="string.Empty"/> for all (string) properties which are supported by the server.</remarks>
        /// <example>To retrieve all WebDAV properties supported by a specific WebDAV server, you can use following code:
        /// <code>
        /// var credentials = new NetworkCredential("MyUserName", "MyPassword");
        /// var httpClientHandler = new HttpClientHandler();
        /// httpClientHandler.Credentials = credentials;
        /// httpClientHandler.PreAuthenticate = true;
        ///
        /// // Use the HttpClientHandler to create the WebDavClient.
        /// var webDavClient = new WebDavClient(httpClientHandler);
        ///
        /// PropFind pf = PropFind.CreatePropFindWithPropName();
        /// var response = await webDavClient.PropFindAsync(@"http://www.myserver.com/webdav/", WebDavDepthHeaderValue.Infinity, pf);
        /// var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);
        ///
        /// foreach (var multistatusResponse in multistatus.Response)
        /// {
        ///     foreach (var item in multistatusResponse.Items)
        ///     {
        ///         var propStat = (Propstat)item;
        ///         var prop = propStat.Prop;
        ///
        ///         if (prop.GetLastModifiedString == string.Empty)
        ///         {
        ///             // If the (string) property is string.Empty upon such a request, the property is supported by the server.
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public static PropFind CreatePropFindWithPropName()
        {
            var propFind = new PropFind()
            {
                Item = new PropName()
            };

            return propFind;
        }

        /// <summary>
        /// Gets or sets the Item. This is either a <see cref="AllProp"/> <see cref="Prop"/> or <see cref="PropName"/> as <see cref="object"/>.
        /// </summary>
        [XmlElement(ElementName = WebDavConstants.AllProp, Type = typeof(AllProp))]
        [XmlElement(ElementName = WebDavConstants.Prop, Type = typeof(Prop))]
        [XmlElement(ElementName = WebDavConstants.PropName, Type =  typeof(PropName))]
        public object Item
        {
            get;
            set;
        }
    }
}
