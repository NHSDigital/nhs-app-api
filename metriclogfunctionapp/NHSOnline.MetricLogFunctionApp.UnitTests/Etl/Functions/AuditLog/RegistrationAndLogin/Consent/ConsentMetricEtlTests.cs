using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.RegistrationAndLogin.Consent
{
    [TestClass]
    public class ConsentMetricEtlTests
    {
        private Mock<IEventsRepository> _repo;
        private Mock<IAuditLogParser<ConsentMetric>> _parser;
        private Mock<IEtlLogger<AuditLogEtl<ConsentMetric>>> _logger;
        private ConsentMetricEtl _etl;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new Mock<IEventsRepository>();
            _parser = new Mock<IAuditLogParser<ConsentMetric>>(MockBehavior.Strict);
            _etl = new ConsentMetricEtl(_repo.Object, _parser.Object);
        }

        [TestMethod]
        public async Task Execute_ShouldCallStoredProcedureWithCorrectParams()
        {
            // Arrange
            var consentTimeStamp = "2021-11-01T09:00:01";
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
                        BuildMetric(consentTimeStamp,
                            $"{id}-metric",
                            $"{id}-AuditId",
                            $"{id}-LoginId",
                            $"{id}-ProofLevel")
                    );
            });

            // Act
            await _etl.Execute(events);

            // Assert
            eventsIds.ForEach(id =>
                _repo.Verify(r =>
                    r.CallStoredProcedure(
                        "CALL events.ConsentMetricInsert({0},{1},{2},{3},{4})",
                        new object[]
                        {
                            new DateTimeOffset(DateTime.Parse(consentTimeStamp)),
                            $"{id}-metric",
                            $"{id}-AuditId",
                            $"{id}-LoginId",
                            $"{id}-ProofLevel"
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
                Referrer = "ref1"
            };
        }

        private static ConsentMetric BuildMetric(
            string timestamp,
            string sessionId,
            string auditId,
            string loginId,
            string proofLevel)

        {
            return new ConsentMetric
            {
                Timestamp = new DateTimeOffset(DateTime.Parse(timestamp)),
                LoginId = loginId,
                SessionId = sessionId,
                AuditId = auditId,
                ProofLevel = proofLevel
            };
        }
    }
}

