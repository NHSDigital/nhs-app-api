using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.FirstLogins
{
    internal static class FirstLoginMetrics
    {
        public static async Task AddLoginMetric(TestEnv env, string loginId, string proofLevel, DateTimeOffset time, string odsCode = "AB123", string sessionId = "SessionId")
        {
            await env.Postgres.Events.LoginMetric.Insert(new LoginMetricRow
            {
                LoginId = loginId,
                OdsCode = odsCode,
                Timestamp = time,
                ProofLevel = proofLevel,
                LoginEventId = "LoginEventId",
                Referrer = "Referrer",
                SessionId = sessionId,
                AuditId = Guid.NewGuid().ToString()
            });
        }

        public static async Task AddConsentMetric(TestEnv env, string loginId, string proofLevel, DateTimeOffset time, string odsCode = "AB123", string sessionId = "SessionId")
        {
            await env.Postgres.Events.ConsentMetric.Insert(new ConsentMetricRow
            {
                LoginId = loginId,
                OdsCode = odsCode,
                Timestamp = time,
                ProofLevel = proofLevel,
                AuditId = Guid.NewGuid().ToString(),
                SessionId = sessionId
            });
        }

        public static AuditRecord BuildEvent(string auditId, string sessionId, DateTime eventTimestamp, string operation, string details, string proofLevel, string odsCode, string referrer, string loginId = "NhsLoginSubject-Test")
        {
            var auditRecord = new AuditRecord()
            {
                AuditId = auditId,
                NhsLoginSubject = loginId,
                NhsNumber = "NhsNumber-Test",
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
                ProofLevel = proofLevel,
                ODS = odsCode,
                Referrer = referrer,
                IntegrationReferrer = $"Int-{referrer}"
            };

            return auditRecord;
        }
    }
}