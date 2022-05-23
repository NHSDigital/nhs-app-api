using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.ReferrerLogin;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.ReferrerLogin;

[TestClass]
public class ReferrerLoginComputeTests
{
    private Mock<IComputeLogger<ReferrerLoginCompute>> _logger;
    private Mock<IEventsRepository> _repository;

    [TestInitialize]
    public void TestInitialize()
    {
        _logger = new Mock<IComputeLogger<ReferrerLoginCompute>>();
        _repository = new Mock<IEventsRepository>();
    }

    [TestMethod]
    public void Execute_WhenCalled_ShouldPerformStepsInComputeProcess()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.Date;
        var endDateTime = startDateTime.AddDays(1);

        var firstLoginsCompute = new ReferrerLoginCompute(_logger.Object, _repository.Object);

        // Act
        var result = firstLoginsCompute.Execute(startDateTime, endDateTime);

        //Assert
        _repository.Verify(r => r.CallStoredProcedure("CALL compute.ReferrerLoginComputation({0},{1})",
            startDateTime, endDateTime));
    }

}