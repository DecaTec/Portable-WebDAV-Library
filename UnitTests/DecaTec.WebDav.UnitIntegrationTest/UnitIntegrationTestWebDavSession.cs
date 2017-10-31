using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using DecaTec.WebDav.WebDavArtifacts;
using DecaTec.WebDav.MessageHandlers;
using DecaTec.WebDav.Tools;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                // Ignore all certificate errors.
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                PreAuthenticate = true,
                Credentials = new NetworkCredential(userName, password)
            };

            var debugHttpMessageHandler = new DebugHttpMessageHandler(httpClientHandler);
            var session = new WebDavSession(debugHttpMessageHandler);
            return session;
        }

        #region List

        [TestMethod]
        public async Task UIT_WebDavSession_List()
        {
            using (var session = CreateWebDavSession())
            {
                var items = await session.ListAsync(webDavRootFolder);

                Assert.IsNotNull(items);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavSession_List_WithBaseUri()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("Test");
                var items = await session.ListAsync("Test/");
                var deleted = await session.DeleteAsync("Test");

                Assert.IsTrue(created);
                Assert.IsNotNull(items);
                Assert.IsTrue(deleted);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task UIT_WebDavSession_List_WithBaseUriMissing()
        {
            try
            {
                using (var session = CreateWebDavSession())
                {
                    var created = await session.CreateDirectoryAsync("Test");
                }
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        [TestMethod]
        public async Task UIT_WebDavSession_List_WithSpaceInFolder()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("a test");
                var items = await session.ListAsync("a test/");
                Assert.AreEqual(items.Count, 0);
                var deleted = await session.DeleteAsync("a test");

                Assert.IsTrue(created);
                Assert.IsNotNull(items);
                Assert.IsTrue(deleted);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavSession_List_WithSpaceInFolderAndSubfolder()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("a test");
                var items = await session.ListAsync("a test/");
                Assert.AreEqual(items.Count, 0);
                var created2 = await session.CreateDirectoryAsync("a test/another test");
                var items2 = await session.ListAsync("a test");
                Assert.AreEqual(items2.Count, 1);
                var deleted = await session.DeleteAsync("a test");

                Assert.IsTrue(created);
                Assert.IsTrue(created2);
                Assert.IsNotNull(items);
                Assert.IsNotNull(items2);
                Assert.IsTrue(deleted);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavSession_List_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("test");
                var created2 = await session.CreateDirectoryAsync("test/test2");
                var items = await session.ListAsync("/");
                Assert.AreEqual(items.Count, 1);
                var list = await session.ListAsync(items[0]);
                Assert.AreEqual("test2", list[0].Name);
                var delete = await session.DeleteAsync(list[0]);
                var delete2 = await session.DeleteAsync("test");

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
        public async Task UIT_WebDavSession_Copy()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("testSource");
                var created2 = await session.CreateDirectoryAsync("testSource/folderToCopy");
                var created3 = await session.CreateDirectoryAsync("testDestination");
                var items = await session.ListAsync("/testSource");
                Assert.AreEqual(items.Count, 1);
                var copy = await session.CopyAsync("testSource", "testDestination", true);
                var items2 = await session.ListAsync("/testSource");
                Assert.AreEqual(items2.Count, 1);
                var items3 = await session.ListAsync("/testDestination");
                Assert.AreEqual(items2.Count, 1);
                Assert.AreEqual("folderToCopy", items3[0].Name);
                var delete = await session.DeleteAsync("testSource");
                var delete2 = await session.DeleteAsync("testDestination");

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
        public async Task UIT_WebDavSession_Copy_WithSpecialCharacters()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("test [Source#]");
                var created2 = await session.CreateDirectoryAsync("test [Source#]/folderToCopy");
                var created3 = await session.CreateDirectoryAsync("test [Destination#]");
                var items = await session.ListAsync("/test [Source#]");
                Assert.AreEqual(items.Count, 1);
                var copy = await session.CopyAsync("test [Source#]", "test [Destination#]", true);
                var items2 = await session.ListAsync("/test [Source#]");
                Assert.AreEqual(items2.Count, 1);
                var items3 = await session.ListAsync("/test [Destination#]");
                Assert.AreEqual(items2.Count, 1);
                Assert.AreEqual("folderToCopy", items3[0].Name);
                var delete = await session.DeleteAsync("test [Source#]");
                var delete2 = await session.DeleteAsync("test [Destination#]");

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
        public async Task UIT_WebDavSession_CopyDelete_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("testSource");
                var created2 = await session.CreateDirectoryAsync("testSource/folderToCopy");
                var created3 = await session.CreateDirectoryAsync("testDestination");
                var items = await session.ListAsync("/testSource");
                Assert.AreEqual(items.Count, 1);
                items = await session.ListAsync("/");
                var copy = await session.CopyAsync(items.FirstOrDefault(x => x.Name == "testSource"), "testDestination", true);
                items = await session.ListAsync("/testSource");
                Assert.AreEqual(items.Count, 1);
                items = await session.ListAsync("/testDestination");
                Assert.AreEqual(items.Count, 1);
                Assert.AreEqual("folderToCopy", items[0].Name);
                items = await session.ListAsync("/");

                var deleted = true;

                foreach (var item in items)
                {
                    deleted &= await session.DeleteAsync(item);
                }

                Assert.IsTrue(created);
                Assert.IsTrue(created2);
                Assert.IsTrue(created3);
                Assert.IsNotNull(items);
                Assert.IsNotNull(copy);
                Assert.IsTrue(deleted);
            }
        }

        #endregion Copy

        #region Move

        [TestMethod]
        public async Task UIT_WebDavSession_Move()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("testSource");
                var created2 = await session.CreateDirectoryAsync("testSource/folderToMove");
                var created3 = await session.CreateDirectoryAsync("testDestination");
                var items = await session.ListAsync("/testSource");
                Assert.AreEqual(items.Count, 1);
                var move = await session.MoveAsync("testSource/folderToMove", "testDestination/folderToMove", true);
                var items2 = await session.ListAsync("/testSource");
                Assert.AreEqual(items2.Count, 0);
                var items3 = await session.ListAsync("/testDestination");
                Assert.AreEqual(items3.Count, 1);
                Assert.AreEqual("folderToMove", items3[0].Name);
                var delete = await session.DeleteAsync("testSource");
                var delete2 = await session.DeleteAsync("testDestination");

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
        public async Task UIT_WebDavSession_Move_WithSpecialCharacters()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("test [Source#]");
                var created2 = await session.CreateDirectoryAsync("test [Source#]/folderToMove");
                var created3 = await session.CreateDirectoryAsync("test [Destination#]");
                var items = await session.ListAsync("/test [Source#]");
                Assert.AreEqual(items.Count, 1);
                var move = await session.MoveAsync("test [Source#]/folderToMove", "test [Destination#]/folderToMove", true);
                var items2 = await session.ListAsync("/test [Source#]");
                Assert.AreEqual(items2.Count, 0);
                var items3 = await session.ListAsync("/test [Destination#]");
                Assert.AreEqual(items3.Count, 1);
                Assert.AreEqual("folderToMove", items3[0].Name);
                var delete = await session.DeleteAsync("test [Source#]");
                var delete2 = await session.DeleteAsync("test [Destination#]");

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
        public async Task UIT_WebDavSession_Move_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUri = new Uri(webDavRootFolder);
                var created = await session.CreateDirectoryAsync("testSource");
                var created2 = await session.CreateDirectoryAsync("testSource/folderToMove");
                var created3 = await session.CreateDirectoryAsync("testDestination");
                var items = await session.ListAsync("/testSource");
                Assert.AreEqual(items.Count, 1);
                var items2 = await session.ListAsync("/testSource");
                var move = await session.MoveAsync(items2[0], "testDestination/folderToMove", true);
                var items3 = await session.ListAsync("/testSource");
                Assert.AreEqual(items3.Count, 0);
                var items4 = await session.ListAsync("/testDestination");
                Assert.AreEqual(items4.Count, 1);
                Assert.AreEqual("folderToMove", items4[0].Name);
                var delete = await session.DeleteAsync("testSource");
                var delete2 = await session.DeleteAsync("testDestination");

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
        public async Task UIT_WebDavSession_Lock()
        {
            // This won't work on ownCloud/Nextcloud because they do not support WebDAV locking.
            // These unit integration test are skipped for ownCloud/Nextcloud.
            if (webDavRootFolder.Contains("nextcloud") || webDavRootFolder.Contains("owncloud"))
                return;

            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var locked = await session.LockAsync("/");
                var requestUrl = UriHelper.CombineUrl(webDavRootFolder, "Test", true);
                var created = await session.CreateDirectoryAsync(requestUrl);
                var deleted = await session.DeleteAsync(requestUrl);
                var unlocked = await session.UnlockAsync(webDavRootFolder);

                Assert.IsTrue(locked);
                Assert.IsTrue(created);
                Assert.IsTrue(deleted);
                Assert.IsTrue(unlocked);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavSession_Lock_ByWebDavSessionItem()
        {
            // This won't work on ownCloud/Nextcloud because they do not support WebDAV locking.
            // These unit integration test are skipped for ownCloud/Nextcloud.
            if (webDavRootFolder.Contains("nextcloud") || webDavRootFolder.Contains("owncloud"))
                return;

            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var created = await session.CreateDirectoryAsync(TestFolder);
                var list = await session.ListAsync("/");
                Assert.AreEqual(1, list.Count);
                var locked = await session.LockAsync(list[0]);
                var requestUrl = UriHelper.CombineUrl(webDavRootFolder, "Test", true);
                var created2 = await session.CreateDirectoryAsync(requestUrl);
                var deleted = await session.DeleteAsync(requestUrl);
                list = await session.ListAsync("/");
                Assert.AreEqual(1, list.Count);
                var unlocked = await session.UnlockAsync(list[0]);

                Assert.IsTrue(created);
                Assert.IsTrue(locked);
                Assert.IsTrue(created2);
                Assert.IsTrue(deleted);
                Assert.IsTrue(unlocked);
            }
        }

        #endregion Lock

        #region Delete

        [TestMethod]
        public async Task UIT_WebDavSession_DeleteFileFolder_WithBaseUri()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var testFile = TestFolder + "/" + TestFile;
                var createdFolder = await session.CreateDirectoryAsync(TestFolder);
                var createdFile = false;

                using (var stream = File.OpenRead(TestFile))
                {
                    createdFile = await session.UploadFileAsync(testFile, stream);
                }

                var deletedFile = await session.DeleteAsync(testFile);
                var deletedFolder = await session.DeleteAsync(TestFolder);

                Assert.IsTrue(createdFolder);
                Assert.IsTrue(createdFile);
                Assert.IsTrue(deletedFile);
                Assert.IsTrue(deletedFolder);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavSession_DeleteFileFolder_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var testFile = TestFolder + "/" + TestFile;
                var createdFolder = await session.CreateDirectoryAsync(TestFolder);
                var createdFile = false;

                using (var stream = File.OpenRead(TestFile))
                {
                    createdFile = await session.UploadFileAsync(testFile, stream);
                }

                var deletedFile = await session.DeleteAsync(testFile);
                var deletedFolder = await session.DeleteAsync(TestFolder);

                Assert.IsTrue(createdFolder);
                Assert.IsTrue(createdFile);
                Assert.IsTrue(deletedFile);
                Assert.IsTrue(deletedFolder);
            }
        }

        #endregion Delete

        #region Upload/Download

        [TestMethod]
        public async Task UIT_WebDavSession_UploadDownload()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;

                // Upload file.
                var responseUpload = false;

                using (var fileStream = File.OpenRead(TestFile))
                {
                    responseUpload = await session.UploadFileAsync(TestFile, fileStream);
                }

                var list = await session.ListAsync("/");
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(TestFile, list[0].Name);

                // Download file.
                var downloadSuccess = false;
                var downloadedString = string.Empty;

                using (var stream = new MemoryStream())
                {
                    downloadSuccess = await session.DownloadFileAsync(TestFile, stream);
                    stream.Position = 0;

                    using (StreamReader sr = new StreamReader(stream))
                    {
                        downloadedString = sr.ReadToEnd();
                    }
                }

                Assert.IsTrue(downloadSuccess);
                Assert.AreEqual("This is a test file for WebDAV.", downloadedString);

                // Delete file.
                var delete = await session.DeleteAsync(TestFile);

                Assert.IsTrue(responseUpload);
                Assert.IsTrue(delete);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavSession_UploadDownload_ByWebDavSessionItem()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;

                // Create directory/Upload file.
                var responseUpload = false;
                var create = await session.CreateDirectoryAsync(TestFolder);
                Assert.IsTrue(create);
                var list = await session.ListAsync("/");
                Assert.AreEqual(1, list.Count);

                using (var fileStream = File.OpenRead(TestFile))
                {
                    responseUpload = await session.UploadFileAsync(list[0], TestFile, fileStream);
                }

                list = await session.ListAsync(TestFolder);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(TestFile, list[0].Name);

                // Download file.
                var downloadSuccess = false;
                var downloadedString = string.Empty;

                using (var stream = new MemoryStream())
                {
                    downloadSuccess = await session.DownloadFileAsync(list[0], stream);
                    stream.Position = 0;

                    using (StreamReader sr = new StreamReader(stream))
                    {
                        downloadedString = sr.ReadToEnd();
                    }
                }

                Assert.IsTrue(downloadSuccess);
                Assert.AreEqual("This is a test file for WebDAV.", downloadedString);

                // Delete directory/file.
                list = await session.ListAsync("/");
                var delete = await session.DeleteAsync(list[0]);

                Assert.IsTrue(responseUpload);
                Assert.IsTrue(delete);
            }
        }

        #endregion Upload      

        #region UpdateItem

        [TestMethod]
        public async Task UIT_WebDavSession_UpdateItem()
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
                    responseUpload = await session.UploadFileAsync(TestFile, fileStream);
                }

                var list = await session.ListAsync("/", propFind);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(TestFile, list[0].Name);
                Assert.IsNull(list[0].DisplayName);

                // Proppatch set (DisplayName).
                var webDavSessionItem = list[0];
                webDavSessionItem.DisplayName = "ChangedDisplayName";
                var proppatchResult = await session.UpdateItemAsync(webDavSessionItem);

                list = await session.ListAsync("/", propFind);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual("ChangedDisplayName", list[0].DisplayName);

                // Proppatch remove (DisplayName).
                webDavSessionItem = list[0];
                webDavSessionItem.DisplayName = null;
                proppatchResult = await session.UpdateItemAsync(webDavSessionItem);

                list = await session.ListAsync("/", propFind);
                Assert.AreEqual(1, list.Count);
                Assert.IsNull(list[0].DisplayName);

                // Delete file
                var delete = await session.DeleteAsync(TestFile);

                Assert.IsTrue(responseUpload);
                Assert.IsTrue(delete);
            }
        }

        [TestMethod]
        public async Task UIT_WebDavSession_UpdateItem_WithUnknowProperty()
        {
            // This won't work on IIS because on IIS the 'Name' is always the same as 'DisplayName'.
            // As the unit integration tests of this library are also run against ownCloud/Nextcloud on a regular basis, just skip this test for IIS.
            if (!(webDavRootFolder.Contains("nextcloud") || webDavRootFolder.Contains("owncloud")))
                return;

            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var propFind = PropFind.CreatePropFindWithEmptyPropertiesAll();

                // Add unknown property to Prop.
                // The additional property is taken from https://docs.nextcloud.com/server/12/developer_manual/client_apis/WebDAV/index.html (mark item as favorite)
                // As the additional item is part of another namespace than "DAV:", the key to access this item is a complete XElement (with namespace and name).
                XNamespace ns = "http://owncloud.org/ns";
                var xElement = new XElement(ns + "favorite");
                var prop = (Prop)propFind.Item;

                var xElementList = new List<XElement>
                {
                    xElement
                };

                prop.AdditionalProperties = xElementList.ToArray();

                // Upload file.
                var responseUpload = false;

                using (var fileStream = File.OpenRead(TestFile))
                {
                    responseUpload = await session.UploadFileAsync(TestFile, fileStream);
                }

                var list = await session.ListAsync("/", propFind);
                // Get unknown property.
                var xName = XName.Get("favorite", "http://owncloud.org/ns");
                var file = list.Where(x => x.Name == TestFile);

                var favoriteItem = file.First().AdditionalProperties["favorite"];
                Assert.IsNotNull(favoriteItem);
                Assert.AreEqual("", favoriteItem);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(TestFile, list[0].Name);

                // Proppatch set (favorite).
                var webDavSessionItem = list[0];
                webDavSessionItem.AdditionalProperties["favorite"] = "1";
                var proppatchResult = await session.UpdateItemAsync(webDavSessionItem);

                list = await session.ListAsync("/", propFind);
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual("1", list[0].AdditionalProperties["favorite"]);

                // Proppatch remove (DisplayName).
                webDavSessionItem = list[0];
                webDavSessionItem.AdditionalProperties["favorite"] = null;
                proppatchResult = await session.UpdateItemAsync(webDavSessionItem);

                list = await session.ListAsync("/", propFind);
                file = list.Where(x => x.Name == TestFile);
                favoriteItem = file.First().AdditionalProperties["favorite"];
                Assert.IsNotNull(favoriteItem);
                Assert.AreEqual("", favoriteItem);
                Assert.AreEqual(1, list.Count);

                // Delete file
                var delete = await session.DeleteAsync(TestFile);

                Assert.IsTrue(responseUpload);
                Assert.IsTrue(delete);
            }
        }

        #endregion UpdateItem

        #region GetSupportedPropNames

        [TestMethod]
        public async Task UIT_WebDavSession_GetSupportedPropNames()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var supportedPropNames = await session.GetSupportedPropertyNamesAsync("/");

                Assert.IsNotNull(supportedPropNames);
                Assert.AreNotEqual(0, supportedPropNames.Count());
            }
        }

        #endregion GetSupportedPropNames

        #region Misc

        [TestMethod]
        public async Task UIT_WebDavSession_CreateAndHead_WithFileUnknown()
        {
            using (var session = CreateWebDavSession())
            {
                session.BaseUrl = webDavRootFolder;
                var createdFile = false;

                using (var stream = File.OpenRead(TestFileUnknownExtension))
                {
                    createdFile = await session.UploadFileAsync(TestFileUnknownExtension, stream);
                }

                var fileExists = await session.ExistsAsync(TestFileUnknownExtension);
                var deletedFile = await session.DeleteAsync(TestFileUnknownExtension);

                Assert.IsTrue(createdFile);
                Assert.IsTrue(fileExists);
                Assert.IsTrue(deletedFile);
            }
        }

        #endregion Misc
    }
}
;