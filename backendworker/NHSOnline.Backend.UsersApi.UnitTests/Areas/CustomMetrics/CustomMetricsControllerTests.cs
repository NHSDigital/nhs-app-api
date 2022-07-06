using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auth.AspNet;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Users.Areas.Devices.Models;
using NHSOnline.Backend.Users.Registrations;
using NHSOnline.Backend.UsersApi.Areas.CustomMetrics;
using NHSOnline.Backend.Auth.CitizenId.Models;
using UnitTestHelper;

namespace NHSOnline.Backend.UsersApi.UnitTests.Areas.CustomMetrics
{
    [TestClass]
    public sealed class CustomMetricsControllerTests :IDisposable
    {
        private CustomMetricsController _systemUnderTest;
        private Mock<IMetricLogger> _mockMetricLogger;
        private Mock<ILogger<CustomMetricsController>> _mockLogger;
        private Mock<INotificationsDecisionAuditService> _mockNotificationsDecisionAuditService;
        private const string NhsNumber = "NhsNumber";

        [TestInitialize]
        public void Setup()
        {
            _mockMetricLogger = new Mock<IMetricLogger>();
            _mockLogger = new Mock<ILogger<CustomMetricsController>>();
            _mockNotificationsDecisionAuditService = new Mock<INotificationsDecisionAuditService>();

            var mockAccessTokenProvider = new Mock<IAccessTokenProvider>();
            mockAccessTokenProvider.SetupGet(x => x.AccessToken)
                .Returns(AccessTokenMock.Generate(nhsNumber: NhsNumber));

            _systemUnderTest = new CustomMetricsController(_mockMetricLogger.Object,
                _mockLogger.Object,
                _mockNotificationsDecisionAuditService.Object,
                mockAccessTokenProvider.Object);
        }

        [TestMethod]
        [DataRow("","providerName","jumpOffId","reason", DisplayName = "Blank ProviderId")]
        [DataRow(null,"providerName","jumpOffId","reason", DisplayName = "Null ProviderId")]
        [DataRow("providerId","","jumpOffId","reason", DisplayName = "Blank ProviderName")]
        [DataRow("providerId",null,"jumpOffId","reason", DisplayName = "Null ProviderName")]
        [DataRow("providerId","providerName","","reason", DisplayName = "Blank JumpOffId")]
        [DataRow("providerId","providerName",null,"reason", DisplayName = "Null JumpOffId")]
        [DataRow("providerId","providerName","jumpOffId","", DisplayName = "Blank Reason")]
        [DataRow("providerId","providerName","jumpOffId",null, DisplayName = "Null Reason")]
        public async Task Post_SilverIntegrationJumpOff_Metric_BadRequest_ReturnsStatus400BadRequest(
            string providerId,string providerName,string jumpOffId,string reason)
        {
            // Arrange
            var silverIntegrationJumpOffBlockedData =
                new SilverIntegrationJumpOffBlockedData(providerId, providerName, jumpOffId, reason);

            // Act
            var result= await _systemUnderTest
                .PostSilverIntegrationJumpOffBlockedMetrics(silverIntegrationJumpOffBlockedData);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_SilverIntegrationJumpOff_Metric_BadRequest_RequestBody_Is_Empty_ReturnsStatus400BadRequest()
        {
            // Act
            var result= await _systemUnderTest.PostSilverIntegrationJumpOffBlockedMetrics(null);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_SilverIntegrationJumpOff_Metric_Success_ReturnsStatus20O_Ok()
        {
            // Arrange
            _mockMetricLogger
                .Setup(s => s.SilverIntegrationJumpOffBlocked(It.IsAny<SilverIntegrationJumpOffBlockedData>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _systemUnderTest.PostSilverIntegrationJumpOffBlockedMetrics(
                new SilverIntegrationJumpOffBlockedData("providerId", "providerName", "jumpOffId", "reason"));

            // Assert
            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task Post_Metric_Success_ReturnsStatus200Ok()
        {
            // Arrange
            var notificationPromptData = new NotificationsPromptData(
                true,
                true,
                "iOS",
                false);

            // Act
            var result = await _systemUnderTest.PostNotificationsPromptMetrics(notificationPromptData);

            // Assert
            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task Post_Metric_BadRequest_ReturnsStatus400BadRequest()
        {
            // Arrange
            var notificationPromptData = new NotificationsPromptData(
                true,
                true,
                "",
                false);

            // Act
            var result = await _systemUnderTest.PostNotificationsPromptMetrics(notificationPromptData);

            // Assert
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [TestMethod]
        public async Task Post_Notifications_LogAudit_ReturnsStatus200Ok()
        {
            // Arrange
            var notificationsAuditData = new NotificationsAuditData(
                true,
                NotificationsDecisionSource.Prompt);

            // Act
            var result = await _systemUnderTest.PostNotificationsLogAudit(notificationsAuditData);

            // Assert
            result.Should().BeOfType<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task Post_Notifications_LogAudit_CallsLogAudit()
        {
            // Arrange
            var notificationsAuditData = new NotificationsAuditData(
                true,
                NotificationsDecisionSource.Prompt);

            _mockNotificationsDecisionAuditService
                .Setup(x => x.LogAudit(It.IsAny<NotificationsAuditData>(), It.IsAny<AccessToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _systemUnderTest.PostNotificationsLogAudit(notificationsAuditData);

            // Assert
            _mockNotificationsDecisionAuditService.Verify(x => x.LogAudit(It.IsAny<NotificationsAuditData>(), It.IsAny<AccessToken>()), Times.Once);
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}