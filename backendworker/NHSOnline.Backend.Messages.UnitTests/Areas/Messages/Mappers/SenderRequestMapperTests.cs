using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Messages.Areas.Messages.Mappers;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class SenderRequestMapperTests
    {
        private SenderRequestMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new SenderRequestMapper();
        }

        [TestMethod]
        public void Map_AllFieldsAreSupplied_MapsAllFields()
        {
            // Arrange
            var request = new Sender
            {
                Id = "senderId",
                Name = "Sender Name"
            };

            // Act
            var result = _systemUnderTest.Map(request);

            // Assert
            result.Id.Should().Be("SENDERID");
            result.Name.Should().Be("Sender Name");
        }
    }
}