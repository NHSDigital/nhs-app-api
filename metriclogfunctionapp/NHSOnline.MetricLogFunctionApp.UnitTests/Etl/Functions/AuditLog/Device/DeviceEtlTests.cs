using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Device;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.Device;

[TestClass]
public class DeviceEtlTests
{
    private Mock<IEventsRepository> _repo;
    private Mock<IAuditLogParser<DeviceMetric>> _parser;
    private DeviceMetricEtl _etl;
    private Mock<IRequestQueueOrchestrator<AuditReportRequest>> _requestQueueOrchestrator;

    [TestInitialize]
    public void TestInitialize()
    {
        _repo = new Mock<IEventsRepository>();
        _parser = new Mock<IAuditLogParser<DeviceMetric>>(MockBehavior.Strict);
        _requestQueueOrchestrator = new Mock<IRequestQueueOrchestrator<AuditReportRequest>>();
        _etl = new DeviceMetricEtl(_repo.Object, _parser.Object, _requestQueueOrchestrator.Object);
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
                        $"{id}-AppVersion",
                        $"{id}-DeviceManufacturer",
                        $"{id}-DeviceModel",
                        $"{id}-DeviceOS",
                        $"{id}-DeviceOSVersion",
                        $"{id}-UserAgent",
                        $"{id}-AuditId")
                );
        });

        //Act
        await _etl.Execute(events);

        //Assert
        eventsIds.ForEach(id =>
            _repo.Verify(r =>
                r.CallStoredProcedure(
                    "CALL events.DeviceInsert({0},{1},{2},{3},{4},{5},{6},{7},{8})",
                    new object[]
                    {
                        DateTimeOffset.Parse(timeStamp, CultureInfo.InvariantCulture),
                        $"{id}-SessionId",
                        $"{id}-AppVersion",
                        $"{id}-DeviceManufacturer",
                        $"{id}-DeviceModel",
                        $"{id}-DeviceOS",
                        $"{id}-DeviceOSVersion",
                        $"{id}-UserAgent",
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

    private static DeviceMetric BuildMetric(
        string timestamp,
        string sessionId,
        string appVersion,
        string deviceManufacturer,
        string deviceModel,
        string deviceOS,
        string deviceOSVersion,
        string userAgent,
        string auditId)
    {
        return new DeviceMetric
        {
            Timestamp = DateTimeOffset.Parse(timestamp, CultureInfo.InvariantCulture),
            SessionId = sessionId,
            AppVersion = appVersion,
            DeviceManufacturer = deviceManufacturer,
            DeviceModel = deviceModel,
            DeviceOS = deviceOS,
            DeviceOSVersion = deviceOSVersion,
            UserAgent = userAgent,
            AuditId = auditId
        };
    }
}
