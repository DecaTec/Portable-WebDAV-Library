using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestCodedUrl
    {
        [TestMethod]
        public void UT_CodedUrl_Construct_ValidCodedUrlFormat()
        {
            var expectedString = "<urn:uuid:my-lock-token>";
            var codedUrl = new CodedUrl(expectedString);

            Assert.IsNotNull(codedUrl);
            Assert.AreEqual(expectedString, codedUrl.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_CodedUrl_Construct_InvalidCodedUrlFormatAbsoluteUri_ShouldThrowArgumentException()
        {
            var codedUrl = new CodedUrl("urn:uuid:my-lock-token");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_CodedUrl_Construct_InvalidCodedUrlFormatParentheses_ShouldThrowArgumentException()
        {
            var codedUrl = new CodedUrl("(urn:uuid:my-lock-token)");
        }

        [TestMethod]
        public void UT_CodedUrl_TryParse_ValidCodedUrlFormat()
        {
            var expectedString = "<urn:uuid:my-lock-token>";
            var parseResult = CodedUrl.TryParse(expectedString, out var codedUrl);

            Assert.IsTrue(parseResult);
            Assert.IsNotNull(codedUrl);
            Assert.AreEqual(expectedString, codedUrl.ToString());
        }

        [TestMethod]
        public void UT_CodedUrl_TryParse_InvalidCodedUrlFormatAbsoluteUri_ShouldReturnFalse()
        {
            var parseResult = CodedUrl.TryParse("urn:uuid:my-lock-token", out var codedUrl);

            Assert.IsFalse(parseResult);
            Assert.IsNull(codedUrl);
        }

        [TestMethod]
        public void UT_CodedUrl_TryParse_InvalidCodedUrlFormatParentheses_ShouldReturnFalse()
        {
            var parseResult = CodedUrl.TryParse("(urn:uuid:my-lock-token)", out var codedUrl);

            Assert.IsFalse(parseResult);
            Assert.IsNull(codedUrl);
        }
    }
}
