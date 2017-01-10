using DecaTec.WebDav.WebDavArtifacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.NetFx.UnitTest
{
    [TestClass]
    public class UnitTestWebDavClient
    {
        private const string FakeUrl = "http://localhost/";       

        [TestMethod]
        public void UT_NetFx_WebDavClient_LockAsync_WithDepthOne()
        {
            var client = CreateWebDavClient();
            var lockInfo = new LockInfo();

            try
            {
                client.LockAsync(FakeUrl, WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(), WebDavDepthHeaderValue.One, lockInfo).Wait();
            }
            catch (AggregateException ae)
            {
                Assert.AreEqual(ae.InnerException.GetType(), typeof(WebDavException));
            }
        }

        [TestMethod]
        public void UT_NetFx_WebDavClient_RefreshLockAsync_WithoutLockToken()
        {
            var client = CreateWebDavClient();
            var lockInfo = new LockInfo();

            try
            {
                client.RefreshLockAsync(FakeUrl, WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(), null).Wait();
            }
            catch (AggregateException ae)
            {
                Assert.AreEqual(ae.InnerException.GetType(), typeof(WebDavException));
            }
        }

        [TestMethod]
        public void UT_NetFx_WebDavClient_PropFindAsync_WithoutDepth()
        {
            var client = CreateWebDavClient();

            try
            {
                client.PropFindAsync(FakeUrl, null).Wait();
            }
            catch (AggregateException ae)
            {
                Assert.AreEqual(ae.InnerException.GetType(), typeof(WebDavException));
            }
        }

        [TestMethod]
        public void UT_NetFx_WebDavClient_UnLockAsync_WithoutLockToken()
        {
            var client = CreateWebDavClient();

            try
            {
                client.UnlockAsync(FakeUrl, null).Wait();
            }
            catch (AggregateException ae)
            {
                Assert.AreEqual(ae.InnerException.GetType(), typeof(WebDavException));
            }
        }

        private WebDavClient CreateWebDavClient()
        {
            // When on NetFx, create with DebugHttpMessageHandler.
            return new WebDavClient(new DebugHttpMessageHandler());
        }
    }
}
