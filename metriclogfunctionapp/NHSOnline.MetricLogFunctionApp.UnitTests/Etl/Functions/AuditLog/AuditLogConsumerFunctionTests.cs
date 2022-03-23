using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog
{
    [TestClass]
    public class AuditLogConsumerFunctionTests
    {
        private Mock<IAuditLogEtl<ConsentMetric>> _consentEtl;
        private Mock<IAuditLogEtl<LoginMetric>> _loginEtl;
        private Mock<IAuditLogEtl<WebIntegrationReferralsMetric>> _webIntegrationReferralEtl;
        private Mock<IEtlLogger<AuditLogConsumerFunction>> _logger;
        private AuditLogConsumerFunction _function;

        [TestInitialize]
        public void TestInitialize()
        {
            _consentEtl = new Mock<IAuditLogEtl<ConsentMetric>>();
            _loginEtl = new Mock<IAuditLogEtl<LoginMetric>>();
            _webIntegrationReferralEtl = new Mock<IAuditLogEtl<WebIntegrationReferralsMetric>>();
            _logger = new Mock<IEtlLogger<AuditLogConsumerFunction>>();

            _function = new AuditLogConsumerFunction(
                _consentEtl.Object,
                _loginEtl.Object,
                _webIntegrationReferralEtl.Object,
                _logger.Object);
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
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _loginEtl.Verify(etl =>
                etl.Execute(It.Is<IList<AuditRecord>>(e => e[0].Operation == "This is a Test Message")));
            _webIntegrationReferralEtl.Verify(etl =>
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

