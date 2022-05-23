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
using NHSOnline.MetricLogFunctionApp.Compute.ReferrerLogin;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Compute.Functions.ReferrerLogin
{
    [TestClass]
    public class ReferrerLoginFunctionTests
    {
        private Mock<ILogger<ReferrerLoginFunction>> _logger;
        private Mock<IReferrerLoginCompute> _compute;
        private Mock<IComputeExecutor<ReferrerLoginReportRequest>> _executor;
        private Mock<IRequestQueueOrchestrator<ReferrerLoginReportRequest>> _queueOrchestrator;

        private ReferrerLoginFunction _function;
        private CloudQueue _testQueue;
        private CloudQueue _testPoisonQueue;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger<ReferrerLoginFunction>>();
            _compute = new Mock<IReferrerLoginCompute>();
            _executor = new Mock<IComputeExecutor<ReferrerLoginReportRequest>>();
            _queueOrchestrator = new Mock<IRequestQueueOrchestrator<ReferrerLoginReportRequest>>();

            _testQueue = new CloudQueue(new Uri("http://testUri"));
            _testPoisonQueue = new CloudQueue(new Uri("http://testUri2"));

            _function = new ReferrerLoginFunction(
                _logger.Object, _compute.Object, _executor.Object, _queueOrchestrator.Object);
        }

        [TestMethod]
        public async Task Timer_WhenCalled_ShouldCreateRequest()
        {
            // Arrange
            Func<ReferrerLoginReportRequest> requestFunction = null;
            _queueOrchestrator.Setup(qo =>
                    qo.QueueScheduledRequest(
                        It.IsAny<ILogger>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<Func<ReferrerLoginReportRequest>>()))
                .Callback<ILogger, CloudQueue, Func<ReferrerLoginReportRequest>>(
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
                    It.IsAny<Func<ReferrerLoginReportRequest>>()));
        }

        [TestMethod]
        public async Task Queue_WhenCalled_ShouldProcessRequest()
        {
            // Arrange
            var startDateTime = DateTime.UtcNow.Date;
            var endDateTime = startDateTime.AddDays(1);

            Func<ReferrerLoginReportRequest, Task> requestFunction = null;
            _executor.Setup(e =>
                    e.Execute(
                        It.IsAny<ILogger>(),
                        It.IsAny<CloudQueueMessage>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<CloudQueue>(),
                        It.IsAny<Func<ReferrerLoginReportRequest, Task>>()))
                .Callback<ILogger, CloudQueueMessage, CloudQueue, CloudQueue, Func<ReferrerLoginReportRequest, Task>>(
                    (l, m, q, qp, func) => requestFunction = func);

            var message = new CloudQueueMessage("TestMessage");

            // Act
            await _function.Queue(message, _testQueue, _testPoisonQueue);
            await requestFunction(new ReferrerLoginReportRequest
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
                    It.IsAny<Func<ReferrerLoginReportRequest, Task>>()));
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
