using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecaTec.WebDav.UnitTest
{
	[TestClass]
    public class UnitTestCodedUrl
    {
	    [TestMethod]
	    public void UT_CodedUrl_TryParse_ValidCodedUrlFormat()
	    {
		    var parseResult = CodedUrl.TryParse("<urn:uuid:my-lock-token>", out var codedUrl);

		    Assert.IsTrue(parseResult);
		    Assert.IsNotNull(codedUrl);
	    }

	    [TestMethod]
	    public void UT_CodedUrl_TryParse_ReturnsFalse_InvalidCodedUrlFormatAbsoluteURI()
	    {
		    var parseResult = CodedUrl.TryParse("urn:uuid:my-lock-token", out var codedUrl);

		    Assert.IsFalse(parseResult);
		    Assert.IsNull(codedUrl);
	    }

		[TestMethod]
	    public void UT_CodedUrl_TryParse_ReturnsFalse_InvalidCodedUrlFormatParentheses()
	    {
		    var parseResult = CodedUrl.TryParse("(urn:uuid:my-lock-token)", out var codedUrl);

			Assert.IsFalse(parseResult);
			Assert.IsNull(codedUrl);
	    }

	    [TestMethod]
	    public void UT_CodedUrl_TryParse_ReturnsFalse_InvalidCodedUrlFormatParenthesesAndBrackets()
	    {
		    var parseResult = CodedUrl.TryParse("(<urn:uuid:my-lock-token>)", out var codedUrl);

		    Assert.IsFalse(parseResult);
		    Assert.IsNull(codedUrl);
	    }
	}
}
