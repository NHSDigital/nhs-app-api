using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Etl.Functions.AuditLog.NominatedPharmacy.Create
{
    [TestClass]
    public class NominatedPharmacyCreateMetricTests
    {
        private string _sessionId1 = "SessionId1";
        private string _auditId1 = "AuditId1";
        private DateTime _timestamp = new(2021, 11, 01, 09, 00, 00, 1);
        private string _operation = "NominatedPharmacy_Update_Response";
        private string _odsCode = "Z0000";
        private string _details = "Successfully created new nominated pharmacy registration to Z0000";

        private AuditRecord _record;
        private NominatedPharmacyCreateMetricRow _expectation;

        [TestInitialize]
        public void Setup()
        {
            _record = new AuditRecord()
            {
                SessionId = _sessionId1, AuditId = _auditId1, Timestamp = _timestamp, Operation = _operation,
                Details = _details, ODS = _odsCode
            };
            _expectation = new NominatedPharmacyCreateMetricRow()
                {Timestamp = _timestamp, SessionId = _sessionId1, AuditId = _auditId1};
        }

        [NhsAppTest]
        public async Task NominatedPharmacyCreateMetric_IsLoaded(TestEnv env)
        {
            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
            status.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.NominatedPharmacyCreateMetric.FetchAll();
            var row = rows.Single(x => x.SessionId == _sessionId1);

            // Assert
            AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
        }

        [NhsAppTest]
        public async Task NominatedPharmacyCreateMetric_WhenEventIsRepeated_DuplicatesNotLoaded(TestEnv env)
        {
            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
            status.Should().Be(HttpStatusCode.OK);

            var duplicateStatus = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
            duplicateStatus.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.NominatedPharmacyCreateMetric.FetchAll();

            var row = rows.Single(x => x.SessionId == _sessionId1);

            // Assert
            AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
        }

        [NhsAppTest]
        public async Task NominatedPharmacyCreateMetric_WhenRowExistsInDB_DuplicatesNotLoaded(TestEnv env)
        {
            // Stage
            await env.Postgres.Events.NominatedPharmacyCreateMetric.Insert(new NominatedPharmacyCreateMetricRow()
                {
                    SessionId = _sessionId1,
                    Timestamp = _timestamp,
                    AuditId = _auditId1
                }
            );

            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
            status.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.NominatedPharmacyCreateMetric.FetchAll();
            var row = rows.Single(x => x.SessionId == _sessionId1);

            // Assert
            AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
        }

        [NhsAppTest]
        public async Task
            NominatedPharmacyCreateMetric_AuditLogEventHubMessagesWithInvalidOperationField_AreNotLoaded(TestEnv env)
        {
            // Stage
            var invalidOperation = "invalid";
            var invalidRecord = new AuditRecord()
            {
                SessionId = _sessionId1, Timestamp = _timestamp, Operation = invalidOperation, Details = _details,
                ODS = _odsCode
            };

            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, invalidRecord);
            status.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.NominatedPharmacyCreateMetric.FetchAll();

            // Assert
            rows.Should().BeEmpty();
        }

        [NhsAppTest]
        public async Task NominatedPharmacyCreateMetric_AuditLogEventHubMessagesWithInvalidDetailsField_AreNotLoaded(
            TestEnv env)
        {
            // Stage
            var invalidDetails = "invalid";
            var invalidRecord = new AuditRecord()
            {
                SessionId = _sessionId1, Timestamp = _timestamp, Operation = _operation, Details = invalidDetails,
                ODS = _odsCode
            };

            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, invalidRecord);
            status.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.NominatedPharmacyCreateMetric.FetchAll();

            // Assert
            rows.Should().BeEmpty();
        }
    }
}
