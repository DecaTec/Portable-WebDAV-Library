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
        public void UnitTestLockTokenToStringIfHeaderWithoutBracketsNetFx()
        {
            var lockToken = new LockToken("my-lock-token");
            var str = lockToken.ToString(LockTokenFormat.IfHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UnitTestLockTokenToStringIfHeaderWithBracketsNetFx()
        {
            var lockToken = new LockToken("(my-lock-token)");
            var str = lockToken.ToString(LockTokenFormat.IfHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UnitTestLockTokenToStringLockTokenHeaderWithoutBracketsNetFx()
        {
            var lockToken = new LockToken("my-lock-token");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("<my-lock-token>", str);
        }

        [TestMethod]
        public void UnitTestLockTokenToStringLockTokenHeaderWithBracketsNetFx()
        {
            var lockToken = new LockToken("(my-lock-token)");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UnitTestLockTokenToStringLockTokenHeaderWithSquaredBracketsNetFx()
        {
            var lockToken = new LockToken("<my-lock-token>");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("<my-lock-token>", str);
        }
    }
}
