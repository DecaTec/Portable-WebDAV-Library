using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestAbsoluteUri
    {
        [TestMethod]
        public void UT_AbsoluteUri_TryParse_FromAbsoluteUrl()
        {
            var url = "http://127.0.0.1/";
            var isParsed = AbsoluteUri.TryParse(url, out var absoluteUri);

            Assert.IsTrue(isParsed);
            Assert.AreEqual(url, absoluteUri.ToString());
        }

        [TestMethod]
        public void UT_AbsoluteUri_TryParse_FromRelativeUrl()
        {
            var url = "/test";
            var isParsed = AbsoluteUri.TryParse(url, out var absoluteUri);

            Assert.IsFalse(isParsed);
            Assert.IsNull(absoluteUri);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ToString_WithEncodedUri()
        {
            var input = "opaquelocktoken:dccce564-412e-11e1-b969-00059a3c7a00:%2fFolder%20Test%2fFile.xml";
            var isParsed = AbsoluteUri.TryParse(input, out AbsoluteUri uri);
            var actual = uri.ToString();

            Assert.IsTrue(isParsed);
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ToString_WithDecodedUri()
        {
            var input = "opaquelocktoken:dccce564-412e-11e1-b969-00059a3c7a00:/Folder Test/File.xml";
            var isParsed = AbsoluteUri.TryParse(input, out AbsoluteUri uri);
            var actual = uri.ToString();

            Assert.IsTrue(isParsed);
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ToString_WithEncodedSegment()
        {
            var input = "opaquelocktoken:dccce564-412e-11e1-b969-00059a3c7a00:/Folder%20Test/File.xml";
            var isParsed = AbsoluteUri.TryParse(input, out AbsoluteUri uri);
            var actual = uri.ToString();

            Assert.IsTrue(isParsed);
            Assert.AreEqual(input, actual);
        }

        [TestMethod]
        public void UT_AbsoluteUri_ToString_WithDecodedSegment()
        {
            var input = "opaquelocktoken:dccce564-412e-11e1-b969-00059a3c7a00:%2fFolder Test%2fFile.xml";
            var isParsed = AbsoluteUri.TryParse(input, out AbsoluteUri uri);
            var actual = uri.ToString();

            Assert.IsTrue(isParsed);
            Assert.AreEqual(input, actual);
        }

    }
}
