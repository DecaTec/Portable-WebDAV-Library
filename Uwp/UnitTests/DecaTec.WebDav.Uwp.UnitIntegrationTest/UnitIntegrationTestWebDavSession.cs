﻿using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.IO;
using System.Threading;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using System.Linq;

namespace DecaTec.WebDav.Uwp.UnitIntegrationTest
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

        private const string TestFile = @"TextFile1.txt";

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
            var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
            httpBaseProtocolFilter.ServerCredential = new Windows.Security.Credentials.PasswordCredential(this.webDavRootFolder, this.userName, this.password);
            var session = new WebDavSession(httpBaseProtocolFilter);
            return session;
        }

        [TestMethod]
        public void UIT_UWP_WebDavSession_List()
        {
            var session = CreateWebDavSession();
            var items = session.ListAsync(this.webDavRootFolder).Result;

            Assert.IsNotNull(items);
        }

        [TestMethod]
        public void UIT_UWP_WebDavSession_ListWithBaseUri()
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
        public void UIT_UWP_WebDavSession_ListWithBaseUriMissing()
        {
            Assert.ThrowsException<ArgumentException>(() =>
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
            });
        }

        [TestMethod]
        public void UIT_UWP_WebDavSession_ListWithSpaceInFolder()
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
        public void UIT_UWP_WebDavSession_ListWithSpaceInFolderAndSubfolder()
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
        public void UIT_UWP_WebDavSession_Locking()
        {
            var session = CreateWebDavSession();
            var locked = session.LockAsync(this.webDavRootFolder).Result;
            var requestUrl = UriHelper.CombineUrl(this.webDavRootFolder, "Test");
            var created = session.CreateDirectoryAsync(requestUrl).Result;
            var deleted = session.DeleteAsync(requestUrl).Result;
            var unlocked = session.UnlockAsync(this.webDavRootFolder).Result;

            Assert.IsTrue(locked);
            Assert.IsTrue(created);
            Assert.IsTrue(deleted);
            Assert.IsTrue(unlocked);
        }

        [TestMethod]
        public void UIT_UWP_WebDavSession_ListWithFolderAndFile()
        {
            var session = CreateWebDavSession();
            session.BaseUri = new Uri(webDavRootFolder);
            var created = session.CreateDirectoryAsync("test").Result;
            var items = session.ListAsync("test").Result;
            Assert.AreEqual(items.Count, 0);

            var testFile = UriHelper.CombineUrl(this.webDavRootFolder, "test/" + TestFile);
            var file = ApplicationData.Current.LocalFolder.CreateFileAsync(TestFile, CreationCollisionOption.OpenIfExists).AsTask().Result;
            var cts = new CancellationTokenSource();
            var progress = new Progress<HttpProgress>();
            var stream = file.OpenAsync(FileAccessMode.ReadWrite).AsTask().Result;
            var uploadResponse = session.UploadFileAsync(testFile, stream, file.ContentType, cts, progress).Result;

            items = session.ListAsync("test").Result;
            Assert.AreEqual(items.Count, 1);

            var fileUrl = UriHelper.CombineUrl(webDavRootFolder, "test/" + TestFile);
            var uploadedFile = items.FirstOrDefault(x => x.Uri.ToString() == fileUrl);
            Assert.IsNotNull(uploadedFile);

            var deleted = session.DeleteAsync("test").Result;

            Assert.IsTrue(created);
            Assert.IsTrue(uploadResponse);
            Assert.IsNotNull(items);
            Assert.IsTrue(deleted);
        }
    }
}
