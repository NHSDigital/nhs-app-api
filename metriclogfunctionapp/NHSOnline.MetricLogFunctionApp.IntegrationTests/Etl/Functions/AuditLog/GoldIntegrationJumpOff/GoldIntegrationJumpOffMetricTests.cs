using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Etl.Functions.AuditLog.GoldIntegrationJumpOff;

[TestClass]
public class GoldIntegrationJumpOffMetricTests
{
        private string _sessionId1 = "SessionId1";
        private string _auditId1 = "AuditId1";
        private DateTime _timestamp = new(2021, 11, 01, 09, 00, 00, 1);
        private string _operation = "GoldIntegration_JumpOff_Click";
        private string _details = "The user has jumped off to an integration partner";
        private string _providerId = "eConsult";
        private string _providerName = "eConsult";
        private string _jumpOffId = "onlineConsultation";


        private AuditRecord _record;
        private GoldIntegrationJumpOffMetricRow _expectation;

        [TestInitialize]
        public void Setup()
        {
            _record = new AuditRecord
            {
                SessionId = _sessionId1, AuditId = _auditId1, Timestamp = _timestamp, Operation = _operation,
                Details = _details,ProviderId = _providerId,ProviderName = _providerName,JumpOffId = _jumpOffId
            };
            _expectation = new GoldIntegrationJumpOffMetricRow
                {Timestamp = _timestamp, SessionId = _sessionId1, AuditId = _auditId1,ProviderId = _providerId,ProviderName = _providerName,JumpOffId = _jumpOffId};
        }

        [NhsAppTest]
        public async Task GoldIntegrationJumpOffMetric_IsLoaded(TestEnv env)
        {
            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
            status.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.GoldIntegrationJumpOffMetric.FetchAll();
            var row = rows.Single(x => x.SessionId == _sessionId1);

            // Assert
            AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
        }

        [NhsAppTest]
        public async Task GoldIntegrationJumpOffMetric_WhenEventIsRepeated_DuplicatesNotLoaded(TestEnv env)
        {
            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
            status.Should().Be(HttpStatusCode.OK);

            var duplicateStatus = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
            duplicateStatus.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.GoldIntegrationJumpOffMetric.FetchAll();

            var row = rows.Single(x => x.SessionId == _sessionId1);

            // Assert
            AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
        }

        [NhsAppTest]
        public async Task GoldIntegrationJumpOffMetric_WhenRowExistsInDB_DuplicatesNotLoaded(TestEnv env)
        {
            // Stage
            await env.Postgres.Events.GoldIntegrationJumpOffMetric.Insert(new GoldIntegrationJumpOffMetricRow
                {
                    SessionId = _sessionId1,
                    Timestamp = _timestamp,
                    AuditId = _auditId1,
                    ProviderId = _providerId,
                    ProviderName = _providerName,
                    JumpOffId = _jumpOffId
                }
            );

            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
            status.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.GoldIntegrationJumpOffMetric.FetchAll();
            var row = rows.Single(x => x.SessionId == _sessionId1);

            // Assert
            AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
        }

        [NhsAppTest]
        public async Task
            GoldIntegrationJumpOffMetric_AuditLogEventHubMessagesWithInvalidOperationField_AreNotLoaded(TestEnv env)
        {
            // Stage
            var invalidOperation = "invalid";
            var invalidRecord = new AuditRecord()
            {
                SessionId = _sessionId1, Timestamp = _timestamp, Operation = invalidOperation, Details = _details
            };

            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, invalidRecord);
            status.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.SilverIntegrationJumpOffMetric.FetchAll();

            // Assert
            rows.Should().BeEmpty();
        }

        [NhsAppTest]
        public async Task GoldIntegrationJumpOffMetric_AuditLogEventHubMessagesWithInvalidDetailsField_AreNotLoaded(
            TestEnv env)
        {
            // Stage
            var invalidDetails = "invalid";
            var invalidRecord = new AuditRecord()
            {
                SessionId = _sessionId1, Timestamp = _timestamp, Operation = _operation, Details = invalidDetails
            };

            // Act
            var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, invalidRecord);
            status.Should().Be(HttpStatusCode.OK);

            var rows = await env.Postgres.Events.GoldIntegrationJumpOffMetric.FetchAll();

            // Assert
            rows.Should().BeEmpty();
        }
}
