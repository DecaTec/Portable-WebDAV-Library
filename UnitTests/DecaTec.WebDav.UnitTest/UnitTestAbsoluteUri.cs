using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
    }
}
