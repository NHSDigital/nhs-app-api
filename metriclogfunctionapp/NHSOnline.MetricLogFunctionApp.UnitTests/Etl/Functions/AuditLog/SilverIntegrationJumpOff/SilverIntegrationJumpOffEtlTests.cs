using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.SilverIntegrationJumpOff;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.SilverIntegrationJumpOff
{
    [TestClass]
    public class SilverIntegrationJumpOffEtlTests
    {
        private Mock<IEventsRepository> _repo;
        private Mock<IAuditLogParser<SilverIntegrationJumpOffMetric>> _parser;
        private SilverIntegrationJumpOffMetricEtl _etl;
        private Mock<IRequestQueueOrchestrator<AuditReportRequest>> _requestQueueOrchestrator;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new Mock<IEventsRepository>();
            _parser = new Mock<IAuditLogParser<SilverIntegrationJumpOffMetric>>(MockBehavior.Strict);
            _requestQueueOrchestrator = new Mock<IRequestQueueOrchestrator<AuditReportRequest>>();
            _etl = new SilverIntegrationJumpOffMetricEtl(_repo.Object, _parser.Object, _requestQueueOrchestrator.Object);
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
            var providerId = "ers";
            var providerName = "ElectronicReferralService";
            var jumpOffId = "manageYourReferral";

            var events = eventsIds.Select(id =>
                    BuildEvent(id))
                .ToList();

            eventsIds.ForEach(id =>
            {
                _parser.Setup(p =>
                        p.Parse(It.Is<AuditRecord>(s => s.SessionId.Contains(id))))
                    .Returns(
                        BuildMetric(timeStamp,
                            $"{id}-sessionId",
                            $"{id}-providerId",
                            $"{id}-providerName",
                            $"{id}-jumpOffId",
                            $"{id}-auditId")
                    );
            });

            //Act
            await _etl.Execute(events);

            //Assert
            eventsIds.ForEach(id =>
                _repo.Verify(r =>
                    r.CallStoredProcedure(
                        "CALL events.SilverIntegrationJumpOffMetricInsert({0},{1},{2},{3},{4},{5})",
                        new object[]
                        {
                            new DateTimeOffset(DateTime.Parse(timeStamp)),
                            $"{id}-sessionId",
                            $"{id}-providerId",
                            $"{id}-providerName",
                            $"{id}-jumpOffId",
                            $"{id}-auditId"
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
                IntegrationReferrer = "IntRef1",
                ProviderId = "ers",
                ProviderName = "Electronic Referral Service",
                JumpOffId = "manageYourReferral"
            };
        }

        private static SilverIntegrationJumpOffMetric BuildMetric(
            string timestamp,
            string sessionId,
            string providerId,
            string providerName,
            string jumpOffId,
            string auditId)
        {
            return new SilverIntegrationJumpOffMetric
            {
                Timestamp = new DateTimeOffset(DateTime.Parse(timestamp)),
                SessionId = sessionId,
                ProviderId = providerId,
                ProviderName = providerName,
                JumpOffId = jumpOffId,
                AuditId = auditId
            };
        }
    }
}
