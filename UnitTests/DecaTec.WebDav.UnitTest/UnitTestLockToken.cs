using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecaTec.WebDav.UnitTest
{
    [TestClass]
    public class UnitTestLockToken
    {
        [TestMethod]
        public void UT_LockToken_IfHeaderNoTagListFormat_IfHeaderWithoutBrackets()
        {
            var parseResult = AbsoluteUri.TryParse("urn:uuid:my-lock-token", out var parsedUri);

            var lockToken = new LockToken(parsedUri);

            var absoluteUri = lockToken.AbsoluteUri;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;
            var noTagList = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("urn:uuid:my-lock-token", absoluteUri.ToString());
            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
            Assert.IsTrue(parseResult);
        }

        [TestMethod]
        public void UT_LockToken_IfHeaderNoTagListFormat_IfHeaderWithBrackets()
        {
            var parseResult = CodedUrl.TryParse("<urn:uuid:my-lock-token>", out var codedUrl);

            var lockToken = new LockToken(codedUrl.AbsoluteUri);
            var absoluteUri = lockToken.AbsoluteUri;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;
            var noTagList = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("urn:uuid:my-lock-token", absoluteUri.ToString());
            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
            Assert.IsTrue(parseResult);
        }

        [TestMethod]
        public void UT_LockToken_LockTokenHeaderFormat_LockTokenHeaderWithoutBrackets()
        {
            var parseResult = AbsoluteUri.TryParse("urn:uuid:my-lock-token", out var parsedUri);

            var lockToken = new LockToken(parsedUri);
            var absoluteUri = lockToken.AbsoluteUri;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;
            var noTagList = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("urn:uuid:my-lock-token", absoluteUri.ToString());
            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
            Assert.IsTrue(parseResult);
        }

        [TestMethod]
        public void UT_LockToken_LockTokenHeaderFormat_LockTokenHeaderWithBrackets()
        {
            var parseResult = CodedUrl.TryParse("<urn:uuid:my-lock-token>", out var codedUrl);

            var lockToken = new LockToken(codedUrl.AbsoluteUri);

            var absoluteUri = lockToken.AbsoluteUri;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;
            var noTagList = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("urn:uuid:my-lock-token", absoluteUri.ToString());
            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
            Assert.IsTrue(parseResult);
        }

        [TestMethod]
        public void UT_LockToken_LockTokenHeaderFormat_LockTokenHeaderWithBothBracketsWithString()
        {
            var lockTokenString = "(<urn:uuid:my-lock-token>)";

            var parseResult = NoTagList.TryParse(lockTokenString, out var parsedNoTagList);
            var lockToken = new LockToken(parsedNoTagList.CodedUrl.AbsoluteUri);

            var absoluteUri = lockToken.AbsoluteUri;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;
            var noTagList = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("urn:uuid:my-lock-token", absoluteUri.ToString());
            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
            Assert.IsTrue(parseResult);
        }
    }
}
