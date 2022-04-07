using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.FirstLogins;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.FirstLogins;

[TestClass]
public class FirstLoginsComputeTests
{
    private Mock<IComputeLogger<FirstLoginsCompute>> _logger;
    private Mock<IEventsRepository> _repository;

    [TestInitialize]
    public void TestInitialize()
    {
        _logger = new Mock<IComputeLogger<FirstLoginsCompute>>();
        _repository = new Mock<IEventsRepository>();
    }

    [TestMethod]
    public void Execute_WhenCalled_ShouldPerformStepsInComputeProcess()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.Date;
        var endDateTime = startDateTime.AddDays(1);
        var loginId = "loginId";

        var firstLoginsCompute = new FirstLoginsCompute(_logger.Object, _repository.Object);

        // Act
        var result = firstLoginsCompute.Execute(loginId, startDateTime,endDateTime);

        //Assert
        _repository.Verify(r => r.CallStoredProcedure("CALL compute.FirstLoginsComputation({0},{1},{2})",
            loginId, startDateTime, endDateTime));
    }

}