using DecaTec.WebDav.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

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

        [TestMethod]
        public void UIT_WebDavSession_List()
        {
            var session = CreateWebDavSession();
            var items = session.ListAsync(webDavRootFolder).Result;

            Assert.IsNotNull(items);
        }

        [TestMethod]
        public void UIT_WebDavSession_List_WithBaseUri()
        {
            var session = CreateWebDavSession();
            session.BaseUri = new Uri(webDavRootFolder);
            var created = session.CreateDirectoryAsync("Test").Result;
            var items = session.ListAsync("Test/").Result;
            var deleted = session.DeleteAsync("Test").Result;

            Assert.IsTrue(created);
            Assert.IsNotNull(items);
            Assert.IsTrue(deleted);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UIT_WebDavSession_List_WithBaseUriMissing()
        {
            try
            {
                var session = CreateWebDavSession();
                var created = session.CreateDirectoryAsync("Test").Result;
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        [TestMethod]
        public void UIT_WebDavSession_List_WithSpaceInFolder()
        {
            var session = CreateWebDavSession();
            session.BaseUri = new Uri(webDavRootFolder);
            var created = session.CreateDirectoryAsync("a test").Result;
            var items = session.ListAsync("a test/").Result;
            Assert.AreEqual(items.Count, 0);
            var deleted = session.DeleteAsync("a test").Result;

            Assert.IsTrue(created);
            Assert.IsNotNull(items);
            Assert.IsTrue(deleted);
        }

        [TestMethod]
        public void UIT_WebDavSession_List_WithSpaceInFolderAndSubfolder()
        {
            var session = CreateWebDavSession();
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

        [TestMethod]
        public void UIT_WebDavSession_Lock()
        {
            var session = CreateWebDavSession();
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

        [TestMethod]
        public void UIT_WebDavSession_DeleteFileFolder_WithBaseUri()
        {
            var session = CreateWebDavSession();
            session.BaseUrl = webDavRootFolder;
            var testFile = TestFolder + "/" + TestFile;
            var createdFolder = session.CreateDirectoryAsync(TestFolder).Result;
            var stream = File.OpenRead(TestFile);
            var createdFile = session.UploadFileAsync(testFile, stream).Result;
            stream.Dispose();

            var deletedFile = session.DeleteAsync(testFile).Result;
            var deletedFolder= session.DeleteAsync(TestFolder).Result;

            Assert.IsTrue(createdFolder);
            Assert.IsTrue(createdFile);
            Assert.IsTrue(deletedFile);
            Assert.IsTrue(deletedFolder);
        }

        [TestMethod]
        public void UIT_WebDavSession_CreateAndHead_WithFileUnknown()
        {
            var session = CreateWebDavSession();
            session.BaseUrl = webDavRootFolder;
            var stream = File.OpenRead(TestFileUnknownExtension);
            var createdFile = session.UploadFileAsync(TestFileUnknownExtension, stream).Result;
            stream.Dispose();

            var fileExists = session.ExistsAsync(TestFileUnknownExtension).Result;
            var deletedFile = session.DeleteAsync(TestFileUnknownExtension).Result;

            Assert.IsTrue(createdFile);
            Assert.IsTrue(fileExists);
            Assert.IsTrue(deletedFile);
        }
    }
}
