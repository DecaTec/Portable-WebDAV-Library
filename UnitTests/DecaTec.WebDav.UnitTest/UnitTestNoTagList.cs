using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void UT_NoTagList_TryParse_ReturnsFalse_InvalidNoTagListFormatAbsoluteURI()
        {
            var parseResult = NoTagList.TryParse("urn:uuid:my-lock-token", out var noTagList);

            Assert.IsFalse(parseResult);
            Assert.IsNull(noTagList);
        }

        [TestMethod]
        public void UT_NoTagList_TryParse_ReturnsFalse_InvalidNoTagListFormatParentheses()
        {
            var parseResult = NoTagList.TryParse("(urn:uuid:my-lock-token)", out var noTagList);

            Assert.IsFalse(parseResult);
            Assert.IsNull(noTagList);
        }

        [TestMethod]
        public void UT_NoTagList_TryParse_ReturnsFalse_InvalidNoTagListFormatBrackets()
        {
            var parseResult = NoTagList.TryParse("<urn:uuid:my-lock-token>", out var noTagList);

            Assert.IsFalse(parseResult);
            Assert.IsNull(noTagList);
        }
    }
}
