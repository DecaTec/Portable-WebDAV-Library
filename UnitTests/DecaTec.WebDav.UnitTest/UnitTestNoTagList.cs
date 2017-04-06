using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecaTec.WebDav.UnitTest
{
	[TestClass]
    public class UnitTestNoTagList
    {
	    [TestMethod]
	    public void UT_NoTagList_TryParse_ValidNoTagListFormat()
	    {
		    var parseResult = NoTagList.TryParse("(<urn:uuid:my-lock-token>)", out var codedUrl);

		    Assert.IsTrue(parseResult);
		    Assert.IsNotNull(codedUrl);
	    }

	    [TestMethod]
	    public void UT_NoTagList_TryParse_ReturnsFalse_InvalidNoTagListFormatAbsoluteURI()
	    {
		    var parseResult = NoTagList.TryParse("urn:uuid:my-lock-token", out var codedUrl);

		    Assert.IsFalse(parseResult);
		    Assert.IsNull(codedUrl);
	    }

	    [TestMethod]
	    public void UT_NoTagList_TryParse_ReturnsFalse_InvalidNoTagListFormatParentheses()
	    {
		    var parseResult = NoTagList.TryParse("(urn:uuid:my-lock-token)", out var codedUrl);

		    Assert.IsFalse(parseResult);
		    Assert.IsNull(codedUrl);
	    }

	    [TestMethod]
	    public void UT_NoTagList_TryParse_ReturnsFalse_InvalidNoTagListFormatBrackets()
	    {
		    var parseResult = NoTagList.TryParse("<urn:uuid:my-lock-token>", out var codedUrl);

		    Assert.IsFalse(parseResult);
		    Assert.IsNull(codedUrl);
	    }
	}
}
