using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Metrics.UnitTests
{
    [TestClass]
    public sealed class MetricLoggerTests
    {
        [TestMethod]
        [DynamicData(nameof(DefaultMetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsTimestampFirst(Func<IMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            MetricLoggerAssert.AssertTimeStamp(consoleOut.ToString());
        }

        [TestMethod]
        [DynamicData(nameof(DefaultMetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsNhsLoginIdFromContext(Func<IMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            mockMetricContext.Setup(x => x.NhsLoginId).Returns("123-456-789");
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            MetricLoggerAssert.AssertContains(consoleOut.ToString(),"NhsLoginId=123-456-789");
        }

        [TestMethod]
        [DynamicData(nameof(DefaultMetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsOdsCodeFromContext(Func<IMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            mockMetricContext.Setup(x => x.OdsCode).Returns("A1234B");
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            MetricLoggerAssert.AssertContains(consoleOut.ToString(),"OdsCode=A1234B");
        }

        [TestMethod]
        [DynamicData(nameof(DefaultMetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsProofLevelFromContext(Func<IMetricLogger, Task> logMethod, string _)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            mockMetricContext.Setup(x => x.ProofLevel).Returns(ProofLevel.P9);
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            MetricLoggerAssert.AssertContains(consoleOut.ToString(),"ProofLevel=P9");
        }

        [TestMethod]
        [DynamicData(nameof(DefaultMetricLogMethods), typeof(MetricLoggerTests), DynamicDataDisplayName = nameof(MetricLogMethodsDisplayName))]
        public async Task MetricLog_LogsAction(Func<IMetricLogger, Task> logMethod, string action)
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await logMethod(metricLogger);

            // Assert
            MetricLoggerAssert.AssertContains(consoleOut.ToString(),$"Action={action}");
        }

        [TestMethod]
        public async Task MessageRead_LogsMessageReadData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var messageReadData = new MessageReadData("messageId_1234", "communicationId_5678", "transmissionId_4567");
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.MessageRead(messageReadData);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("MessageId=messageId_1234");
            splitConsoleMessage.Should().Contain("CommunicationId=communicationId_5678");
            splitConsoleMessage.Should().Contain("TransmissionId=transmissionId_4567");
        }

        [TestMethod]
        public async Task SilverIntegrationJumpOff_LogsSilverIntegrationAndVersionData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var silverIntegrationData = new SilverIntegrationData("sessionId", "providerId", "providerName", "jumpOffId");
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.SilverIntegrationJumpOff(silverIntegrationData);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("SessionId=sessionId");
            splitConsoleMessage.Should().Contain("ProviderId=providerId");
            splitConsoleMessage.Should().Contain("ProviderName=providerName");
            splitConsoleMessage.Should().Contain("JumpOffId=jumpOffId");
        }

        [TestMethod]
        public async Task Login_LogsLoginData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var data = new LoginData("requestId_1234", "sessionId_1234", "userAgent_1234");
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.Login(data);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("RequestId=requestId_1234");
            splitConsoleMessage.Should().Contain("SessionId=sessionId_1234");
            splitConsoleMessage.Should().Contain("UserAgent=userAgent_1234");
        }

        [TestMethod]
        public async Task OrganDonationGetRegistration_LogsOrganDonationData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var data = new OrganDonationData("sessionId_1234");
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.OrganDonationGetRegistration(data);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("SessionId=sessionId_1234");
        }

        [TestMethod]
        public async Task OrganDonationCreateRegistration_LogsOrganDonationData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var data = new OrganDonationData("sessionId_1234");
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.OrganDonationCreateRegistration(data);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("SessionId=sessionId_1234");
        }

        [TestMethod]
        public async Task OrganDonationUpdateRegistration_LogsOrganDonationData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var data = new OrganDonationData("sessionId_1234");
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.OrganDonationUpdateRegistration(data);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("SessionId=sessionId_1234");
        }

        [TestMethod]
        public async Task OrganDonationWithdrawRegistration_LogsOrganDonationData()
        {
            // Arrange
            var mockMetricContext = new Mock<IMetricContext>();
            var metricLogger = CreateMetricLogger(mockMetricContext);
            var data = new OrganDonationData("sessionId_1234");
            using var consoleOut = new CaptureConsoleOut();

            // Act
            await metricLogger.OrganDonationWithdrawRegistration(data);

            // Assert
            var splitConsoleMessage = MetricLoggerAssert.AssertSingleLine(consoleOut.ToString()).Split(" ");
            splitConsoleMessage.Should().Contain("SessionId=sessionId_1234");
        }

        private static IMetricLogger CreateMetricLogger(Mock<IMetricContext> mockMetricContext)
        {
            var services = new ServiceCollection();
            new ServiceConfigurationModule().ConfigureServices(services, new Mock<IConfiguration>().Object);
            services.AddSingleton(mockMetricContext.Object);

            var metricLogger = services.BuildServiceProvider().GetRequiredService<IMetricLogger>();
            return metricLogger;
        }

        private static IEnumerable<object[]> DefaultMetricLogMethods
        {
            get
            {
                var loginData = new LoginData("requestId", "sessionId", "userAgent");
                yield return new object[] { Method(metricLogger => metricLogger.Login(loginData)), "Login" };
                yield return new object[] { Method(metricLogger => metricLogger.UpliftStarted()), "UpliftStarted" };
                yield return new object[] { Method(metricLogger => metricLogger.UserResearchOptIn()), "UserResearchOptIn" };
                yield return new object[] { Method(metricLogger => metricLogger.UserResearchOptOut()), "UserResearchOptOut" };
                yield return new object[] { Method(metricLogger => metricLogger.TermsAndConditionsInitialConsent()), "TermsAndConditionsInitialConsent" };

                var messageReadData = new MessageReadData("messageId", "communicationId", "transmissionId");
                yield return new object[] { Method(metricLogger => metricLogger.MessageRead(messageReadData)), "MessageRead" };

                yield return new object[] { Method(metricLogger => metricLogger.NotificationsEnabled()), "NotificationsEnabled" };
                yield return new object[] { Method(metricLogger => metricLogger.NotificationsDisabled()), "NotificationsDisabled" };

                var silverIntegrationData = new SilverIntegrationData("sessionId", "providerId", "providerName", "jumpOffId");
                yield return new object[] { Method(metricLogger => metricLogger.SilverIntegrationJumpOff(silverIntegrationData)), "SilverIntegrationJumpOff" };

                var organDonationData = new OrganDonationData("sessionId");
                yield return new object[] { Method(metricLogger => metricLogger.OrganDonationGetRegistration(organDonationData)), "OrganDonationGetRegistration" };
                yield return new object[] { Method(metricLogger => metricLogger.OrganDonationCreateRegistration(organDonationData)), "OrganDonationCreateRegistration" };
                yield return new object[] { Method(metricLogger => metricLogger.OrganDonationUpdateRegistration(organDonationData)), "OrganDonationUpdateRegistration" };
                yield return new object[] { Method(metricLogger => metricLogger.OrganDonationWithdrawRegistration(organDonationData)), "OrganDonationWithdrawRegistration" };

                static Func<IMetricLogger, Task> Method(Func<IMetricLogger, Task> method) => method;
            }
        }

        public static string MetricLogMethodsDisplayName(MethodInfo methodInfo, object[] data) => $"{methodInfo.Name}({data[1]})";
    }
}
