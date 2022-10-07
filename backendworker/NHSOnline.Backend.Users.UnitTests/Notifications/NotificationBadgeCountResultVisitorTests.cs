using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.UnitTests.Notifications
{
    [TestClass]
    public class NotificationBadgeCountResultVisitorTests
    {
        private NotificationBadgeCountResultVisitor _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _systemUnderTest = new NotificationBadgeCountResultVisitor();
        }

        [TestMethod]
        public void Visit_WhenResultIsSuccess_ReturnsResultCount()
        {
            // Arrange
            var result = new UnreadMessageCountResult.Success(5);

            // Act
            var count = _systemUnderTest.Visit(result);

            // Assert
            count.Should().Be(5);
        }

        [TestMethod]
        public void Visit_WhenResultIsFailure_ReturnsMinusOne()
        {
            // Arrange
            var result = new UnreadMessageCountResult.Failure();

            // Act
            var count = _systemUnderTest.Visit(result);

            // Assert
            count.Should().Be(0);
        }
    }
}