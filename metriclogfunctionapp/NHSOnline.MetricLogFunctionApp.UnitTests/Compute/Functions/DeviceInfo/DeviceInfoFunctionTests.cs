using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute;
using NHSOnline.MetricLogFunctionApp.Compute.DeviceInfo;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.DeviceInfo;

[TestClass]
public class DeviceInfoFunctionTests
{
    private Mock<ILogger<DeviceInfoFunction>> _logger;
    private Mock<IDeviceInfoCompute> _compute;
    private Mock<IComputeExecutor<DeviceInfoReportRequest>> _executor;
    private Mock<IRequestQueueOrchestrator<DeviceInfoReportRequest>> _queueOrchestrator;

    private DeviceInfoFunction _function;
    private CloudQueue _testQueue;
    private CloudQueue _testPoisonQueue;

    [TestInitialize]
    public void TestInitialize()
    {
        _logger = new Mock<ILogger<DeviceInfoFunction>>();
        _compute = new Mock<IDeviceInfoCompute>();
        _executor = new Mock<IComputeExecutor<DeviceInfoReportRequest>>();
        _queueOrchestrator = new Mock<IRequestQueueOrchestrator<DeviceInfoReportRequest>>();

        _testQueue = new CloudQueue(new Uri("http://testUri"));
        _testPoisonQueue = new CloudQueue(new Uri("http://testUri2"));

        _function = new DeviceInfoFunction(
            _logger.Object, _compute.Object, _executor.Object, _queueOrchestrator.Object);
    }

    [TestMethod]
    public async Task Timer_WhenCalled_ShouldCreateRequest()
    {
        // Arrange
        Func<DeviceInfoReportRequest> requestFunction = null;
        _queueOrchestrator.Setup(qo =>
                qo.QueueScheduledRequest(
                    It.IsAny<ILogger>(),
                    It.IsAny<CloudQueue>(),
                    It.IsAny<Func<DeviceInfoReportRequest>>()))
            .Callback<ILogger, CloudQueue, Func<DeviceInfoReportRequest>>(
                (l, q, func) => requestFunction = func);

        var timerInfo = new TimerInfo(
            new ConstantSchedule(TimeSpan.MaxValue), new ScheduleStatus());

        // Act
        await _function.Timer(timerInfo, _testQueue);
        var request = requestFunction();

        // Assert
        request.StartDateTime.Should().Be(DateTime.UtcNow.Date.AddDays(-1));
        request.EndDateTime.Should().Be(request.StartDateTime.AddDays(1));
        _queueOrchestrator.Verify(qo =>
            qo.QueueScheduledRequest(
                It.Is<ILogger>(l => l == _logger.Object),
                It.Is<CloudQueue>(q => q == _testQueue),
                It.IsAny<Func<DeviceInfoReportRequest>>()));
    }

    [TestMethod]
    public async Task Queue_WhenCalled_ShouldProcessRequest()
    {
        // Arrange
        var startDateTime = DateTime.UtcNow.Date;
        var endDateTime = startDateTime.AddDays(1);

        Func<DeviceInfoReportRequest, Task> requestFunction = null;
        _executor.Setup(e =>
                e.Execute(
                    It.IsAny<ILogger>(),
                    It.IsAny<CloudQueueMessage>(),
                    It.IsAny<CloudQueue>(),
                    It.IsAny<CloudQueue>(),
                    It.IsAny<Func<DeviceInfoReportRequest, Task>>()))
            .Callback<ILogger, CloudQueueMessage, CloudQueue, CloudQueue, Func<DeviceInfoReportRequest, Task>>(
                (l, m, q, qp, func) => requestFunction = func);

        var message = new CloudQueueMessage("TestMessage");

        // Act
        await _function.Queue(message, _testQueue, _testPoisonQueue);
        await requestFunction(new DeviceInfoReportRequest
        {
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
                It.IsAny<Func<DeviceInfoReportRequest, Task>>()));
        _compute.Verify(e =>
            e.Execute(startDateTime, endDateTime));
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
