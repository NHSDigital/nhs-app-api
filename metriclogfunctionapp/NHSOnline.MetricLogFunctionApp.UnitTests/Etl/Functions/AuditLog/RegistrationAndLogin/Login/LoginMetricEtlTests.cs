using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.RegistrationAndLogin.Login
{
    [TestClass]
    public class LoginMetricEtlTests

    {
        private Mock<IEventsRepository> _repo;
        private Mock<IAuditLogParser<LoginMetric>> _parser;
        private LoginMetricEtl _etl;

        [TestInitialize]
        public void TestInitialize()
        {
            _repo = new Mock<IEventsRepository>();
            _parser = new Mock<IAuditLogParser<LoginMetric>>(MockBehavior.Strict);
            _etl = new LoginMetricEtl(_repo.Object, _parser.Object);
        }

        [TestMethod]
        public async Task Execute_ShouldCallStoredProcedureWithCorrectParams()
        {
            // Arrange
            var loginTimeStamp = "2021-11-01T09:00:01";
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
                        BuildMetric(loginTimeStamp,
                            $"{id}-metric",
                            $"{id}-AuditId")
                    );
            });

            // Act
            await _etl.Execute(events);

            // Assert
            eventsIds.ForEach(id =>
                _repo.Verify(r =>
                    r.CallStoredProcedure(
                        "CALL events.LoginMetricInsert({0},{1},{2},{3},{4},{5},{6});",
                        new object[]
                        {
                            new DateTimeOffset(DateTime.Parse(loginTimeStamp)),
                            "ODS-Test",
                            "Login-Test",
                            "P5",
                            "Referrer-Test",
                            $"{id}-metric",
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

        private static LoginMetric BuildMetric(
            string timestamp,
            string sessionId,
            string auditId)
        {
            return new LoginMetric
            {
                Timestamp = new DateTimeOffset(DateTime.Parse(timestamp)),
                OdsCode = "ODS-Test",
                LoginId = "Login-Test",
                ProofLevel = "P5",
                LoginEventId = "LoginEventId-Test",
                Referrer = "Referrer-Test",
                SessionId = sessionId,
                AuditId = auditId
            };
        }
    }
};

