using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace DecaTec.WebDav.Uwp.UnitTest
{
    [TestClass]
    public class UnitTestTextHelper
    {
        [TestMethod]
        public void UT_UWP_TextHelper_StringContainsRawUnicodeWithRawUnicode()
        {
            string strUni = "\u03a0";
            var res = TextHelper.StringContainsRawUnicode(strUni);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void UT_UWP_TextHelper_StringContainsRawUnicodeWithOutRawUnicode()
        {
            string strUni = "Test";
            var res = TextHelper.StringContainsRawUnicode(strUni);
            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void UT_UWP_TextHelper_StringContainsRawUnicodeWithEmptyString()
        {
            string strUni = string.Empty; ;
            var res = TextHelper.StringContainsRawUnicode(strUni);
            Assert.AreEqual(false, res);
        }
    }
}
