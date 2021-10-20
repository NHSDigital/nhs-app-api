using System;
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
        private const string Url = "https://www.testing.com/valid/url/";

        [TestInitialize]
        public void TestInitialise()
        {
            _subjectUnderTest = new MessageLinkClickedValidationService(
                new Mock<ILogger<MessageLinkClickedValidationService>>().Object
            );
        }

        [DataTestMethod]
        [DataRow(null, MessageId, Url, false)]
        [DataRow(Blank, MessageId, Url, false)]
        [DataRow(NhsLoginId, null, Url, false)]
        [DataRow(NhsLoginId, Blank, Url, false)]
        [DataRow(NhsLoginId, MessageId, null, false)]
        [DataRow(NhsLoginId, MessageId, Url, true)]
        public void IsServiceRequestValid(string nhsLoginId, string messageId, string url, bool expected)
        {
            // Arrange
            var link = url == null
                ? null
                : new Uri(url);

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