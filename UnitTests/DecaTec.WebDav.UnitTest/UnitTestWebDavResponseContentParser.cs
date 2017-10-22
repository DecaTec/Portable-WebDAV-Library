using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using DecaTec.WebDav.WebDavArtifacts;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestWebDavResponseContentParser
    {
        [TestMethod]
        public async Task UT_WebDavContentParser_ParseMultistatusResponseContentAsync()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>/container/</D:href><D:propstat><D:prop xmlns:R=\"http://ns.example.com/boxschema/\"><R:bigbox><R:BoxType>Box type A</R:BoxType></R:bigbox><R:author><R:Name>Hadrian</R:Name></R:author><D:creationdate>1997-12-01T17:42:21-08:00</D:creationdate><D:displayname>Example collection</D:displayname><D:resourcetype><D:collection/></D:resourcetype><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response><D:response><D:href>/container/front.html</D:href><D:propstat><D:prop xmlns:R=\"http://ns.example.com/boxschema/\"><R:bigbox><R:BoxType>Box type B</R:BoxType></R:bigbox><D:creationdate>1997-12-01T18:27:21-08:00</D:creationdate><D:displayname>Example HTML resource</D:displayname><D:getcontentlength>4525</D:getcontentlength><D:getcontenttype>text/html</D:getcontenttype><D:getetag>\"zzyzx\"</D:getetag><D:getlastmodified>Mon, 12 Jan 1998 09:25:56 GMT</D:getlastmodified><D:resourcetype/><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            var content = new StringContent(str);
            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(content);
            Assert.IsNotNull(multistatus);
        }

        [TestMethod]
        public async Task UT_WebDavContentParser_ParsePropResponseContentAsync()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:prop xmlns:D=\"DAV:\"><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>infinity</D:depth><D:owner><D:href>http://example.org/~ejw/contact.html</D:href></D:owner><D:timeout>Second-604800</D:timeout><D:locktoken><D:href>urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4</D:href></D:locktoken><D:lockroot><D:href>http://example.com/workspace/webdav/proposal.doc</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop>";
            var content = new StringContent(str);
            var multistatus = await WebDavResponseContentParser.ParsePropResponseContentAsync(content);

            Assert.IsNotNull(multistatus);
        }

        [TestMethod]
        public async Task UT_WebDavContentParser_ParsePropResponseContentAsyncWithHrefAndAttributeOwner()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:multistatus xmlns:D='DAV:'><D:response><D:href>http://www.example.com/container/</D:href><D:propstat><D:prop><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>0</D:depth><D:owner><D:href>http://example.org/~ejw/contact.html</D:href><x:author xmlns:x='http://example.com/ns'><x:name>Jane Doe</x:name></x:author></D:owner><D:timeout>Infinite</D:timeout><D:locktoken><D:href>urn:uuid:f81de2ad-7f3d-a1b2-4f3c-00a0c91a9d76</D:href></D:locktoken><D:lockroot><D:href>http://www.example.com/container/</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            var content = new StringContent(str);
            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(content);
            var ownerRaw = ((Propstat)multistatus.Response[0].Items[0]).Prop.LockDiscovery.ActiveLock[0].OwnerRaw;
            var ownerHref = ((Propstat)multistatus.Response[0].Items[0]).Prop.LockDiscovery.ActiveLock[0].OwnerHref;
            var ownerString = ownerRaw.ToString(SaveOptions.DisableFormatting);
            var expectedOwnerRaw = "<owner xmlns=\"DAV:\"><href>http://example.org/~ejw/contact.html</href><x:author xmlns:x=\"http://example.com/ns\"><x:name>Jane Doe</x:name></x:author></owner>";
            var expectedOwnerHref = @"http://example.org/~ejw/contact.html";

            Assert.IsNotNull(multistatus);
            Assert.AreEqual(expectedOwnerRaw, ownerString);
            Assert.AreEqual(expectedOwnerHref, ownerHref);
        }

        [TestMethod]
        public async Task UT_WebDavContentParser_ParsePropResponseContentAsyncWithRawOwner()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:multistatus xmlns:D='DAV:'><D:response><D:href>http://www.example.com/container/</D:href><D:propstat><D:prop><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>0</D:depth><D:owner>Bob</D:owner><D:timeout>Infinite</D:timeout><D:locktoken><D:href>urn:uuid:f81de2ad-7f3d-a1b2-4f3c-00a0c91a9d76</D:href></D:locktoken><D:lockroot><D:href>http://www.example.com/container/</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            var content = new StringContent(str);
            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(content);
            var ownerRaw = ((Propstat)multistatus.Response[0].Items[0]).Prop.LockDiscovery.ActiveLock[0].OwnerRaw;
            var ownerHref = ((Propstat)multistatus.Response[0].Items[0]).Prop.LockDiscovery.ActiveLock[0].OwnerHref;
            var ownerString = ownerRaw.ToString(SaveOptions.DisableFormatting);
            var expected = "<owner xmlns=\"DAV:\">Bob</owner>";
            var expectedOwnerHref = string.Empty;

            Assert.IsNotNull(multistatus);
            Assert.AreEqual(expected, ownerString);
            Assert.AreEqual(expectedOwnerHref, ownerHref);
        }

        [TestMethod]
        public async Task UT_WebDavContentParser_ParsePropResponseContentAsyncWithMixedAndAttibuteOwner()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:multistatus xmlns:D='DAV:'><D:response><D:href>http://www.example.com/container/</D:href><D:propstat><D:prop><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>0</D:depth><D:owner xmlns:x='urn:other-schema-provider:other-schema' x:myattr='test-attr'>Test<x:whatever>test</x:whatever></D:owner><D:timeout>Infinite</D:timeout><D:locktoken><D:href>urn:uuid:f81de2ad-7f3d-a1b2-4f3c-00a0c91a9d76</D:href></D:locktoken><D:lockroot><D:href>http://www.example.com/container/</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            var content = new StringContent(str);
            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(content);
            var ownerRaw = ((Propstat)multistatus.Response[0].Items[0]).Prop.LockDiscovery.ActiveLock[0].OwnerRaw;
            var ownerHref = ((Propstat)multistatus.Response[0].Items[0]).Prop.LockDiscovery.ActiveLock[0].OwnerHref;
            var ownerString = ownerRaw.ToString(SaveOptions.DisableFormatting);
            var expected = "<owner xmlns:x=\"urn:other-schema-provider:other-schema\" x:myattr=\"test-attr\" xmlns=\"DAV:\">Test<x:whatever>test</x:whatever></owner>";
            var expectedOwnerHref = string.Empty;

            Assert.IsNotNull(multistatus);
            Assert.AreEqual(expected, ownerString);
            Assert.AreEqual(expectedOwnerHref, ownerHref);
        }

        [TestMethod]
        public async Task UT_WebDavContentParser_ParseMultistatusResponseContentAsyncWithPropWithXmlLangAttribute()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>/container/</D:href><D:propstat><D:prop xml:lang=\"de\" xmlns:R=\"http://ns.example.com/boxschema/\"><R:bigbox><R:BoxType>Box type A</R:BoxType></R:bigbox><R:author><R:Name>Hadrian</R:Name></R:author><D:creationdate>1997-12-01T17:42:21-08:00</D:creationdate><D:displayname>Example collection</D:displayname><D:resourcetype><D:collection/></D:resourcetype><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response><D:response><D:href>/container/front.html</D:href><D:propstat><D:prop xmlns:R=\"http://ns.example.com/boxschema/\"><R:bigbox><R:BoxType>Box type B</R:BoxType></R:bigbox><D:creationdate>1997-12-01T18:27:21-08:00</D:creationdate><D:displayname>Example HTML resource</D:displayname><D:getcontentlength>4525</D:getcontentlength><D:getcontenttype>text/html</D:getcontenttype><D:getetag>\"zzyzx\"</D:getetag><D:getlastmodified>Mon, 12 Jan 1998 09:25:56 GMT</D:getlastmodified><D:resourcetype/><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            var content = new StringContent(str);
            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(content);
            Assert.IsNotNull(multistatus);
            var language = ((Propstat)multistatus.Response[0].Items[0]).Prop.Language;
            var expected = "de";
            Assert.AreEqual(expected, language);
        }

        [TestMethod]
        public async Task UT_WebDavContentParser_ParseMultistatusResponseContentAsyncWithPropWithInvalidXmlLangAttribute()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>/container/</D:href><D:propstat><D:prop xml:lang=\"invalid\" xmlns:R=\"http://ns.example.com/boxschema/\"><R:bigbox><R:BoxType>Box type A</R:BoxType></R:bigbox><R:author><R:Name>Hadrian</R:Name></R:author><D:creationdate>1997-12-01T17:42:21-08:00</D:creationdate><D:displayname>Example collection</D:displayname><D:resourcetype><D:collection/></D:resourcetype><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response><D:response><D:href>/container/front.html</D:href><D:propstat><D:prop xmlns:R=\"http://ns.example.com/boxschema/\"><R:bigbox><R:BoxType>Box type B</R:BoxType></R:bigbox><D:creationdate>1997-12-01T18:27:21-08:00</D:creationdate><D:displayname>Example HTML resource</D:displayname><D:getcontentlength>4525</D:getcontentlength><D:getcontenttype>text/html</D:getcontenttype><D:getetag>\"zzyzx\"</D:getetag><D:getlastmodified>Mon, 12 Jan 1998 09:25:56 GMT</D:getlastmodified><D:resourcetype/><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            var content = new StringContent(str);
            var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(content);
            Assert.IsNotNull(multistatus);
            var language = ((Propstat)multistatus.Response[0].Items[0]).Prop.Language;
            var expected = "invalid";
            Assert.AreEqual(expected, language);
        }
    }
}
