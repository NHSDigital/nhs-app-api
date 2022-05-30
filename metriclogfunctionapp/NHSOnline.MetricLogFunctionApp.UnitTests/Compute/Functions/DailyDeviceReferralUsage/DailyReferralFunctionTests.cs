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
using NHSOnline.MetricLogFunctionApp.Compute.DailyDeviceReferralUsage;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.DailyDeviceReferralUsage
{
    [TestClass]
    public class DailyReferralFunctionTests
    {
        private Mock<ILogger<DailyDeviceReferralUsageFunction>> _logger;
        private Mock<IDailyDeviceReferralUsageCompute> _compute;
        private Mock<IComputeExecutor<DailyDeviceReferralUsageReportRequest>> _executor;
        private Mock<IRequestQueueOrchestrator<DailyDeviceReferralUsageReportRequest>> _queueOrchestrator;

        private DailyDeviceReferralUsageFunction _function;
        private CloudQueue _testQueue;
        private CloudQueue _testPoisonQueue;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger<DailyDeviceReferralUsageFunction>>();
            _compute = new Mock<IDailyDeviceReferralUsageCompute>();
            _executor = new Mock<IComputeExecutor<DailyDeviceReferralUsageReportRequest>>();
            _queueOrchestrator = new Mock<IRequestQueueOrchestrator<DailyDeviceReferralUsageReportRequest>>();

            _testQueue = new CloudQueue(new Uri("http://testUri"));
            _testPoisonQueue = new CloudQueue(new Uri("http://testUri2"));

            _function = new DailyDeviceReferralUsageFunction(
                _logger.Object, _compute.Object, _executor.Object, _queueOrchestrator.Object);
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

        [TestMethod]
        public async Task Timer_WhenCalled_ShouldCreateRequest()
        {
            // Arrange
            Func<DailyDeviceReferralUsageReportRequest> requestFunction = null;
            _queueOrchestrator.Setup(qo =>
                    qo.QueueScheduledRequest(
                        It.IsAny<ILogger>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<Func<DailyDeviceReferralUsageReportRequest>>()))
                .Callback<ILogger, CloudQueue, Func<DailyDeviceReferralUsageReportRequest>>(
                    (l, q, func) => requestFunction = func);

            var timerInfo = new TimerInfo(
                new ConstantSchedule(TimeSpan.MaxValue), new ScheduleStatus());

            // Act
            await _function.Timer(timerInfo, _testQueue);
            var request = requestFunction();

            // Assert
            request.EndDateTime.Should().Be(DateTime.UtcNow.Date);
            request.StartDateTime.Should().Be(request.EndDateTime.AddDays(-1));

            _queueOrchestrator.Verify(qo =>
                qo.QueueScheduledRequest(
                    It.Is<ILogger>(l => l == _logger.Object),
                    It.Is<CloudQueue>(q => q == _testQueue),
                    It.IsAny<Func<DailyDeviceReferralUsageReportRequest>>()));
        }

        [TestMethod]
        public async Task Queue_WhenCalled_ShouldProcessRequest()
        {
            // Arrange
            var endDateTime = DateTime.UtcNow.Date;
            var startDateTime = endDateTime.AddDays(-1);

            Func<DailyDeviceReferralUsageReportRequest, Task> requestFunction = null;
            _executor.Setup(e =>
                    e.Execute(
                        It.IsAny<ILogger>(),
                        It.IsAny<CloudQueueMessage>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<Func<DailyDeviceReferralUsageReportRequest, Task>>()))
                .Callback<ILogger, CloudQueueMessage, CloudQueue, CloudQueue, Func<DailyDeviceReferralUsageReportRequest, Task>>(
                    (l, m, q, qp, func) => requestFunction = func);

            var message = new CloudQueueMessage("TestMessage");

            // Act
            await _function.Queue(message, _testQueue, _testPoisonQueue);
            await requestFunction(new DailyDeviceReferralUsageReportRequest
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
                    It.IsAny<Func<DailyDeviceReferralUsageReportRequest, Task>>()));
            _compute.Verify(e =>
                e.Execute(startDateTime, endDateTime));
        }
    }
}