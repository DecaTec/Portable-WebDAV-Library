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
        public void UnitTestWebDavClientLockAsyncWithDepthOneNetFx()
        {
            var client = CreateWebDavClientWithDebugHttpMessageHandler();
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
        public void UnitTestWebDavClientRefreshLockAsyncWithoutLockTokenNetFx()
        {
            var client = CreateWebDavClientWithDebugHttpMessageHandler();
            var lockInfo = new LockInfo();

            try
            {
                client.RefreshLockAsync(FakeUrl,WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout(), null).Wait();
            }
            catch (AggregateException ae)
            {
                Assert.AreEqual(ae.InnerException.GetType(), typeof(WebDavException));
            }
        }

        [TestMethod]
        public void UnitTestWebDavClientPropFindAsyncWithoutDepthNetFx()
        {
            var client = CreateWebDavClientWithDebugHttpMessageHandler();

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
        public void UnitTestWebDavClientUnLockAsyncWithoutLockTokenNetFx()
        {
            var client = CreateWebDavClientWithDebugHttpMessageHandler();

            try
            {
                client.UnlockAsync(FakeUrl, null).Wait();
            }
            catch (AggregateException ae)
            {
                Assert.AreEqual(ae.InnerException.GetType(), typeof(WebDavException));
            }
        }

        private WebDavClient CreateWebDavClientWithDebugHttpMessageHandler()
        {
            return new WebDavClient(new DebugHttpMessageHandler());
        }
    }
}
