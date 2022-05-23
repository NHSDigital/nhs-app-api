using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.ReferrerLogin
{
    internal static class ReferrerLoginMetrics
    {
        public static async Task AddConsentMetric(TestEnv env, string loginId, string proofLevel, DateTimeOffset time,
            string sessionId, string odsCode = "AB123")
        {
            await env.Postgres.Events.ConsentMetric.Insert(new ConsentMetricRow
            {
                LoginId = loginId,
                OdsCode = odsCode,
                Timestamp = time,
                ProofLevel = proofLevel,
                SessionId = sessionId
            });
        }

        public static async Task AddLoginMetric(TestEnv env, string loginId, string proofLevel, DateTimeOffset time,
            string sessionId, string odsCode = "AB123")
        {
            await env.Postgres.Events.LoginMetric.Insert(new LoginMetricRow
            {
                LoginId = loginId,
                OdsCode = odsCode,
                Timestamp = time,
                ProofLevel = proofLevel,
                LoginEventId = "LoginEventId",
                SessionId = sessionId
            });
        }

        public static async Task AddWebIntegrationReferralsMetric(TestEnv env, DateTimeOffset time, string referrer, string sessionId)
        {
            await env.Postgres.Events.WebIntegrationReferrals.Insert(new WebIntegrationReferralsMetricRow()
            {
                Timestamp = time,
                Referrer = referrer,
                SessionId = sessionId,
                AuditId = Guid.NewGuid().ToString()
            });
        }
    }
}
