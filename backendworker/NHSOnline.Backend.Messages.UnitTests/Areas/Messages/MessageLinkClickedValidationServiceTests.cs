using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.MessagesApi.Areas.Messages;
using NHSOnline.Backend.MessagesApi.Areas.Messages.Models;

namespace NHSOnline.Backend.MessagesApi.UnitTests.Areas.Messages
{
    [TestClass]
    public class MessageLinkClickedValidationServiceTests
    {
        private MessageLinkClickedValidationService _subjectUnderTest;

        private const string Blank = "";
        private const string NhsLoginId = "NhsLoginId";
        private const string MessageId = "MessageId";
        private const string Link = "https://www.testing.com/valid/url/";

        [TestInitialize]
        public void TestInitialise()
        {
            _subjectUnderTest = new MessageLinkClickedValidationService(
                new Mock<ILogger<MessageLinkClickedValidationService>>().Object
            );
        }

        [DataTestMethod]
        [DataRow(null, MessageId, Link, false)]
        [DataRow(Blank, MessageId, Link, false)]
        [DataRow(NhsLoginId, null, Link, false)]
        [DataRow(NhsLoginId, Blank, Link, false)]
        [DataRow(NhsLoginId, MessageId, null, false)]
        [DataRow(NhsLoginId, MessageId, Link, true)]
        public void IsServiceRequestValid(string nhsLoginId, string messageId, string link, bool expected)
        {
            // Arrange
            var messageLink = new MessageLink
            {
                MessageId = messageId,
                Link = link
            };

            // Act
            var actual = _subjectUnderTest.IsServiceRequestValid(nhsLoginId, messageLink);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}