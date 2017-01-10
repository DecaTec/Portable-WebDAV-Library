using DecaTec.WebDav.NetFx.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace DecaTec.WebDav.NetFx.UnitIntegrationTest
{
    /// <summary>
    /// Unit integration test class for WebDavSession.
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
        private string userName;
        private string password;
        private string webDavRootFolder;

        private const string ConfigurationFile = @"TestConfiguration.txt";

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

        private WebDavSession CreateWebDavSession()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.PreAuthenticate = true;
            httpClientHandler.Credentials = new NetworkCredential(this.userName, this.password);
            var debugHttpMessageHandler = new DebugHttpMessageHandler(httpClientHandler);
            var session = new WebDavSession(debugHttpMessageHandler);
            return session;
        }

        [TestMethod]
        public void UIT_NetFx_WebDavSession_List()
        {
            var session = CreateWebDavSession();
            var items = session.ListAsync(this.webDavRootFolder).Result;

            Assert.IsNotNull(items);
        }

        [TestMethod]
        public void UIT_NetFx_WebDavSession_List_WithBaseUri()
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
        public void UIT_NetFx_WebDavSession_List_WithBaseUriMissing()
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
        public void UIT_NetFx_WebDavSession_List_WithSpaceInFolder()
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
        public void UIT_NetFx_WebDavSession_List_WithSpaceInFolderAndSubfolder()
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
        public void UIT_NetFx_WebDavSession_Lock()
        {
            var session = CreateWebDavSession();
            var locked = session.LockAsync(this.webDavRootFolder).Result;
            var requestUrl = UriHelper.CombineUrl(this.webDavRootFolder, "Test", true);
            var created = session.CreateDirectoryAsync(requestUrl).Result;
            var deleted = session.DeleteAsync(requestUrl).Result;
            var unlocked = session.UnlockAsync(this.webDavRootFolder).Result;

            Assert.IsTrue(locked);
            Assert.IsTrue(created);
            Assert.IsTrue(deleted);
            Assert.IsTrue(unlocked);
        }
    }
}
