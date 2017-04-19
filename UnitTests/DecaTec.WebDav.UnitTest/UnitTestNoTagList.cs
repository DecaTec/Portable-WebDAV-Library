using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestNoTagList
    {
        [TestMethod]
        public void UT_NoTagList_TryParse_ValidNoTagListFormat()
        {
            var expectedString = "(<urn:uuid:my-lock-token>)";
            var parseResult = NoTagList.TryParse(expectedString, out var noTagList);

            Assert.AreEqual(expectedString, noTagList.ToString());
            Assert.IsTrue(parseResult);
            Assert.IsNotNull(noTagList);
        }

        [TestMethod]
        public void UT_NoTagList_TryParse_InvalidNoTagListFormatAbsoluteURI_ShouldReturnFalse()
        {
            var parseResult = NoTagList.TryParse("urn:uuid:my-lock-token", out var noTagList);

            Assert.IsFalse(parseResult);
            Assert.IsNull(noTagList);
        }

        [TestMethod]
        public void UT_NoTagList_TryParse_InvalidNoTagListFormatParentheses_ShouldReturnFalse()
        {
            var parseResult = NoTagList.TryParse("(urn:uuid:my-lock-token)", out var noTagList);

            Assert.IsFalse(parseResult);
            Assert.IsNull(noTagList);
        }

        [TestMethod]
        public void UT_NoTagList_ReturnsFalse_InvalidNoTagListFormatBrackets_ShouldReturnFalse()
        {
            var parseResult = NoTagList.TryParse("<urn:uuid:my-lock-token>", out var noTagList);

            Assert.IsFalse(parseResult);
            Assert.IsNull(noTagList);
        }
    }
}
