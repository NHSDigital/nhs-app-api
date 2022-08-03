using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Etl.Functions
{
    public static class AuditLogEtlTestHelper
    {
        public static async Task<HttpStatusCode> CreateAndPostAuditRecords(TestEnv env, AuditRecord[] records)
        {
            var filledRecords = records.ToList().Select(FillAuditRecord);
            var result = await env.HttpEndpointCallers.PostAuditLogConsumer(filledRecords.ToArray());
            return result.StatusCode;
        }

        public static async Task<HttpStatusCode> CreateAndPostAuditRecord(TestEnv env, AuditRecord record)
            => await CreateAndPostAuditRecords(env, new[] {record});

        public static void AssertRowMatching<TRow>(TRow row, TRow expectation)
        {
            var fieldsToAssertOn = expectation.GetType().GetProperties();
            fieldsToAssertOn.ToList().ForEach(
                p => AssertProperty(p, row, expectation)
            );
        }

        private static void AssertProperty<TRow>(PropertyInfo field, TRow row, TRow expectedRow)
        {
            var actual = field.GetValue(row);
            var expected = field.GetValue(expectedRow);
            var assertionFailMessage = $"{field.Name} was expected to be ${expected}, but was ${actual}";

            if (expected == null || expected as DateTimeOffset? == default(DateTimeOffset))
            {
                return;
            }

            // If needed we can add assertions for specific field names or types
            actual.Should().Be(expected, assertionFailMessage);
        }

        private static readonly AuditRecord DefaultRecord = new()
        {
            AuditId = "AuditId",
            NhsLoginSubject = "NhsLoginSubject-Test",
            NhsNumber = "NhsNumber-Test",
            Supplier = "Supplier-Test",
            Operation = "operation",
            Details = "details",
            ApiVersion = "Api-Test",
            WebVersion = "Web-Test",
            NativeVersion = "NativeVersion-Test",
            Environment = "localtest",
            SessionId = "sessionId",
            Timestamp = DateTime.Parse("2020-10-08T09:30:00.000Z"),
            ProofLevel = "P9",
            ODS = "AB001",
            Referrer = "ref",
            IntegrationReferrer = "Int-ref",
            ProviderId = "ProviderId",
            ProviderName = "ProviderName",
            JumpOffId = "JumpOffId"
        };

        private static AuditRecord FillAuditRecord(AuditRecord record)
        {
            if (String.IsNullOrEmpty(record.AuditId) && String.IsNullOrEmpty(record.SessionId))
            {
                throw new ArgumentException(
                    "Audit or session ID must be set to later retrieve and assert on the record"
                    );
            }

            var result = new AuditRecord()
            {
                AuditId = FieldOrDefault(record.AuditId, DefaultRecord.AuditId),
                NhsLoginSubject = FieldOrDefault(record.NhsLoginSubject, DefaultRecord.NhsLoginSubject),
                NhsNumber = FieldOrDefault(record.NhsNumber, DefaultRecord.NhsNumber),
                IsActingOnBehalfOfAnother = record.IsActingOnBehalfOfAnother,
                Supplier = FieldOrDefault(record.Supplier, DefaultRecord.Supplier),
                Operation = FieldOrDefault(record.Operation, DefaultRecord.Operation),
                Details = FieldOrDefault(record.Details, DefaultRecord.Details),
                ApiVersion = FieldOrDefault(record.ApiVersion, DefaultRecord.ApiVersion),
                WebVersion = FieldOrDefault(record.WebVersion, DefaultRecord.WebVersion),
                NativeVersion = FieldOrDefault(record.NativeVersion, DefaultRecord.NativeVersion),
                Environment = FieldOrDefault(record.Environment, DefaultRecord.Environment),
                SessionId = FieldOrDefault(record.SessionId, DefaultRecord.SessionId),
                Timestamp =  FieldOrDefault(record.Timestamp, DefaultRecord.Timestamp),
                ProofLevel = FieldOrDefault(record.ProofLevel, DefaultRecord.ProofLevel),
                ODS = FieldOrDefault(record.ODS, DefaultRecord.ODS),
                Referrer = FieldOrDefault(record.Referrer, DefaultRecord.Referrer),
                ProviderId = FieldOrDefault(record.ProviderId, DefaultRecord.ProviderId),
                ProviderName = FieldOrDefault(record.ProviderName, DefaultRecord.ProviderName),
                JumpOffId = FieldOrDefault(record.JumpOffId, DefaultRecord.JumpOffId)
            };

            if (String.IsNullOrEmpty(record.IntegrationReferrer))
            {
                result.IntegrationReferrer =
                    String.IsNullOrEmpty(record.Referrer)
                    ? DefaultRecord.IntegrationReferrer
                    : $@"int-{record.Referrer}";
            }

            return result;
        }

        private static string FieldOrDefault(string field, string fieldDefault)
            => String.IsNullOrEmpty(field) ? fieldDefault : field;

        private static DateTime FieldOrDefault(DateTime field, DateTime fieldDefault)
            => field == default(DateTime)? fieldDefault: field;
    }
}
