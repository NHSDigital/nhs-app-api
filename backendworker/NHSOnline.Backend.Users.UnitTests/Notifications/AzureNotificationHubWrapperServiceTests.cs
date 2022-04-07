using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Users.Notifications;

namespace NHSOnline.Backend.Users.UnitTests.Notifications
{
    [TestClass]
    public class AzureNotificationHubWrapperServiceTests
    {
        private const string NhsLoginId = "NhsLoginId";

        private IAzureNotificationHubWrapperService _systemUnderTest;
        private readonly List<IAzureNotificationHubWrapper> _wrappers = new List<IAzureNotificationHubWrapper>();

        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void AllFor_Empty_ThrowsExceptions()
        {
            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            Assert.ThrowsException<NotificationHubNotFoundException>(() => _systemUnderTest.AllFor(NhsLoginId));
        }

        [TestMethod]
        public void AllFor_SingleWrapper_NotForSpecifiedNhsLoginId_ThrowsException()
        {
            _wrappers.Add(GetMockWrapperForReading(1, false).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            Assert.ThrowsException<NotificationHubNotFoundException>(() => _systemUnderTest.AllFor(NhsLoginId));
        }

        [TestMethod]
        public void AllFor_SingleWrapper_ForSpecifiedNhsLoginId_ReturnsWrapper()
        {
            _wrappers.Add(GetMockWrapperForReading(1, true).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            var result = _systemUnderTest.AllFor(NhsLoginId).ToList();

            result.Should().NotBeEmpty();
            result.Count.Should().Be(1);

            Assert.IsNotNull(result[0]);
            Assert.AreEqual(1, result[0].Generation);
        }

        [TestMethod]
        public void AllFor_MultipleWrappers_NoneForSpecifiedNhsLoginId_ThrowsException()
        {
            _wrappers.Add(GetMockWrapperForReading(1, false).Object);
            _wrappers.Add(GetMockWrapperForReading(2, false).Object);
            _wrappers.Add(GetMockWrapperForReading(3, false).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            Assert.ThrowsException<NotificationHubNotFoundException>(() => _systemUnderTest.AllFor(NhsLoginId));
        }

        [TestMethod]
        public void AllFor_MultipleWrappers_OneForSpecifiedNhsLoginId_ReturnsWrapper()
        {
            _wrappers.Add(GetMockWrapperForReading(1, true).Object);
            _wrappers.Add(GetMockWrapperForReading(2, false).Object);
            _wrappers.Add(GetMockWrapperForReading(3, false).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            var result = _systemUnderTest.AllFor(NhsLoginId).ToList();

            result.Should().NotBeEmpty();
            result.Count.Should().Be(1);

            Assert.IsNotNull(result[0]);
            Assert.AreEqual(1, result[0].Generation);
        }

        [TestMethod]
        public void AllFor_MultipleWrappers_MultipleForSpecifiedNhsLoginId_ReturnsCorrectWrappers()
        {
            _wrappers.Add(GetMockWrapperForReading(1, true).Object);
            _wrappers.Add(GetMockWrapperForReading(2, false).Object);
            _wrappers.Add(GetMockWrapperForReading(3, true).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            var result = _systemUnderTest.AllFor(NhsLoginId).ToList();

            result.Should().NotBeEmpty();
            result.Count.Should().Be(2);

            Assert.IsNotNull(result[0]);
            Assert.AreEqual(3, result[0].Generation);

            Assert.IsNotNull(result[1]);
            Assert.AreEqual(1, result[1].Generation);
        }

        [TestMethod]
        public void CurrentFor_Empty_ThrowsException()
        {
            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            Assert.ThrowsException<NotificationHubNotFoundException>(() => _systemUnderTest.CurrentFor(NhsLoginId));
        }

        [TestMethod]
        public void CurrentFor_SingleWrapper_NotForSpecifiedNhsLoginId_ThrowsException()
        {
            _wrappers.Add(GetMockWrapperForWriting(1, false).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            Assert.ThrowsException<NotificationHubNotFoundException>(() => _systemUnderTest.CurrentFor(NhsLoginId));
        }

        [TestMethod]
        public void CurrentFor_SingleWrapper_ForSpecifiedNhsLoginId_ReturnsWrapper()
        {
            _wrappers.Add(GetMockWrapperForWriting(1, true).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            var result = _systemUnderTest.CurrentFor(NhsLoginId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Generation);
        }

        [TestMethod]
        public void CurrentFor_MultipleWrappers_NoneForSpecifiedNhsLoginId_ThrowsException()
        {
            _wrappers.Add(GetMockWrapperForWriting(1, false).Object);
            _wrappers.Add(GetMockWrapperForWriting(2, false).Object);
            _wrappers.Add(GetMockWrapperForWriting(3, false).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            Assert.ThrowsException<NotificationHubNotFoundException>(() => _systemUnderTest.CurrentFor(NhsLoginId));
        }

        [TestMethod]
        public void CurrentFor_MultipleWrappers_OneForSpecifiedNhsLoginId_ReturnsWrapper()
        {
            _wrappers.Add(GetMockWrapperForWriting(1, true).Object);
            _wrappers.Add(GetMockWrapperForWriting(2, false).Object);
            _wrappers.Add(GetMockWrapperForWriting(3, false).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            var result = _systemUnderTest.CurrentFor(NhsLoginId);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Generation);
        }

        [TestMethod]
        public void CurrentFor_MultipleWrappers_MultipleForSpecifiedNhsLoginId_ThrowsException()
        {
            _wrappers.Add(GetMockWrapperForWriting(1, true).Object);
            _wrappers.Add(GetMockWrapperForWriting(2, false).Object);
            _wrappers.Add(GetMockWrapperForWriting(3, true).Object);

            _systemUnderTest = new AzureNotificationHubWrapperService(_wrappers);

            Assert.ThrowsException<NotificationHubNotFoundException>(() => _systemUnderTest.CurrentFor(NhsLoginId));
        }

        private static Mock<IAzureNotificationHubWrapper> GetMockWrapperForReading(int generation, bool handlesNhsLoginId)
        {
            var wrapper = new Mock<IAzureNotificationHubWrapper>(MockBehavior.Strict);

            wrapper
                .Setup(x => x.Generation)
                .Returns(generation);

            wrapper
                .Setup(x => x.Path)
                .Returns(generation.ToString);

            wrapper
                .Setup(x => x.CanReadFor(NhsLoginId))
                .Returns(handlesNhsLoginId);

            return wrapper;
        }

        private static Mock<IAzureNotificationHubWrapper> GetMockWrapperForWriting(int generation, bool handlesNhsLoginId)
        {
            var wrapper = new Mock<IAzureNotificationHubWrapper>(MockBehavior.Strict);

            wrapper
                .Setup(x => x.Generation)
                .Returns(generation);

            wrapper
                .Setup(x => x.Path)
                .Returns(generation.ToString);

            wrapper
                .Setup(x => x.CanWriteFor(NhsLoginId))
                .Returns(handlesNhsLoginId);

            return wrapper;
        }
    }
}
