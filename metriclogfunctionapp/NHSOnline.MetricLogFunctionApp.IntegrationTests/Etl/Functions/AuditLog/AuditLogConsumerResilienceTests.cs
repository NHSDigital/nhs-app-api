using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Etl.Functions.AuditLog
{
    [TestClass]
    public class AuditLogConsumerResilienceTests
    {
        [NhsAppTest]
        public async Task AuditLog_AuditLogEventHubMessagesWhenErrorEncountered_ShouldRetry(TestEnv env)
        {
            var triggerBody = @"
    IF NEW.""AuditId"" = 'AuditIdThrowsError' THEN
        RAISE EXCEPTION 'Simulating failure to insert with AuditId %', NEW.""AuditId"" USING ERRCODE = 'integrity_constraint_violation';
    END IF;
";
            await env.Postgres.Events.LoginMetric.SetupTrigger(triggerBody);

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(
                BuildEvent("AuditIdThrowsError", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1),
                    "Login_Success",
                    "Successful Login with"));
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var loginMetricRows = await env.Postgres.Events.LoginMetric.FetchAll();
            loginMetricRows.Should().BeEmpty();

            var functionLogs = env.FunctionLogs();
            functionLogs
                .Where(log => log.Contains("Encountered error while processing events."))
                .Should().HaveCount(3);
            functionLogs.Should()
                .ContainSingle(log => log.Contains("Max retries reached: 2"));
        }

        private static AuditRecord BuildEvent(string auditId, string sessionId, DateTime eventTimestamp, string operation, string details)
        {
            var auditRecord = new AuditRecord()
            {
                AuditId = auditId,
                NhsLoginSubject = "NhsLoginSubject-Test",
                NhsNumber = null,
                IsActingOnBehalfOfAnother = false,
                Supplier = "Supplier-Test",
                Operation = operation,
                Details = details,
                ApiVersion = "Api-Test",
                WebVersion = "Web-Test",
                NativeVersion = "NativeVersion-Test",
                Environment = "localtest",
                SessionId = sessionId,
                Timestamp = eventTimestamp,
                ProofLevel = "P5",
                ODS = "ODS1",
                Referrer = "Ref1"
            };

            return auditRecord;
        }
    }
}

