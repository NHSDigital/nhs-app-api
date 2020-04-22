using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientPracticeMessaging
{
    [TestClass]
    public class TppPatientMessagesUnreadIdsMapperTests
    {
        private List<MessageDetails> _testMessages;

        private TppPatientMessagesUnreadIdsMapper _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _testMessages = Enumerable.Range(1, 5)
                .Select(i =>
                    new MessageDetails
                    {
                        ConversationId = "1",
                        MessageId = $"00{i}",
                        Read = YesNo.n
                    })
                .ToList();

            _systemUnderTest = new TppPatientMessagesUnreadIdsMapper();
        }

        [TestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty")]
        [DataRow("\t\n  \r\n", DisplayName = "blank")]
        public void Map_WhenConversationIdIsNullOrEmpty_ThenArgumentExceptionIsThrown(string value)
        {
            _systemUnderTest.Invoking(s => s.Map(new List<MessageDetails>(), value))
                .Should()
                .Throw<ArgumentException>();
        }

        [TestMethod]
        public void Map_WhenConversationMessagesIsNull_ThenEmptyListIsReturned()
        {
            _systemUnderTest.Map(null, "someId")
                .Should()
                .BeEmpty();
        }

        [TestMethod]
        public void Map_WhenConversationMessagesIsEmpty_ThenEmptyListIsReturned()
        {
            _systemUnderTest.Map(new List<MessageDetails>(), "someId")
                .Should()
                .BeEmpty();
        }

        [TestMethod]
        public void Map_WhenConversationMessagesHasUnreadItemsMatchingConversationId_ThenItemIdsAreReturned()
        {
            _systemUnderTest.Map(_testMessages, "1")
                .Should()
                .Equal("001", "002", "003", "004", "005");
        }

        [TestMethod]
        public void Map_WhenConversationMessagesHasReadItemsMatchingConversationId_ThenItemsAreIgnored()
        {
            _testMessages.Add(new MessageDetails
            {
                ConversationId = "1",
                MessageId = $"006",
                Read = YesNo.y
            });

            _systemUnderTest.Map(_testMessages, "1")
                .Should()
                .Equal("001", "002", "003", "004", "005");
        }

        [TestMethod]
        public void Map_WhenConversationMessagesHasReadItemsWithoutMatchingConversationId_ThenItemsAreIgnored()
        {
            _testMessages.Add(new MessageDetails
            {
                ConversationId = "2",
                MessageId = $"006",
                Read = YesNo.n
            });

            _systemUnderTest.Map(_testMessages, "1")
                .Should()
                .Equal("001", "002", "003", "004", "005");
        }

        [TestMethod]
        [DataRow(null, DisplayName = "null")]
        [DataRow("", DisplayName = "empty")]
        [DataRow("\t\n  \r\n", DisplayName = "blank")]
        public void Map_WhenConversationMessagesHasUnreadItemsWithMatchingConversationId_AndItemMessageIdsAreNullOrEmpty_ThenItemsAreIgnored(string value)
        {
            _testMessages.Add(new MessageDetails
            {
                ConversationId = "1",
                MessageId = value,
                Read = YesNo.n
            });

            _systemUnderTest.Map(_testMessages, "1")
                .Should()
                .Equal("001", "002", "003", "004", "005");
        }
    }
}
