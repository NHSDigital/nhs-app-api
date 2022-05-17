using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages.Mappers;
using NHSOnline.Backend.Messages.Areas.Messages.Models;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages.Mappers
{
    [TestClass]
    public class MessageSenderContextEventLogDataMapperTests
    {
        private MessageSenderContextEventLogDataMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest =
                new MessageSenderContextEventLogDataMapper(new Mock<ILogger<MessageSenderContextEventLogDataMapper>>()
                    .Object);
        }

        [TestMethod]
        public void Map_WhenSenderContextIsNull_ThrowsArgumentNullException()
        {
            // Act and Assert
            Action act = () => _systemUnderTest.Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("source");
        }

        [TestMethod]
        public void Map_WithSenderContext_MapsToEventLogData()
        {
            // Arrange
            var senderContext = new SenderContext
            {
                CampaignId = "Campaign ID",
                CommunicationCreatedDateTime = new DateTime(2021, 04, 22, 01, 05, 25),
                CommunicationId = "Communication ID",
                NhsLoginId = "NHS Login ID",
                NhsNumber = "NHS Number",
                OdsCode = "ODS Code",
                RequestReference = "Request Reference",
                SenderId = "Sender ID",
                SupplierId = "Supplier ID",
                TransmissionId = "Transmission ID"
            };

            // Act
            var result = _systemUnderTest.Map(senderContext);

            // Assert
            result.Should().NotBeNull();
            var allData = result.ToKeyValuePairs(true).ToList();

            allData.Should().HaveCount(10);
            AssertContain(allData, "SupplierId", "Supplier ID");
            AssertContain(allData, "SenderId", "Sender ID");
            AssertContain(allData, "CommunicationId", "Communication ID");
            AssertContain(allData, "TransmissionId", "Transmission ID");
            AssertContain(allData, "CommunicationCreatedDateTime", "2021-04-22T01:05:25:000");
            AssertContain(allData, "RequestReference", "Request Reference");
            AssertContain(allData, "CampaignId", "Campaign ID");
            AssertContain(allData, "OdsCode", "ODS Code");
            AssertContain(allData, "NhsLoginId", "NHS Login ID");
            AssertContain(allData, "NhsNumber", "NHS Number");
        }

        private static void AssertContain(IEnumerable<KeyValuePair<string, string>> values, string key, string value) =>
            values.Should().Contain(x => x.Key == key && x.Value == value);
    }
}