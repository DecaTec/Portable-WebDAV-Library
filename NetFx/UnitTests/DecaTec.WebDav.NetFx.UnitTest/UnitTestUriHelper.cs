using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    [TestClass]
    public class UnitTestUriHelper
    {
        [TestMethod]
        public void UT_NetFx_UriHelper_TrailingSlashNeededUrl()
        {
            string url = "http://www.google.de/test";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = url + "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_TrailingSlashNeededUri()
        {
            Uri uri = new Uri("http://www.google.de/test");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_TrailingSlashNotNeededUrl()
        {
            string url = "http://www.google.de/test/";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_TrailingSlashNotNeededUri()
        {
            Uri uri = new Uri("http://www.google.de/test/");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_TrailingSlashTooManySlashes()
        {
            string url = "http://www.google.de//test//";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            string expected = "http://www.google.de/test/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_TrailingSlashFileUrl()
        {
            string url = "http://www.google.de/test/test.txt";
            string urlWithTrailingSlash = UriHelper.AddTrailingSlash(url);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_TrailingSlashFileUri()
        {
            Uri uri = new Uri("http://www.google.de/test/test.txt");
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_TrailingSlashRelativeUri()
        {
            Uri uri = new Uri("/webdav/test", UriKind.Relative);
            Uri uriWithTrailingSlash = UriHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("/webdav/test/", UriKind.Relative);
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetAbsoluteUriWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("test2/", UriKind.Relative);
            Uri combinedUri = UriHelper.GetAbsoluteUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetAbsoluteUriWithDoubleBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UriHelper.GetAbsoluteUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetAbsoluteUriWithBaseUriAndAbsoluteUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test2");
            Uri relativeUri = new Uri("test2/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.GetAbsoluteUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetAbsoluteUriWithBaseUriAndAbsoluteUriAndPathRepetition()
        {
            Uri baseUri = new Uri("http://www.google.de/test/test");
            Uri relativeUri = new Uri("test/test.txt", UriKind.Relative);
            Uri combinedUri = UriHelper.GetAbsoluteUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test/test.txt");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetAbsoluteUriFromSameBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.google.de/test");
            Uri combinedUri = UriHelper.GetAbsoluteUri(baseUri, baseUri2);
            Uri expected = new Uri("http://www.google.de/test");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFxUriHelper_GetAbsoluteUriFromSameBaseUriAndPath()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.google.de/test/test2");
            Uri combinedUri = UriHelper.GetAbsoluteUri(baseUri, baseUri2);
            Uri expected = new Uri("http://www.google.de/test/test2");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetAbsoluteUriWithoutBaseUri()
        {
            Uri baseUri = null;
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UriHelper.GetAbsoluteUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetAbsoluteUriWithTrailingSlashWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de");
            Uri relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetAbsoluteUriWithTrailingSlash(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UT_NetFx_UriHelper_GetAbsoluteUriWithTrailingSlashWithTooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de//");
            Uri relativeUri = new Uri("/test2//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UriHelper.GetAbsoluteUriWithTrailingSlash(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_NetFx_UriHelper_GetAbsoluteUriFromDifferentBaseUris()
        {
            Uri baseUri = new Uri("http://www.google.de/test");
            Uri baseUri2 = new Uri("http://www.github.com/test2");
            Uri combinedUri = UriHelper.GetAbsoluteUriWithTrailingSlash(baseUri, baseUri2);
        }
    }
}
