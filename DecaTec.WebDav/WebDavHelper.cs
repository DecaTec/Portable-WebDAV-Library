using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Helper functions for WebDAV.
    /// </summary>
    public static class WebDavHelper
    {
        /// <summary>
        /// Gets a UTF-8 encoded string by serializing the object specified.
        /// </summary>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer"/> to use.</param>
        /// <param name="objectToSerialize">The object which should be serialized.</param>
        /// <returns>A UTF-8 encoded string containing the serializes object.</returns>
        public static string GetUtf8EncodedXmlWebDavRequestString(XmlSerializer xmlSerializer, object objectToSerialize)
        {
            var xmlNamespace = new KeyValuePair<string, string>[1];
            xmlNamespace[0] = new KeyValuePair<string, string>("D", WebDavConstants.DAV);
            return GetUtf8EncodedXmlWebDavRequestString(xmlSerializer, objectToSerialize, xmlNamespace);
        }

        /// <summary>
        /// Gets a UTF-8 encoded string by serializing the object specified.
        /// </summary>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer"/> to use.</param>
        /// <param name="objectToSerialize">The object which should be serialized.</param>
        /// <param name="xmlNamespaces">The namespaces to include.</param>
        /// <returns>A UTF-8 encoded string containing the serializes object.</returns>
        public static string GetUtf8EncodedXmlWebDavRequestString(XmlSerializer xmlSerializer, object objectToSerialize, KeyValuePair<string, string>[] xmlNamespaces)
        {
            try
            {
                using (var mStream = new MemoryStream())
                {
                    var xnameSpace = new XmlSerializerNamespaces();

                    foreach (var kvp in xmlNamespaces)
                    {
                        xnameSpace.Add(kvp.Key, kvp.Value);
                    }

                    // Always add WebDAV namespace.
                    xnameSpace.Add("D", WebDavConstants.DAV);

                    var utf8Encoding = new UTF8Encoding();
                    var xmlWriter = XmlWriter.Create(mStream, new XmlWriterSettings() { Encoding = utf8Encoding });
                    xmlSerializer.Serialize(xmlWriter, objectToSerialize, xnameSpace);
                    byte[] bArr = mStream.ToArray();
                    return utf8Encoding.GetString(bArr, 0, bArr.Length);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a <see cref="LockToken"/> from a <see cref="WebDavResponseMessage"/>.
        /// </summary>
        /// <param name="responseMessage">The <see cref="WebDavResponseMessage"/> whose <see cref="LockToken"/> should be retrieved.</param>
        /// <returns>The <see cref="LockToken"/> of the <see cref="WebDavResponseMessage"/> or null if the WebDavResponseMessage does not contain a lock token.</returns>
        public static LockToken GetLockTokenFromWebDavResponseMessage(WebDavResponseMessage responseMessage)
        {
            // Try to get lock token from response header.
            if (responseMessage.Headers.TryGetValues(WebDavRequestHeader.LockToken, out IEnumerable<string> lockTokenHeaderValues))
            {
                // We assume only one Lock-Token header is sent, based on the spec: https://tools.ietf.org/html/rfc4918#section-9.10.1
                var lockTokenHeaderValue = lockTokenHeaderValues.FirstOrDefault();

                // Make sure the lockTokenHeaderValue is valid according to spec (https://tools.ietf.org/html/rfc4918#section-10.5).
                if (lockTokenHeaderValue != null && CodedUrl.TryParse(lockTokenHeaderValue, out var codedUrl))
                    return new LockToken(codedUrl.AbsoluteUri);
            }

            // If lock token was not submitted by response header, it should be found in the response content.
            try
            {
                var prop = WebDavResponseContentParser.ParsePropResponseContentAsync(responseMessage.Content).Result;
                var href = prop.LockDiscovery.ActiveLock[0].LockToken.Href;

                if (AbsoluteUri.TryParse(href, out var absoluteUri))
                    return new LockToken(absoluteUri);
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }
    }
}
