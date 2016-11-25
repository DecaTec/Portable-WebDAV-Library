using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.IO;
using System.Threading;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace DecaTec.WebDav.Uwp.UnitIntegrationTest
{
    /// <summary>
    /// Unit integration test class for WebDavClient.
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
        private string userName;
        private string password;
        private string webDavRootFolder;

        private const string ConfigurationFile = @"TestConfiguration.txt";
        private const string TestFile = @"TextFile1.txt";
        private const string TestCollection = "TestCollection";

        [TestInitialize]
        public void ReadTestConfiguration()
        {
            try
            {
                var configuration = File.ReadAllLines(ConfigurationFile);
                this.userName = configuration[0];
                this.password = configuration[1];
                this.webDavRootFolder = configuration[2];
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException("The configuration file cannot be found. Make sure that there is a file 'TestConfiguration.txt' in the test's output folder containing data about the WebDAV server to test against.", ConfigurationFile, ex);
            }
        }

        private WebDavClient CreateWebDavClient()
        {
            var baseHttpFilter = new HttpBaseProtocolFilter();
            baseHttpFilter.ServerCredential = new Windows.Security.Credentials.PasswordCredential(this.webDavRootFolder, this.userName, this.password);
            var wdc = new WebDavClient(baseHttpFilter);
            return wdc;
        }

        #region PropFind

        [TestMethod]
        public void UIT_UWP_WebDavClient_PropFindAllProp()
        {
            var client = CreateWebDavClient();
            PropFind pf = PropFind.CreatePropFindAllProp();
            var response = client.PropFindAsync(this.webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess, "propFindResponseSuccess");
            Assert.IsNotNull(multistatus, "multistatus");
        }

        [TestMethod]
        public void UIT_UWP_WebDavClient_PropFindNamedProperties()
        {
            var client = CreateWebDavClient();
            PropFind pf = PropFind.CreatePropFindWithEmptyProperties("name");
            var response = client.PropFindAsync(this.webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess, "propFindResponseSuccess");
            Assert.IsNotNull(multistatus, "multistatus");
        }

        [TestMethod]
        public void UIT_UWP_WebDavClient_PropFindPropName()
        {
            var client = CreateWebDavClient();
            PropFind pf = PropFind.CreatePropFindWithPropName();
            var response = client.PropFindAsync(this.webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

            Assert.IsTrue(propFindResponseSuccess, "propFindResponseSuccess");
            Assert.IsNotNull(multistatus, "multistatus");
        }

        #endregion PropFind

        #region PropPatch / put / delete file

        [TestMethod]
        public void UIT_UWP_WebDavClient_PropPatch()
        {
            var client = CreateWebDavClient();
            var testFile = this.webDavRootFolder + TestFile;

            // Put file.
            StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///TextFile1.txt")).AsTask().Result;
            HttpStreamContent content;
            HttpResponseMessage response;

            using (Stream stream = (file.OpenReadAsync().AsTask().Result).AsStreamForRead())
            {
                content = new HttpStreamContent(stream.AsInputStream());
                response = client.PutAsync(testFile, content).Result;
            }

            var putResponseSuccess = response.IsSuccessStatusCode;

            // PropPatch (set).
            var propertyUpdate = new PropertyUpdate();
            var set = new Set();
            var prop = new Prop();
            prop.DisplayName = "TestFileDisplayName";
            set.Prop = prop;
            propertyUpdate.Items = new object[] { set };
            response = client.PropPatchAsync(testFile, propertyUpdate).Result;
            var propPatchResponseSuccess = response.IsSuccessStatusCode;

            // PropFind.
            PropFind pf = PropFind.CreatePropFindWithEmptyProperties("displayname");
            response = client.PropFindAsync(testFile, WebDavDepthHeaderValue.Zero, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = (Multistatus)WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
            var displayName = ((Propstat)multistatus.Response[0].Items[0]).Prop.DisplayName;
            // IIS ignores display name and always puts the file name as display name.
            var displayNameResult = "TestFileDisplayName" == displayName || TestFile == displayName;

            // PropPatch (remove).
            propertyUpdate = new PropertyUpdate();
            var remove = new Remove();
            prop = Prop.CreatePropWithEmptyProperties("displayname");
            remove.Prop = prop;
            propertyUpdate.Items = new object[] { remove };
            response = client.PropPatchAsync(testFile, propertyUpdate).Result;
            var propPatchRemoveResponseSuccess = response.IsSuccessStatusCode;
            multistatus = (Multistatus)WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
            var multistatusResult = ((Propstat)multistatus.Response[0].Items[0]).Prop.DisplayName;

            // Delete file.
            response = client.DeleteAsync(testFile).Result;
            var deleteResponseSuccess = response.IsSuccessStatusCode;

            Assert.IsTrue(putResponseSuccess, "putResponseSuccess");
            Assert.IsTrue(propPatchResponseSuccess, "propPatchResponseSuccess");
            Assert.IsTrue(propFindResponseSuccess, "propFindResponseSuccess");
            Assert.IsTrue(displayNameResult, "displayNameResult");
            Assert.IsTrue(propPatchRemoveResponseSuccess, "propPatchRemoveResponseSuccess");
            Assert.AreEqual(string.Empty, multistatusResult, "multistatusResult");
            Assert.IsTrue(deleteResponseSuccess, "deleteResponseSuccess");
        }

        #endregion PropPatch / put / delete file

        #region Upload / PropPatch / delete file

        [TestMethod]
        public void UIT_UWP_WebDavClient_Upload()
        {
            var client = CreateWebDavClient();
            var testFile = this.webDavRootFolder + TestFile;

            // Upload file.
            var file = ApplicationData.Current.LocalFolder.CreateFileAsync(TestFile, CreationCollisionOption.OpenIfExists).AsTask().Result;
            HttpResponseMessage response;
            var cts = new CancellationTokenSource();
            var progress = new Progress<HttpProgress>();
            var stream = file.OpenAsync(FileAccessMode.ReadWrite).AsTask().Result;
            response = client.UploadFileAsync(testFile, stream, file.ContentType, cts, progress, null).Result;
            var uploadResponseSuccess = response.IsSuccessStatusCode;

            // PropPatch (set).
            var propertyUpdate = new PropertyUpdate();
            var set = new Set();
            var prop = new Prop();
            prop.DisplayName = "TestFileDisplayName";
            set.Prop = prop;
            propertyUpdate.Items = new object[] { set };
            response = client.PropPatchAsync(testFile, propertyUpdate).Result;
            var propPatchResponseSuccess = response.IsSuccessStatusCode;

            // PropFind.
            PropFind pf = PropFind.CreatePropFindWithEmptyProperties("displayname");
            response = client.PropFindAsync(testFile, WebDavDepthHeaderValue.Zero, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;
            var multistatus = (Multistatus)WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
            var displayName = ((Propstat)multistatus.Response[0].Items[0]).Prop.DisplayName;
            // IIS ignores display name and always puts the file name as display name.
            var displayNameResult = "TestFileDisplayName" == displayName || TestFile == displayName;

            // PropPatch (remove).
            propertyUpdate = new PropertyUpdate();
            var remove = new Remove();
            prop = Prop.CreatePropWithEmptyProperties("displayname");
            remove.Prop = prop;
            propertyUpdate.Items = new object[] { remove };
            response = client.PropPatchAsync(testFile, propertyUpdate).Result;
            var propPatchRemoveResponseSuccess = response.IsSuccessStatusCode;
            multistatus = (Multistatus)WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;
            var multistatusResult = ((Propstat)multistatus.Response[0].Items[0]).Prop.DisplayName;

            // Download.
            cts = new CancellationTokenSource();
            progress = new Progress<HttpProgress>();
            var buffer = client.DownloadFileAsync(testFile, cts, progress).Result;

            // Delete file.
            response = client.DeleteAsync(testFile).Result;
            var deleteResponseSuccess = response.IsSuccessStatusCode;

            Assert.IsTrue(uploadResponseSuccess, "putResponseSuccess");
            Assert.IsTrue(propPatchResponseSuccess, "propPatchResponseSuccess");
            Assert.IsTrue(propFindResponseSuccess, "propFindResponseSuccess");
            Assert.IsTrue(displayNameResult, "displayNameResult");
            Assert.IsTrue(propPatchRemoveResponseSuccess, "propPatchRemoveResponseSuccess");
            Assert.AreEqual(string.Empty, multistatusResult, "multistatusResult");
            Assert.IsTrue(deleteResponseSuccess, "deleteResponseSuccess");
            Assert.IsNotNull(buffer);
        }

        #endregion Upload / PropPatch / delete file

        #region Mkcol / delete collection

        [TestMethod]
        public void UIT_UWP_WebDavClient_Mkcol()
        {
            var client = CreateWebDavClient();
            var testCollection = this.webDavRootFolder + TestCollection;

            // Create collection.
            var response = client.MkcolAsync(testCollection).Result;
            var mkColResponseSuccess = response.IsSuccessStatusCode;

            // PropFind.
            PropFind pf = PropFind.CreatePropFindAllProp();
            response = client.PropFindAsync(this.webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;

            var multistatus = (Multistatus)WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

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

            Assert.IsTrue(mkColResponseSuccess, "mkColResponseSuccess");
            Assert.IsTrue(propFindResponseSuccess, "propFindResponseSuccess");
            Assert.IsTrue(collectionFound, "collectionFound");
            Assert.IsTrue(deleteResponseSuccess, "deleteResponseSuccess");
        }

        #endregion Mkcol / delete collection

        #region Get

        [TestMethod]
        public void UIT_UWP_WebDavClient_Get()
        {
            var client = CreateWebDavClient();
            var testFile = this.webDavRootFolder + TestFile;

            // Put file.
            StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///TextFile1.txt")).AsTask().Result;
            HttpStreamContent content;
            HttpResponseMessage response;

            using (Stream stream = (file.OpenReadAsync().AsTask().Result).AsStreamForRead())
            {
                content = new HttpStreamContent(stream.AsInputStream());
                response = client.PutAsync(testFile, content).Result;
            }

            var putResponseSuccess = response.IsSuccessStatusCode;

            // Get file.
            response = client.GetAsync(testFile).Result;
            var getResponseSuccess = response.IsSuccessStatusCode;

            var responseContent = response.Content.ReadAsStringAsync().GetResults();
            var readResponseContent = response.Content.ReadAsStringAsync().GetResults();

            // Delete file.
            response = client.DeleteAsync(testFile).Result;
            var deleteResponseSuccess = response.IsSuccessStatusCode;

            Assert.IsTrue(putResponseSuccess, "putResponseSuccess");
            Assert.IsTrue(getResponseSuccess, "getResponseSuccess");
            Assert.AreEqual("This is a test file for WebDAV.", readResponseContent, "readResponseContent");
            Assert.IsTrue(deleteResponseSuccess, "deleteResponseSuccess");
        }

        #endregion Get

        #region Copy

        [TestMethod]
        public void UIT_UWP_WebDavClient_Copy()
        {
            var client = CreateWebDavClient();
            var testCollectionSource = this.webDavRootFolder + TestCollection;
            var testCollectionDestination = this.webDavRootFolder + TestCollection + "2";
            var testFile = testCollectionSource + "/" + TestFile;

            // Create source collection.
            var response = client.MkcolAsync(testCollectionSource).Result;
            var mkColResponseSuccess = response.IsSuccessStatusCode;

            // Put file.
            StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///TextFile1.txt")).AsTask().Result;
            HttpStreamContent content;

            using (Stream stream = (file.OpenReadAsync().AsTask().Result).AsStreamForRead())
            {
                content = new HttpStreamContent(stream.AsInputStream());
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

            Assert.IsTrue(mkColResponseSuccess, "mkColResponseSuccessc");
            Assert.IsTrue(putResponseSuccess, "putResponseSuccess");
            Assert.IsTrue(copyResponseSuccess, "copyResponseSuccess");
            Assert.IsTrue(propFindResponseSuccess, "propFindResponseSuccess");
            Assert.IsTrue(collectionfound, "collectionfound");
            Assert.IsTrue(deleteSourceResponseSuccess, "deleteSourceResponseSuccess");
            Assert.IsTrue(deleteDestinationResponseSuccess, "deleteDestinationResponseSuccess");
        }

        #endregion Copy

        #region Move

        [TestMethod]
        public void UIT_UWP_WebDavClient_Move()
        {
            var client = CreateWebDavClient();
            var testCollectionSource = this.webDavRootFolder + TestCollection;
            var testCollectionDestination = this.webDavRootFolder + TestCollection + "2";
            var testFile = testCollectionSource + "/" + TestFile;

            // Create source collection.
            var response = client.MkcolAsync(testCollectionSource).Result;
            var mkColResponseSuccess = response.IsSuccessStatusCode;

            // Put file.
            StorageFile file = StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///TextFile1.txt")).AsTask().Result;
            HttpStreamContent content;

            using (Stream stream = (file.OpenReadAsync().AsTask().Result).AsStreamForRead())
            {
                content = new HttpStreamContent(stream.AsInputStream());
                response = client.PutAsync(testFile, content).Result;
            }

            var putResponseSuccess = response.IsSuccessStatusCode;

            // Move.
            response = client.MoveAsync(testCollectionSource, testCollectionDestination).Result;
            var moveResponseSuccess = response.IsSuccessStatusCode;

            // PropFind.
            PropFind pf = PropFind.CreatePropFindAllProp();
            response = client.PropFindAsync(this.webDavRootFolder, WebDavDepthHeaderValue.Infinity, pf).Result;
            var propFindResponseSuccess = response.IsSuccessStatusCode;

            var multistatus = (Multistatus)WebDavResponseContentParser.ParseMultistatusResponseContentAsync(response.Content).Result;

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

            Assert.IsTrue(mkColResponseSuccess, "mkColResponseSuccess");
            Assert.IsTrue(putResponseSuccess, "putResponseSuccess");
            Assert.IsTrue(moveResponseSuccess, "moveResponseSuccess");
            Assert.IsTrue(propFindResponseSuccess, "propFindResponseSuccess");
            Assert.IsFalse(foundCollection1, "foundCollection1");
            Assert.IsTrue(foundCollection2, "foundCollection2");
            Assert.IsTrue(deleteResponseSuccess, "deleteResponseSuccess");
        }

        #endregion Move

        #region Lock / unlock

        [TestMethod]
        public void UIT_UWP_WebDavClient_LockRefreshLockUnlock()
        {
            var client = CreateWebDavClient();

            // Lock.
            var lockInfo = new LockInfo();
            lockInfo.LockScope = LockScope.CreateExclusiveLockScope();
            lockInfo.LockType = LockType.CreateWriteLockType();
            lockInfo.Owner = new OwnerHref("test@test.com");
            var response = client.LockAsync(this.webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(15)), WebDavDepthHeaderValue.Infinity, lockInfo).Result;
            var lockResponseSuccess = response.IsSuccessStatusCode;
            LockToken lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);

            // Refresh lock.
            response = client.RefreshLockAsync(this.webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(10)), lockToken).Result;
            var refreshLockResponseSuccess = response.IsSuccessStatusCode;

            // Unlock.
            response = client.UnlockAsync(this.webDavRootFolder, lockToken).Result;
            var unlockResponseSuccess = response.IsSuccessStatusCode;

            Assert.IsTrue(lockResponseSuccess, "lockResponseSuccess");
            Assert.IsNotNull(lockToken, "lockToken");
            Assert.IsTrue(refreshLockResponseSuccess, "refreshLockResponseSuccess");
            Assert.IsTrue(unlockResponseSuccess, "unlockResponseSuccess");
        }

        [TestMethod]
        public void UIT_UWP_WebDavClient_LockAndPutWithoutToken()
        {
            var client = CreateWebDavClient();

            // Lock.
            var lockInfo = new LockInfo();
            lockInfo.LockScope = LockScope.CreateExclusiveLockScope();
            lockInfo.LockType = LockType.CreateWriteLockType();
            lockInfo.Owner = new OwnerHref("test@test.com");
            var response = client.LockAsync(this.webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(15)), WebDavDepthHeaderValue.Infinity, lockInfo).Result;
            var lockResponseSuccess = response.IsSuccessStatusCode;

            LockToken lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);

            // Put file (without lock token) -> this should fail.
            var content = new HttpStreamContent(File.OpenRead(TestFile).AsInputStream());
            response = client.PutAsync(this.webDavRootFolder + TestFile, content).Result;
            var putResponseSuccess = response.IsSuccessStatusCode;

            // Unlock.
            response = client.UnlockAsync(this.webDavRootFolder, lockToken).Result;
            var unlockResponseSuccess = response.IsSuccessStatusCode;

            Assert.IsTrue(lockResponseSuccess, "lockResponseSuccess");
            Assert.IsNotNull(lockToken, "lockToken");
            Assert.IsFalse(putResponseSuccess, "putResponseSuccess");
            Assert.IsTrue(unlockResponseSuccess, "unlockResponseSuccess");
        }

        [TestMethod]
        public void UIT_UWP_WebDavClient_LockAndPutWithToken()
        {
            var client = CreateWebDavClient();

            // Lock.
            var lockInfo = new LockInfo();
            lockInfo.LockScope = LockScope.CreateExclusiveLockScope();
            lockInfo.LockType = LockType.CreateWriteLockType();
            lockInfo.Owner = new OwnerHref("test@test.com");
            var response = client.LockAsync(this.webDavRootFolder, WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(15)), WebDavDepthHeaderValue.Infinity, lockInfo).Result;
            var lockResponseSuccess = response.IsSuccessStatusCode;
            LockToken lockToken = WebDavHelper.GetLockTokenFromWebDavResponseMessage(response);

            // Put file.
            var content = new HttpStreamContent(File.OpenRead(TestFile).AsInputStream());
            response = client.PutAsync(this.webDavRootFolder + TestFile, content, lockToken).Result;
            var putResponseSuccess = response.IsSuccessStatusCode;

            // Delete file.
            response = client.DeleteAsync(this.webDavRootFolder + TestFile, lockToken).Result;
            var deleteResponseSuccess = response.IsSuccessStatusCode;

            // Unlock.
            response = client.UnlockAsync(this.webDavRootFolder, lockToken).Result;
            var unlockResponseSuccess = response.IsSuccessStatusCode;

            Assert.IsTrue(lockResponseSuccess, "lockResponseSuccess");
            Assert.IsNotNull(lockToken, "lockToken");
            Assert.IsTrue(putResponseSuccess, "putResponseSuccess");
            Assert.IsTrue(deleteResponseSuccess, "deleteResponseSuccess");
            Assert.IsTrue(unlockResponseSuccess, "unlockResponseSuccess");
        }

        #endregion Lock / unlock
    }
}
