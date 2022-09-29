using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.ReferrerLogin
{
    [TestClass]
    public class ReferrerLoginTests
    {
        [NhsAppTest]
        public async Task ReferrerLogin_SingleLoginWithNewConsentNoReferrer_ZeroRowsAddedToComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddConsentMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 11, 30, 00, TimeSpan.Zero), sessionId1);

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(0);
            }
        }
        
        [NhsAppTest]
        public async Task ReferrerLogin_SingleLoginWithNoConsentWithReferrer_ZeroRowsAddedToComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(0);
            }
        }

        [NhsAppTest]
        public async Task ReferrerLogin_SingleLoginWithNewConsentAndReferrer_NewUserRecordedInComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddConsentMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 11, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);

                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
                row.ReferrerId.Should().Be(referrerId);
                row.NewUsers.Should().Be(1);
                row.ExistingUsers.Should().Be(0);
            }
        }

        [NhsAppTest]
        public async Task ReferrerLogin_SingleLoginWithExistingConsentAndReferrer_ExistingUserRecordedInComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddConsentMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 16, 11, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);

                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
                row.ReferrerId.Should().Be(referrerId);
                row.NewUsers.Should().Be(0);
                row.ExistingUsers.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task ReferrerLogin_MultipleLoginsWithConsentAndReferrer_NewAndExistingUsersRecordedInComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string loginId2 = "LoginId2";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string sessionId2 = "SessionId2";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddConsentMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 11, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

            await AddMetricHelper.AddLoginMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 13, 30, 00, TimeSpan.Zero), sessionId2);
            await AddMetricHelper.AddConsentMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 16, 14, 30, 00, TimeSpan.Zero), sessionId2);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 13, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId2);

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);

                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
                row.ReferrerId.Should().Be(referrerId);
                row.NewUsers.Should().Be(1);
                row.ExistingUsers.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task ReferrerLogin_SameUsersWithMultipleLoginsOnSameDay_NewAndExistingUsersRecordedInComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string loginId2 = "LoginId2";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string sessionId2 = "SessionId2";
            const string sessionId3 = "SessionId3";
            const string sessionId4 = "SessionId4";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddConsentMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 11, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 13, 30, 00, TimeSpan.Zero), sessionId2);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 13, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId2);

            await AddMetricHelper.AddLoginMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 15, 30, 00, TimeSpan.Zero), sessionId3);
            await AddMetricHelper.AddConsentMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 16, 11, 30, 00, TimeSpan.Zero), sessionId3);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 15, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId3);

            await AddMetricHelper.AddLoginMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 16, 30, 00, TimeSpan.Zero), sessionId4);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 16, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId4);

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);

                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
                row.ReferrerId.Should().Be(referrerId);
                row.NewUsers.Should().Be(1);
                row.ExistingUsers.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task ReferrerLogin_SingleLoginWithNoConsentP5P9NewLogin_NewUserRecordedInComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";
            var p5FirstLoginDateTime = new DateTime(2022, 05, 17, 08, 30, 00);
            var p9FirstLoginDateTime = new DateTime(2022, 05, 17, 09, 30, 00);

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);
            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = loginId1,
                FirstP5LoginDate = p5FirstLoginDateTime.Date,
                FirstP9LoginDate = p9FirstLoginDateTime.Date,
                ConsentProofLevel = p9ProofLevel
            });

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);

                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
                row.ReferrerId.Should().Be(referrerId);
                row.NewUsers.Should().Be(1);
                row.ExistingUsers.Should().Be(0);
            }
        }

        [NhsAppTest]
        public async Task ReferrerLogin_SingleLoginWithNoConsentP5P9ExistingLogin_ExistingUserRecordedInComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";
            var p5FirstLoginDateTime = new DateTime(2022, 05, 16, 08, 30, 00);
            var p9FirstLoginDateTime = new DateTime(2022, 05, 16, 09, 30, 00);

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);
            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = loginId1,
                FirstP5LoginDate = p5FirstLoginDateTime.Date,
                FirstP9LoginDate = p9FirstLoginDateTime.Date,
                ConsentProofLevel = p9ProofLevel
            });

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);

                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
                row.ReferrerId.Should().Be(referrerId);
                row.NewUsers.Should().Be(0);
                row.ExistingUsers.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task ReferrerLogin_MultipleLoginsWithConsentAndReferrerPlusSingleLoginWithNoConsentP5P9ExistingLogin_UsersRecordedInComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string loginId2 = "LoginId2";
            const string loginId3 = "LoginId3";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string sessionId2 = "SessionId2";
            const string sessionId3 = "SessionId3";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";
            var p5FirstLoginDateTime = new DateTime(2022, 05, 16, 08, 30, 00);
            var p9FirstLoginDateTime = new DateTime(2022, 05, 16, 09, 30, 00);

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddConsentMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 11, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

            await AddMetricHelper.AddLoginMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 13, 30, 00, TimeSpan.Zero), sessionId2);
            await AddMetricHelper.AddConsentMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 14, 30, 00, TimeSpan.Zero), sessionId2);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 13, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId2);

            await AddMetricHelper.AddLoginMetric(env, loginId3, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 15, 30, 00, TimeSpan.Zero), sessionId3);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 15, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId3);
            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = loginId3,
                FirstP5LoginDate = p5FirstLoginDateTime.Date,
                FirstP9LoginDate = p9FirstLoginDateTime.Date,
                ConsentProofLevel = p9ProofLevel
            });

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);

                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
                row.ReferrerId.Should().Be(referrerId);
                row.NewUsers.Should().Be(2);
                row.ExistingUsers.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task ReferrerLogin_MultipleLoginsWithConsentAndReferrerPlusSingleLoginWithNoConsentP5P9NewLogin_UsersRecordedInComputeTable(TestEnv env)
        {
            var startDateTime = "2022-05-17T00:00:00";
            var endDateTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string loginId2 = "LoginId2";
            const string loginId3 = "LoginId3";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string sessionId2 = "SessionId2";
            const string sessionId3 = "SessionId3";
            const string referrerId = "nhs-uk";
            const string referrerOrigin = "test";
            var p5FirstLoginDateTime = new DateTime(2022, 05, 17, 15, 30, 00);
            var p9FirstLoginDateTime = new DateTime(2022, 05, 17, 15, 30, 00);

            // Arrange
            await AddMetricHelper.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddConsentMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 11, 30, 00, TimeSpan.Zero), sessionId1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

            await AddMetricHelper.AddLoginMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 13, 30, 00, TimeSpan.Zero), sessionId2);
            await AddMetricHelper.AddConsentMetric(env, loginId2, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 14, 30, 00, TimeSpan.Zero), sessionId2);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 13, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId2);

            await AddMetricHelper.AddLoginMetric(env, loginId3, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 15, 30, 00, TimeSpan.Zero), sessionId3);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 15, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId3);
            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = loginId3,
                FirstP5LoginDate = p5FirstLoginDateTime.Date,
                FirstP9LoginDate = p9FirstLoginDateTime.Date,
                ConsentProofLevel = p9ProofLevel
            });

            // Act
            var response = await env.HttpEndpointCallers.PostReferrerLogin(startDateTime, endDateTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);

                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
                row.ReferrerId.Should().Be(referrerId);
                row.NewUsers.Should().Be(3);
                row.ExistingUsers.Should().Be(0);
            }
        }
    }
}
