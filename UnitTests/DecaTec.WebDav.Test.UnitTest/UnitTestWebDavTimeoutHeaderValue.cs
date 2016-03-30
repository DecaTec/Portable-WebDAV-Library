using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DecaTec.WebDav.Test.UnitTest
{
    [TestClass]
    public class UnitTestWebDavTimeoutHeaderValue
    {
        [TestMethod]
        public void UnitTestWebDavTimeoutHeaderValueToStringInfinite()
        {
            var wdthv = WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeout();

            Assert.AreEqual(wdthv.ToString(), "Infinite");
        }

        [TestMethod]
        public void UnitTestWebDavTimeoutHeaderValueToStringTimeout()
        {
            var wdthv = WebDavTimeoutHeaderValue.CreateWebDavTimeout(TimeSpan.FromSeconds(500));

            Assert.AreEqual(wdthv.ToString(), "Second-500");
        }

        [TestMethod]
        public void UnitTestWebDavTimeoutHeaderValueToStringInfiniteWithAlternativeTimeout()
        {
            var wdthv = WebDavTimeoutHeaderValue.CreateInfiniteWebDavTimeoutWithAlternative(TimeSpan.FromSeconds(500));

            Assert.AreEqual(wdthv.ToString(), "Infinite, Second-500");
        }
    }
}
