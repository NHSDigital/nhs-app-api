using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSearch;

namespace NHSOnline.Backend.Worker.UnitTests.GpSearch
{
    [TestClass]
    public class PostcodeParserTests
    {
        private IPostcodeParser _postcodeParser; 
        
        [TestInitialize]
        public void TestInitialize()
        {
            _postcodeParser = new PostcodeParser();
        }
        
        [TestMethod]
        [DataRow("L15BW", "5BW")]
        [DataRow("L1 5BW", " 5BW")]
        [DataRow("l1 5bw", " 5bw")]
        [DataRow("L1", "")] 
        public void ParseSearchTermForPostcodeMatch_WhenCalledWithValidPostCode_ReturnsPostcodeMatchedData(string postcode, string expectedInward)
        {   
            var result = _postcodeParser.ParseSearchTermForPostcodeMatch(postcode);

            Assert.IsTrue(result.IsPostcode);
            Assert.AreEqual(postcode, result.Postcode);
            Assert.AreEqual(expectedInward, result.Inward);
        }
        
        [TestMethod]
        public void ParseSearchTermForPostcodeMatch_WhenCalledWithInvalidPostcode_ReturnsIsPostcodeFalse()
        {
            const string validPostCode = "Liverpool";
            
            var result = _postcodeParser.ParseSearchTermForPostcodeMatch(validPostCode);

            Assert.IsTrue(!result.IsPostcode);
        }
    }
}