using DecaTec.WebDav.WebDavArtifacts;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DecaTec.WebDav
{
    /// <summary>
    /// Class for parsing the content of WebDAV responses.
    /// </summary>
    public static class WebDavResponseContentParser
    {
        private static readonly XmlSerializer MultistatusSerializer = new XmlSerializer(typeof(Multistatus));
        private static readonly XmlSerializer PropSerializer = new XmlSerializer(typeof(Prop));

        /// <summary>
        /// Extreacts a <see cref="DecaTec.WebDav.WebDavArtifacts.Multistatus"/> from a <see cref="System.Net.Http.HttpContent"/>.
        /// </summary>
        /// <param name="content">The HttpContent containing the <see cref="DecaTec.WebDav.WebDavArtifacts.Multistatus"/> as XML.</param>
        /// <returns>The <see cref="DecaTec.WebDav.WebDavArtifacts.Multistatus"/> object.</returns>
        public static async Task<Multistatus> ParseMultistatusResponseContentAsync(HttpContent content)
        {
            if (content == null)
                return null;

            try
            {
                var contentStream = await content.ReadAsStreamAsync();
                var multistatus = (Multistatus)MultistatusSerializer.Deserialize(contentStream);
                return multistatus;
            }
            catch (Exception ex)
            {
                throw new WebDavException("Failed to parse a multistatus response", ex);
            }
        }

        /// <summary>
        /// Extracts a <see cref="DecaTec.WebDav.WebDavArtifacts.Prop"/> from a <see cref="System.Net.Http.HttpContent"/>.
        /// </summary>
        /// <param name="content">The HttpContent containing the <see cref="DecaTec.WebDav.WebDavArtifacts.Prop"/> as XML.</param>
        /// <returns>The <see cref="DecaTec.WebDav.WebDavArtifacts.Prop"/> object.</returns>
        public static async Task<Prop> ParsePropResponseContentAsync(HttpContent content)
        {
            if (content == null)
                return null;

            try
            {
                var contentStream = await content.ReadAsStreamAsync();
                var prop = (Prop)PropSerializer.Deserialize(contentStream);
                return prop;
            }
            catch (Exception ex)
            {
                throw new WebDavException("Failed to parse a WebDAV Prop", ex);
            }
        }
    }
}
