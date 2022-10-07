using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Messages.Areas.Messages;
using NHSOnline.Backend.Messages.Areas.Messages.Models;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.UnitTests.Areas.Messages
{
    [TestClass]
    public class RepositoryUnreadMessageCountResultVisitorTests
    {
        private Mock<IUnreadMessageCountResultVisitor<long>> _mockUnreadMessageCountResultVisitor;

        private RepositoryUnreadMessageCountResultVisitor _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockUnreadMessageCountResultVisitor = new Mock<IUnreadMessageCountResultVisitor<long>>(MockBehavior.Strict);

            _systemUnderTest = new RepositoryUnreadMessageCountResultVisitor();
        }

        [TestMethod]
        public void Visit_WhenResultIsFound_ReturnsSuccessResultWithUnreadCount()
        {
            // Arrange
            const int unreadMessageCount = 15;

            var result = new RepositoryCountResult.Found(unreadMessageCount);

            // Act
            var count = _systemUnderTest.Visit(result);

            // Assert
            count.Should().BeAssignableTo<UnreadMessageCountResult.Success>()
                .Subject.UnreadMessageCount.Should().Be(unreadMessageCount);
        }

        [TestMethod]
        public void Visit_WhenResultIsAnError_ReturnsFailureResultWithMinusOneUnreadCount()
        {
            // Arrange
            var result = new RepositoryCountResult.RepositoryError();

            // Act
            var count = _systemUnderTest.Visit(result);

            // Assert
            count.Should().BeAssignableTo<UnreadMessageCountResult.Failure>();
        }
    }
}