using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;
using LastLoginPatientIdentifierMetric = NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier.LastLoginPatientIdentifier;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier;

[TestClass]
public class LastLoginPatientIdentifierEtlTests
{
    private Mock<IEventsRepository> _repo;
    private Mock<IAuditLogParser<LastLoginPatientIdentifierMetric>> _parser;
    private LastLoginPatientIdentifierEtl _etl;
    private Mock<IRequestQueueOrchestrator<AuditReportRequest>> _requestQueueOrchestrator;

    [TestInitialize]
    public void TestInitialize()
    {
        _repo = new Mock<IEventsRepository>();
        _parser = new Mock<IAuditLogParser<LastLoginPatientIdentifierMetric>>(MockBehavior.Strict);
        _requestQueueOrchestrator = new Mock<IRequestQueueOrchestrator<AuditReportRequest>>();
        _etl = new LastLoginPatientIdentifierEtl(_repo.Object, _parser.Object, _requestQueueOrchestrator.Object);
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
        var nhsNumber = "NhsNumber-Test";
        var events = eventsIds.Select(id => BuildEvent(id)).ToList();
        eventsIds.ForEach(id =>
        {
            _parser.Setup(p =>
                    p.Parse(It.Is<AuditRecord>(s => s.SessionId.Contains(id))))
                .Returns(
                    BuildMetric($"{id}-LoginId",
                                nhsNumber,
                                timeStamp,
                                $"{id}-AuditId")
                );
        });

        //Act
        await _etl.Execute(events);

        //Assert
        eventsIds.ForEach(id =>
            _repo.Verify(r =>
                r.CallStoredProcedure(
                    "CALL events.LastLoginPatientIdentifierInsert({0},{1},{2},{3})",
                    new object[]
                    {
                        $"{id}-LoginId",
                        nhsNumber,
                        new DateTimeOffset(DateTime.Parse(timeStamp)),
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

    private static LastLoginPatientIdentifierMetric BuildMetric(
        string loginId,
        string nhsNumber,
        string timestamp,
        string auditId)
    {
        return new LastLoginPatientIdentifierMetric
        {
            LoginId = loginId,
            NhsNumber = nhsNumber,
            Timestamp = new DateTimeOffset(DateTime.Parse(timestamp)),
            AuditId = auditId
        };
    }
}
