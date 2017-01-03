using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    [TestClass]
    public class UnitTestTextHelper
    {
        [TestMethod]
        public void UT_NetFx_TextHelper_StringContainsRawUnicodeWithRawUnicode()
        {
            string strUni = "\u03a0";
            var res = TextHelper.StringContainsRawUnicode(strUni);
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void UT_NetFx_TextHelper_StringContainsRawUnicodeWithOutRawUnicode()
        {
            string strUni = "Test";
            var res = TextHelper.StringContainsRawUnicode(strUni);
            Assert.AreEqual(false, res);
        }

        [TestMethod]
        public void UT_NetFx_TextHelper_StringContainsRawUnicodeWithEmptyString()
        {
            string strUni = string.Empty; ;
            var res = TextHelper.StringContainsRawUnicode(strUni);
            Assert.AreEqual(false, res);
        }
    }
}
