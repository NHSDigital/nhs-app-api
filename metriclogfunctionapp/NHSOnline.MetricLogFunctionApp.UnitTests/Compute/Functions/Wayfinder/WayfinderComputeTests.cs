using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.Wayfinder;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.Wayfinder;

[TestClass]
public class WayfinderComputeTests
{
    private Mock<IComputeLogger<WayfinderCompute>> _logger;
    private Mock<IEventsRepository> _repository;

    [TestInitialize]
    public void TestInitialize()
    {
        _logger = new Mock<IComputeLogger<WayfinderCompute>>();
        _repository = new Mock<IEventsRepository>();
    }

    [TestMethod]
    public void Execute_WhenCalled_ShouldPerformStepsInComputeProcess()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.Date;
        var endDateTime = startDateTime.AddDays(1);

        var compute = new WayfinderCompute(_logger.Object, _repository.Object);

        // Act
        var result = compute.Execute(startDateTime, endDateTime);

        //Assert
        _repository.Verify(r => r.CallStoredProcedure("CALL compute.WayfinderComputation({0},{1})",
            startDateTime, endDateTime));
    }
}
