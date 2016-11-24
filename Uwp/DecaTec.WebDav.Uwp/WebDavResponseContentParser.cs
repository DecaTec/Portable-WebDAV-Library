using DecaTec.WebDav.WebDavArtifacts;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Web.Http;

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
        /// Extracts a <see cref="DecaTec.WebDav.WebDavArtifacts.Multistatus"/> from a <see cref="System.Net.Http.HttpContent"/>.
        /// </summary>
        /// <param name="content">The HttpContent containing the <see cref="DecaTec.WebDav.WebDavArtifacts.Multistatus"/> as XML.</param>
        /// <returns>The <see cref="DecaTec.WebDav.WebDavArtifacts.Multistatus"/> object.</returns>
        public static async Task<Multistatus> ParseMultistatusResponseContentAsync(IHttpContent content)
        {
            if (content == null)
                return null;

            try
            {
                var contentString = await content.ReadAsStringAsync();
                Multistatus multistatus;

                using (Stream stream = WebDavHelper.GenerateStreamFromString(contentString))
                {
                    multistatus = (Multistatus)MultistatusSerializer.Deserialize(stream);
                }

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
        public static async Task<Prop> ParsePropResponseContentAsync(IHttpContent content)
        {
            if (content == null)
                return null;

            try
            {
                var contentString = await content.ReadAsStringAsync();
                Prop prop;

                using (Stream stream  = WebDavHelper.GenerateStreamFromString(contentString))
                {
                    prop = (Prop)PropSerializer.Deserialize(stream);
                }
                                
                return prop;
            }
            catch (Exception ex)
            {
                throw new WebDavException("Failed to parse a WebDAV Prop", ex);
            }
        }
    }
}
