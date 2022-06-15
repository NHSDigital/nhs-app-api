using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Cancel;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.Appointment.Cancel
{
    [TestClass]
    public class AppointmentCancelEtlTests
    {
        private Mock<IEventsRepository> _repo;
        private Mock<IAuditLogParser<AppointmentCancelMetric>> _parser;
        private AppointmentCancelMetricEtl _etl;
        private Mock<IRequestQueueOrchestrator<AuditReportRequest>> _requestQueueOrchestrator;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new Mock<IEventsRepository>();
            _parser = new Mock<IAuditLogParser<AppointmentCancelMetric>>(MockBehavior.Strict);
            _requestQueueOrchestrator = new Mock<IRequestQueueOrchestrator<AuditReportRequest>>();
            _etl = new AppointmentCancelMetricEtl(_repo.Object, _parser.Object,_requestQueueOrchestrator.Object);
        }

        [TestMethod]
        public async Task Execute_ShouldCallStoredProcedureWithCorrectParams()
        {
            //Arrange
            var timeStamp = "2021-11-01T09:00:01";
            var eventsIds = new List<string>
            {
                "TestCommunicationId1", "TestCommunicationId2"
            };

            var events = eventsIds.Select(id =>
                    BuildEvent(id))
                .ToList();

            eventsIds.ForEach(id =>
            {
                _parser.Setup(p =>
                        p.Parse(It.Is<AuditRecord>(s => s.SessionId.Contains(id))))
                    .Returns(
                        BuildMetric(timeStamp,
                            $"{id}-SessionId",
                            $"{id}-AuditId")
                    );
            });

            //Act
            await _etl.Execute(events);

            //Assert
            eventsIds.ForEach(id =>
                _repo.Verify(r =>
                    r.CallStoredProcedure(
                        "CALL events.AppointmentCancelMetricInsert({0},{1},{2})",
                        new object[]
                        {
                            new DateTimeOffset(DateTime.Parse(timeStamp)),
                            $"{id}-SessionId",
                            $"{id}-AuditId"
                        })
                )
            );

            VerifyMocks();
        }

        private void VerifyMocks()
        {
            _parser.VerifyAll();
        }

        private static AuditRecord BuildEvent(string sessionId)
        {
            return new AuditRecord()
            {
                AuditId = "AuditId-Test",
                NhsLoginSubject = "NhsLoginSubject-Test",
                NhsNumber = "NhsNumber-Test",
                IsActingOnBehalfOfAnother = false,
                Supplier = "Supplier-Test",
                Operation = "Operation-Test",
                Details = "Details-Test",
                ApiVersion = "Api-Test",
                WebVersion = "Web-Test",
                NativeVersion = "NativeVersion-Test",
                Environment = "localtest",
                SessionId = sessionId,
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 01),
                ProofLevel = "P5",
                ODS = "ods1",
                Referrer = "ref1",
                IntegrationReferrer = "IntRef1"
            };
        }

        private static AppointmentCancelMetric BuildMetric(
            string timestamp,
            string sessionId,
            string auditId)
        {
            return new AppointmentCancelMetric
            {
                Timestamp = new DateTimeOffset(DateTime.Parse(timestamp)),
                SessionId = sessionId,
                AuditId = auditId
            };
        }
    }
}