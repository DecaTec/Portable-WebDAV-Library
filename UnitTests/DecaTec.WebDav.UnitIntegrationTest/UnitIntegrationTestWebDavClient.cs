using DecaTec.WebDav.Headers;
using DecaTec.WebDav.MessageHandlers;
using DecaTec.WebDav.Tools;
using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

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
        public void UIT_WebDavClient_Copy()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testCollectionSource = UriHelper.CombineUrl(webDavRootFolder, TestCollection, true);
                var testCollectionDestination = UriHelper.CombineUrl(webDavRootFolder, TestCollection + "2", true);
                var testFile = UriHelper.CombineUrl(testCollectionSource, TestFile, true);

                // Create source collection.
                var response = client.MkcolAsync(testCollectionSource).Result;
                var mkColResponseSuccess = response.IsSuccessStatusCode;

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    response = client.PutAsync(testFile, content).Result;
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Copy.
                response = client.CopyAsync(testCollectionSource, testCollectionDestination).Result;
                var copyResponseSuccess = response.IsSuccessStatusCode;

                // PropFind.
                PropFind pf = PropFind.CreatePropFindAllProp();
                response = client.PropFindAsync(testCollectionDestination, WebDavDepthHeaderValue.Infinity, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;

                var multistatus = (Multistatus)WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

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
                response = client.DeleteAsync(testCollectionSource).Result;
                var deleteSourceResponseSuccess = response.IsSuccessStatusCode;

                response = client.DeleteAsync(testCollectionDestination).Result;
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
        public void UIT_WebDavClient_PropFind_AllPropDepthInfinity()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_AllPropDepthOne()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.One, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_AllPropDepthZero()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindAllProp();
                var response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Zero, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_NamedProperties()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindWithEmptyProperties("name");
                var response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_NamedPropertiesAll()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindWithEmptyPropertiesAll();
                var response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        [TestMethod]
        public void UIT_WebDavClient_PropFind_PropName()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindWithPropName();
                var response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }

        #endregion PropFind

        #region PropPatch / put / delete file

        [TestMethod]
        public void UIT_WebDavClient_PropPatch()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testFile = UriHelper.CombineUrl(webDavRootFolder, TestFile, true);
                var putResponseSuccess = false;

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    var responsePut = client.PutAsync(testFile, content).Result;
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
                var response = client.PropPatchAsync(testFile, propertyUpdate).Result;
                var multistatusPropPatchSet = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
                var propPatchResponseSuccess = response.IsSuccessStatusCode;

                // PropFind.
                PropFind pf = PropFind.CreatePropFindWithEmptyProperties(PropNameConstants.DisplayName);
                response = client.PropFindAsync(testFile, WebDavDepthHeaderValue.Zero, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatusPropFind = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
                var displayName = ((Propstat)multistatusPropFind.Response[0].Items[0]).Prop.DisplayName;
                // IIS ignores display name and always puts the file name as display name.
                var displayNameResult = "TestFileDisplayName" == displayName || TestFile == displayName;

                // PropPatch (remove).
                propertyUpdate = new PropertyUpdate();
                var remove = new Remove();
                prop = Prop.CreatePropWithEmptyProperties(PropNameConstants.DisplayName);
                remove.Prop = prop;
                propertyUpdate.Items = new object[] { remove };
                response = client.PropPatchAsync(testFile, propertyUpdate).Result;
                var propPatchRemoveResponseSuccess = response.IsSuccessStatusCode;
                multistatusPropFind = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
                var multistatusPropFindResult = string.IsNullOrEmpty(((Propstat)multistatusPropFind.Response[0].Items[0]).Prop.DisplayName);

                // Delete file.
                response = client.DeleteAsync(testFile).Result;
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
        public void UIT_WebDavClient_Mkcol()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testCollection = UriHelper.CombineUrl(webDavRootFolder, TestCollection, true);

                // Create collection.
                var response = client.MkcolAsync(testCollection).Result;
                var mkColResponseSuccess = response.IsSuccessStatusCode;

                // PropFind.
                PropFind pf = PropFind.CreatePropFindAllProp();
                response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;

                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

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
                response = client.DeleteAsync(testCollection).Result;
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
        public void UIT_WebDavClient_Get()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testFile = UriHelper.CombineUrl(webDavRootFolder, TestFile, true);
                WebDavResponseMessage response = null;

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    response = client.PutAsync(testFile, content).Result;
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Head
                response = client.HeadAsync(testFile).Result;
                var headResponseSuccess = response.IsSuccessStatusCode;

                // Get file.
                response = client.GetAsync(testFile).Result;
                var getResponseSuccess = response.IsSuccessStatusCode;
                var responseContentStringGet = response.Content.ReadAsStringAsync().Result;

                // Delete file.
                response = client.DeleteAsync(testFile).Result;
                var deleteResponseSuccess = response.IsSuccessStatusCode;


                Assert.IsTrue(putResponseSuccess);
                Assert.IsTrue(headResponseSuccess);
                Assert.IsTrue(getResponseSuccess);
                Assert.AreEqual("This is a test file for WebDAV.", responseContentStringGet);
                Assert.IsTrue(deleteResponseSuccess);
            }
        }


        [TestMethod]
        public void UIT_WebDavClient_Get_WithFileWithUnknownExtension()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testFile = UriHelper.CombineUrl(webDavRootFolder, TestFileUnknownExtension, true);

                WebDavResponseMessage response = null;

                // Put file.
                using (var fileStream = File.OpenRead(TestFileUnknownExtension))
                {
                    var content = new StreamContent(fileStream);
                    response = client.PutAsync(testFile, content).Result;
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Head
                response = client.HeadAsync(testFile).Result;
                var headResponseSuccess = response.IsSuccessStatusCode;

                // Get file.
                response = client.GetAsync(testFile).Result;
                var getResponseSuccess = response.IsSuccessStatusCode;
                var responseContentStringGet = response.Content.ReadAsStringAsync().Result;

                // Delete file.
                response = client.DeleteAsync(testFile).Result;
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
        public void UIT_WebDavClient_Move()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                var testCollectionSource = UriHelper.CombineUrl(webDavRootFolder, TestCollection, true);
                var testCollectionDestination = UriHelper.CombineUrl(webDavRootFolder, TestCollection + "2", true);
                var testFile = UriHelper.CombineUrl(testCollectionSource, TestFile, true);

                // Create source collection.
                var response = client.MkcolAsync(testCollectionSource).Result;
                var mkColResponseSuccess = response.IsSuccessStatusCode;

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    response = client.PutAsync(testFile, content).Result;
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Move.
                response = client.MoveAsync(testCollectionSource, testCollectionDestination).Result;
                var moveResponseSuccess = response.IsSuccessStatusCode;

                // PropFind.
                PropFind pf = PropFind.CreatePropFindAllProp();
                response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;

                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

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
                response = client.DeleteAsync(testCollectionDestination).Result;
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
        public void UIT_WebDavClient_LockRefreshLockUnlock()
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

                var response = client.LockAsync(webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromMinutes(1)), WebDavDepthHeaderValue.Infinity, lockInfo).Result;
                var lockResponseSuccess = response.IsSuccessStatusCode;
                LockToken lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);
                ActiveLock activeLock = WebDavHelper.GetActiveLockFromWebDavResponseMessage(response);

                // Refresh lock.
                response = client.RefreshLockAsync(webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(10)), lockToken).Result;
                var refreshLockResponseSuccess = response.IsSuccessStatusCode;

                // Unlock.
                response = client.UnlockAsync(webDavRootFolder, lockToken).Result;
                var unlockResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(lockResponseSuccess);
                Assert.IsNotNull(lockToken);
                Assert.AreEqual(userEmail, activeLock.OwnerHref);
                Assert.IsTrue(refreshLockResponseSuccess);
                Assert.IsTrue(unlockResponseSuccess);
            }
        }

        [TestMethod]
        public void UIT_WebDavClient_LockAndPutWithoutToken()
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

                var response = client.LockAsync(webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(15)), WebDavDepthHeaderValue.Infinity, lockInfo).Result;
                var lockResponseSuccess = response.IsSuccessStatusCode;

                LockToken lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);

                // Put file (without lock token) -> this should fail.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    var requestUrl = UriHelper.CombineUrl(webDavRootFolder, TestFile, true);
                    response = client.PutAsync(requestUrl, content).Result;
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Unlock.
                response = client.UnlockAsync(webDavRootFolder, lockToken).Result;
                var unlockResponseSuccess = response.IsSuccessStatusCode;

                Assert.IsTrue(lockResponseSuccess);
                Assert.IsNotNull(lockToken);
                Assert.IsFalse(putResponseSuccess);
                Assert.IsTrue(unlockResponseSuccess);
            }
        }

        [TestMethod]
        public void UIT_WebDavClient_LockAndPutWithToken()
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

                var response = client.LockAsync(webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(15)), WebDavDepthHeaderValue.Infinity, lockInfo).Result;
                var lockResponseSuccess = response.IsSuccessStatusCode;
                LockToken lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);
                var requestUrl = UriHelper.CombineUrl(webDavRootFolder, TestFile, true);

                // Put file.
                using (var fileStream = File.OpenRead(TestFile))
                {
                    var content = new StreamContent(fileStream);
                    response = client.PutAsync(requestUrl, content, lockToken).Result;
                }

                var putResponseSuccess = response.IsSuccessStatusCode;

                // Delete file.
                response = client.DeleteAsync(requestUrl, lockToken).Result;
                var deleteResponseSuccess = response.IsSuccessStatusCode;

                // Unlock.
                response = client.UnlockAsync(webDavRootFolder, lockToken).Result;
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
        public void UIT_WebDavClient_PropName()
        {
            using (var client = CreateWebDavClientWithDebugHttpMessageHandler())
            {
                PropFind pf = PropFind.CreatePropFindWithPropName();
                var response = client.PropFindAsync(webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
                var propFindResponseSuccess = response.IsSuccessStatusCode;
                var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

                Assert.IsTrue(propFindResponseSuccess);
                Assert.IsNotNull(multistatus);
            }
        }
        #endregion PropName
    }
}
