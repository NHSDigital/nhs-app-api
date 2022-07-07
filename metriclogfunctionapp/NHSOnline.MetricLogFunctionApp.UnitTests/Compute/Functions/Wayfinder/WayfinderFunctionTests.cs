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
using NHSOnline.MetricLogFunctionApp.Compute.ReferrerServiceJourney;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.Wayfinder
{
    [TestClass]
    public class WayfinderFunctionTests
    {
        private Mock<ILogger<ReferrerServiceJourneyFunction>> _logger;
        private Mock<IReferrerServiceJourneyCompute> _compute;
        private Mock<IComputeExecutor<ReferrerServiceJourneyReportRequest>> _executor;
        private Mock<IRequestQueueOrchestrator<ReferrerServiceJourneyReportRequest>> _queueOrchestrator;

        private ReferrerServiceJourneyFunction _function;
        private CloudQueue _testQueue;
        private CloudQueue _testPoisonQueue;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger<ReferrerServiceJourneyFunction>>();
            _compute = new Mock<IReferrerServiceJourneyCompute>();
            _executor = new Mock<IComputeExecutor<ReferrerServiceJourneyReportRequest>>();
            _queueOrchestrator = new Mock<IRequestQueueOrchestrator<ReferrerServiceJourneyReportRequest>>();

            _testQueue = new CloudQueue(new Uri("http://testUri"));
            _testPoisonQueue = new CloudQueue(new Uri("http://testUri2"));

            _function = new ReferrerServiceJourneyFunction(
                _logger.Object, _compute.Object, _executor.Object, _queueOrchestrator.Object);
        }

        [TestMethod]
        public async Task Timer_WhenCalled_ShouldCreateRequest()
        {
            // Arrange
            Func<ReferrerServiceJourneyReportRequest> requestFunction = null;
            _queueOrchestrator.Setup(qo =>
                    qo.QueueScheduledRequest(
                        It.IsAny<ILogger>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<Func<ReferrerServiceJourneyReportRequest>>()))
                .Callback<ILogger, CloudQueue, Func<ReferrerServiceJourneyReportRequest>>(
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
                    It.IsAny<Func<ReferrerServiceJourneyReportRequest>>()));
        }

        [TestMethod]
        public async Task Queue_WhenCalled_ShouldProcessRequest()
        {
            // Arrange
            var startDateTime = DateTime.UtcNow.Date;
            var endDateTime = startDateTime.AddDays(1);

            Func<ReferrerServiceJourneyReportRequest, Task> requestFunction = null;
            _executor.Setup(e =>
                    e.Execute(
                        It.IsAny<ILogger>(),
                        It.IsAny<CloudQueueMessage>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<Func<ReferrerServiceJourneyReportRequest, Task>>()))
                .Callback<ILogger, CloudQueueMessage, CloudQueue, CloudQueue, Func<ReferrerServiceJourneyReportRequest, Task>>(
                    (l, m, q, qp, func) => requestFunction = func);

            var message = new CloudQueueMessage("TestMessage");

            // Act
            await _function.Queue(message, _testQueue, _testPoisonQueue);
            await requestFunction(new ReferrerServiceJourneyReportRequest
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
                    It.IsAny<Func<ReferrerServiceJourneyReportRequest, Task>>()));
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
}
