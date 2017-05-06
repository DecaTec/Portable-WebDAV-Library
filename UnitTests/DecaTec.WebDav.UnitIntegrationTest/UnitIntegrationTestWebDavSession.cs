using DecaTec.WebDav.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using DecaTec.WebDav.WebDavArtifacts;

namespace DecaTec.WebDav.UnitIntegrationTest
{
    /// <summary>
    /// Unit integration test class for WebDavSession.
    /// 
    /// IMPORTANT:  This is a playground when testing this library against a specific WebDAV server implementation.
    ///             Not all methods of WebDavSession will be tested here.
    ///             For unit tests of WebDavSession (with a mocked HTTP handler), see the UnitTestWebDavSession class in the unit test project.
    ///             
    /// You'll need a file 'TestConfiguration.txt' in the test's output folder with the following content:
    /// Line 1: The user name to use for WebDAV connections
    /// Line 2: The password to use for WebDAV connections
    /// Line 3: The URL of an already existing WebDAV folder in the server used for tests
    ///  
    /// If this file is not present, all test will fail!
    /// </summary>
    [TestClass]
    public class UnitIntegrationTestWebDavSession
    {
        private static string userName;
        private static string password;
        private static string webDavRootFolder;

        private const string ConfigurationFile = @"TestConfiguration.txt";

        private const string TestFile = @"TextFile1.txt";
        private const string TestFileUnknownExtension = @"TestFile1.01";
        private const string TestFolder = "TestFolder";

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

        private WebDavSession CreateWebDavSession()
        {
            var httpClientHandler = new HttpClientHandler()
            {
                PreAuthenticate = true,
                Credentials = new NetworkCredential(userName, password)
            };

            var debugHttpMessageHandler = new DebugHttpMessageHandler(httpClientHandler);
            var session = new WebDavSession(debugHttpMessageHandler);
            return session;
        }

        #region List

        [TestMethod]
        public void UIT_WebDavSession_List()
        {
            using (var session = CreateWebDavSession())
            {
                var items = session.ListAsync(webDavRootFolder).Result;

                Assert.IsNotNull(items);
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_List_WithBaseUri()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = session.CreateDirectoryAsync("Test").Result;
                var items = session.ListAsync("Test/").Result;
                var deleted = session.DeleteAsync("Test").Result;

                Assert.IsTrue(created);
                Assert.IsNotNull(items);
                Assert.IsTrue(deleted);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UIT_WebDavSession_List_WithBaseUriMissing()
        {
            try
            {
                using (var session = CreateWebDavSession())
                {
                    var created = session.CreateDirectoryAsync("Test").Result;
                }
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_List_WithSpaceInFolder()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = session.CreateDirectoryAsync("a test").Result;
                var items = session.ListAsync("a test/").Result;
                Assert.AreEqual(items.Count, 0);
                var deleted = session.DeleteAsync("a test").Result;

                Assert.IsTrue(created);
                Assert.IsNotNull(items);
                Assert.IsTrue(deleted);
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_List_WithSpaceInFolderAndSubfolder()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = session.CreateDirectoryAsync("a test").Result;
                var items = session.ListAsync("a test/").Result;
                Assert.AreEqual(items.Count, 0);
                var created2 = session.CreateDirectoryAsync("a test/another test").Result;
                var items2 = session.ListAsync("a test").Result;
                Assert.AreEqual(items2.Count, 1);
                var deleted = session.DeleteAsync("a test").Result;

                Assert.IsTrue(created);
                Assert.IsTrue(created2);
                Assert.IsNotNull(items);
                Assert.IsNotNull(items2);
                Assert.IsTrue(deleted);
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_List_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = session.CreateDirectoryAsync("test").Result;
                var created2 = session.CreateDirectoryAsync("test/test2").Result;
                var items = session.ListAsync("/").Result;
                Assert.AreEqual(items.Count, 1);
                var list = session.ListAsync(items[0]).Result;
                Assert.AreEqual("test2", list[0].Name);
                var delete = session.DeleteAsync(list[0]).Result;
                var delete2 = session.DeleteAsync("test").Result;

                Assert.IsTrue(created);
                Assert.IsTrue(created2);
                Assert.IsNotNull(items);
                Assert.IsNotNull(list);
                Assert.IsTrue(delete);
                Assert.IsTrue(delete2);
            }
        }

        #endregion List

        #region Copy

        [TestMethod]
        public void UIT_WebDavSession_Copy()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = session.CreateDirectoryAsync("testSource").Result;
                var created2 = session.CreateDirectoryAsync("testSource/folderToCopy").Result;
                var created3 = session.CreateDirectoryAsync("testDestination").Result;
                var items = session.ListAsync("/testSource").Result;
                Assert.AreEqual(items.Count, 1);
                var copy = session.CopyAsync("testSource", "testDestination", true).Result;
                var items2 = session.ListAsync("/testSource").Result;
                Assert.AreEqual(items2.Count, 1);
                var items3 = session.ListAsync("/testDestination").Result;
                Assert.AreEqual(items2.Count, 1);
                Assert.AreEqual("folderToCopy", items3[0].Name);
                var delete = session.DeleteAsync("testSource").Result;
                var delete2 = session.DeleteAsync("testDestination").Result;

                Assert.IsTrue(created);
                Assert.IsTrue(created2);
                Assert.IsTrue(created3);
                Assert.IsNotNull(items);
                Assert.IsNotNull(copy);
                Assert.IsTrue(delete);
                Assert.IsTrue(delete2);
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_Copy_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = session.CreateDirectoryAsync("testSource").Result;
                var created2 = session.CreateDirectoryAsync("testSource/folderToCopy").Result;
                var created3 = session.CreateDirectoryAsync("testDestination").Result;
                var items = session.ListAsync("/testSource").Result;
                Assert.AreEqual(items.Count, 1);
                var items2 = session.ListAsync("/").Result;
                var copy = session.CopyAsync(items2.FirstOrDefault(x => x.Name == "testSource"), "testDestination", true).Result;
                var items3 = session.ListAsync("/testSource").Result;
                Assert.AreEqual(items3.Count, 1);
                var items4 = session.ListAsync("/testDestination").Result;
                Assert.AreEqual(items4.Count, 1);
                Assert.AreEqual("folderToCopy", items4[0].Name);
                var delete = session.DeleteAsync("testSource").Result;
                var delete2 = session.DeleteAsync("testDestination").Result;

                Assert.IsTrue(created);
                Assert.IsTrue(created2);
                Assert.IsTrue(created3);
                Assert.IsNotNull(items);
                Assert.IsNotNull(copy);
                Assert.IsTrue(delete);
                Assert.IsTrue(delete2);
            }
        }

        #endregion Copy

        #region Move

        [TestMethod]
        public void UIT_WebDavSession_Move()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = session.CreateDirectoryAsync("testSource").Result;
                var created2 = session.CreateDirectoryAsync("testSource/folderToMove").Result;
                var created3 = session.CreateDirectoryAsync("testDestination").Result;
                var items = session.ListAsync("/testSource").Result;
                Assert.AreEqual(items.Count, 1);
                var move = session.MoveAsync("testSource/folderToMove", "testDestination/folderToMove", true).Result;
                var items2 = session.ListAsync("/testSource").Result;
                Assert.AreEqual(items2.Count, 0);
                var items3 = session.ListAsync("/testDestination").Result;
                Assert.AreEqual(items3.Count, 1);
                Assert.AreEqual("folderToMove", items3[0].Name);
                var delete = session.DeleteAsync("testSource").Result;
                var delete2 = session.DeleteAsync("testDestination").Result;

                Assert.IsTrue(created);
                Assert.IsTrue(created2);
                Assert.IsTrue(created3);
                Assert.IsNotNull(items);
                Assert.IsNotNull(move);
                Assert.IsTrue(delete);
                Assert.IsTrue(delete2);
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_Move_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = session.CreateDirectoryAsync("testSource").Result;
                var created2 = session.CreateDirectoryAsync("testSource/folderToMove").Result;
                var created3 = session.CreateDirectoryAsync("testDestination").Result;
                var items = session.ListAsync("/testSource").Result;
                Assert.AreEqual(items.Count, 1);
                var items2 = session.ListAsync("/testSource").Result;
                var move = session.MoveAsync(items2[0], "testDestination/folderToMove", true).Result;
                var items3 = session.ListAsync("/testSource").Result;
                Assert.AreEqual(items3.Count, 0);
                var items4 = session.ListAsync("/testDestination").Result;
                Assert.AreEqual(items4.Count, 1);
                Assert.AreEqual("folderToMove", items4[0].Name);
                var delete = session.DeleteAsync("testSource").Result;
                var delete2 = session.DeleteAsync("testDestination").Result;

                Assert.IsTrue(created);
                Assert.IsTrue(created2);
                Assert.IsTrue(created3);
                Assert.IsNotNull(items);
                Assert.IsNotNull(move);
                Assert.IsTrue(delete);
                Assert.IsTrue(delete2);
            }
        }

        #endregion Move

        #region Lock

        [TestMethod]
        public void UIT_WebDavSession_Lock()
        {
            using (var session = CreateWebDavSession())
            {
                var locked = session.LockAsync(webDavRootFolder).Result;
                var requestUrl = UriHelper.CombineUrl(webDavRootFolder, "Test", true);
                var created = session.CreateDirectoryAsync(requestUrl).Result;
                var deleted = session.DeleteAsync(requestUrl).Result;
                var unlocked = session.UnlockAsync(webDavRootFolder).Result;

                Assert.IsTrue(locked);
                Assert.IsTrue(created);
                Assert.IsTrue(deleted);
                Assert.IsTrue(unlocked);
            }
        }

        #endregion Lock

        #region Delete

        [TestMethod]
        public void UIT_WebDavSession_DeleteFileFolder_WithBaseUri()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var testFile = TestFolder + "/" + TestFile;
                var createdFolder = session.CreateDirectoryAsync(TestFolder).Result;
                var createdFile = false;

                using (var stream = File.OpenRead(TestFile))
                {
                    createdFile = session.UploadFileAsync(testFile, stream).Result;
                }

                var deletedFile = session.DeleteAsync(testFile).Result;
                var deletedFolder = session.DeleteAsync(TestFolder).Result;

                Assert.IsTrue(createdFolder);
                Assert.IsTrue(createdFile);
                Assert.IsTrue(deletedFile);
                Assert.IsTrue(deletedFolder);
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_DeleteFileFolder_ByWedDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var testFile = TestFolder + "/" + TestFile;
                var createdFolder = session.CreateDirectoryAsync(TestFolder).Result;
                var createdFile = false;

                using (var stream = File.OpenRead(TestFile))
                {
                    createdFile = session.UploadFileAsync(testFile, stream).Result;
                }

                var deletedFile = session.DeleteAsync(testFile).Result;
                var deletedFolder = session.DeleteAsync(TestFolder).Result;

                Assert.IsTrue(createdFolder);
                Assert.IsTrue(createdFile);
                Assert.IsTrue(deletedFile);
                Assert.IsTrue(deletedFolder);
            }
        }

        #endregion Delete

        #region Upload

        [TestMethod]
        public void UIT_WebDavSession_Upload()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var responseUpload = false;

                using (var fileStream = File.OpenRead(TestFile))
                {
                    responseUpload = session.UploadFileAsync(TestFile, fileStream).Result;
                }

                var list = session.ListAsync("/").Result;
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(TestFile, list[0].Name);
                var delete = session.DeleteAsync(TestFile).Result;

                Assert.IsTrue(responseUpload);
                Assert.IsTrue(delete);
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_Upload_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var responseUpload = false;
                var create = session.CreateDirectoryAsync(TestFolder).Result;
                Assert.IsTrue(create);
                var list = session.ListAsync("/").Result;
                Assert.AreEqual(1, list.Count);

                using (var fileStream = File.OpenRead(TestFile))
                {
                    responseUpload = session.UploadFileAsync(list[0], TestFile, fileStream).Result;
                }

                list = session.ListAsync(TestFolder).Result;
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(TestFile, list[0].Name);
                var delete = session.DeleteAsync(TestFolder).Result;

                Assert.IsTrue(responseUpload);
                Assert.IsTrue(delete);
            }
        }

        #endregion Upload      

        #region UpdateItem

        [TestMethod]
        public void UIT_WebDavSession_UpdateItem()
        {
            // This won't work on IIS because on IIS the 'Name' is always the same as 'DisplayName'.
            // As the unit integration tests of this library are also run against ownCloud/Nextcloud on a regular basis, just skip this test for IIS.
            if (!(webDavRootFolder.Contains("nextcloud") || webDavRootFolder.Contains("owncloud")))
                return;

            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                // We need an "extended" Propfind in order to get the 'DisplayName' in the ListAsync response.
                var propFind = PropFind.CreatePropFindWithEmptyPropertiesAll();

                // Upload file.
                var responseUpload = false;

                using (var fileStream = File.OpenRead(TestFile))
                {
                    responseUpload = session.UploadFileAsync(TestFile, fileStream).Result;
                }

                var list = session.ListAsync("/", propFind).Result;
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(TestFile, list[0].Name);
                Assert.IsNull(list[0].DisplayName);

                // Proppatch set (DisplayName).
                var webDavSessionItem = list[0];
                webDavSessionItem.DisplayName = "ChangedDisplayName";
                var proppatchResult = session.UpdateItemAsync(webDavSessionItem).Result;

                list = session.ListAsync("/", propFind).Result;
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual("ChangedDisplayName", list[0].DisplayName);

                // Proppatch remove (DisplayName).
                webDavSessionItem = list[0];
                webDavSessionItem.DisplayName = null;
                proppatchResult = session.UpdateItemAsync(webDavSessionItem).Result;

                list = session.ListAsync("/", propFind).Result;
                Assert.AreEqual(1, list.Count);
                Assert.IsNull(list[0].DisplayName);

                // Delete file
                var delete = session.DeleteAsync(TestFile).Result;

                Assert.IsTrue(responseUpload);
                Assert.IsTrue(delete);
            }
        }

        #endregion UpdateItem

        #region Misc

        [TestMethod]
        public void UIT_WebDavSession_CreateAndHead_WithFileUnknown()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var createdFile = false;

                using (var stream = File.OpenRead(TestFileUnknownExtension))
                {
                    createdFile = session.UploadFileAsync(TestFileUnknownExtension, stream).Result;
                }

                var fileExists = session.ExistsAsync(TestFileUnknownExtension).Result;
                var deletedFile = session.DeleteAsync(TestFileUnknownExtension).Result;

                Assert.IsTrue(createdFile);
                Assert.IsTrue(fileExists);
                Assert.IsTrue(deletedFile);
            }
        }

        #endregion Misc
    }
}
