using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;

namespace DecaTec.WebDav.Uwp.UnitTest
{
    [TestClass]
    public class UnitTestUriHelper
    {
        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashTrailingSlashNeededUrl()
        {
            string url = "http://www.google.de/test";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = url + "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashTrailingSlashNeededUri()
        {
            Uri uri = new Uri("http://www.google.de/test");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashTrailingSlashNotNeededUrl()
        {
            string url = "http://www.google.de/test/";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashTrailingSlashNotNeededUri()
        {
            Uri uri = new Uri("http://www.google.de/test/");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashTrailingSlashTooManySlashes()
        {
            string url = "http://www.google.de//test//";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "http://www.google.de/test/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashTrailingSlashFileUrl()
        {
            string url = "http://www.google.de/test/test.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashTrailingSlashFileUri()
        {
            Uri uri = new Uri("http://www.google.de/test/test.txt");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri, true);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashTrailingSlashRelativeUri()
        {
            Uri uri = new Uri("/webdav/test", UriKind.Relative);
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("/webdav/test/", UriKind.Relative);
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashUrlWithDot()
        {
            string url = "http://www.google.de/a.test/file.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "http://www.google.de/a.test/file.txt/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_AddTrailingSlashUrlWithDotFileExcpected()
        {
            string url = "http://www.google.de/a.test/file.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url, true);
            string expected = "http://www.google.de/a.test/file.txt";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_GetCombinedUriWithTrailingSlashWithTrailingSlashWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de");
            Uri relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("test2/", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriWithDoubleBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriWithBaseUriAndAbsoluteUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriWithBaseUriAndAbsoluteUriRemoveDuplicatePath()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriWithBaseUriAndAbsoluteUriNoRemoveDuplicatePath()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, false);
            Uri expected = new Uri("http://www.google.de/test/test2/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriFromSameBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.google.de/test");
            Uri combinedUri = UriHelper.CombineUri(baseUri, baseUri2, true);
            Uri expected = new Uri("http://www.google.de/test");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriFromSameBaseUriAndPath()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.google.de/test/test2");
            Uri combinedUri = UriHelper.CombineUri(baseUri, baseUri2, true);
            Uri expected = new Uri("http://www.google.de/test/test2");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriWithoutBaseUri()
        {
            Uri baseUri = null;
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriWithTrailingSlashWithTooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de//");
            Uri relativeUri = new Uri("/test2//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriFromDifferentBaseUris()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                Uri baseUri = new Uri("http://www.google.de/test");
                Uri baseUri2 = new Uri("http://www.github.com/test2");
                Uri combinedUri = UriHelper.GetCombinedUriWithTrailingSlash(baseUri, baseUri2);
            });
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriFromBaseUriAndSlash()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri relativeUri = new Uri("/", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriFromBaseUriAndSlashTooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de/test//");
            Uri relativeUri = new Uri("//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriRelativeUriFirst()
        {
            Assert.ThrowsException<ArgumentException>(() =>
            {
                try
                {
                    Uri baseUri = new Uri("/test2", UriKind.RelativeOrAbsolute);
                    Uri relativeUri = new Uri("http://www.google.de/test/");
                    Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
                    Uri expected = new Uri("http://www.google.de/test/");
                    Assert.AreEqual(expected, combinedUri);
                }
                catch (AggregateException ex)
                {
                    throw ex.InnerException;
                }
            });
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriTwoRelativeUris()
        {
            Uri baseUri = new Uri("/test", UriKind.RelativeOrAbsolute);
            Uri relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("/test/test2", UriKind.Relative);
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriFirstUriEndWithSecondUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            Uri relativeUri = new Uri("/test2/test.txt", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_UWP_UriHelper_CombineUriFirstUriEndWithSecondUriWithoutSlashSecondUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2/test.txt");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.CombineUri(baseUri, relativeUri, true);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }
    }
}
