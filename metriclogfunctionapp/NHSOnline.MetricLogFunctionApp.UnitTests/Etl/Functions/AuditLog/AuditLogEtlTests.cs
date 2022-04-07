using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog
{
    [TestClass]
    public class AuditLogEtlTests
    {
        private Mock<IAuditLogParser<TestMetric>> _parser;
        private Mock<IAuditLogParser<LoginMetric>> _loginMetricParser;
        private Mock<IAuditLogParser<ConsentMetric>> _consentMetricParser;
        private Mock<IEventsRepository> _repo;
        private Mock<ILogger> _logger;
        private Mock<IRequestQueueOrchestrator<AuditReportRequest>> _requestQueueOrchestrator;
        private TestEtl _etl;
        private LoginMetricEtl _dependentLoginEtl;
        private ConsentMetricEtl _dependentConsentEtl;

        private readonly DateTimeOffset _timestamp =
            new DateTimeOffset(DateTime.Parse("2021-01-01T09:00:00"));

        [TestInitialize]
        public void TestInitialize()
        {
            _parser = new Mock<IAuditLogParser<TestMetric>>(MockBehavior.Strict);
            _loginMetricParser = new Mock<IAuditLogParser<LoginMetric>>();
            _consentMetricParser = new Mock<IAuditLogParser<ConsentMetric>>();
            _repo = new Mock<IEventsRepository>();
            _requestQueueOrchestrator = new Mock<IRequestQueueOrchestrator<AuditReportRequest>>();
            _etl = new TestEtl(_repo.Object, _parser.Object, _requestQueueOrchestrator.Object);
            _logger = new Mock<ILogger>();
            _dependentLoginEtl = new LoginMetricEtl(_repo.Object, _loginMetricParser.Object, _requestQueueOrchestrator.Object);
            _dependentConsentEtl = new ConsentMetricEtl(_repo.Object, _consentMetricParser.Object, _requestQueueOrchestrator.Object);
        }

        [TestMethod]
        public async Task Execute_ShouldCallStoredProcedureWithCorrectParams()
        {
            // Arrange
            var eventsIds = new List<string>
            {
                "TestId1", "TestId2"
            };

            var events = new List<AuditRecord>
            {
                new AuditRecord{Operation = "TestId1"},
                new AuditRecord{Operation = "TestId2"}
            };

            eventsIds.ForEach(id =>
            {
                _parser.Setup(p =>
                        p.Parse(It.Is<AuditRecord>(s => s.Operation.Contains(id))))
                    .Returns(new TestMetric { Id = id, Timestamp = _timestamp });
            });

            // Act
            await _etl.Execute(events);

            // Assert
            eventsIds.ForEach(id =>
                _repo.Verify(r =>
                    r.CallStoredProcedure("CALL TestSP({0},{1})", new object[] { id, _timestamp })
                )
            );

            VerifyMocks();
        }

        [TestMethod]
        public async Task ExecuteDependentEvent_WhenLoginEventLogged_MessageShouldBePushedToFirstLoginsQueue()
        {
            // Arrange
            var eventsIds = new List<string>
            {
                "TestId1", "TestId2"
            };

            var events = new List<AuditRecord>
            {
                new() {Operation = "TestId1"},
                new() {Operation = "TestId2"}
            };

            eventsIds.ForEach(id =>
            {
                _loginMetricParser.Setup(p =>
                        p.Parse(It.Is<AuditRecord>(s => s.Operation.Contains(id))))
                    .Returns(new LoginMetric() { AuditId = id, LoginId = id, Timestamp = _timestamp });
            });

            // Act
            await _dependentLoginEtl.ExecuteDependentEvent(_logger.Object,events);

            _requestQueueOrchestrator.Verify(r =>
                r.AddMessage(It.IsAny<ILogger>(), "first-logins-metric-", It.Is<AuditReportRequest>(x =>
                    x.LoginId == "TestId1")));
            _requestQueueOrchestrator.Verify(r =>
                r.AddMessage(It.IsAny<ILogger>(), "first-logins-metric-", It.Is<AuditReportRequest>(x =>
                    x.LoginId == "TestId2")));
        }

        [TestMethod]
        public async Task ExecuteDependentEvent_WhenConsentEventLogged_MessageShouldBePushedToFirstLoginsQueue()
        {
            // Arrange
            var eventsIds = new List<string>
            {
                "TestId1", "TestId2"
            };

            var events = new List<AuditRecord>
            {
                new() {Operation = "TestId1"},
                new() {Operation = "TestId2"}
            };

            eventsIds.ForEach(id =>
            {
                _consentMetricParser.Setup(p =>
                        p.Parse(It.Is<AuditRecord>(s => s.Operation.Contains(id))))
                    .Returns(new ConsentMetric() { AuditId = id, LoginId = id, Timestamp = _timestamp });
            });

            // Act
            await _dependentConsentEtl.ExecuteDependentEvent(_logger.Object,events);

            _requestQueueOrchestrator.Verify(r =>
                r.AddMessage(It.IsAny<ILogger>(), "first-logins-metric-", It.Is<AuditReportRequest>(x =>
                    x.LoginId == "TestId1")));
            _requestQueueOrchestrator.Verify(r =>
                r.AddMessage(It.IsAny<ILogger>(), "first-logins-metric-", It.Is<AuditReportRequest>(x =>
                    x.LoginId == "TestId2")));
        }

        [TestMethod]
        public async Task Execute_ShouldNotCallStoredProcedureForNullRows()
        {
            // Arrange
            var events = new List<AuditRecord>
            {
                new AuditRecord() { Operation =  "TestId1"},
                new AuditRecord() { Operation =  "TestId2"}
            };

            _parser.Setup(p =>
                    p.Parse(It.Is<AuditRecord>(s => s.Operation.Contains("TestId1"))))
                .Returns(new TestMetric { Id = "TestId1", Timestamp = _timestamp });

            _parser.Setup(p =>
                    p.Parse(It.Is<AuditRecord>(s => s.Operation.Contains("TestId2"))))
                .Returns((TestMetric)null);

            // Act
            await _etl.Execute(events);

            // Assert
            _repo.Verify(r =>
                r.CallStoredProcedure("CALL TestSP({0},{1})", new object[] { "TestId1", _timestamp }));
            _repo.VerifyNoOtherCalls();

            VerifyMocks();
        }

        [TestMethod]
        public void Execute_WhenExceptionEncounteredWhenCallingStoredProcedure_ShouldBubbleException()
        {
            // Arrange
            var events = new List<AuditRecord> { new AuditRecord { AuditId = "TestRecord1" } };
            var metric = new TestMetric { Id = "TestMetricId" };

            _parser.Setup(p =>
                    p.Parse(It.Is<AuditRecord>(s => s.AuditId.Contains("TestRecord1"))))
                .Returns((TestMetric)null);

            _repo.Setup(r =>
                    r.CallStoredProcedure(
                        It.IsAny<string>(),
                        It.IsAny<object[]>()
                    ))
                .Throws(new Exception("This is a test exception"));

            // Act & Assert
            Assert.ThrowsExceptionAsync<Exception>(() => _etl.Execute(events));

            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _parser.VerifyAll();
        }

        public class TestMetric : IEventRepositoryRow
        {
            public string Id { get; set; }
            public DateTimeOffset Timestamp { get; set; }
        }

        public class TestEtl : AuditLogEtl<TestMetric>
        {
            public TestEtl(IEventsRepository repo, IAuditLogParser<TestMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
                : base(repo, parser, queueOrchestrator)
            {
            }

            protected override string StoredProcedureName => "CALL TestSP({0},{1})";
            protected override object[] ReturnParams(TestMetric metric)
            {
                return new object[]
                {
                    metric.Id,
                    metric.Timestamp
                };
            }
        }
    }
}

