using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    [TestClass]
    public class UnitTestWebDavResponseContentParser
    {
        [TestMethod]
        public void UnitTestParseMultistatusResponseContentAsyncNetFx()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>/container/</D:href><D:propstat><D:prop xmlns:R=\"http://ns.example.com/boxschema/\"><R:bigbox><R:BoxType>Box type A</R:BoxType></R:bigbox><R:author><R:Name>Hadrian</R:Name></R:author><D:creationdate>1997-12-01T17:42:21-08:00</D:creationdate><D:displayname>Example collection</D:displayname><D:resourcetype><D:collection/></D:resourcetype><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response><D:response><D:href>/container/front.html</D:href><D:propstat><D:prop xmlns:R=\"http://ns.example.com/boxschema/\"><R:bigbox><R:BoxType>Box type B</R:BoxType></R:bigbox><D:creationdate>1997-12-01T18:27:21-08:00</D:creationdate><D:displayname>Example HTML resource</D:displayname><D:getcontentlength>4525</D:getcontentlength><D:getcontenttype>text/html</D:getcontenttype><D:getetag>\"zzyzx\"</D:getetag><D:getlastmodified>Mon, 12 Jan 1998 09:25:56 GMT</D:getlastmodified><D:resourcetype/><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            var content = new StringContent(str);
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(content).Result;

            Assert.IsNotNull(multistatus);
        }

        [TestMethod]
        public void UnitTestParsePropResponseContentAsyncNetFx()
        {
            var str = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><D:prop xmlns:D=\"DAV:\"><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>infinity</D:depth><D:owner><D:href>http://example.org/~ejw/contact.html</D:href></D:owner><D:timeout>Second-604800</D:timeout><D:locktoken><D:href>urn:uuid:e71d4fae-5dec-22d6-fea5-00a0c91e6be4</D:href></D:locktoken><D:lockroot><D:href>http://example.com/workspace/webdav/proposal.doc</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop>";
            var content = new StringContent(str);
            var multistatus = WebDavResponseContentParser.ParsePropResponseContentAsync(content).Result;

            Assert.IsNotNull(multistatus);
        }
    }
}
