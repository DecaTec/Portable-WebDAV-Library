using DecaTec.WebDav.Headers;
using DecaTec.WebDav.MessageHandlers;
using DecaTec.WebDav.Tools;
using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestWebDavSession
    {
        private const string UserName = "testuser";
        private const string Password = "testpassword";
        private const string WebDavRootFolder = @"http://127.0.0.1/webdav";

        private const string TestFile = @"TextFile1.txt";
        private const string TestFolder = "TestFolder";

        public TestContext TestContext
        {
            get;
            set;
        }

        [ClassInitialize]
        public static void ClassSetup(TestContext ctx)
        {

        }

        private WebDavSession CreateWebDavSession(MockHttpMessageHandler mockHandler)
        {
            var credentials = new NetworkCredential(UserName, Password);

            var httpClientHandler = new HttpClientHandler()
            {
                Credentials = credentials,
                PreAuthenticate = true
            };

            var debugHttpMessageHandler = new DebugHttpMessageHandler(httpClientHandler)
            {
                InnerHandler = mockHandler
            };

            var session = new WebDavSession(debugHttpMessageHandler)
            {
                BaseUri = new Uri(WebDavRootFolder, UriKind.Absolute)
            };

            return session;
        }

        #region Constructor

        [TestMethod]
        public void UT_WebDavSession_ConstructorWithDefaultHttpVersion()
        {
            var credentials = new NetworkCredential(UserName, Password);
            var session = new WebDavSession(credentials);
            Assert.AreEqual(new Version(2, 0), session.HttpVersion);
        }


        [TestMethod]
        public void UT_WebDavSession_ConstructorWithNomDefaultHttpVersion()
        {
            var credentials = new NetworkCredential(UserName, Password);
            var session = new WebDavSession(credentials, new Version(1, 1));
            Assert.AreEqual(new Version(1, 1), session.HttpVersion);
        }


        #endregion Constructor

        #region General

        [TestMethod]
        public void UT_WebDavSession_WithExpectHeaderDefault()
        {
            var mockHandler = new MockHttpMessageHandler();
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).With(req => !req.Headers.ExpectContinue.HasValue).Respond((HttpStatusCode)207, new StringContent(responseContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                var list = session.ListAsync(WebDavRootFolder).Result;

                Assert.IsNotNull(list);
            }
        }

        [TestMethod]
        public void UT_WebDavSession_WithExpectHeaderTrue()
        {
            var mockHandler = new MockHttpMessageHandler();
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).With(req => req.Headers.ExpectContinue.Value).Respond((HttpStatusCode)207, new StringContent(responseContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                session.DefaultRequestHeaders.ExpectContinue = true;
                var list = session.ListAsync(WebDavRootFolder).Result;

                Assert.IsNotNull(list);
            }
        }

        [TestMethod]
        public void UT_WebDavSession_WithExpectHeaderFalse()
        {
            var mockHandler = new MockHttpMessageHandler();
            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).With(req => !req.Headers.ExpectContinue.Value).Respond((HttpStatusCode)207, new StringContent(responseContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                session.DefaultRequestHeaders.ExpectContinue = false;
                var list = session.ListAsync(WebDavRootFolder).Result;

                Assert.IsNotNull(list);
            }
        }

        #endregion General

        #region Copy

        [TestMethod]
        public void UT_WebDavSession_Copy()
        {
            var testFolderSource = TestFolder;
            var testFolderDestination = TestFolder + "2";
            var testFolderSourceExpected = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);
            var testFolderDestinationExpected = UriHelper.CombineUrl(WebDavRootFolder, TestFolder + "2", true);

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Destination, testFolderDestinationExpected),
                new KeyValuePair<string, string>(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.NoOverwrite)
            };

            mockHandler.When(WebDavMethod.Copy, testFolderSourceExpected).WithHeaders(requestHeaders).Respond(HttpStatusCode.Created);

            using (var sesion = CreateWebDavSession(mockHandler))
            {
                var success = sesion.CopyAsync(testFolderSource, testFolderDestination).Result;

                Assert.IsTrue(success);
            }
        }

        #endregion Copy

        #region Create directory

        [TestMethod]
        public void UT_WebDavSesion_CreateDirectory()
        {
            var testFolder = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(WebDavMethod.Mkcol, testFolder).Respond(HttpStatusCode.Created);

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = session.CreateDirectoryAsync(TestFolder).Result;

                Assert.IsTrue(success);
            }
        }

        #endregion Crfeate directory

        #region Delete

        [TestMethod]
        public void UT_WebDavSession_DeleteFile()
        {
            var testFileUrl = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Delete, testFileUrl).Respond(HttpStatusCode.NoContent);

            using (var session = CreateWebDavSession(mockHandler))
            {
                var successs = session.DeleteAsync(TestFile).Result;

                Assert.IsTrue(successs);
            }
        }

        [TestMethod]
        public void UT_WebDavSession_DeleteFolder()
        {
            var testFolderUrl = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Delete, testFolderUrl).Respond(HttpStatusCode.NoContent);

            using (var session = CreateWebDavSession(mockHandler))
            {
                var successs = session.DeleteAsync(TestFolder).Result;

                Assert.IsTrue(successs);
            }
        }

        #endregion Delete

        #region Download file

        [TestMethod]
        public void UT_WebDavSession_DownloadFile()
        {
            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var downloadFileContent = "This is a downloaded file";

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Get, testFile).Respond(HttpStatusCode.OK, new StringContent(downloadFileContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = false;
                string downloadedString;

                using (var stream = new MemoryStream())
                {
                    success = session.DownloadFileAsync(TestFile, stream).Result;
                    stream.Position = 0;

                    using (StreamReader sr = new StreamReader(stream))
                    {
                        downloadedString = sr.ReadToEnd();
                    }
                }

                Assert.IsTrue(success);
                Assert.AreEqual(downloadFileContent, downloadFileContent);
            }
        }

        [TestMethod]
        public void UT_WebDavSession_DownloadFileWithProgress()
        {
            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var downloadFileContent = "This is a file downloaded with progress";
            Progress<WebDavProgress> progress = new Progress<WebDavProgress>();
            var progressHandlerIndicator = false;

            EventHandler<WebDavProgress> progressHandler = (sender, e) =>
            {
                progressHandlerIndicator = true;
            };

            progress.ProgressChanged += progressHandler;

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Get, testFile).Respond(HttpStatusCode.OK, new StringContent(downloadFileContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = false;
                string downloadedString;

                using (var stream = new MemoryStream())
                {
                    success = session.DownloadFileWithProgressAsync(TestFile, stream, progress).Result;
                    stream.Position = 0;

                    using (StreamReader sr = new StreamReader(stream))
                    {
                        downloadedString = sr.ReadToEnd();
                    }
                }

                Assert.AreEqual(downloadFileContent, downloadedString);
                Assert.IsTrue(progressHandlerIndicator);
                Assert.IsTrue(success);
            }
        }

        #endregion DownloadFile

        #region Exists

        [TestMethod]
        public void UT_WebDavSession_Exists()
        {
            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Head, testFile).Respond(HttpStatusCode.OK);

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = session.ExistsAsync(testFile).Result;

                Assert.IsTrue(success);
            }
        }

        #endregion Exists

        #region GetSupportedPropNames

        [TestMethod]
        public void UT_WebDavSession_GetSupportedPropNames()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:propname /></D:propfind>";

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Zero.ToString())
            };

            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://192.168.178.20:8080/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop xmlns:T=\"http://test.com/\"><D:getcontenttype/><D:getlastmodified/><D:lockdiscovery/><D:ishidden/><D:supportedlock/><D:getetag/><D:displayname/><D:getcontentlanguage/><D:getcontentlength/><D:iscollection/><D:creationdate/><D:resourcetype/><T:unknownproperty/></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond((HttpStatusCode)207, new StringContent(responseContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                var propNames = session.GetSupportedPropertyNamesAsync(WebDavRootFolder).Result;

                Assert.IsNotNull(propNames);
                Assert.AreEqual(13, propNames.Length);
            }
        }

        #endregion GetSupportedPropNames

        #region List

        [TestMethod]
        public void UT_WebDavSession_List()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.One.ToString())
            };

            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond((HttpStatusCode)207, new StringContent(responseContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                var list = session.ListAsync(WebDavRootFolder).Result;

                Assert.IsNotNull(list);
            }
        }

        #endregion List

        #region Lock

        [TestMethod]
        public void UT_WebDavSession_LockSingleFile()
        {
            var testFileToLock = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var lockRequestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:lockinfo xmlns:D=\"DAV:\"><D:lockscope><D:exclusive /></D:lockscope><D:locktype><D:write /></D:locktype></D:lockinfo>";
            var lockResponseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:prop xmlns:D=\"DAV:\"><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>infinity</D:depth><D:timeout>Infinite</D:timeout><D:locktoken><D:href>opaquelocktoken:d32688a4-478f-46eb-bcc4-cfe6129a207e.96bb01d2b440d8db</D:href></D:locktoken><D:lockroot><D:href>http://127.0.0.1/webdav/TextFile1.txt</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop>";
            var infiniteTimeout = WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout();
            var depth = WebDavDepthHeaderValue.Infinity;

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, depth.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Timeout, infiniteTimeout.ToString())
            };

            mockHandler.When(WebDavMethod.Lock, testFileToLock).WithHeaders(requestHeaders).WithContent(lockRequestContent).Respond(HttpStatusCode.OK, new StringContent(lockResponseContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = session.LockAsync(TestFile).Result;

                Assert.IsTrue(success);
            }
        }

        [TestMethod]
        public void UT_WebDavSession_LockRootFolder()
        {
            var lockRequestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:lockinfo xmlns:D=\"DAV:\"><D:lockscope><D:exclusive /></D:lockscope><D:locktype><D:write /></D:locktype></D:lockinfo>";
            var lockResponseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:prop xmlns:D=\"DAV:\"><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>infinity</D:depth><D:timeout>Infinite</D:timeout><D:locktoken><D:href>opaquelocktoken:d32688a4-478f-46eb-bcc4-cfe6129a207e.96bb01d2b440d8db</D:href></D:locktoken><D:lockroot><D:href>http://127.0.0.1/webdav/</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop>";
            var infiniteTimeout = WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout();
            var depth = WebDavDepthHeaderValue.Infinity;

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, depth.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Timeout, infiniteTimeout.ToString())
            };

            mockHandler.When(WebDavMethod.Lock, WebDavRootFolder).WithHeaders(requestHeaders).WithContent(lockRequestContent).Respond(HttpStatusCode.OK, new StringContent(lockResponseContent));

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = session.LockAsync(WebDavRootFolder).Result;

                Assert.IsTrue(success);
            }
        }

        #endregion Lock

        #region Move

        [TestMethod]
        public void UT_WebDavSession_Move()
        {
            var testFolderSource = TestFolder;
            var testFolderDestination = TestFolder + "2";
            var testFolderSourceExpected = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);
            var testFolderDestinationExpected = UriHelper.CombineUrl(WebDavRootFolder, TestFolder + "2", true);

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavRequestHeader.Destination, testFolderDestinationExpected),
                new KeyValuePair<string, string>(WebDavRequestHeader.Depth, WebDavDepthHeaderValue.Infinity.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.NoOverwrite)
            };

            mockHandler.When(WebDavMethod.Move, testFolderSourceExpected).WithHeaders(requestHeaders).Respond(HttpStatusCode.Created);

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = session.MoveAsync(testFolderSource, testFolderDestination).Result;

                Assert.IsTrue(success);
            }
        }

        #endregion Move

        #region Upload file

        [TestMethod]
        public void UT_WebDavSession_UploadFile()
        {
            var mockHandler = new MockHttpMessageHandler();

            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var uploadFileContent = "This is a test file for WebDAV.";
            mockHandler.When(HttpMethod.Put, testFile).WithContent(uploadFileContent).Respond(HttpStatusCode.Created);

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = false;

                using (var stream = new MemoryStream())
                {
                    using (StreamWriter wr = new StreamWriter(stream))
                    {
                        wr.Write(uploadFileContent);
                        wr.Flush();
                        stream.Position = 0;
                        success = session.UploadFileAsync(TestFile, stream).Result;
                    }
                }

                Assert.IsTrue(success);
            }
        }

        [TestMethod]
        public void UT_WebDavSession_UploadFileWithProgress()
        {
            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var uploadFileContent = "This is a file uploaded with progress";
            var contentType = "application/octet-stream";
            Progress<WebDavProgress> progress = new Progress<WebDavProgress>();
            var progressHandlerIndicator = false;

            EventHandler<WebDavProgress> progressHandler = (sender, e) =>
            {
                progressHandlerIndicator = true;
            };

            progress.ProgressChanged += progressHandler;

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(HttpHeaderNames.ContentType, contentType)
            };

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Put, testFile).WithHeaders(requestHeaders).Respond(HttpStatusCode.Created);

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = false;

                using (var stream = new MemoryStream())
                {
                    using (StreamWriter wr = new StreamWriter(stream))
                    {
                        wr.Write(uploadFileContent);
                        wr.Flush();
                        stream.Position = 0;
                        success = session.UploadFileWithProgressAsync(TestFile, stream, contentType, progress).Result;
                    }
                }

                Assert.IsTrue(progressHandlerIndicator);
                Assert.IsTrue(success);
            }
        }

        #endregion Upload file

        #region Unlock

        [TestMethod]
        public void UT_WebDavSesion_Unlock()
        {
            // We have to lock before we can unlock because the lock token information 
            // is managed by the WebDavSession internally.
            var testFileToLock = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var lockRequestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:lockinfo xmlns:D=\"DAV:\"><D:lockscope><D:exclusive /></D:lockscope><D:locktype><D:write /></D:locktype></D:lockinfo>";
            var lockResponseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:prop xmlns:D=\"DAV:\"><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>infinity</D:depth><D:timeout>Infinite</D:timeout><D:locktoken><D:href>opaquelocktoken:d32688a4-478f-46eb-bcc4-cfe6129a207e.96bb01d2b440d8db</D:href></D:locktoken><D:lockroot><D:href>http://127.0.0.1/webdav/TextFile1.txt</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop>";
            var infiniteTimeout = WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout();
            var depth = WebDavDepthHeaderValue.Infinity;
            var lockTokenString = "<opaquelocktoken:d32688a4-478f-46eb-bcc4-cfe6129a207e.96bb01d2b440d8db>";

            var mockHandler = new MockHttpMessageHandler();

            var requestHeadersLock = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, depth.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Timeout, infiniteTimeout.ToString())
            };

            mockHandler.When(WebDavMethod.Lock, testFileToLock).WithHeaders(requestHeadersLock).WithContent(lockRequestContent).Respond(HttpStatusCode.OK, new StringContent(lockResponseContent));

            var requestHeadersUnlock = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavRequestHeader.LockToken, lockTokenString)
            };

            mockHandler.When(WebDavMethod.Unlock, testFileToLock).WithHeaders(requestHeadersUnlock).Respond(HttpStatusCode.NoContent);

            using (var session = CreateWebDavSession(mockHandler))
            {
                var success = session.LockAsync(TestFile).Result;

                Assert.IsTrue(success);
                success = session.UnlockAsync(testFileToLock).Result;

                Assert.IsTrue(success);
            }
        }

        #endregion Unlock
    }
}
