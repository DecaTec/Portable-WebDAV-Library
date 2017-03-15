using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DecaTec.WebDav.UnitTest
{
	[TestClass]
	public class UnitTestWebDavResponseMessage
	{
        [TestMethod]
        public void UT_WebDavResponseMessage_Transparently_Wrap_Headers()
        {
            const string expected = "Jetty(9.3.9.v20160517)";
            var responseToWrap = new HttpResponseMessage();
            responseToWrap.Headers.TryAddWithoutValidation("Server", expected);

            var wrapper = new WebDavResponseMessage(responseToWrap);
            var actual = wrapper.Headers.GetValues("Server").FirstOrDefault();

			Assert.AreEqual(expected, actual);
		}
	}
}
