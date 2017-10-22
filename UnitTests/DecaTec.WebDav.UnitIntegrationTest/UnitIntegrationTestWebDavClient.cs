using DecaTec.WebDav.Headers;
using DecaTec.WebDav.MessageHandlers;
using DecaTec.WebDav.Tools;
using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DecaTec.WebDav.UnitIntegrationTest
{
    /// <summary>
    /// Unit integration test class for WebDavClient.
    /// 
    /// IMPORTANT:  This is a playground when testing this library against a specific WebDAV server implementation.
    ///             Not all methods of WebDavCliebnt will be tested here.
    ///             For unit tests of WebDavClient (with a mocked HTTP handler), see the UnitTestWebDavClient class in the unit test project.
    ///     
    /// You'll need a file 'TestConfiguration.txt' in the test's output folder with the following content:
    /// Line 1: The user name to use for WebDAV connections
    /// Line 2: The password to use for WebDAV connections
    /// Line 3: The URL of an already existing WebDAV folder in the server used for tests
    ///  
    /// If this file is not present, all test will fail!
    /// </summary>
    [TestClass]
    public class UnitIntegrationTestWebDavClient
    {
        private static string userName;
        private static string password;
        private static string webDavRootFolder;

        private const string ConfigurationFile = @"TestConfiguration.txt";
        private const string TestFile = @"TextFile1.txt";
        private const string TestFileUnknownExtension = @"TestFile1.01";
        private const string TestCollection = "TestCollection";

        public TestContext TestContext
        {
            get;
            set;
        }

        [ClassInitialize]
        public static void ClassSetup(TestContext ctx)
        {
            try
            {
                var configuration = File.ReadAllLines(ConfigurationFile);
                userName = configuration[0];
                password = configuration[1];
                webDavRootFolder = configuration[2];
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("The configuration file cannot be found. Make sure that there is a file 'TestConfiguration.txt' in the test's output folder containing data about the WebDAV server to test against.", ConfigurationFile, ex);
            }
        }

        private WebDavClient CreateWebDavClientWithDebugHttpMessageHandler()
        {
            var credentials = new NetworkCredential(userName, password);

            var httpClientHandler = new HttpClientHandler()
            {
                // Ignore invalid SSL certificates.
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                Credentials = credentials,
                PreAuthenticate = true
            };

            var debugHttpMessageHandler = new DebugHttpMessageHandler(httpClientHandler);
            var wdc = new WebDavClient(debugHttpMessageHandler);
            return wdc;
        }

        #region Copy

        [TestMethod]
        public async Task UIT_WebDavClient_Copy()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testCollectionSource = UriHelper.CombineUrl(webDavRootFolder, TestCollection, true);
                var testCollectionDestination = UriHelper.CombineUrl(webDavRootFolder, TestCollection + "2", true);
                var testFile = UriHelper.CombineUrl(testCollectionSource, TestFile, true);

                // Create source collection.
                var response = await client.MkcolAsync(testCollectionSource);
                var mkColResponseSuccess = response.IsSuccessStatusCode;

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    response = await client.PutAsync(testFile, content);
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Copy.
                response = await client.CopyAsync(testCollectionSource, testCollectionDestination);
                var copyResponseSuccess = response.IsSuccessStatusCode;

                // PropFind.
                PropFind pf = PropFind.CreatePropFindAllProp();
                response = await client.PropFindAsync(testCollectionDestination, WebDavDepthHeaderValue.Infinity, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;

                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                bool collectionfound = false;

                foreach (var item in multistatus.Response)
                {
                    if (item.Href.EndsWith(TestFile))
                    {
                        collectionfound = true;
                        break;
                    }
                }

                // Delete source and destination.
                response = await client.DeleteAsync(testCollectionSource);
                var deleteSourceResponseSuccess = response.IsSuccessStatusCode;

                response = await client.DeleteAsync(testCollectionDestination);
                var deleteDestinationResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(mkColResponseSuccess);
                Assert.IsTrue(putResponseSuccess);
                Assert.IsTrue(copyResponseSuccess);
                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsTrue(collectionfound);
                Assert.IsTrue(deleteSourceResponseSuccess);
                Assert.IsTrue(deleteDestinationResponseSuccess);
            }
        }

        #endregion Copy

        #region PropFind

        [TestMethod]
        public async Task UIT_WebDavClient_PropFind_AllPropDepthInfinity()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavClient_PropFind_AllPropDepthOne()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.One, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavClient_PropFind_AllPropDepthZero()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Zero, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavClient_PropFind_NamedProperties()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindWithEmptyProperties("name");
                var response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavClient_PropFind_NamedPropertiesAll()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindWithEmptyPropertiesAll();
                var response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavClient_PropFind_PropName()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindWithPropName();
                var response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        #endregion PropFind

        #region PropPatch / put / delete file

        [TestMethod]
        public async Task UIT_WebDavClient_PropPatch()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testFile = UriHelper.CombineUrl(webDavRootFolder, TestFile, true);
                var putResponseSuccess = false;

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    var responsePut = await client.PutAsync(testFile, content);
                    putResponseSuccess = responsePut.IsSuccessStatusCode;
                }

                // PropPatch (set).
                var propertyUpdate = new PropertyUpdate();
                var set = new Set();

                var prop = new Prop()
                {
                    DisplayName = "TestFileDisplayName"
                };

                set.Prop = prop;
                propertyUpdate.Items = new object[] { set };
                var response = await client.PropPatchAsync(testFile, propertyUpdate);
                var multistatusPropPatchSet = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);
                var propPatchResponseSuccess = response.IsSuccessStatusCode;

                // PropFind.
                PropFind pf = PropFind.CreatePropFindWithEmptyProperties(PropNameConstants.DisplayName);
                response = await client.PropFindAsync(testFile, WebDavDepthHeaderValue.Zero, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatusPropFind = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);
                var displayName = ((Propstat)multistatusPropFind.Response[0].Items[0]).Prop.DisplayName;
                // IIS ignores display name and always puts the file name as display name.
                var displayNameResult = "TestFileDisplayName" == displayName || TestFile == displayName;

                // PropPatch (remove).
                propertyUpdate = new PropertyUpdate();
                var remove = new Remove();
                prop = Prop.CreatePropWithEmptyProperties(PropNameConstants.DisplayName);
                remove.Prop = prop;
                propertyUpdate.Items = new object[] { remove };
                response = await client.PropPatchAsync(testFile, propertyUpdate);
                var propPatchRemoveResponseSuccess = response.IsSuccessStatusCode;
                multistatusPropFind = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);
                var multistatusPropFindResult = string.IsNullOrEmpty(((Propstat)multistatusPropFind.Response[0].Items[0]).Prop.DisplayName);

                // Delete file.
                response = await client.DeleteAsync(testFile);
                var deleteResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(putResponseSuccess);
                Assert.IsNotNull(multistatusPropPatchSet);
                Assert.IsTrue(propPatchResponseSuccess);
                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsTrue(displayNameResult);
                Assert.IsTrue(propPatchRemoveResponseSuccess);
                Assert.IsTrue(multistatusPropFindResult);
                Assert.IsTrue(deleteResponseSuccess);
            }
        }

        #endregion PropPatch / put / delete file

        #region Mkcol / delete collection

        [TestMethod]
        public async Task UIT_WebDavClient_Mkcol()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testCollection = UriHelper.CombineUrl(webDavRootFolder, TestCollection, true);

                // Create collection.
                var response = await client.MkcolAsync(testCollection);
                var mkColResponseSuccess = response.IsSuccessStatusCode;

                // PropFind.
                PropFind pf = PropFind.CreatePropFindAllProp();
                response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;

                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                bool collectionFound = false;

                foreach (var item in multistatus.Response)
                {
                    if (item.Href.EndsWith(TestCollection + "/"))
                    {
                        collectionFound = true;
                        break;
                    }
                }

                // Delete collection.
                response = await client.DeleteAsync(testCollection);
                var deleteResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(mkColResponseSuccess);
                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsTrue(collectionFound);
                Assert.IsTrue(deleteResponseSuccess);
            }
        }

        #endregion Mkcol / delete collection

        #region Get / head

        [TestMethod]
        public async Task UIT_WebDavClient_Get()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testFile = UriHelper.CombineUrl(webDavRootFolder, TestFile, true);
                WebDavResponseMessage response = null;

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    response = await client.PutAsync(testFile, content);
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Head
                response = await client.HeadAsync(testFile);
                var headResponseSuccess = response.IsSuccessStatusCode;

                // Get file.
                response = await client.GetAsync(testFile);
                var getResponseSuccess = response.IsSuccessStatusCode;
                var responseContentStringGet = await response.Content.ReadAsStringAsync();

                // Delete file.
                response = await client.DeleteAsync(testFile);
                var deleteResponseSuccess = response.IsSuccessStatusCode;


                Assert.IsTrue(putResponseSuccess);
                Assert.IsTrue(headResponseSuccess);
                Assert.IsTrue(getResponseSuccess);
                Assert.AreEqual("This is a test file for WebDAV.", responseContentStringGet);
                Assert.IsTrue(deleteResponseSuccess);
            }
        }


        [TestMethod]
        public async Task UIT_WebDavClient_Get_WithFileWithUnknownExtension()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testFile = UriHelper.CombineUrl(webDavRootFolder, TestFileUnknownExtension, true);

                WebDavResponseMessage response = null;

                // Put file.
                using (var fileStream = File.OpenRead(TestFileUnknownExtension))
                {
                    var content = new StreamContent(fileStream);
                    response = await client.PutAsync(testFile, content);
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Head
                response = await client.HeadAsync(testFile);
                var headResponseSuccess = response.IsSuccessStatusCode;

                // Get file.
                response = await client.GetAsync(testFile);
                var getResponseSuccess = response.IsSuccessStatusCode;
                var responseContentStringGet = await response.Content.ReadAsStringAsync();

                // Delete file.
                response = await client.DeleteAsync(testFile);
                var deleteResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(putResponseSuccess);
                Assert.IsTrue(headResponseSuccess);
                Assert.IsTrue(getResponseSuccess);
                Assert.AreEqual("This a a file with unknown extension.", responseContentStringGet);
                Assert.IsTrue(deleteResponseSuccess);
            }
        }

        #endregion Get / head

        #region Move

        [TestMethod]
        public async Task UIT_WebDavClient_Move()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testCollectionSource = UriHelper.CombineUrl(webDavRootFolder, TestCollection, true);
                var testCollectionDestination = UriHelper.CombineUrl(webDavRootFolder, TestCollection + "2", true);
                var testFile = UriHelper.CombineUrl(testCollectionSource, TestFile, true);

                // Create source collection.
                var response = await client.MkcolAsync(testCollectionSource);
                var mkColResponseSuccess = response.IsSuccessStatusCode;

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    response = await client.PutAsync(testFile, content);
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Move.
                response = await client.MoveAsync(testCollectionSource, testCollectionDestination);
                var moveResponseSuccess = response.IsSuccessStatusCode;

                // PropFind.
                PropFind pf = PropFind.CreatePropFindAllProp();
                response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;

                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                bool foundCollection1 = false;
                bool foundCollection2 = false;

                foreach (var item in multistatus.Response)
                {
                    if (item.Href.EndsWith(TestCollection + "/"))
                        foundCollection1 = true;

                    if (item.Href.EndsWith(TestCollection + "2/"))
                        foundCollection2 = true;
                }

                // Delete source and destination.
                // Delete file.
                response = await client.DeleteAsync(testCollectionDestination);
                var deleteResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(mkColResponseSuccess);
                Assert.IsTrue(putResponseSuccess);
                Assert.IsTrue(moveResponseSuccess);
                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsFalse(foundCollection1);
                Assert.IsTrue(foundCollection2);
                Assert.IsTrue(deleteResponseSuccess);
            }
        }

        #endregion Move

        #region Lock / unlock

        [TestMethod]
        public async Task UIT_WebDavClient_LockRefreshLockUnlock()
        {
            // This won't work on ownCloud/Nextcloud because they do not support WebDAV locking.
            // These unit integration test are skipped for ownCloud/Nextcloud.
            if (webDavRootFolder.Contains("nextcloud") || webDavRootFolder.Contains("owncloud"))
                return;

            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var userEmail = "test@test.com";

                // Lock.
                var lockInfo = new LockInfo()
                {
                    LockScope = LockScope.CreateExclusiveLockScope(),
                    LockType = LockType.CreateWriteLockType(),
                    OwnerHref = userEmail
                };

                var response = await client.LockAsync(webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromMinutes(1)), WebDavDepthHeaderValue.Infinity, lockInfo);
                var lockResponseSuccess = response.IsSuccessStatusCode;
                LockToken lockToken = await WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);
                ActiveLock activeLock = await WebDavHelper.GetActiveLockFromWebDavResponseMessage(response);

                // Refresh lock.
                response = await client.RefreshLockAsync(webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(10)), lockToken);
                var refreshLockResponseSuccess = response.IsSuccessStatusCode;

                // Unlock.
                response = await client.UnlockAsync(webDavRootFolder, lockToken);
                var unlockResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(lockResponseSuccess);
                Assert.IsNotNull(lockToken);
                Assert.AreEqual(userEmail, activeLock.OwnerHref);
                Assert.IsTrue(refreshLockResponseSuccess);
                Assert.IsTrue(unlockResponseSuccess);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavClient_LockAndPutWithoutToken()
        {
            // This won't work on ownCloud/Nextcloud because they do not support WebDAV locking.
            // These unit integration test are skipped for ownCloud/Nextcloud.
            if (webDavRootFolder.Contains("nextcloud") || webDavRootFolder.Contains("owncloud"))
                return;

            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                // Lock.
                var lockInfo = new LockInfo()
                {
                    LockScope = LockScope.CreateExclusiveLockScope(),
                    LockType = LockType.CreateWriteLockType(),
                    OwnerHref = "test@test.com"
                };

                var response = await client.LockAsync(webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(15)), WebDavDepthHeaderValue.Infinity, lockInfo);
                var lockResponseSuccess = response.IsSuccessStatusCode;

                LockToken lockToken = await WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);

                // Put file (without lock token) -> this should fail.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    var requestUrl = UriHelper.CombineUrl(webDavRootFolder, TestFile, true);
                    response = await client.PutAsync(requestUrl, content);
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Unlock.
                response = await client.UnlockAsync(webDavRootFolder, lockToken);
                var unlockResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(lockResponseSuccess);
                Assert.IsNotNull(lockToken);
                Assert.IsFalse(putResponseSuccess);
                Assert.IsTrue(unlockResponseSuccess);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavClient_LockAndPutWithToken()
        {
            // This won't work on ownCloud/Nextcloud because they do not support WebDAV locking.
            // These unit integration test are skipped for ownCloud/Nextcloud.
            if (webDavRootFolder.Contains("nextcloud") || webDavRootFolder.Contains("owncloud"))
                return;

            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                // Lock.
                var lockInfo = new LockInfo()
                {
                    LockScope = LockScope.CreateExclusiveLockScope(),
                    LockType = LockType.CreateWriteLockType(),
                    OwnerHref = "test@test.com"
                };

                var response = await client.LockAsync(webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(15)), WebDavDepthHeaderValue.Infinity, lockInfo);
                var lockResponseSuccess = response.IsSuccessStatusCode;
                LockToken lockToken = await WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);
                var requestUrl = UriHelper.CombineUrl(webDavRootFolder, TestFile, true);

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    response = await client.PutAsync(requestUrl, content, lockToken);
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Delete file.
                response = await client.DeleteAsync(requestUrl, lockToken);
                var deleteResponseSuccess = response.IsSuccessStatusCode;

                // Unlock.
                response = await client.UnlockAsync(webDavRootFolder, lockToken);
                var unlockResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(lockResponseSuccess);
                Assert.IsNotNull(lockToken);
                Assert.IsTrue(putResponseSuccess);
                Assert.IsTrue(deleteResponseSuccess);
                Assert.IsTrue(unlockResponseSuccess);
            }
        }

        #endregion Lock / unlock        

        #region PropName

        [TestMethod]
        public async Task UIT_WebDavClient_PropName()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindWithPropName();
                var response = await client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf);
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = await WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content);

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }
        #endregion PropName
    }
}
