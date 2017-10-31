using DecaTec.WebDav.Exceptions;
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
using System.Threading;
using System.Threading.Tasks;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestWebDavClient
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

        private WebDavClient CreateWebDavClient(MockHttpMessageHandler mockHandler)
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

            var wdc = new WebDavClient(debugHttpMessageHandler);
            return wdc;
        }

        #region Constructor

        [TestMethod]
        public void UT_WebDavClient_ConstructorWithDefaultHttpVersion()
        {
            var client = new WebDavClient();
            Assert.AreEqual(new Version(1, 1), client.HttpVersion);
        }

        [TestMethod]
        public void UT_WebDavClient_ConstructorWithNonDefaultHttpVersion()
        {
            var client = new WebDavClient(new Version(2, 0));
            Assert.AreEqual(new Version(2, 0), client.HttpVersion);
        }

        #endregion Constructor

        #region General

        [TestMethod]
        public void UT_WebDavClient_LockAsync_WithDepthOne()
        {
            using (var client = CreateWebDavClient(new MockHttpMessageHandler()))
            {
                var lockInfo = new LockInfo();
                Assert.ThrowsExceptionAsync<WebDavException>(() => client.LockAsync(WebDavRootFolder, WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(), WebDavDepthHeaderValue.One, lockInfo));
            }
        }

        [TestMethod]
        public void UT_WebDavClient_RefreshLockAsync_WithoutLockToken()
        {
            using (var client = CreateWebDavClient(new MockHttpMessageHandler()))
            {
                var lockInfo = new LockInfo();
                Assert.ThrowsExceptionAsync<WebDavException>(() => client.RefreshLockAsync(WebDavRootFolder, WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(), null));
            }
        }

        [TestMethod]
        public void UT_WebDavClient_PropFindAsync_WithoutDepth()
        {
            using (var client = CreateWebDavClient(new MockHttpMessageHandler()))
            {
                Assert.ThrowsExceptionAsync<WebDavException>(() => client.PropFindAsync(WebDavRootFolder, null));
            }
        }

        [TestMethod]
        public void UT_WebDavClient_UnLockAsync_WithoutLockToken()
        {
            using (var client = CreateWebDavClient(new MockHttpMessageHandler()))
            {
                Assert.ThrowsExceptionAsync<WebDavException>(() => client.UnlockAsync(WebDavRootFolder, null));
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_WithHttpVersionDefault()
        {
            var mockHandler = new MockHttpMessageHandler();
            var defaultHttpVersion = new Version(1, 1);

            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).With(req => req.Version == defaultHttpVersion).Respond(HttpStatusCode.OK);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.PropFindAsync(WebDavRootFolder);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_WithHttpVersion11()
        {
            var mockHandler = new MockHttpMessageHandler();
            var defaultHttpVersion = new Version(1, 1);

            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).With(req => req.Version == defaultHttpVersion).Respond(HttpStatusCode.OK);

            using (var client = CreateWebDavClient(mockHandler))
            {
                client.HttpVersion = new Version(1, 1);
                var response = await client.PropFindAsync(WebDavRootFolder);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion General

        #region Copy

        [TestMethod]
        public async Task UT_WebDavClient_Copy()
        {
            var testFolderSource = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);
            var testFolderDestination = UriHelper.CombineUrl(WebDavRootFolder, TestFolder + "2", true);

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Destination, testFolderDestination),
                new KeyValuePair<string, string>(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.NoOverwrite)
            };

            mockHandler.When(WebDavMethod.Copy, testFolderSource).WithHeaders(requestHeaders).Respond(HttpStatusCode.Created);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.CopyAsync(testFolderSource, testFolderDestination);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Copy

        #region Delete

        [TestMethod]
        public async Task UT_WebDavClient_Delete()
        {
            var testFileUrl = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Delete, testFileUrl).Respond(HttpStatusCode.NoContent);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.DeleteAsync(testFileUrl);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Delete

        #region Download file

        [TestMethod]
        public async Task UT_WebDavClient_DownloadFileWithProgress()
        {
            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var downloadFileContent = "This is a file downloaded with progress";
            Progress<WebDavProgress> progress = new Progress<WebDavProgress>();
            var progressHandlerIndicator = false;

            progress.ProgressChanged += (sender, e) =>
            {
                progressHandlerIndicator = true;
            };

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Get, testFile).Respond(HttpStatusCode.OK, new StringContent(downloadFileContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                WebDavResponseMessage response;
                string downloadedString;

                using (var stream = new MemoryStream())
                {
                    response = await client.DownloadFileWithProgressAsync(testFile, stream, CancellationToken.None, progress);
                    stream.Position = 0;

                    using (StreamReader sr = new StreamReader(stream))
                    {
                        downloadedString = await sr.ReadToEndAsync();
                    }
                }

                Assert.AreEqual(downloadFileContent, downloadedString);
                Assert.IsTrue(progressHandlerIndicator);
                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Download file

        #region Get

        [TestMethod]
        public async Task UT_WebDavClient_Get()
        {
            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var downloadFileContent = "This is a file downloaded with progress";

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Get, testFile).Respond(HttpStatusCode.OK, new StringContent(downloadFileContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.GetAsync(testFile);
                var responseContentString = await response.Content.ReadAsStringAsync();

                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.AreEqual(downloadFileContent, responseContentString);
            }
        }

        #endregion Get

        #region Head

        [TestMethod]
        public async Task UT_WebDavClient_Head()
        {
            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(HttpMethod.Head, testFile).Respond(HttpStatusCode.OK);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.HeadAsync(testFile);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Head

        #region Lock

        [TestMethod]
        public async Task UT_WebDavClient_LockSingleFile()
        {
            var testFileToLock = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var lockRequestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:lockinfo xmlns:D=\"DAV:\"><D:lockscope><D:exclusive /></D:lockscope><D:locktype><D:write /></D:locktype><D:owner><D:href>test@test.com</D:href></D:owner></D:lockinfo>";
            var lockResponseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:prop xmlns:D=\"DAV:\"><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>0</D:depth><D:owner><D:href>test@test.com</D:href></D:owner><D:timeout>Second-120</D:timeout><D:locktoken><D:href>opaquelocktoken:a2324814-cbe3-4fb4-9c55-cba99a62ef5d.008f01d2b2dafb9e</D:href></D:locktoken><D:lockroot><D:href>http://127.0.0.1/webdav/TextFile1.txt</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop>";
            var oneMinuteTimeout = WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromMinutes(1));
            var depth = WebDavDepthHeaderValue.Zero;

            var lockInfo = new LockInfo()
            {
                LockScope = LockScope.CreateExclusiveLockScope(),
                LockType = LockType.CreateWriteLockType(),
                OwnerHref = "test@test.com"
            };

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, depth.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Timeout, oneMinuteTimeout.ToString())
            };

            mockHandler.When(WebDavMethod.Lock, testFileToLock).WithHeaders(requestHeaders).WithContent(lockRequestContent).Respond(HttpStatusCode.OK, new StringContent(lockResponseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.LockAsync(testFileToLock, oneMinuteTimeout, depth, lockInfo);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_LockRootFolder()
        {
            var lockRequestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:lockinfo xmlns:D=\"DAV:\"><D:lockscope><D:exclusive /></D:lockscope><D:locktype><D:write /></D:locktype><D:owner><D:href>test@test.com</D:href></D:owner></D:lockinfo>";
            var lockResponseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:prop xmlns:D=\"DAV:\"><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>infinity</D:depth><D:owner><D:href>test@test.com</D:href></D:owner><D:timeout>Second-60</D:timeout><D:locktoken><D:href>opaquelocktoken:a2324814-cbe3-4fb4-9c55-cba99a62ef5d.008f01d2b2dafba0</D:href></D:locktoken><D:lockroot><D:href>http://127.0.0.1/webdav/</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop>";
            var oneMinuteTimeout = WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromMinutes(1));
            var depth = WebDavDepthHeaderValue.Infinity;

            var lockInfo = new LockInfo()
            {
                LockScope = LockScope.CreateExclusiveLockScope(),
                LockType = LockType.CreateWriteLockType(),
                OwnerHref = "test@test.com"
            };

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, depth.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Timeout, oneMinuteTimeout.ToString())
            };

            mockHandler.When(WebDavMethod.Lock, WebDavRootFolder).WithHeaders(requestHeaders).WithContent(lockRequestContent).Respond(HttpStatusCode.OK, new StringContent(lockResponseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.LockAsync(WebDavRootFolder, oneMinuteTimeout, depth, lockInfo);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        [TestMethod]
        public void UT_WebDavClient_LockWithDepthOneShouldThrowException_ShouldThrowWebDavException()
        {
            var testFileToLock = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var oneMinuteTimeout = WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromMinutes(1));
            var depth = WebDavDepthHeaderValue.One;

            var lockInfo = new LockInfo()
            {
                LockScope = LockScope.CreateExclusiveLockScope(),
                LockType = LockType.CreateWriteLockType(),
                OwnerHref = "test@test.com"
            };

            using (var client = CreateWebDavClient(new MockHttpMessageHandler()))
            {
                Assert.ThrowsExceptionAsync<WebDavException>(() => client.LockAsync(testFileToLock, oneMinuteTimeout, depth, lockInfo));
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_LockRefreshLock()
        {
            var lockResponseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:prop xmlns:D=\"DAV:\"><D:lockdiscovery><D:activelock><D:locktype><D:write/></D:locktype><D:lockscope><D:exclusive/></D:lockscope><D:depth>infinity</D:depth><D:owner><D:href>test@test.com</D:href></D:owner><D:timeout>Second-10</D:timeout><D:locktoken><D:href>opaquelocktoken:0af5a3d3-2ccd-42fb-b8c7-9c59c9b90944.22bc01d2b2e0e947</D:href></D:locktoken><D:lockroot><D:href>http://127.0.0.1/webdav/</D:href></D:lockroot></D:activelock></D:lockdiscovery></D:prop>";
            var oneMinuteTimeout = WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromMinutes(1));
            var lockTokenString = "(<opaquelocktoken:0af5a3d3-2ccd-42fb-b8c7-9c59c9b90944.22bc01d2b2e0e947>)";
            var parseResult = NoTagList.TryParse(lockTokenString, out NoTagList noTagList);
            Assert.IsTrue(parseResult);
            var lockToken = new LockToken(noTagList.CodedUrl.AbsoluteUri);

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavRequestHeader.Timeout, oneMinuteTimeout.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.If, lockTokenString)
            };

            mockHandler.When(WebDavMethod.Lock, WebDavRootFolder).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(lockResponseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.RefreshLockAsync(WebDavRootFolder, oneMinuteTimeout, lockToken);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Lock

        #region Mkcol

        [TestMethod]
        public async Task UT_WebDavClient_Mkcol()
        {
            var testFolder = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.When(WebDavMethod.Mkcol, testFolder).Respond(HttpStatusCode.Created);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.MkcolAsync(testFolder);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Mkcol

        #region Move

        [TestMethod]
        public async Task UT_WebDavClient_Move()
        {
            var testFolderSource = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);
            var testFolderDestination = UriHelper.CombineUrl(WebDavRootFolder, TestFolder + "Dest", true);

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavRequestHeader.Destination, testFolderDestination),
                new KeyValuePair<string, string>(WebDavRequestHeader.Depth, WebDavDepthHeaderValue.Infinity.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.NoOverwrite)
            };

            mockHandler.When(WebDavMethod.Move, testFolderSource).WithHeaders(requestHeaders).Respond(HttpStatusCode.Created);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.MoveAsync(testFolderSource, testFolderDestination);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_Move_WithOverwrite()
        {
            var testFolderSource = UriHelper.CombineUrl(WebDavRootFolder, TestFolder, true);
            var testFolderDestination = UriHelper.CombineUrl(WebDavRootFolder, TestFolder + "Dest", true);

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavRequestHeader.Destination, testFolderDestination),
                new KeyValuePair<string, string>(WebDavRequestHeader.Depth, WebDavDepthHeaderValue.Infinity.ToString()),
                new KeyValuePair<string, string>(WebDavRequestHeader.Overwrite, WebDavOverwriteHeaderValue.Overwrite)
            };

            mockHandler.When(WebDavMethod.Move, testFolderSource).WithHeaders(requestHeaders).Respond(HttpStatusCode.Created);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.MoveAsync(testFolderSource, testFolderDestination, true);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Move        

        #region PropFind

        [TestMethod]
        public async Task UT_WebDavClient_PropFind_AllPropDepthInfinity()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString())
            };

            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/file1.txt</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype>text/plain</D:getcontenttype><D:getlastmodified>Sat, 08 Apr 2017 10:08:15 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag>\"4840af950b0d21:0\"</D:getetag><D:displayname>file1.txt</D:displayname><D:getcontentlanguage/><D:getcontentlength>6</D:getcontentlength><D:iscollection>0</D:iscollection><D:creationdate>2017-04-08T10:07:48.579Z</D:creationdate><D:resourcetype/></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/test1_1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:08:00 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1_1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:42.302Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/test1_1/file2.txt</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype>text/plain</D:getcontenttype><D:getlastmodified>Sat, 08 Apr 2017 10:08:09 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag>\"25f646650b0d21:0\"</D:getetag><D:displayname>file2.txt</D:displayname><D:getcontentlanguage/><D:getcontentlength>6</D:getcontentlength><D:iscollection>0</D:iscollection><D:creationdate>2017-04-08T10:07:58.137Z</D:creationdate><D:resourcetype/></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = await client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_PropFind_AllPropDepthOne()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.One.ToString())
            };

            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = await client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.One, pf);
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_PropFind_AllPropDepthZero()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Zero.ToString())
            };

            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = await client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Zero, pf);
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_PropFind_AllPropWithXmlContentString()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:allprop /></D:propfind>";

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString())
            };

            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:38 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:54 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:32.205Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test2/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:07:35 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test2</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:35.866Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/file1.txt</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype>text/plain</D:getcontenttype><D:getlastmodified>Sat, 08 Apr 2017 10:08:15 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag>\"4840af950b0d21:0\"</D:getetag><D:displayname>file1.txt</D:displayname><D:getcontentlanguage/><D:getcontentlength>6</D:getcontentlength><D:iscollection>0</D:iscollection><D:creationdate>2017-04-08T10:07:48.579Z</D:creationdate><D:resourcetype/></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/test1_1/</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Sat, 08 Apr 2017 10:08:00 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>test1_1</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-08T10:07:42.302Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response><D:response><D:href>http://127.0.0.1/webdav/test1/test1_1/file2.txt</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype>text/plain</D:getcontenttype><D:getlastmodified>Sat, 08 Apr 2017 10:08:09 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag>\"25f646650b0d21:0\"</D:getetag><D:displayname>file2.txt</D:displayname><D:getcontentlanguage/><D:getcontentlength>6</D:getcontentlength><D:iscollection>0</D:iscollection><D:creationdate>2017-04-08T10:07:58.137Z</D:creationdate><D:resourcetype/></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Infinity, requestContent);
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_PropFind_NamedProperties()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:prop><D:name /></D:prop></D:propfind>";

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString())
            };

            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified>Fri, 07 Apr 2017 16:56:40 GMT</D:getlastmodified><D:lockdiscovery/><D:ishidden>0</D:ishidden><D:supportedlock><D:lockentry><D:lockscope><D:exclusive/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry><D:lockentry><D:lockscope><D:shared/></D:lockscope><D:locktype><D:write/></D:locktype></D:lockentry></D:supportedlock><D:getetag/><D:displayname>/</D:displayname><D:getcontentlanguage/><D:getcontentlength>0</D:getcontentlength><D:iscollection>1</D:iscollection><D:creationdate>2017-04-06T09:32:20.983Z</D:creationdate><D:resourcetype><D:collection/></D:resourcetype></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                PropFind pf = PropFind.CreatePropFindWithEmptyProperties("name");
                var response = await client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_PropFind_PropName()
        {
            var mockHandler = new MockHttpMessageHandler();
            var requestContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propfind xmlns:D=\"DAV:\"><D:propname /></D:propfind>";

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavConstants.Depth, WebDavDepthHeaderValue.Infinity.ToString())
            };

            var responseContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/webdav</D:href><D:propstat><D:status>HTTP/1.1 200 OK</D:status><D:prop><D:getcontenttype/><D:getlastmodified/><D:lockdiscovery/><D:ishidden/><D:supportedlock/><D:getetag/><D:displayname/><D:getcontentlanguage/><D:getcontentlength/><D:iscollection/><D:creationdate/><D:resourcetype/></D:prop></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropFind, WebDavRootFolder).WithContent(requestContent).WithHeaders(requestHeaders).Respond(HttpStatusCode.OK, new StringContent(responseContent));

            using (var client = CreateWebDavClient(mockHandler))
            {
                PropFind pf = PropFind.CreatePropFindWithPropName();
                var response = await client.PropFindAsync(WebDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(response.IsSuccessStatusCode);
                Assert.IsNotNull(multistatus);
            }
        }

        #endregion PropFind

        #region PropPatch

        [TestMethod]
        public async Task UT_WebDavClient_PropPatchSet()
        {
            var mockHandler = new MockHttpMessageHandler();

            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var requestContentProppatch = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propertyupdate xmlns:D=\"DAV:\"><D:set><D:prop><D:displayname>TestFileDisplayName</D:displayname></D:prop></D:set></D:propertyupdate>";
            var responseContentProppatch = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/TextFile1.txt</D:href><D:propstat><D:prop><D:displayname/></D:prop><D:status>HTTP/1.1 200 OK</D:status></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropPatch, testFile).WithContent(requestContentProppatch).Respond(HttpStatusCode.OK, new StringContent(responseContentProppatch));

            var propertyUpdate = new PropertyUpdate();
            var set = new Set();

            var prop = new Prop()
            {
                DisplayName = "TestFileDisplayName"
            };

            set.Prop = prop;
            propertyUpdate.Items = new object[] { set };

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.PropPatchAsync(testFile, propertyUpdate);
                var multistatusPropPatch = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsNotNull(multistatusPropPatch);
                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        [TestMethod]
        public async Task UT_WebDavClient_PropPatchRemove()
        {
            var mockHandler = new MockHttpMessageHandler();

            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var requestContentProppatch = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:propertyupdate xmlns:D=\"DAV:\"><D:remove><D:prop><D:displayname /></D:prop></D:remove></D:propertyupdate>";
            var responseContentProppatch = "<?xml version=\"1.0\" encoding=\"utf-8\"?><D:multistatus xmlns:D=\"DAV:\"><D:response><D:href>http://127.0.0.1/TextFile1.txt</D:href><D:propstat><D:prop><D:displayname/></D:prop><D:status>HTTP/1.1 204 No Content</D:status></D:propstat></D:response></D:multistatus>";
            mockHandler.When(WebDavMethod.PropPatch, testFile).WithContent(requestContentProppatch).Respond(HttpStatusCode.NoContent, new StringContent(responseContentProppatch));

            var propertyUpdate = new PropertyUpdate();
            var remove = new Remove();
            var prop = Prop.CreatePropWithEmptyProperties(PropNameConstants.DisplayName);
            remove.Prop = prop;
            propertyUpdate.Items = new object[] { remove };

            using (var client = new WebDavClient(mockHandler))
            {
                var response = await client.PropPatchAsync(testFile, propertyUpdate);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion PropPatch

        #region Put

        [TestMethod]
        public async Task UT_WebDavClient_Put()
        {
            var mockHandler = new MockHttpMessageHandler();

            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var requestContentPut = "This is a test file for WebDAV.";
            mockHandler.When(HttpMethod.Put, testFile).WithContent(requestContentPut).Respond(HttpStatusCode.Created);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var response = await client.PutAsync(testFile, new StringContent(requestContentPut));

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Put

        #region Send

        [TestMethod]
        public async Task UT_WebDavClient_Send()
        {
            var mockHandler = new MockHttpMessageHandler();

            var testFile = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var requestContentPut = "This is a test file for WebDAV.";
            mockHandler.When(HttpMethod.Put, testFile).WithContent(requestContentPut).Respond(HttpStatusCode.Created);

            using (var client = new WebDavClient(mockHandler))
            {

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, testFile)
                {
                    Content = new StringContent(requestContentPut)
                };

                var response = await client.SendAsync(httpRequestMessage);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Send

        #region Unlock

        [TestMethod]
        public async Task UT_WebDavClient_Unlock()
        {
            var testFileToUnlock = UriHelper.CombineUrl(WebDavRootFolder, TestFile, true);
            var lockTokenString = "<opaquelocktoken:cb2b7d6d-98ea-47cf-b569-5b98126b8f13.6df801d2b41b3b6e>";
            CodedUrl.TryParse(lockTokenString, out var codedUrl);

            var mockHandler = new MockHttpMessageHandler();

            var requestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(WebDavRequestHeader.LockToken, lockTokenString)
            };

            mockHandler.When(WebDavMethod.Unlock, testFileToUnlock).WithHeaders(requestHeaders).Respond(HttpStatusCode.NoContent);

            using (var client = CreateWebDavClient(mockHandler))
            {
                var lockToken = new LockToken(codedUrl.AbsoluteUri);
                var response = await client.UnlockAsync(testFileToUnlock, lockToken);

                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Unlock

        #region Upload file

        [TestMethod]
        public async Task UT_WebDavClient_UploadFileWithProgress()
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

            using (var client = CreateWebDavClient(mockHandler))
            {
                WebDavResponseMessage response;

                using (var stream = new MemoryStream())
                {
                    using (StreamWriter wr = new StreamWriter(stream))
                    {
                        await wr.WriteAsync(uploadFileContent);
                        await wr.FlushAsync();
                        stream.Position = 0;
                        response = await client.UploadFileWithProgressAsync(testFile, stream, contentType, progress);
                    }
                }

                Assert.IsTrue(progressHandlerIndicator);
                Assert.IsTrue(response.IsSuccessStatusCode);
            }
        }

        #endregion Upload file
    }
}
