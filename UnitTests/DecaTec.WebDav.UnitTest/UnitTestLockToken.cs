using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecaTec.WebDav.UnitTest
{
    /// <summary>
    /// Summary description for UnitTestLockToken
    /// </summary>
    [TestClass]
    public class UnitTestLockToken
    {
        [TestMethod]
        public void UT_LockToken_IfHeaderNoTagListFormat_IfHeaderWithoutBrackets()
        {
            var parseResult = AbsoluteUri.TryParse("urn:uuid:my-lock-token", out var absoluteUri);

            var lockToken = new LockToken(absoluteUri);
            var noTagList = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
            Assert.IsTrue(parseResult);
        }

        [TestMethod]
        public void UT_LockToken_IfHeaderNoTagListFormat_IfHeaderWithBrackets()
        {
            var parseResult = CodedUrl.TryParse("<urn:uuid:my-lock-token>", out var codedUrl);

            var lockToken = new LockToken(codedUrl.AbsoluteUri);
            var noTagList = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
            Assert.IsTrue(parseResult);
        }

        [TestMethod]
        public void UT_LockToken_LockTokenHeaderFormat_LockTokenHeaderWithoutBrackets()
        {
            var parseResult = AbsoluteUri.TryParse("urn:uuid:my-lock-token", out var absoluteUri);

            var lockToken = new LockToken(absoluteUri);
            var codedUrl = lockToken.LockTokenHeaderFormat;

            Assert.AreEqual("<urn:uuid:my-lock-token>", codedUrl.ToString());
            Assert.IsTrue(parseResult);
        }

        [TestMethod]
        public void UT_LockToken_LockTokenHeaderFormat_LockTokenHeaderWithBrackets()
        {
            var parseResult = CodedUrl.TryParse("<urn:uuid:my-lock-token>", out var codedUrl);

            var lockToken = new LockToken(codedUrl.AbsoluteUri);
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;

            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.IsTrue(parseResult);
        }
    }
}
