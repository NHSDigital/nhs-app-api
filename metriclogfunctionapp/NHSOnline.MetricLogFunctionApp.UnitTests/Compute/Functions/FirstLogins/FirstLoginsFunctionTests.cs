using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute;
using NHSOnline.MetricLogFunctionApp.Compute.FirstLogins;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.FirstLogins;

[TestClass]
public class FirstLoginsFunctionTests
{

    private Mock<ILogger<FirstLoginsFunction>> _logger;
    private Mock<IFirstLoginsCompute> _compute;
    private Mock<IComputeExecutor<AuditReportRequest>> _executor;
    private Mock<IRequestQueueOrchestrator<AuditReportRequest>> _queueOrchestrator;

    private FirstLoginsFunction _function;
    private CloudQueue _testQueue;
    private CloudQueue _testPoisonQueue;

    [TestInitialize]
    public void TestInitialize()
    {
        _logger = new Mock<ILogger<FirstLoginsFunction>>();
        _compute = new Mock<IFirstLoginsCompute>();
        _executor = new Mock<IComputeExecutor<AuditReportRequest>>();
        _queueOrchestrator = new Mock<IRequestQueueOrchestrator<AuditReportRequest>>();

        _testQueue = new CloudQueue(new Uri("http://testUri"));
        _testPoisonQueue = new CloudQueue(new Uri("http://testUri2"));

        _function = new FirstLoginsFunction(_logger.Object, _compute.Object, _executor.Object, _queueOrchestrator.Object);
    }

    [TestMethod]
    public async Task Queue_WhenCalled_ShouldProcessRequest()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.Date;
        var endDateTime = startDateTime.AddDays(1);
        var loginId = "loginID";

        Func<AuditReportRequest, Task> requestFunction = null;
        _executor.Setup(e =>
                e.Execute(
                    It.IsAny<ILogger>(),
                    It.IsAny<CloudQueueMessage>(),
                    It.IsAny<CloudQueue>(),
                    It.IsAny<CloudQueue>(),
                    It.IsAny<Func<AuditReportRequest, Task>>()))
            .Callback<ILogger, CloudQueueMessage, CloudQueue, CloudQueue, Func<AuditReportRequest, Task>>(
                (l, m, q, qp, func) => requestFunction = func);

        var message = new CloudQueueMessage("TestMessage");

        // Act
        await _function.Queue(message, _testQueue, _testPoisonQueue);
        await requestFunction(new AuditReportRequest
        {
            LoginId = loginId,
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
        });

        // Assert
        _executor.Verify(e =>
            e.Execute(
                It.IsAny<ILogger>(),
                It.Is<CloudQueueMessage>(m => m == message),
                It.Is<CloudQueue>(q => q == _testQueue),
                It.Is<CloudQueue>(q => q == _testPoisonQueue),
                It.IsAny<Func<AuditReportRequest, Task>>()));
        _compute.Verify(e =>
            e.Execute(loginId,startDateTime, endDateTime));
    }

    [TestMethod]
    public async Task Http_WhenCalled_ShouldCreateRequest()
    {
        // Arrange
        var expectedHttpResponse = new HttpResponseMessage(HttpStatusCode.OK);
        var httpRequest = new HttpRequestMessage();
        var queue = new CloudQueue(new Uri("http://testUri"));

        _queueOrchestrator.Setup(qo =>
                qo.QueueManualRequest(
                    It.IsAny<ILogger>(),
                    It.Is<HttpRequestMessage>(r => r == httpRequest),
                    It.Is<CloudQueue>(q => q == queue)))
            .ReturnsAsync(expectedHttpResponse);

        // Act
        var response = await _function.Http(httpRequest, queue);

        // Assert
        response.Should().Be(expectedHttpResponse);
    }

}