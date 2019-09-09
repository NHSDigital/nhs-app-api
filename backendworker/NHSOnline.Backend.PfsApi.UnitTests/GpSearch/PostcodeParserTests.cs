using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.GpSearch;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSearch
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
            // Act
            var result = _postcodeParser.ParseSearchTermForPostcodeMatch(postcode);

            // Assert
            result.IsPostcode.Should().BeTrue();
            result.Postcode.Should().Be(postcode);
            result.Inward.Should().Be(expectedInward);
        }
        
        [TestMethod]
        public void ParseSearchTermForPostcodeMatch_WhenCalledWithInvalidPostcode_ReturnsIsPostcodeFalse()
        {
            // Arrange
            const string invalidPostcode = "Liverpool";
            
            // Act
            var result = _postcodeParser.ParseSearchTermForPostcodeMatch(invalidPostcode);

            // Assert
            result.IsPostcode.Should().BeFalse();
        }
    }
}