using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Net.Http;
using DecaTec.WebDav.WebDavArtifacts;
using DecaTec.WebDav.Headers;
using DecaTec.WebDav.Exceptions;

namespace DecaTec.WebDav.Tools
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

        /// <summary>
        /// Gets the <see cref="ActiveLock"/> from a <see cref="WebDavResponseMessage"/>.
        /// </summary>
        /// <param name="responseMessage">The <see cref="WebDavResponseMessage"/> whose <see cref="ActiveLock"/> should be retrieved.</param>
        /// <returns>The <see cref="ActiveLock"/> of the <see cref="WebDavResponseMessage"/> or null if the <see cref="WebDavResponseMessage"/> does not contain a lock token.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="responseMessage"/> is null.</exception>
        public static ActiveLock GetActiveLockFromWebDavResponseMessage(WebDavResponseMessage responseMessage)
        {
            if (responseMessage == null)
                throw new ArgumentNullException(nameof(responseMessage));

            var prop = WebDavResponseContentParser.ParsePropResponseContentAsync(responseMessage.Content).Result;
            var activeLock = prop.LockDiscovery?.ActiveLock.FirstOrDefault();

            if (activeLock == null)
                return null;

            // If lock token was not be found in the response content, it should be submitted by response header.
            if (activeLock.LockToken == null)
            {
                // Try to get lock token from response header.
                if (responseMessage.Headers.TryGetValues(WebDavRequestHeader.LockToken, out IEnumerable<string> lockTokenHeaderValues))
                {
                    // We assume only one Lock-Token header is sent, based on the spec: https://tools.ietf.org/html/rfc4918#section-9.10.1
                    var lockTokenHeaderValue = lockTokenHeaderValues.FirstOrDefault();

                    // Make sure the lockTokenHeaderValue is valid according to spec (https://tools.ietf.org/html/rfc4918#section-10.5).
                    if (lockTokenHeaderValue != null && CodedUrl.TryParse(lockTokenHeaderValue, out var _))
                        activeLock.LockToken = new WebDavLockToken { Href = lockTokenHeaderValue };
                }
            }

            return activeLock;
        }

        /// <summary>
        /// Extracts the property names (known and unknown) from a <see cref="HttpContent"/>.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/> containing the <see cref="Multistatus"/> as XML.</param>
        /// <returns>The <see cref="Task"/>t representing the asynchronous operation.</returns>
        public static async Task<string[]> GetPropertyNamesKnownAndUnknownFromMultiStatusContentAsync(HttpContent content)
        {
            if (content == null)
                return null;

            try
            {
                var propertyList = new HashSet<string>();
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(content);

                foreach (var response in multistatus.Response)
                {
                    foreach (var item in response.Items)
                    {
                        var propStat = item as Propstat;

                        if (propStat == null)
                            continue;

                        var prop = propStat.Prop;

                        // Add known properties.
                        if (prop.ChildCountString != null)
                            propertyList.Add(PropNameConstants.ChildCount);

                        if (prop.ContentClass != null)
                            propertyList.Add(PropNameConstants.ContentClass);

                        if (prop.CreationDateString != null)
                            propertyList.Add(PropNameConstants.CreationDate);

                        if (prop.DefaultDocument != null)
                            propertyList.Add(PropNameConstants.DefaultDocument);

                        if (prop.DisplayName != null)
                            propertyList.Add(PropNameConstants.DisplayName);

                        if (prop.GetContentLanguage != null)
                            propertyList.Add(PropNameConstants.GetContentLanguage);

                        if (prop.GetContentLengthString != null)
                            propertyList.Add(PropNameConstants.GetContentLength);

                        if (prop.GetContentType != null)
                            propertyList.Add(PropNameConstants.GetContentType);

                        if (prop.GetEtag != null)
                            propertyList.Add(PropNameConstants.GetEtag);

                        if (prop.GetLastModifiedString != null)
                            propertyList.Add(PropNameConstants.GetLastModified);

                        if (prop.HasSubsString != null)
                            propertyList.Add(PropNameConstants.HasSubs);

                        if (prop.HrefString != null)
                            propertyList.Add(PropNameConstants.Href);

                        if (prop.Id != null)
                            propertyList.Add(PropNameConstants.Id);

                        if (prop.IsFolderString != null)
                            propertyList.Add(PropNameConstants.IsFolder);

                        if (prop.IsHiddenString != null)
                            propertyList.Add(PropNameConstants.IsHidden);

                        if (prop.IsReadonlyString != null)
                            propertyList.Add(PropNameConstants.IsReadonly);

                        if (prop.IsRootString != null)
                            propertyList.Add(PropNameConstants.IsRoot);

                        if (prop.IsStructuredDocumentString != null)
                            propertyList.Add(PropNameConstants.IsStructuredDocument);

                        if (prop.LastAccessedString != null)
                            propertyList.Add(PropNameConstants.LastAccessed);

                        if (prop.LockDiscovery != null)
                            propertyList.Add(PropNameConstants.LockDiscovery);

                        if (prop.Name != null)
                            propertyList.Add(PropNameConstants.Name);

                        if (prop.NoSubsString != null)
                            propertyList.Add(PropNameConstants.NoSubs);

                        if (prop.ObjectCountString != null)
                            propertyList.Add(PropNameConstants.ObjectCount);

                        if (prop.ParentName != null)
                            propertyList.Add(PropNameConstants.ParentName);

                        if (prop.QuotaAvailableBytesString != null)
                            propertyList.Add(PropNameConstants.QuotaAvailableBytes);

                        if (prop.QuotaUsedBytesString != null)
                            propertyList.Add(PropNameConstants.QuotaUsedBytes);

                        if (prop.ReservedString != null)
                            propertyList.Add(PropNameConstants.Reserved);

                        if (prop.ResourceType != null)
                            propertyList.Add(PropNameConstants.ResourceType);

                        if (prop.SupportedLock != null)
                            propertyList.Add(PropNameConstants.SupportedLock);

                        if (prop.VisibleCountString != null)
                            propertyList.Add(PropNameConstants.VisibleCount);

                        // Add unknown properties.
                        if (prop.AdditionalProperties != null)
                        {
                            foreach (var unknownElement in prop.AdditionalProperties)
                            {
                                propertyList.Add(unknownElement.Name.LocalName);
                            }
                        }
                    }
                }

                return propertyList.ToArray();
            }
            catch (Exception ex)
            {
                throw new WebDavException("Failed to retrieve property names from the HttpContent.", ex);
            }
        }
    }
}
