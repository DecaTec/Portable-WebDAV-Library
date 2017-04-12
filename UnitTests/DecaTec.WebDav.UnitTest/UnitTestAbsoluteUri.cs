using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestAbsoluteUri
    {
        [TestMethod]
        public void UT_AbsoluteUri_Consctuct_FromAbsoluteUri()
        {
            var uri = new Uri("http://127.0.0.1/", UriKind.Absolute);
            var absoluteUri = new AbsoluteUri(uri);

            Assert.AreEqual(uri.ToString(), absoluteUri.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_AbsoluteUri_Consctuct_FromRelativeUri_ShouldThrowArgumentException()
        {
            var uri = new Uri("/test", UriKind.Relative);
            var absoluteUri = new AbsoluteUri(uri);
        }

        [TestMethod]
        public void UT_AbsoluteUri_Consctuct_FromAbsoluteUrl()
        {
            var url = "http://127.0.0.1/";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual(url, absoluteUri.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_AbsoluteUri_Consctuct_FromRelativeUrl_ShouldThrowArgumentException()
        {
            var url = "/test";
            var absoluteUri = new AbsoluteUri(url);

            Assert.AreEqual(url, absoluteUri.ToString());
        }

        [TestMethod]
        public void UT_AbsoluteUri_TryParse_FromAbsoluteUri()
        {
            var url = "http://127.0.0.1/";
            var result = AbsoluteUri.TryParse(url, out var absoluteUri);

            Assert.IsTrue(result);
            Assert.AreEqual(url, absoluteUri.ToString());
        }

        [TestMethod]
        public void UT_AbsoluteUri_TryParse_FromRelativeUri()
        {
            var url = "/test";
            var result = AbsoluteUri.TryParse(url, out var absoluteUri);

            Assert.IsFalse(result);
        }
    }
}
