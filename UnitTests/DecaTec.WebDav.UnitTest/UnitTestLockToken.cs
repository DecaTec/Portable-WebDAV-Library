using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecaTec.WebDav.UnitTest
{
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
        public void UT_LockToken_Construct_IfHeaderWithoutBracketsWithString()
        {
            var lockTokenString = "urn:uuid:my-lock-token";

            var lockToken = new LockToken(lockTokenString);
            var absoluteUri = lockToken.AbsoluteUri;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;
            var noTagList = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("urn:uuid:my-lock-token", absoluteUri.ToString());
            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
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
        public void UT_LockToken_Construct_IfHeaderWithBracketsWithString()
        {
            var lockTokenString = "<urn:uuid:my-lock-token>";

            var lockToken = new LockToken(lockTokenString);
            var absoluteUri = lockToken.AbsoluteUri;
            var noTagList = lockToken.IfHeaderNoTagListFormat;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;

            Assert.IsNull(absoluteUri);
            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", noTagList.ToString());
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
        public void UT_LockToken_Construct_LockTokenHeaderWithoutBracketsWithString()
        {
            var lockTokenString = "urn:uuid:my-lock-token";

            var lockToken = new LockToken(lockTokenString);
            var absoluteUri = lockToken.AbsoluteUri;
            var codedUrl = lockToken.LockTokenHeaderFormat;
            var ifHeaderNoTagListFormat = lockToken.IfHeaderNoTagListFormat;

            Assert.AreEqual("urn:uuid:my-lock-token", absoluteUri.ToString());
            Assert.AreEqual("<urn:uuid:my-lock-token>", codedUrl.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", ifHeaderNoTagListFormat.ToString());
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

        [TestMethod]
        public void UT_LockToken_Construct_LockTokenHeaderWithBracketsWithString()
        {
            var lockTokenString = "<urn:uuid:my-lock-token>";

            var lockToken = new LockToken(lockTokenString);
            var absoluteUri = lockToken.AbsoluteUri;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;
            var ifHeaderNoTagListFormat = lockToken.IfHeaderNoTagListFormat;

            Assert.IsNull(absoluteUri);
            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.AreEqual("(<urn:uuid:my-lock-token>)", ifHeaderNoTagListFormat.ToString());
        }

        [TestMethod]
        public void UT_LockToken_LockTokenHeaderFormat_LockTokenHeaderWithBothBracketsWithString()
        {
            var lockTokenString = "(<urn:uuid:my-lock-token>)";

            var parseResult = NoTagList.TryParse(lockTokenString, out var noTagList);

            var lockToken = new LockToken(noTagList.CodedUrl.AbsoluteUri);
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;

            Assert.AreEqual("<urn:uuid:my-lock-token>", lockTokenHeaderFormat.ToString());
            Assert.IsTrue(parseResult);
        }

        [TestMethod]
        public void UT_LockToken_Construct_LockTokenHeaderFormat_LockTokenHeaderWithBothBracketsWithString()
        {
            var lockTokenString = "(<urn:uuid:my-lock-token>)";

            var lockToken = new LockToken(lockTokenString);
            var absoluteUri = lockToken.AbsoluteUri;
            var lockTokenHeaderFormat = lockToken.LockTokenHeaderFormat;
            var ifHeaderNoTagListFormat = lockToken.IfHeaderNoTagListFormat;

            Assert.IsNull(absoluteUri);
            Assert.IsNull(lockTokenHeaderFormat);
            Assert.AreEqual(lockTokenString, ifHeaderNoTagListFormat.ToString());
        }
    }
}
