using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Book;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Cancel;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.Toggle;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.InitialPrompt;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Create;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog
{
    [TestClass]
    public class AuditLogConsumerFunctionTests
    {
        private Mock<IAuditLogEtl<ConsentMetric>> _consentEtl;
        private Mock<IAuditLogEtl<LoginMetric>> _loginEtl;
        private Mock<IAuditLogEtl<WebIntegrationReferralsMetric>> _webIntegrationReferralEtl;
        private Mock<IAuditLogEtl<SecondaryCareSummaryMetric>> _secondaryCareSummaryEtl;
        private Mock<IAuditLogEtl<MedicalRecordViewMetric>> _medicalRecordView;
        private Mock<IAuditLogEtl<NotificationToggleMetric>> _notificationToggleEtl;
        private Mock<IAuditLogEtl<InitialPromptMetric>> _initialPromptEtl;
        private Mock<IAuditLogEtl<AppointmentCancelMetric>> _appointmentCancelEtl;
        private Mock<IAuditLogEtl<AppointmentBookMetric>> _appointmentBookEtl;
        private Mock<IAuditLogEtl<OrganDonationRegistrationCreateMetric>> _organDonationRegistrationCreateEtl;
        private Mock<IEtlLogger<AuditLogConsumerFunction>> _logger;
        private Mock<ILogger<AuditLogConsumerFunction>> _queueLogger;
        private AuditLogConsumerFunction _function;

        [TestInitialize]
        public void TestInitialize()
        {
            _consentEtl = new Mock<IAuditLogEtl<ConsentMetric>>();
            _loginEtl = new Mock<IAuditLogEtl<LoginMetric>>();
            _webIntegrationReferralEtl = new Mock<IAuditLogEtl<WebIntegrationReferralsMetric>>();
            _secondaryCareSummaryEtl = new Mock<IAuditLogEtl<SecondaryCareSummaryMetric>>();
            _medicalRecordView = new Mock<IAuditLogEtl<MedicalRecordViewMetric>>();
            _notificationToggleEtl = new Mock<IAuditLogEtl<NotificationToggleMetric>>();
            _appointmentCancelEtl = new Mock<IAuditLogEtl<AppointmentCancelMetric>>();
            _appointmentBookEtl = new Mock<IAuditLogEtl<AppointmentBookMetric>>();
            _organDonationRegistrationCreateEtl = new Mock<IAuditLogEtl<OrganDonationRegistrationCreateMetric>>();
            _logger = new Mock<IEtlLogger<AuditLogConsumerFunction>>();
            _queueLogger = new Mock<ILogger<AuditLogConsumerFunction>>();
            _initialPromptEtl = new Mock<IAuditLogEtl<InitialPromptMetric>>();

            _function = new AuditLogConsumerFunction(
                _consentEtl.Object,
                _loginEtl.Object,
                _webIntegrationReferralEtl.Object,
                _secondaryCareSummaryEtl.Object,
                _medicalRecordView.Object,
                _notificationToggleEtl.Object,
                _initialPromptEtl.Object,
                _appointmentCancelEtl.Object,
                _appointmentBookEtl.Object,
                _organDonationRegistrationCreateEtl.Object,
                _logger.Object,
                _queueLogger.Object);
        }

        [TestMethod]
        public async Task AuditLogEventHubTrigger_ShouldProcessEvents()
        {
            // Arrange
            var executionContext = new ExecutionContext();
            var eventData = new[] { new AuditRecord { Operation = "This is a Test Message" } };

            // Act
            await _function.AuditLog_Etl_EventHub(eventData, executionContext);

            // Assert
            _consentEtl.Verify(etl =>
                etl.ExecuteDependentEvent(It.IsAny<ILogger<AuditLogConsumerFunction>>(),It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _loginEtl.Verify(etl =>
                etl.ExecuteDependentEvent(It.IsAny<ILogger<AuditLogConsumerFunction>>(),It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _webIntegrationReferralEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _secondaryCareSummaryEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _medicalRecordView.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _notificationToggleEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _initialPromptEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _appointmentCancelEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _appointmentBookEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _organDonationRegistrationCreateEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
        }

        [TestMethod]
        public void AuditLogEventHubTrigger_WhenExceptionEncountered_ShouldRethrow()
        {
            // Arrange
            var executionContext = new ExecutionContext();
            var retryContext = new RetryContext
            {
                RetryCount = 5,
                MaxRetryCount = 5
            };
            executionContext.RetryContext = retryContext;
            var eventData = new[] { new AuditRecord { Operation = "This is a Test Message" } };

            _consentEtl.Setup(etl =>
                    etl.Execute(It.IsAny<IList<AuditRecord>>()))
                .ThrowsAsync(new ArgumentException("This is a test exception"));

            // Act & Assert
            Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                _function.AuditLog_Etl_EventHub(eventData, executionContext));
        }
    }
}
