using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    [TestClass]
    public class UnitTestUrlHelper
    {
        [TestMethod]
        public void UnitTestUrlHelperTrailingSlashNeededUrlNetFx()
        {
            string url = "http://www.google.de/test";
            string urlWithTrailingSlash = UrlHelper.AddTrailingSlash(url);
            string expected = url + "/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UnitTestUrlHelperTrailingSlashNeededUriNetFx()
        {
            Uri uri = new Uri("http://www.google.de/test");
            Uri uriWithTrailingSlash = UrlHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("http://www.google.de/test/");
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UnitTestUrlHelperTrailingSlashNotNeededUrlNetFx()
        {
            string url = "http://www.google.de/test/";
            string urlWithTrailingSlash = UrlHelper.AddTrailingSlash(url);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UnitTestUrlHelperTrailingSlashTooManySlashesNetFx()
        {
            string url = "http://www.google.de//test//";
            string urlWithTrailingSlash = UrlHelper.AddTrailingSlash(url);
            string expected = "http://www.google.de/test/";
            Assert.AreEqual(expected, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UnitTestUrlHelperTrailingSlashNotNeededUriNetFx()
        {
            Uri uri = new Uri("http://www.google.de/test/");
            Uri uriWithTrailingSlash = UrlHelper.AddTrailingSlash(uri);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UnitTestUrlHelperTrailingSlashFileUrlNetFx()
        {
            string url = "http://www.google.de/test/test.txt";
            string urlWithTrailingSlash = UrlHelper.AddTrailingSlash(url);
            Assert.AreEqual(url, urlWithTrailingSlash);
        }

        [TestMethod]
        public void UnitTestUrlHelperTrailingSlashFileUri()
        {
            Uri uri = new Uri("http://www.google.de/test/test.txt");
            Uri uriWithTrailingSlash = UrlHelper.AddTrailingSlash(uri);
            Assert.AreEqual(uri, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UnitTestUrlHelperTrailingSlashRelativeUri()
        {
            Uri uri = new Uri("/webdav/test", UriKind.Relative);
            Uri uriWithTrailingSlash = UrlHelper.AddTrailingSlash(uri);
            Uri expected = new Uri("/webdav/test/", UriKind.Relative);
            Assert.AreEqual(expected, uriWithTrailingSlash);
        }

        [TestMethod]
        public void UnitTestUrlHelperGetAbsoluteUriWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("test2/", UriKind.Relative);
            Uri combinedUri = UrlHelper.GetAbsoluteUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UnitTestUrlHelperGetAbsoluteUriWithDoubleBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de/test/");
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UrlHelper.GetAbsoluteUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UnitTestUrlHelperGetAbsoluteUriWithoutBaseUri()
        {
            Uri baseUri = null;
            Uri relativeUri = new Uri("http://www.google.de/test/test2/");
            Uri combinedUri = UrlHelper.GetAbsoluteUri(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UnitTestUrlHelperGetAbsoluteUriWithTrailingSlashWithBaseUri()
        {
            Uri baseUri = new Uri("http://www.google.de");
            Uri relativeUri = new Uri("test2", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UrlHelper.GetAbsoluteUriWithTrailingSlash(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }

        [TestMethod]
        public void UnitTestUrlHelperGetAbsoluteUriWithTrailingSlashWithTooManySlashes()
        {
            Uri baseUri = new Uri("http://www.google.de//");
            Uri relativeUri = new Uri("/test2//", UriKind.RelativeOrAbsolute);
            Uri combinedUri = UrlHelper.GetAbsoluteUriWithTrailingSlash(baseUri, relativeUri);
            Uri expected = new Uri("http://www.google.de/test2/");
            Assert.AreEqual(expected, combinedUri);
        }
    }
}
