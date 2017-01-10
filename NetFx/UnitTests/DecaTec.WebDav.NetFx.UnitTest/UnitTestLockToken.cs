using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    /// <summary>
    /// Summary description for UnitTestLockToken
    /// </summary>
    [TestClass]
    public class UnitTestLockToken
    {
        [TestMethod]
        public void UT_NetFx_LockToken_ToString_IfHeaderWithoutBrackets()
        {
            var lockToken = new LockToken("my-lock-token");
            var str = lockToken.ToString(LockTokenFormat.IfHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UT_NetFx_LockToken_ToString_IfHeaderWithBrackets()
        {
            var lockToken = new LockToken("(my-lock-token)");
            var str = lockToken.ToString(LockTokenFormat.IfHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UT_NetFx_LockToken_ToString_LockTokenHeaderWithoutBrackets()
        {
            var lockToken = new LockToken("my-lock-token");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("<my-lock-token>", str);
        }

        [TestMethod]
        public void UT_NetFx_LockToken_ToString_LockTokenHeaderWithBrackets()
        {
            var lockToken = new LockToken("(my-lock-token)");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UT_NetFx_LockToken_ToString_LockTokenHeaderWithSquaredBrackets()
        {
            var lockToken = new LockToken("<my-lock-token>");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("<my-lock-token>", str);
        }
    }
}
