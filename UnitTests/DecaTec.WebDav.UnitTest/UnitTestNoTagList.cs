using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestNoTagList
    {
        [TestMethod]
        public void UT_NoTagList_Construct_ValidNoTagListFormat()
        {
            var expectedString = "(<urn:uuid:my-lock-token>)";
            var noTagList = new NoTagList(expectedString);

            Assert.AreEqual(expectedString, noTagList.ToString());
            Assert.IsNotNull(noTagList);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_NoTagList_Construct_InvalidNoTagListFormatAbsoluteURI_ShouldThrowArgumentException()
        {
            var noTagList = new NoTagList("urn:uuid:my-lock-token");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_NoTagList_Construct_InvalidNoTagListFormatParentheses_ShouldThrowArgumentException()
        {
            var noTagList = new NoTagList("(urn:uuid:my-lock-token)");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UT_NoTagList_Construct_InvalidNoTagListFormatBrackets_ShouldThrowArgumentException()
        {
            var noTagList = new NoTagList("<urn:uuid:my-lock-token>");
        }

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
