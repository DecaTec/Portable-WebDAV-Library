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
            Assert.IsNull(absoluteUri);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ToString_WithEncodedUri()
        {
            var input = "opaquelocktoken:dccce564-412e-11e1-b969-00059a3c7a00:%2fFolder%20Test%2fFile.xml";
            AbsoluteUri uri;
            var isParsed = AbsoluteUri.TryParse(input, out uri);
            var actual = uri.ToString();

            Assert.IsTrue(isParsed);
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ToString_WithDecodedUri()
        {
            var input = "opaquelocktoken:dccce564-412e-11e1-b969-00059a3c7a00:/Folder Test/File.xml";
            AbsoluteUri uri;
            var isParsed = AbsoluteUri.TryParse(input, out uri);
            var actual = uri.ToString();

            Assert.IsTrue(isParsed);
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ToString_WithEncodedSegment()
        {
            var input = "opaquelocktoken:dccce564-412e-11e1-b969-00059a3c7a00:/Folder%20Test/File.xml";
            AbsoluteUri uri;
            var isParsed = AbsoluteUri.TryParse(input, out uri);
            var actual = uri.ToString();

            Assert.IsTrue(isParsed);
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ToString_WithDecodedSegment()
        {
            var input = "opaquelocktoken:dccce564-412e-11e1-b969-00059a3c7a00:%2fFolder Test%2fFile.xml";
            AbsoluteUri uri;
            var isParsed = AbsoluteUri.TryParse(input, out uri);
            var actual = uri.ToString();

            Assert.IsTrue(isParsed);
            Assert.AreEqual(input, actual);
        }

    }
}
