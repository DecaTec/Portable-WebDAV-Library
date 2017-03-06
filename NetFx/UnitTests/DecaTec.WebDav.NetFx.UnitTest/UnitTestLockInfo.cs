using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    [TestClass]
    public class UnitTestLockInfo
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UT_NetFx_LockInfo_ThrowsInvalidOperationExceptionWhenOwnerRawIsSetAndOwnerHrefShouldBeSet()
        {
            var lockInfo = new LockInfo();
            var ownerRawString = "<owner xmlns=\"DAV:\"><href>http://example.org/~ejw/contact.html</href><x:author xmlns:x=\"http://example.com/ns\"><x:name>Jane Doe</x:name></x:author></owner>";
            lockInfo.OwnerRaw = XElement.Parse(ownerRawString);
            lockInfo.OwnerHref = "test@test.com";
        }

        [TestMethod]
        public void UT_NetFx_LockInfo_CanSetOwnerHref()
        {
            var lockInfo = new LockInfo {OwnerHref = "http://localhost/test"};
            Assert.IsNotNull(lockInfo);
        }
    }
}
