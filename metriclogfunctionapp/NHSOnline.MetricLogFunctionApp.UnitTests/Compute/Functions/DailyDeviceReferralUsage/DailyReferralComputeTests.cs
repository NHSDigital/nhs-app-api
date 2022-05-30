using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.DailyDeviceReferralUsage;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.DailyDeviceReferralUsage
{
    [TestClass]
    public class DailyReferralComputeTests
    {
        private Mock<IComputeLogger<DailyDeviceReferralUsageCompute>> _logger;
        private Mock<IEventsRepository> _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<IComputeLogger<DailyDeviceReferralUsageCompute>>();
            _repository = new Mock<IEventsRepository>();
        }

        [TestMethod]
        public void Execute_WhenCalled_ShouldPerformStepsInComputeProcess()
        {
            // Arrange
            var startDateTime = DateTime.UtcNow.Date;
            var endDateTime = startDateTime.AddDays(1);

            var firstLoginsCompute = new DailyDeviceReferralUsageCompute(_logger.Object, _repository.Object);

            // Act
            var result = firstLoginsCompute.Execute(startDateTime, endDateTime);

            //Assert
            _repository.Verify(r => r.CallStoredProcedure("CALL compute.devicereferralcompute({0},{1})",
                startDateTime, endDateTime));
        }

    }
}