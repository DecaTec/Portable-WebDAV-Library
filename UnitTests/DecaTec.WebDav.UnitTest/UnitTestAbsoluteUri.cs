using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestAbsoluteUri
    {
        [TestMethod]
        public void UT_AbsoluteUri_ConsctuctFromAbsoluteUri()
        {
            var uri = new Uri("http://127.0.0.1/", UriKind.Absolute);
            var absoluteUri = new AbsoluteUri(uri);

            Assert.AreEqual(uri.ToString(), absoluteUri.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(WebDavException))]
        public void UT_AbsoluteUri_ConsctuctFromRelativeUri_ShouldThrowException()
        {
            var uri = new Uri("/test", UriKind.Relative);
            var absoluteUri = new AbsoluteUri(uri);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ConsctuctFromAbsoluteUrl()
        {
            var url = "http://127.0.0.1/";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual(url, absoluteUri.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(WebDavException))]
        public void UT_AbsoluteUri_ConsctuctFromRelativeUrl_ShouldThrowException()
        {
            var url = "/test";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual(url, absoluteUri.ToString());
        }

        [TestMethod]
        public void UT_AbsoluteUri_TryParseFromAbsoluteUri()
        {
            var url = "http://127.0.0.1/";
            var result = AbsoluteUri.TryParse(url, out var absoluteUri);

            Assert.IsTrue(result);
            Assert.AreEqual(url, absoluteUri.ToString());
        }

        [TestMethod]
        public void UT_AbsoluteUri_TryParseFromRelativeUri()
        {
            var url = "/test";
            var result = AbsoluteUri.TryParse(url, out var absoluteUri);

            Assert.IsFalse(result);
        }
    }
}
