using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DecaTec.WebDav.WebDavArtifacts;
using System.Net.Http;
using System.IO;
using System.Net;
using RichardSzalay.MockHttp;
using DecaTec.WebDav.UnitTest;
using System.Collections.Generic;

namespace DecaTec.WebDav.UnitIntegrationTest
{   
    [TestClass]
    public class UnitIntegrationTestWebDavClient
    {
        private const string UserName = "testuser";
        private const string Password = "testpassword";
        private const string WebDavRootFolder = @"http://127.0.0.1/webdav";

        private const string TestFile = @"TextFile1.txt";
        private const string TestFolder = "TestFolder";

        private WebDavClient CreateWebDavClient(MockHttpMessageHandler mockHandler)
        {
            var credentials = new NetworkCredential(UserName, Password);

            var httpClientHandler = new HttpClientHandler()
            {
                Credentials = credentials,
                PreAuthenticate = true
            };

            var debugHttpMessageHandler = new DebugHttpMessageHandler(httpClientHandler);
            debugHttpMessageHandler.InnerHandler = mockHandler;
            var wdc = new WebDavClient(debugHttpMessageHandler);
            return wdc;
        }

        #region Copy

        [TestMethod]
        public void UIT_WebDavClient_Copy()
        {
            var testFolderSource = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);
            var testFolderDestination = UriHelper.CombineUrl(WebDavRootFolder, TestFolder + "2", true);
            var testFile = UriHelper.CombineUrl(testFolderSource, TestFile, true);

            var mockHandler = new MockHttpMessageHandler();
            var requestHeaders = new List<KeyValuePair<string, string>>();
            requestHeaders.Add(new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString()));
            requestHeaders.Add(new KeyValuePair<string, string>(WebDavRequestHeader.Destination, testFolderDestination));
            mockHandler.When(WebDavMethod.Copy, testFolderSource).WithHeaders(requestHeaders).Respond(HttpStatusCode.Created);

            var client = CreateWebDavClient(mockHandler);
            var response = client.CopyAsync(testFolderSource, testFolderDestination).Result;
            var copyResponseSuccess = response.IsSuccessStatusCode;            

            Assert.IsTrue(copyResponseSuccess);
        }

        #endregion Copy

        #region PropFind

        [TestMethod]
        public void UIT_WebDavClient_PropFind_AllPropDepthInfinity()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";
            var requestHeaders = new List<KeyValuePair<string, string>>();
            requestHeaders.Add(new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString()));
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/file1.txt</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype>text/plain</D:getcontenttype><D:getlastmodified>Sat, 08 Apr 2017 10:08:15 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag>\"4840af950b0d21:0\"</D:getetag><D:displayname>file1.txt</D:displayname><D:getcontentlanguage/><D:getcontentlength>6</D:getcontentlength><D:iscollection>0</D:iscollection><D:creationdate>2017-04-08T10:07:48.579Z</D:creationdate><D:resourcetype/></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/test1_1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:08:00 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1_1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:42.302Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/test1_1/file2.txt</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype>text/plain</D:getcontenttype><D:getlastmodified>Sat, 08 Apr 2017 10:08:09 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag>\"25f646650b0d21:0\"</D:getetag><D:displayname>file2.txt</D:displayname><D:getcontentlanguage/><D:getcontentlength>6</D:getcontentlength><D:iscollection>0</D:iscollection><D:creationdate>2017-04-08T10:07:58.137Z</D:creationdate><D:resourcetype/></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            var client = CreateWebDavClient(mockHandler);
            PropFind pf = PropFind.CreatePropFindAllProp();
            var response = client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess);
            Assert.IsNotNull(multistatus);
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_AllPropDepthOne()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";
            var requestHeaders = new List<KeyValuePair<string, string>>();
            requestHeaders.Add(new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.One.ToString()));
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            var client = CreateWebDavClient(mockHandler);
            PropFind pf = PropFind.CreatePropFindAllProp();
            var response = client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.One, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess);
            Assert.IsNotNull(multistatus);
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_AllPropDepthZero()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";
            var requestHeaders = new List<KeyValuePair<string, string>>();
            requestHeaders.Add(new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Zero.ToString()));
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            var client = CreateWebDavClient(mockHandler);
            PropFind pf = PropFind.CreatePropFindAllProp();
            var response = client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Zero, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess);
            Assert.IsNotNull(multistatus);
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_AllPropWithXmlContentString()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";
            var requestHeaders = new List<KeyValuePair<string, string>>();
            requestHeaders.Add(new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString()));
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/file1.txt</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype>text/plain</D:getcontenttype><D:getlastmodified>Sat, 08 Apr 2017 10:08:15 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag>\"4840af950b0d21:0\"</D:getetag><D:displayname>file1.txt</D:displayname><D:getcontentlanguage/><D:getcontentlength>6</D:getcontentlength><D:iscollection>0</D:iscollection><D:creationdate>2017-04-08T10:07:48.579Z</D:creationdate><D:resourcetype/></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/test1_1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:08:00 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1_1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:42.302Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/test1_1/file2.txt</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype>text/plain</D:getcontenttype><D:getlastmodified>Sat, 08 Apr 2017 10:08:09 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag>\"25f646650b0d21:0\"</D:getetag><D:displayname>file2.txt</D:displayname><D:getcontentlanguage/><D:getcontentlength>6</D:getcontentlength><D:iscollection>0</D:iscollection><D:creationdate>2017-04-08T10:07:58.137Z</D:creationdate><D:resourcetype/></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            var client = CreateWebDavClient(mockHandler);          
            var response = client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Infinity, requestContent).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess);
            Assert.IsNotNull(multistatus);
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_NamedProperties()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:prop><D:name /></D:prop></D:propfind>";
            var requestHeaders = new List<KeyValuePair<string, string>>();
            requestHeaders.Add(new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString()));
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Fri, 07 Apr 2017 16:56:40 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            var client = CreateWebDavClient(mockHandler);
            PropFind pf = PropFind.CreatePropFindWithEmptyProperties("name");
            var response = client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess);
            Assert.IsNotNull(multistatus);
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_PropName()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:propname /></D:propfind>";
            var requestHeaders = new List<KeyValuePair<string, string>>();
            requestHeaders.Add(new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString()));
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified/><D:lockdiscovery/><D:ishidden/><D:supportedlock/><D:getetag/><D:displayname/><D:getcontentlanguage/><D:getcontentlength/><D:iscollection/><D:creationdate/><D:resourcetype/></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            var client = CreateWebDavClient(mockHandler);
            PropFind pf = PropFind.CreatePropFindWithPropName();
            var response = client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess);
            Assert.IsNotNull(multistatus);
        }

        #endregion PropFind

        #region PropPatch / put / delete file

        [TestMethod]
        public void UIT_WebDavClient_PropPatch()
        {
            var mockHandler = new MockHttpMessageHandler();

            // Put
            var requestUrl = WebDavRootFolder + @"/TextFile1.txt";
            var requestContentPut = "This is a test file for WebDAV.";            
            mockHandler.When(HttpMethod.Put, requestUrl).WithContent(requestContentPut).Respond(HttpStatusCode.Created);

            // Proppatch (set)
            var requestContentProppatchSet = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propertyupdate xmlns:D=\"DAV:\"><D:set><D:prop><D:displayname>TestFileDisplayName</D:displayname></D:prop></D:set></D:propertyupdate>";
            var responseContentProppatchSet = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/TextFile1.txt</D:href><D:propstat><D:prop><D:displayname/></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropPatch, requestUrl).WithContent(requestContentProppatchSet).Respond(HttpStatusCode.OK, new StringContent(responseContentProppatchSet));

            // Propfind
            var requestContentPropFind = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:prop><D:displayname /></D:prop></D:propfind>";
            var requestHeadersPropFind = new List<KeyValuePair<string, string>>();
            requestHeadersPropFind.Add(new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Zero.ToString()));
            var responseContentPropfind = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/TextFile1.txt</D:href><D:propstat><D:prop><D:displayname>TestFileDisplayName</D:displayname></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, requestUrl).WithHeaders(requestHeadersPropFind).WithContent(requestContentPropFind).Respond(HttpStatusCode.OK, new StringContent(responseContentPropfind));

            // Proppatch (remove)
            var requestContentProppatchRemove = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propertyupdate xmlns:D=\"DAV:\"><D:remove><D:prop><D:displayname /></D:prop></D:remove></D:propertyupdate>";
            var responseContentProppatchRemove = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/TextFile1.txt</D:href><D:propstat><D:prop><D:displayname/></D:prop><D:status>HTTP/1.1 204 No Content</D:status></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropPatch, requestUrl).WithContent(requestContentProppatchRemove).Respond(HttpStatusCode.NoContent, new StringContent(responseContentProppatchRemove));

            // Delete
            mockHandler.When(HttpMethod.Delete, requestUrl).Respond(HttpStatusCode.NoContent);

            var client = CreateWebDavClient(mockHandler);
            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);

            // Put file.
            var content = new StreamContent(File.OpenRead(TestFile));
            var response = client.PutAsync(testFile, content).Result;
            var putResponseSuccess = response.IsSuccessStatusCode;

            // PropPatch (set).
            var propertyUpdate = new PropertyUpdate();
            var set = new Set();

            var prop = new Prop()
            {
                DisplayName = "TestFileDisplayName"
            };

            set.Prop = prop;
            propertyUpdate.Items = new object[] { set };
            response = client.PropPatchAsync(testFile, propertyUpdate).Result;
            var multistatusPropPatchSet = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
            var propPatchResponseSuccess = response.IsSuccessStatusCode;

            // PropFind.
            PropFind pf = PropFind.CreatePropFindWithEmptyProperties(PropNameConstants.DisplayName);
            response = client.PropFindAsync(testFile, WebDavDepthHeaderValue.Zero, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatusPropFind = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
            var displayName = ((Propstat)multistatusPropFind.Response[0].Items[0]).Prop.DisplayName;
            var displayNameResult = "TestFileDisplayName" == displayName;

            // PropPatch (remove).
            propertyUpdate = new PropertyUpdate();
            var remove = new Remove();
            prop = Prop.CreatePropWithEmptyProperties(PropNameConstants.DisplayName);
            remove.Prop = prop;
            propertyUpdate.Items = new object[] { remove };
            response = client.PropPatchAsync(testFile, propertyUpdate).Result;
            var propPatchRemoveResponseSuccess = response.IsSuccessStatusCode;
            multistatusPropFind = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
            var multistatusResult = ((Propstat)multistatusPropFind.Response[0].Items[0]).Prop.DisplayName;

            // Delete file.
            response = client.DeleteAsync(testFile).Result;
            var deleteResponseSuccess = response.IsSuccessStatusCode;

            Assert.IsTrue(putResponseSuccess);
            Assert.IsNotNull(multistatusPropPatchSet);
            Assert.IsTrue(propPatchResponseSuccess);
            Assert.IsTrue(propFindResponseSuccess);
            Assert.IsTrue(displayNameResult);
            Assert.IsTrue(propPatchRemoveResponseSuccess);
            Assert.AreEqual(string.Empty, multistatusResult);
            Assert.IsTrue(deleteResponseSuccess);
        }

        #endregion PropPatch / put / delete file       
    }
}