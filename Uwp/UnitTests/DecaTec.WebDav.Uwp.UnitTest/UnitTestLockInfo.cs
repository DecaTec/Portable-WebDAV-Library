using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Xml.Linq;

namespace DecaTec.WebDav.Uwp.UnitTest
{
    [TestClass]
    public class UnitTestLockInfo
    {
        [TestMethod]
        public void UT_UWP_LockInfo_ThrowsInvalidOperationExceptionWhenOwnerRawIsSetAndOwnerHrefShouldBeSet()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                var lockInfo = new LockInfo();
                var ownerRawString = "<owner xmlns=\"DAV:\"><href>http://example.org/~ejw/contact.html</href><x:author xmlns:x=\"http://example.com/ns\"><x:name>Jane Doe</x:name></x:author></owner>";
                lockInfo.OwnerRaw = XElement.Parse(ownerRawString);
                lockInfo.OwnerHref = "test@test.com";
            });
        }

        [TestMethod]
        public void UT_UWP_LockInfo_CanSetOwnerHref()
        {
            var lockInfo = new LockInfo { OwnerHref = "http://localhost/test" };
            Assert.IsNotNull(lockInfo);
        }
    }
}
