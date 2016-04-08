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
        public void UnitTestLockTokenToStringIfHeaderWithoutBrackets()
        {
            var lockToken = new LockToken("my-lock-token");
            var str = lockToken.ToString(LockTokenFormat.IfHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UnitTestLockTokenToStringIfHeaderWithBrackets()
        {
            var lockToken = new LockToken("(my-lock-token)");
            var str = lockToken.ToString(LockTokenFormat.IfHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UnitTestLockTokenToStringLockTokenHeaderWithoutBrackets()
        {
            var lockToken = new LockToken("my-lock-token");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("<my-lock-token>", str);
        }

        [TestMethod]
        public void UnitTestLockTokenToStringLockTokenHeaderWithBrackets()
        {
            var lockToken = new LockToken("(my-lock-token)");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("(my-lock-token)", str);
        }

        [TestMethod]
        public void UnitTestLockTokenToStringLockTokenHeaderWithSquaredBrackets()
        {
            var lockToken = new LockToken("<my-lock-token>");
            var str = lockToken.ToString(LockTokenFormat.LockTokenHeader);

            Assert.AreEqual("<my-lock-token>", str);
        }
    }
}
