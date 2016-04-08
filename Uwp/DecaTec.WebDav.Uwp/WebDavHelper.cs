using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using Windows.Web.Http;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Helper functions for WebDAV.
    /// </summary>
    public static class WebDavHelper
    {
        /// <summary>
        /// Gets a LockToken from a WebDavResponseMessage.
        /// </summary>
        /// <param name="responseMessage">The WebDavResponseMessage whose LockToken should be retrieved.</param>
        /// <returns>The LockToken of the WebDavResponseMessage or null if the WebDavResponseMessage does not contain a lock token.</returns>
        public static LockToken GetLockTokenFromWebDavResponseMessage(HttpResponseMessage responseMessage)
        {
            // Try to get lock token from response header.
            string lockTokenHeaderValue;
            var success = responseMessage.Headers.TryGetValue(WebDavRequestHeader.LockTocken, out lockTokenHeaderValue);

            if (!success)
                return new LockToken(lockTokenHeaderValue);

            // If lock token was not submitted by response header, it should be found in the response content.
            try
            {
                var prop = WebDavResponseContentParser.ParsePropResponseContentAsync(responseMessage.Content).Result;
                return new LockToken(prop.LockDiscovery.ActiveLock[0].LockToken.Href);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a UTF-8 encoded string by serializing the object specified.
        /// </summary>
        /// <param name="xmlSerializer">The XmlSerializer to use.</param>
        /// <param name="objectToSerialize">The object which should be serialized.</param>
        /// <returns>A UTF-8 encoded string containing the serializes object.</returns>
        public static string GetUtf8EncodedXmlWebDavRequestString(XmlSerializer xmlSerializer, object objectToSerialize)
        {
            var xmlNamespace = new KeyValuePair<string, string>[1];
            xmlNamespace[0] = new KeyValuePair<string, string>("D", "DAV:");
            return GetUtf8EncodedXmlWebDavRequestString(xmlSerializer, objectToSerialize, xmlNamespace);
        }

        /// <summary>
        /// Gets a UTF-8 encoded string by serializing the object specified.
        /// </summary>
        /// <param name="xmlSerializer">The XmlSerializer to use.</param>
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

                    // Allways add WebDAV namespace.
                    xnameSpace.Add("D", "DAV:");

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
        /// Gets a stream from a string.
        /// </summary>
        /// <param name="s">The string to get the corresponding stream from.</param>
        /// <returns>The stream of the string.</returns>
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
