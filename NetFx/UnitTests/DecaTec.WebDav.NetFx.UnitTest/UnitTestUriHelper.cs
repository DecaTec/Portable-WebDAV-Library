using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    [TestClass]
    public class UnitTestUriHelper
    {
        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashTrailingSlashNeededUrl()
        {
            string url = "http://www.google.de/test";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = url + "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashTrailingSlashNeededUri()
        {
            Uri uri = new Uri("http://www.google.de/test");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashTrailingSlashNotNeededUrl()
        {
            string url = "http://www.google.de/test/";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashTrailingSlashNotNeededUri()
        {
            Uri uri = new Uri("http://www.google.de/test/");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashTrailingSlashTooManySlashes()
        {
            string url = "http://www.google.de//test//";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "http://www.google.de/test/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashTrailingSlashFileUrl()
        {
            string url = "http://www.google.de/test/test.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashTrailingSlashFileUri()
        {
            Uri uri = new Uri("http://www.google.de/test/test.txt");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri, true);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashTrailingSlashRelativeUri()
        {
            Uri uri = new Uri("/webdav/test", UriKind.Relative);
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("/webdav/test/", UriKind.Relative);
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashUrlWithDot()
        {
            string url = "http://www.google.de/a.test/file.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "http://www.google.de/a.test/file.txt/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_AddTrailingSlashUrlWithDotFileExcpected()
        {
            string url = "http://www.google.de/a.test/file.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            string expected = "http://www.google.de/a.test/file.txt";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetCombinedUriWithTrailingSlashWithTrailingSlashWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de");
            Uri relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("test2/", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithDoubleBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithBaseUriAndAbsoluteUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithBaseUriAndAbsoluteUriRemoveDuplicatePath()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithBaseUriAndAbsoluteUriNoRemoveDuplicatePath()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, false);
            Uri expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriFromSameBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.google.de/test");
            Uri combinedUri = UriHelper.CombineUri(baseUri, baseUri2, true);
            Uri expected = new Uri("http://www.google.de/test");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriFromSameBaseUriAndPath()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.google.de/test/test2");
            Uri combinedUri = UriHelper.CombineUri(baseUri, baseUri2, true);
            Uri expected = new Uri("http://www.google.de/test/test2");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithoutBaseUri()
        {
            Uri baseUri = null;
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithTrailingSlashWithTooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de//");
            Uri relativeUri = new Uri("/test2//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_NetFx_UriHelper_CombineUriFromDifferentBaseUris()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.github.com/test2");
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, baseUri2, true);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriFromBaseUriAndSlash()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri relativeUri = new Uri("/", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriFromBaseUriAndSlashTooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de/test//");
            Uri relativeUri = new Uri("//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_NetFx_UriHelper_CombineUriRelativeUriFirst()
        {
            Uri baseUri = new Uri("/test2", UriKind.RelativeOrAbsolute);
            Uri relativeUri = new Uri("http://www.google.de/test/");
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriTwoRelativeUris()
        {
            Uri baseUri = new Uri("/test", UriKind.RelativeOrAbsolute);
            Uri relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriFirstUriEndWithSecondUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            Uri relativeUri = new Uri("/test2/test.txt", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriFirstUriEndWithSecondUriWithoutSlashSecondUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_RemovePortFromUri()
        {
            Uri uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.RemovePort(uri);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_RemovePortFromUrl()
        {
            string url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.RemovePort(url);
            string expected = @"http://www.google.de/test/test2/test.txt";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_NetFx_UriHelperSetPortToUri()
        {
            Uri uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.SetPort(uri, 9999);
            Uri expected = new Uri("http://www.google.de:9999/test/test2/test.txt");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_NetFx_UriHelperSetPortToUrl()
        {
            string url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.SetPort(url, 9999);
            string expected = @"http://www.google.de:9999/test/test2/test.txt";
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void UT_NetFx_UriHelper_GetPortFromUri()
        {
            Uri uri = new Uri("http://www.google.de:8080/test/test2/test.txt");
            var actual = UriHelper.GetPort(uri);
            int expected = 8080;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetPortFromUrl()
        {
            string url = @"http://www.google.de:8080/test/test2/test.txt";
            var actual = UriHelper.GetPort(url);
            var expected = 8080;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithSpacesAndBrackets()
        {
            Uri baseUri = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/");
            Uri relativeUri;

            if (!Uri.TryCreate("/remote.php/webdav/folder/folder%20with%20space%20and%20%28%29/x.mp3", UriKind.RelativeOrAbsolute, out relativeUri))
                Assert.Fail();

            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://google.de/remote.php/webdav/folder/folder with space and ()/x.mp3");
            Assert.AreEqual(expected, combinedUri);
        }


        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithDuplicatePathEntriesRemoveDuplicate()
        {
            Uri baseUri = new Uri("http://google.de/remote.php/webdav/folder-sync/x/");
            Uri relativeUri;

            if (!Uri.TryCreate("/remote.php/webdav/folder-sync/x/folder/", UriKind.RelativeOrAbsolute, out relativeUri))
                Assert.Fail();

            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/folder/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_CombineUriWithDuplicatePathEntries()
        {
            Uri baseUri = new Uri("http://google.de/remote.php/webdav/folder-sync/x/");
            Uri relativeUri;

            if (!Uri.TryCreate("/remote.php/webdav/folder-sync/x/folder/", UriKind.RelativeOrAbsolute, out relativeUri))
                Assert.Fail();

            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri);
            Uri expected = new Uri("http://google.de/remote.php/webdav/folder-sync/x/remote.php/webdav/folder-sync/x/folder/");
            Assert.AreEqual(expected, combinedUri);
        }
    }
}
