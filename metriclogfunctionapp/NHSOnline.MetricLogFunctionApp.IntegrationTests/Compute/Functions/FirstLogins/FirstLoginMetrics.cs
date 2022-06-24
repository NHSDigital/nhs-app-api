using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.FirstLogins
{
    internal static class FirstLoginMetrics
    {
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
