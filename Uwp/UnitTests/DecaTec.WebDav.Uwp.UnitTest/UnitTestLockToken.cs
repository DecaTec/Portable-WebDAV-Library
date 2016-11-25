using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace DecaTec.WebDav.Uwp.UnitTest
{
    /// <summary>
    /// Summary description for UnitTestLockToken
    /// </summary>
    [TestClass]
    public class UnitTestLockToken
    {
        [TestMethod]
        public void UT_UWP_LockToken_ToStringIfHeaderWithoutBrackets()
        {
            var lockToken = new LockToken("my-lock-token");
            var str = lockToken.ToString(LockTokenFormat.IfHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UT_UWP_LockToken_ToStringIfHeaderWithBrackets()
        {
            var lockToken = new LockToken("(my-lock-token)");
            var str = lockToken.ToString(LockTokenFormat.IfHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UT_UWP_LockToken_ToStringLockTokenHeaderWithoutBrackets()
        {
            var lockToken = new LockToken("my-lock-token");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("<my-lock-token>", str);
        }

        [TestMethod]
        public void UT_UWP_LockToken_ToStringLockTokenHeaderWithBrackets()
        {
            var lockToken = new LockToken("(my-lock-token)");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UT_UWP_LockToken_ToStringLockTokenHeaderWithSquaredBrackets()
        {
            var lockToken = new LockToken("<my-lock-token>");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("<my-lock-token>", str);
        }
    }
}
