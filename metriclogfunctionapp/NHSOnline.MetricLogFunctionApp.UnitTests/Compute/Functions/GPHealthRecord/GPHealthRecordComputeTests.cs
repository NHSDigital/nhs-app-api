using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.GPHealthRecord;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.GPHealthRecord;

[TestClass]
public class GPHealthRecordComputeTests
{
    private Mock<IComputeLogger<GPHealthRecordCompute>> _logger;
    private Mock<IEventsRepository> _repository;

    [TestInitialize]
    public void TestInitialize()
    {
        _logger = new Mock<IComputeLogger<GPHealthRecordCompute>>();
        _repository = new Mock<IEventsRepository>();
    }

    [TestMethod]
    public void Execute_WhenCalled_ShouldPerformStepsInComputeProcess()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.Date;
        var endDateTime = startDateTime.AddDays(1);

        var compute = new GPHealthRecordCompute(_logger.Object, _repository.Object);

        // Act
        var result = compute.Execute(startDateTime, endDateTime);

        //Assert
        _repository.Verify(r => r.CallStoredProcedure("CALL compute.GPRecordViewsComputation({0},{1})",
            startDateTime, endDateTime));
    }
}
