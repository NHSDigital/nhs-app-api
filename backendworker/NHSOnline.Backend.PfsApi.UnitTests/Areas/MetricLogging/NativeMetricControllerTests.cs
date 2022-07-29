using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.MetricLogging;
using NHSOnline.Backend.PfsApi.Areas.MetricLogging.Models;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.MetricLogging
{
    [TestClass]
    public sealed class NativeMetricControllerTests : IDisposable
    {
        private NativeMetricsController _systemUnderTest;
        private Mock<IAuditor> _mockAuditor;

        private const string RequestAuditType = "BiometricsRegistration_Decision";

        [TestInitialize]
        public void TestInitialize()
        {
            _mockAuditor = new Mock<IAuditor>();

            _systemUnderTest = new NativeMetricsController(
                new Mock<ILogger<NativeMetricsController>>().Object,
                _mockAuditor.Object);
        }

        [TestMethod]
        public async Task Post_BiometricsOptInMetrics_AuditsOptInDecision()
        {
            // Act
            await _systemUnderTest.PostBiometricsOptInMetrics();

            // Assert
            _mockAuditor.Verify(x => x.PostOperationAudit(RequestAuditType, "Biometrics toggled. optIn=True"));
        }

        [TestMethod]
        public async Task Post_BiometricsOptOutMetrics_AuditsOptOutDecision()
        {
            // Act
            await _systemUnderTest.PostBiometricsOptOutMetrics();

            // Assert
            _mockAuditor.Verify(x => x.PostOperationAudit(RequestAuditType, "Biometrics toggled. optIn=False"));
        }

        [TestMethod]
        public async Task Post_PostOperationAudit_ReturnsStatus200Ok()
        {
            // Arrange
            var operationAuditData = new OperationAuditData(
                "PatientRecord_Section_View_Response",
                "Patient record TEST RESULTS successfully retrieved.");

            // Act
            var result = await _systemUnderTest.PostOperationAudit(operationAuditData);

            // Assert
            result.Should().BeOfType<StatusCodeResult>().Subject.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [TestMethod]
        public async Task Post_PostOperationAudit_CallsLogAudit()
        {
            // Arrange
            var operation = "PatientRecord_Section_View_Response";
            var details = "Patient record TEST RESULTS successfully retrieved.";

            var operationAuditData = new OperationAuditData(
                operation,
                details);

            // Act
            var result = await _systemUnderTest.PostOperationAudit(operationAuditData);

            // Assert
            _mockAuditor.Verify(x => x.PostOperationAudit(operation, details));
        }

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();

    }
}
