extern alias r4;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.APIM;
using NHSOnline.Backend.PfsApi.Areas.SecondaryCare;
using NHSOnline.Backend.PfsApi.NHSApim;
using NHSOnline.Backend.PfsApi.NHSApim.Models;
using NHSOnline.Backend.PfsApi.SecondaryCare;
using NHSOnline.Backend.PfsApi.SecondaryCare.Mappers;
using NHSOnline.Backend.PfsApi.SecondaryCare.Models;
using NHSOnline.Backend.PfsApi.UnitTests.Extensions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Settings;
using r4::Hl7.Fhir.Model;
using RichardSzalay.MockHttp;
using UnitTestHelper;
using ServiceProvider = Microsoft.Extensions.DependencyInjection.ServiceProvider;
using WayfinderServiceProvider = NHSOnline.Backend.PfsApi.SecondaryCare.Models.ServiceProvider;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.SecondaryCare
{
    internal sealed class SecondaryCareControllerTestContext : IDisposable
    {
        internal TestMocks Mocks { get; }
        internal TestData Data { get; }
        private ServiceCollection ServiceCollection { get; }
        private ServiceProvider ServiceProvider { get; set; }

        private const string NhsNumber = "1111111111";
        private const string NhsLoginIdToken = "nhs-login-id-token";
        private const string ClientAssertion = "client-assertion";

        // APIM Auth
        private const string ApimBaseUrl = "http://stubs.local.bitraft.io:8080";
        private const string ApimOauthPath = "oauth2/token";
        private static string ApimOauthUrl => $"{ApimBaseUrl}/{ApimOauthPath}";

        private const string ApimCertPath = "test-path";
        private const string ApimCertPass = "test-phrase";
        private const string ApimKey = "key";
        private const string ApimKid = "kid";
        private static string ApimRequestContent =>
            $"subject_token={NhsLoginIdToken}&" +
            $"client_assertion={ClientAssertion}&" +
            "subject_token_type=urn%3Aietf%3Aparams%3Aoauth%3Atoken-type%3Aid_token&" +
            "client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer&" +
            "grant_type=urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Atoken-exchange";

        // Aggregator
        private const string SecondaryCareAggregatorBaseUrl = "http://stubs.local.bitraft.io:8080";
        private const string SecondaryCareAggregatorEventsPath = "patient-care-aggregator-api/aggregator/events";
        private static string SecondaryCareAggregatorEventsUrl => $"{SecondaryCareAggregatorBaseUrl}/{SecondaryCareAggregatorEventsPath}";

        // Aggregator Headers
        private const string OAuthAccessToken = "oauth-access-token";
        private const string NHSDTargetIdentifierHeaderValue =
            "ewrCoCDCoCAic3lzdGVtIjogInVybjppZXRmOnJmYzozOTg2IiwKwqAgwqAgInZh" +
            "bHVlIjogImRiNzE2OThiLWNkN2MtNGRkNS05NWM0LTBhYTk3NzY1OTVmNSIKfQ==";
        private static readonly Guid CorrelationId = Guid.Parse("64fc48ff-af19-43f9-a92c-9374970b7e85");
        private static readonly Guid RequestId = Guid.Parse("7fd33937-b9ea-4152-824e-88168ac29b48");

        public SecondaryCareControllerTestContext()
        {
            Data = new TestData();
            Mocks = new TestMocks();

            ServiceCollection = new ServiceCollection();

            InitializeServiceProvider();
        }

        private void InitializeServiceProvider()
        {
            new PfsApi.SecondaryCare.ServiceConfigurationModule().ConfigureServices(ServiceCollection, Mocks.Configuration.Object);
            new Support.ResponseParsers.ServiceConfigurationModule().ConfigureServices(ServiceCollection, Mocks.Configuration.Object);
            new Support.ServiceConfigurationModule().ConfigureServices(ServiceCollection, Mocks.Configuration.Object);
            new NHSApim.ServiceConfigurationModule().ConfigureServices(ServiceCollection, Mocks.Configuration.Object);

            Mocks.ConfigureServices(ServiceCollection);
            ConfigureHttpServices(ServiceCollection);

            ServiceCollection.AddTransient<SecondaryCareController>();

            ServiceProvider = ServiceCollection.BuildServiceProvider();
        }

        private void ConfigureHttpServices(ServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton(typeof(HttpTimeoutHandler<>))
                .AddSingleton(typeof(HttpRequestIdentificationHandler<>))
                .AddHttpClient<NhsApimHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<MockHttpMessageHandler>();

            serviceCollection
                .ReplacePrimaryHttpMessageHandler<SecondaryCareHttpClient, MockHttpMessageHandler>();
        }

        internal SecondaryCareController CreateSystemUnderTest() => ServiceProvider.GetRequiredService<SecondaryCareController>();

        internal void MockSecondaryCareHttpClientGetSummaryReturnsResponseWithData(
            HttpStatusCode httpStatusCode,
            string data)
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareAggregatorEventsUrl)
                .WithHeaders(Data.RequestHeaders)
                .Respond(httpStatusCode, "application/json", data);
        }

        internal void MockSecondaryCareHttpClientGetSummaryReturnsSuccessfulResponseWithData(string data)
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareAggregatorEventsUrl)
                .WithHeaders(Data.RequestHeaders)
                .Respond("application/json", data);
        }

        internal void MockSecondaryCareHttpClientGetSummaryReturnsUnsuccessfulResponse()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareAggregatorEventsUrl)
                .WithHeaders(Data.RequestHeaders)
                .Respond(HttpStatusCode.BadRequest);
        }

        internal void MockSecondaryCareHttpClientGetSummaryTimesOut()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareAggregatorEventsUrl)
                .WithHeaders(Data.RequestHeaders)
                .Throw(new OperationCanceledException());
        }

        internal sealed class TestData
        {
            public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                {"Authorization", $"Bearer {OAuthAccessToken}"},
                {"NHSD-Target-Identifier", NHSDTargetIdentifierHeaderValue},
                {"X-Correlation-Id", CorrelationId.ToString()},
                {"X-Request-Id", RequestId.ToString()}
            };

            public P9UserSession P9UserSession { get; } = new P9UserSession(
                "csrfToken",
                NhsNumber,
                new CitizenIdUserSession{ NhsLoginIdToken = NhsLoginIdToken },
                "im1ConnectionToken");

            public SummaryResponse SummaryResponse { get; } = new SummaryResponse
            {
                ReferralsNotInReview = new[]
                {
                    new Referral
                    {
                        ReferralId = "861710366336",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 17, 15, 11, 45, TimeSpan.Zero),
                        ReferrerOrganisation = "Willow GP Surgery",
                        ReviewDueDate = null,
                        ServiceSpecialty = "Paediatrics",
                        Status = ReferralStatus.bookable.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=861710366336",
                    },
                    new Referral
                    {
                        ReferralId = "530793722623",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 17, 15, 11, 45, TimeSpan.Zero),
                        ReferrerOrganisation = "Willow GP Surgery",
                        ReviewDueDate = null,
                        ServiceSpecialty = null,
                        Status = ReferralStatus.bookable.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=530793722623",
                    },
                    new Referral
                    {
                        ReferralId = "839416493852",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 24, 15, 11, 45, TimeSpan.Zero),
                        ReferrerOrganisation = "Birch GP Surgery",
                        ReviewDueDate = null,
                        ServiceSpecialty = "Neurology",
                        Status = ReferralStatus.bookableWasCancelled.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=839416493852",
                    },
                    new Referral
                    {
                        ReferralId = "879675036211",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 24, 15, 11, 45, TimeSpan.Zero),
                        ReferrerOrganisation = "Birch GP Surgery",
                        ReviewDueDate = null,
                        ServiceSpecialty = null,
                        Status = ReferralStatus.bookableWasCancelled.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=879675036211",
                    },
                },
                ReferralsInReview = new[]
                {
                    new Referral
                    {
                        ReferralId = "156168111459",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 25, 15, 11, 45, TimeSpan.Zero),
                        ReferrerOrganisation = "Willow GP Surgery",
                        ReviewDueDate = new DateTime(2022, 05, 08),
                        ServiceSpecialty = "Cardiology",
                        Status = ReferralStatus.inReview.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=156168111459",
                    },
                    new Referral
                    {
                        ReferralId = "628932202760",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 25, 15, 11, 45, TimeSpan.Zero),
                        ReferrerOrganisation = "Willow GP Surgery",
                        ReviewDueDate = new DateTime(2022, 05, 08),
                        ServiceSpecialty = null,
                        Status = ReferralStatus.inReview.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=628932202760",
                    },
                },
                ConfirmedAppointments = new[]
                {
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/upcoming-appointments?ubrn=217478550345",
                        AppointmentDateTime = new DateTimeOffset(2022, 05, 10, 15, 11, 45, TimeSpan.Zero),
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/upcoming-appointments?ubrn=927486036201",
                        AppointmentDateTime = new DateTimeOffset(2022, 05, 19, 15, 11, 45, TimeSpan.Zero),
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/upcoming-appointments?ubrn=681078755944",
                        AppointmentDateTime = new DateTimeOffset(2022, 05, 23, 15, 11, 45, TimeSpan.Zero),
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/upcoming-appointments?ubrn=681078755944",
                        AppointmentDateTime = new DateTimeOffset(2300, 05, 05, 15, 11, 45, TimeSpan.Zero),
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/upcoming-appointments?ubrn=681078755944",
                        AppointmentDateTime = new DateTimeOffset(2300, 05, 10, 15, 11, 45, TimeSpan.Zero),
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/upcoming-appointments?ubrn=681078755944",
                        AppointmentDateTime = new DateTimeOffset(2300, 05, 30, 15, 11, 45, TimeSpan.Zero),
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/upcoming-appointments?ubrn=681078755944",
                        AppointmentDateTime = new DateTimeOffset(2300, 06, 24, 15, 11, 45, TimeSpan.Zero),
                    },
                },
                UnconfirmedAppointments = new[]
                {
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/upcoming-appointments?ubrn=325308672657",
                        AppointmentDateTime = null,
                    },
                }
            };
        }

        internal sealed class TestMocks : IDisposable
        {
            internal Mock<IConfiguration> Configuration { get; }
                = new Mock<IConfiguration>();

            internal Mock<ILogger<SecondaryCareController>> ControllerLogger { get; }
                = new Mock<ILogger<SecondaryCareController>>();

            internal Mock<ILogger<SecondaryCareSummaryService>> ServiceLogger { get; }
                = new Mock<ILogger<SecondaryCareSummaryService>>();

            internal Mock<ILogger<SecondaryCareSummaryMapper>> SummaryMapperLogger { get; }
                = new Mock<ILogger<SecondaryCareSummaryMapper>>();
            internal Mock<IAuditor> Auditor { get; }
                = new Mock<IAuditor>();

            internal MockHttpMessageHandler MockHttpMessageHandler { get; }
                = new MockHttpMessageHandler();
            internal Mock<IHttpTimeoutConfigurationSettings> HttpTimeoutConfigurationSettings { get; }
                = new Mock<IHttpTimeoutConfigurationSettings>();

            internal Mock<IApimJwtHelper> ApimJwtHelper { get; }
                = new Mock<IApimJwtHelper>();

            internal Mock<IGuidCreator> GuidCreator { get; }
                = new Mock<IGuidCreator>();

            public TestMocks()
            {
                Configuration
                    .SetupGet(x => x["SECONDARY_CARE_AGGREGATOR_BASE_URL"])
                    .Returns(SecondaryCareAggregatorBaseUrl);

                Configuration
                    .SetupGet(x => x["SECONDARY_CARE_AGGREGATOR_EVENTS_PATH"])
                    .Returns(SecondaryCareAggregatorEventsPath);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_BASE_URL"])
                    .Returns(ApimBaseUrl);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_PFX"])
                    .Returns(ApimCertPath);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_PFX_PASSPHRASE"])
                    .Returns(ApimCertPass);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_KEY"])
                    .Returns(ApimKey);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_KID"])
                    .Returns(ApimKid);

                HttpTimeoutConfigurationSettings
                    .Setup(x => x.DefaultHttpTimeoutSeconds).Returns(10);

                GuidCreator
                    .SetupSequence(c => c.CreateGuid())
                    .Returns(CorrelationId)
                    .Returns(RequestId);

                ApimJwtHelper
                    .Setup(x => x.CreateApimJwt(
                        new Uri(ApimOauthUrl),
                        ApimCertPath,
                        ApimCertPass,
                        ApimKey,
                        ApimKid))
                    .Returns(ClientAssertion);

                MockHttpMessageHandler
                    .When(HttpMethod.Post, ApimOauthUrl)
                    .WithContent(ApimRequestContent)
                    .Respond("application/json", JsonConvert.SerializeObject(new ApimAccessToken
                    {
                        ExpiresIn = "123",
                        IssuedTokenType = "urn:ietf:params:oauth:token-type:access_token",
                        AccessToken = OAuthAccessToken,
                        TokenType = "Bearer"
                    }));
            }

            public void ConfigureServices(ServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(Auditor.Object)
                    .AddSingleton(ControllerLogger.Object)
                    .AddSingleton(ServiceLogger.Object)
                    .AddSingleton(SummaryMapperLogger.Object)
                    .AddSingleton(Configuration.Object)
                    .AddSingleton(HttpTimeoutConfigurationSettings.Object)
                    .AddSingleton(ApimJwtHelper.Object)
                    .AddSingleton(GuidCreator.Object)
                    .AddSingleton(MockHttpMessageHandler)
                    .AddMockLoggers();
            }

            public void Dispose() => MockHttpMessageHandler?.Dispose();
        }

        public void Dispose() => Mocks?.Dispose();
    }
}
