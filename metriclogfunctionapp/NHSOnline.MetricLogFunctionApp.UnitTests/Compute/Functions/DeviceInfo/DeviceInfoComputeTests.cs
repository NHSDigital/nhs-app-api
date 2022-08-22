using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.DeviceInfo;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.DeviceInfo;

[TestClass]
public class DeviceInfoComputeTests
{
    private Mock<IComputeLogger<DeviceInfoCompute>> _logger;
    private Mock<IEventsRepository> _repository;

    [TestInitialize]
    public void TestInitialize()
    {
        _logger = new Mock<IComputeLogger<DeviceInfoCompute>>();
        _repository = new Mock<IEventsRepository>();
    }

    [TestMethod]
    public void Execute_WhenCalled_ShouldPerformStepsInComputeProcess()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.Date;
        var endDateTime = startDateTime.AddDays(1);

        var deviceInfoCompute = new DeviceInfoCompute(_logger.Object, _repository.Object);

        // Act
        var result = deviceInfoCompute.Execute(startDateTime, endDateTime);

        //Assert
        _repository.Verify(r => r.CallStoredProcedure("CALL compute.DeviceInfoCompute({0},{1})",
            startDateTime, endDateTime));
    }
}
