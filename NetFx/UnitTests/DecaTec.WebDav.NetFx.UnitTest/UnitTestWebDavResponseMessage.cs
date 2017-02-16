using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecaTec.WebDav.NetFx.UnitTest
{
	[TestClass]
	public class UnitTestWebDavResponseMessage
	{
		[TestMethod]
		public void UT_NetFx_WebDavResponseMessage_Transparently_Wrap_Headers()
		{
			const string expected = "Jetty(9.3.9.v20160517)";
			var responseToWrap = new HttpResponseMessage();
			responseToWrap.Headers.TryAddWithoutValidation("Server", expected);

			var wrapper = new WebDavResponseMessage(responseToWrap);
			var actual = wrapper.Headers.GetValues("Server").ToString();

			Assert.AreEqual(expected, actual);
		}
	}
}
