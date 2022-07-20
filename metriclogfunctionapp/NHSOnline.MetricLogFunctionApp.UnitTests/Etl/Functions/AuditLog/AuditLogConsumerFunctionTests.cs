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
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.BiometricsToggle;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.SectionView;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Create;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Update;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.Toggle;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.InitialPrompt;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Get;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Create;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Withdraw;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Update;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RepeatPrescription;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog
{
    [TestClass]
    public class AuditLogConsumerFunctionTests
    {
        private Mock<IAuditLogEtl<AppointmentCancelMetric>> _appointmentCancelEtl;
        private Mock<IAuditLogEtl<AppointmentBookMetric>> _appointmentBookEtl;
        private Mock<IAuditLogEtl<BiometricsToggleMetric>> _biometricsToggleEtl;
        private Mock<IAuditLogEtl<ConsentMetric>> _consentEtl;
        private Mock<IAuditLogEtl<LoginMetric>> _loginEtl;
        private Mock<IAuditLogEtl<MedicalRecordViewMetric>> _medicalRecordView;
        private Mock<IAuditLogEtl<NominatedPharmacyCreateMetric>> _nominatedPharmacyCreateEtl;
        private Mock<IAuditLogEtl<NominatedPharmacyUpdateMetric>> _nominatedPharmacyUpdateEtl;
        private Mock<IAuditLogEtl<NotificationToggleMetric>> _notificationToggleEtl;
        private Mock<IAuditLogEtl<InitialPromptMetric>> _initialPromptEtl;
        private Mock<IAuditLogEtl<OrganDonationRegistrationGetMetric>> _organDonationRegistrationGetEtl;
        private Mock<IAuditLogEtl<OrganDonationRegistrationCreateMetric>> _organDonationRegistrationCreateEtl;
        private Mock<IAuditLogEtl<OrganDonationRegistrationWithdrawMetric>> _organDonationRegistrationWithdrawEtl;
        private Mock<IAuditLogEtl<OrganDonationRegistrationUpdateMetric>> _organDonationRegistrationUpdateEtl;
        private Mock<IAuditLogEtl<RepeatPrescriptionMetric>> _repeatPrescriptionEtl;
        private Mock<IAuditLogEtl<SecondaryCareSummaryMetric>> _secondaryCareSummaryEtl;
        private Mock<IAuditLogEtl<WebIntegrationReferralsMetric>> _webIntegrationReferralEtl;
        private Mock<IAuditLogEtl<MedicalRecordSectionViewMetric>> _medicalRecordSectionViewMetricEtl;
        private Mock<IEtlLogger<AuditLogConsumerFunction>> _logger;
        private Mock<ILogger<AuditLogConsumerFunction>> _queueLogger;
        private AuditLogConsumerFunction _function;

        [TestInitialize]
        public void TestInitialize()
        {
            _appointmentCancelEtl = new Mock<IAuditLogEtl<AppointmentCancelMetric>>();
            _appointmentBookEtl = new Mock<IAuditLogEtl<AppointmentBookMetric>>();
            _biometricsToggleEtl = new Mock<IAuditLogEtl<BiometricsToggleMetric>>();
            _consentEtl = new Mock<IAuditLogEtl<ConsentMetric>>();
            _initialPromptEtl = new Mock<IAuditLogEtl<InitialPromptMetric>>();
            _loginEtl = new Mock<IAuditLogEtl<LoginMetric>>();
            _medicalRecordView = new Mock<IAuditLogEtl<MedicalRecordViewMetric>>();
            _nominatedPharmacyCreateEtl = new Mock<IAuditLogEtl<NominatedPharmacyCreateMetric>>();
            _nominatedPharmacyUpdateEtl = new Mock<IAuditLogEtl<NominatedPharmacyUpdateMetric>>();
            _notificationToggleEtl = new Mock<IAuditLogEtl<NotificationToggleMetric>>();
            _organDonationRegistrationGetEtl = new Mock<IAuditLogEtl<OrganDonationRegistrationGetMetric>>();
            _organDonationRegistrationCreateEtl = new Mock<IAuditLogEtl<OrganDonationRegistrationCreateMetric>>();
            _organDonationRegistrationWithdrawEtl = new Mock<IAuditLogEtl<OrganDonationRegistrationWithdrawMetric>>();
            _organDonationRegistrationUpdateEtl = new Mock<IAuditLogEtl<OrganDonationRegistrationUpdateMetric>>();
            _repeatPrescriptionEtl = new Mock<IAuditLogEtl<RepeatPrescriptionMetric>>();
            _secondaryCareSummaryEtl = new Mock<IAuditLogEtl<SecondaryCareSummaryMetric>>();
            _webIntegrationReferralEtl = new Mock<IAuditLogEtl<WebIntegrationReferralsMetric>>();
            _medicalRecordSectionViewMetricEtl = new Mock<IAuditLogEtl<MedicalRecordSectionViewMetric>>();
            _logger = new Mock<IEtlLogger<AuditLogConsumerFunction>>();
            _queueLogger = new Mock<ILogger<AuditLogConsumerFunction>>();

            _function = new AuditLogConsumerFunction(
                _appointmentCancelEtl.Object,
                _appointmentBookEtl.Object,
                _biometricsToggleEtl.Object,
                _consentEtl.Object,
                _initialPromptEtl.Object,
                _loginEtl.Object,
                _medicalRecordView.Object,
                _nominatedPharmacyCreateEtl.Object,
                _nominatedPharmacyUpdateEtl.Object,
                _notificationToggleEtl.Object,
                _organDonationRegistrationGetEtl.Object,
                _organDonationRegistrationCreateEtl.Object,
                _organDonationRegistrationWithdrawEtl.Object,
                _organDonationRegistrationUpdateEtl.Object,
                _repeatPrescriptionEtl.Object,
                _secondaryCareSummaryEtl.Object,
                _webIntegrationReferralEtl.Object,
                _medicalRecordSectionViewMetricEtl.Object,
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
            _organDonationRegistrationGetEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _organDonationRegistrationCreateEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _organDonationRegistrationWithdrawEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _repeatPrescriptionEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _organDonationRegistrationUpdateEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _biometricsToggleEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _medicalRecordSectionViewMetricEtl.Verify(etl =>
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
