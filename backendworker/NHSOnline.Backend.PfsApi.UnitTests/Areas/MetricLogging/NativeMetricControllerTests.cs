using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Areas.MetricLogging;

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

        [TestCleanup]
        public void Dispose() => _systemUnderTest?.Dispose();

    }
}
