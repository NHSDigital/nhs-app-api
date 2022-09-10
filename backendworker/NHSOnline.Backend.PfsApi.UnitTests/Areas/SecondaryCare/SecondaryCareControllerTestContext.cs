extern alias r4;

using System;
using System.Collections.Generic;
using System.Globalization;
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
                .AddSingleton(typeof(HttpRequestIdentificationHandler<>));

            serviceCollection
                .ReplacePrimaryHttpMessageHandler<SecondaryCareHttpClient, MockHttpMessageHandler>();

            serviceCollection
                .ReplacePrimaryHttpMessageHandler<NhsApimHttpClient, MockHttpMessageHandler>();
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

        internal void MockNhsApimHttpClientGetTokenReturnsUnsuccessfulResponse()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Post, ApimOauthUrl)
                .WithContent(ApimRequestContent)
                .Respond(HttpStatusCode.BadRequest);
        }

        internal void MockNhsApimHttpClientGetTokenReturnsSuccessfulResponseWithAuthToken()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Post, ApimOauthUrl)
                .WithContent(ApimRequestContent)
                .Respond("application/json", JsonConvert.SerializeObject(new ApimAccessToken
                    {
                        ExpiresIn = "123",
                        IssuedTokenType = "urn:ietf:params:oauth:token-type:access_token",
                        AccessToken = OAuthAccessToken,
                        TokenType = "Bearer"
                    }
                ));
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
                ActionableReferralsAndAppointments = new SecondaryCareSummaryItem[]
                {
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        LocationDescription = "SA Spine 2 Int Service DW - RL Req - 2WW -",
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/nhslogin?ubrn=000049631182",
                        AppointmentDateTime = null,
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "SA Spine 2 Int DBS - AB - RL Req -",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/nhslogin?ubrn=000049631181",
                        AppointmentDateTime = null,
                    },
                    new Referral
                    {
                        ReferralId = "861710366336",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 17, 15, 11, 40, TimeSpan.Zero),
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
                        ReviewDueDate = null,
                        ServiceSpecialty = null,
                        Status = ReferralStatus.bookable.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=530793722623",
                    },
                    new Referral
                    {
                        ReferralId = "839416493852",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 24, 15, 11, 40, TimeSpan.Zero),
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
                        ReviewDueDate = null,
                        ServiceSpecialty = null,
                        Status = ReferralStatus.bookableWasCancelled.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=879675036211",
                    },
                    new Referral
                    {
                        ReferralId = "156168111459",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 25, 15, 11, 45, TimeSpan.Zero),
                        ReviewDueDate = new DateTime(2022, 05, 08),
                        ServiceSpecialty = "Cardiology",
                        Status = ReferralStatus.inReview.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=156168111459",
                    },
                    new Referral
                    {
                        ReferralId = "628932202760",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 26, 15, 11, 45, TimeSpan.Zero),
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
                        AppointmentDateTime = DateTimeOffset.Parse("2022-08-26T09:00:00+01:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Unknown - NEWCASTLE, NORTH TYNESIDE AND NORTHUMBERLAND MENTAL HEALTH NHS TRUST",
                        Provider = WayfinderServiceProvider.DrDoctor.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/appointments/558205ef-cc53-444c-83bf-7fbcebb25c68?from=nhsApp"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-08-27T09:00:00+01:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Unknown - NEWCASTLE, NORTH TYNESIDE AND NORTHUMBERLAND MENTAL HEALTH NHS TRUST",
                        Provider = WayfinderServiceProvider.DrDoctor.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/appointments/360a266b-93aa-4e50-9d29-d17aae7e6961?from=nhsApp"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-08-28T09:00:00+01:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "NEUROSURGERY - NEWCASTLE, NORTH TYNESIDE AND NORTHUMBERLAND MENTAL HEALTH NHS TRUST",
                        Provider = WayfinderServiceProvider.DrDoctor.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/appointments/033279c3-225b-4e4f-a73e-9d077cd91c0c?from=nhsApp"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-08-29T09:00:00+01:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "NEUROSURGERY - NEWCASTLE, NORTH TYNESIDE AND NORTHUMBERLAND MENTAL HEALTH NHS TRUST",
                        Provider = WayfinderServiceProvider.DrDoctor.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/appointments/a41fd591-4d0d-4076-a6ab-e2500e9f691b?from=nhsApp"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-11-18T11:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Urology Appointment - HULL UNIVERSITY TEACHING HOSPITALS NHS TRUST",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D14571284-8fac-466b-aa8f-6648e8f6b992%26nhsStyle%3Dtrue"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-16T11:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Urology Appointment - HULL UNIVERSITY TEACHING HOSPITALS NHS TRUST",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3De9fd2f0c-dbf1-43f8-9c4f-a63dd9733006%26nhsStyle%3Dtrue"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-17T11:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Urology Appointment 2 - HULL UNIVERSITY TEACHING HOSPITALS NHS TRUST",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D9c252d68-014f-4a50-9809-819fb1edc648%26nhsStyle%3Dtrue"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-18T11:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Urology Appointment 2 - HULL UNIVERSITY TEACHING HOSPITALS NHS TRUST",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3Dd503318f-61ac-43f5-956a-67c04c17a04a%26nhsStyle%3Dtrue"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-03T09:30:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Adult Mental Illness - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/zesty/nhs/origin_appointment?resource_id=533a08c8-150d-4d74-9652-7413dc91d0df"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-10T09:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/zesty/nhs/origin_appointment?resource_id=bd3d370d-1154-46b0-8b1e-fb00070f2a7c"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-17T10:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/zesty/nhs/origin_appointment?resource_id=1d5cda93-1ada-4617-86dc-6c0740911b18"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-02-07T09:45:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Cardiac Rehabilitation - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/zesty/nhs/origin_appointment?resource_id=04fa3491-4862-4828-b388-eb4d814f2164"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-08-23T12:13:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Orthodontics appointment - Jarvis Building",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/Appointments?id=45014aca-87fa-4043-ae60-24eb7a16d7d1&trust=RFS"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-08-25T14:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Orthodontics follow up appointment - Jarvis Building",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/Appointments?id=b438a9f5-c875-4f09-8dde-c71941f150e6&trust=RFS"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-09-20T07:32:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Oral & Maxillo Facial Surgery appointment - Jarvis Building",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/Appointments?id=c89d21e6-0821-4738-afae-caf4efb50973&trust=RFS"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-09-25T14:35:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Oral & Maxillo Facial Surgery follow up appointment - Jarvis Building",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/Appointments?id=3bc662f0-658a-48e3-bdd9-a753533b678e&trust=RFS"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-03T09:30:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Adult Mental Illness - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Accurx.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/accurx/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken=533a08c8-150d-4d74-9652-7413dc91d0df"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-10T09:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Accurx.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/accurx/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken=bd3d370d-1154-46b0-8b1e-fb00070f2a7c"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-17T10:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Accurx.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/accurx/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken=1d5cda93-1ada-4617-86dc-6c0740911b18"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-02-07T09:45:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Cardiac Rehabilitation - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Accurx.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/accurx/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken=04fa3491-4862-4828-b388-eb4d814f2164"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-03T09:30:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Adult Mental Illness - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.HealthcareComms.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/hcc/appointments/350667f5-1ba7-4253-ae2e-fd42c8599cea"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-10T09:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.HealthcareComms.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/hcc/appointments/350667f5-1ba7-4253-ae2e-fd42c8599ceb"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-17T10:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.HealthcareComms.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/hcc/appointments/350667f5-1ba7-4253-ae2e-fd42c8599cec"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-02-07T09:45:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Cancelled.GetLiteral(),
                        LocationDescription = "Cardiac Rehabilitation - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.HealthcareComms.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/hcc/appointments/350667f5-1ba7-4253-ae2e-fd42c8599ced"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-08-21T09:00:00+01:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "NEUROSURGERY - NEWCASTLE, NORTH TYNESIDE AND NORTHUMBERLAND MENTAL HEALTH NHS TRUST",
                        Provider = WayfinderServiceProvider.DrDoctor.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/appointments/d1898c0b-72c7-406a-9998-c61ee4bf77f4?from=nhsApp"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-08-23T09:00:00+01:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "NEUROSURGERY - NEWCASTLE, NORTH TYNESIDE AND NORTHUMBERLAND MENTAL HEALTH NHS TRUST",
                        Provider = WayfinderServiceProvider.DrDoctor.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/appointments/d79811ef-475e-4100-8c68-c9457741f658?from=nhsApp"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-08-24T09:00:00+01:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Unknown - NEWCASTLE, NORTH TYNESIDE AND NORTHUMBERLAND MENTAL HEALTH NHS TRUST",
                        Provider = WayfinderServiceProvider.DrDoctor.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/appointments/c5c6225f-71ce-4f8e-b3b6-cd113636cf98?from=nhsApp"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-08-25T09:00:00+01:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Unknown - NEWCASTLE, NORTH TYNESIDE AND NORTHUMBERLAND MENTAL HEALTH NHS TRUST",
                        Provider = WayfinderServiceProvider.DrDoctor.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/drdoctor/appointments/df2db05e-4d27-4fcc-840c-f2665ca438c4?from=nhsApp"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-12T10:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Rheumatology Outpatient Appointment - HULL UNIVERSITY TEACHING HOSPITALS NHS TRUST",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D5b7082ba-5d0d-4963-ad62-96d9cadccab1%26nhsStyle%3Dtrue"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-13T10:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Rheumatology Outpatient Appointment - HULL UNIVERSITY TEACHING HOSPITALS NHS TRUST",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D912d5bc4-db15-4b82-bf52-8d34813f253b%26nhsStyle%3Dtrue"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-14T10:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Rheumatology Outpatient Appointment - HULL UNIVERSITY TEACHING HOSPITALS NHS TRUST",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D56c945e8-0b54-4581-900f-c97ff06be536%26nhsStyle%3Dtrue"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-15T11:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Rheumatology Outpatient Appointment - HULL UNIVERSITY TEACHING HOSPITALS NHS TRUST",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D0bb8b688-d580-46d6-a991-a4d64c237a9c%26nhsStyle%3Dtrue"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-15T09:45:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Accurx.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/accurx/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken=799bf452-84c7-4a13-a163-be19b43b8ae0"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-09T09:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Allergy Service - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Accurx.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/accurx/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken=666dbe5e-6805-49f4-a28b-3f85c57ceabf"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-15T10:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Accurx.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/accurx/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken=50096959-5fdc-493b-8f9e-bf5615f14895"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-02-04T10:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Diabetic Medicine - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Accurx.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/accurx/api/OpenIdConnect/AuthenticateManageAppointment?appointmentToken=5f641f44-a881-40e3-a416-e9c212df42e6"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-03-25T11:11:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Restorative Dentistry appointment - Jarvis Building",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/Appointments?id=62568da8-e25a-4bc3-af74-2981436e6360&trust=RFS"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-05-05T12:08:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Restorative Dentistry follow up appointment - Jarvis Building",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/Appointments?id=21b22563-8894-48f5-9aac-7669ecf92187&trust=RFS"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-06-17T09:42:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Paediatric Dentistry appointment - Jarvis Building",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/Appointments?id=ee186728-c9e0-4dd5-bdc2-63137f488ece&trust=RFS"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-06-29T10:45:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Paediatric Dentistry follow up appointment - Jarvis Building",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/Appointments?id=9bc44616-8270-4815-bf0e-9f92b23dc130&trust=RFS"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-15T09:45:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/zesty/nhs/origin_appointment?resource_id=799bf452-84c7-4a13-a163-be19b43b8ae0"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-09T09:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Allergy Service - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/zesty/nhs/origin_appointment?resource_id=666dbe5e-6805-49f4-a28b-3f85c57ceabf"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-15T10:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/zesty/nhs/origin_appointment?resource_id=50096959-5fdc-493b-8f9e-bf5615f14895"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-02-04T10:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Diabetic Medicine - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/zesty/nhs/origin_appointment?resource_id=5f641f44-a881-40e3-a416-e9c212df42e6"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2022-12-15T09:45:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.HealthcareComms.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/hcc/appointments/350667f5-1ba7-4253-ae2e-fd42c8599caa"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-09T09:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Allergy Service - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.HealthcareComms.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/hcc/appointments/350667f5-1ba7-4253-ae2e-fd42c8599cab"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-01-15T10:15:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "UNKNOWN-999 - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/hcc/appointments/350667f5-1ba7-4253-ae2e-fd42c8599cac"
                    },
                    new UpcomingAppointment
                    {
                        AppointmentDateTime = DateTimeOffset.Parse("2023-02-04T10:00:00+00:00", CultureInfo.CurrentCulture).UtcDateTime,
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.GetLiteral(),
                        LocationDescription = "Diabetic Medicine - MILTON KEYNES UNIVERSITY HOSPITAL NHS FOUNDATION TRUST",
                        Provider = WayfinderServiceProvider.Zesty.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/hcc/appointments/350667f5-1ba7-4253-ae2e-fd42c8599cad"
                    },
                },
                ReferralsInReviewNotOverdue = new []
                {
                    new Referral
                    {
                        ReferralId = "156168111460",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 25, 15, 11, 45, TimeSpan.Zero),
                        ReviewDueDate = new DateTime(2090, 05, 08),
                        ServiceSpecialty = "Cardiology",
                        Status = ReferralStatus.inReview.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=156168111460",
                    },
                    new Referral
                    {
                        ReferralId = "156168111461",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 04, 25, 00, 00, 00, TimeSpan.Zero),
                        ReviewDueDate = new DateTime(2090, 05, 07),
                        ServiceSpecialty = "Cardiology",
                        Status = ReferralStatus.inReview.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=156168111461",
                    },
                },
                AppointmentCount = 34,
                ReferralCount = 7,
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