extern alias r4;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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

        private static string SecondaryCareApiBaseUrl = "http://stubs.local.bitraft.io:8080/fhir/secondary-care/";
        private static readonly string SecondaryCareSummaryUrl = $"{SecondaryCareApiBaseUrl}summary/$evaluate";

        private const string ApimOathBaseUrl = "http://stubs.local.bitraft.io:8080/";
        private const string ApimOathUrl = "http://stubs.local.bitraft.io:8080/oauth2/token";
        private const string ApimCertPath = "testPath";
        private const string ApimCertPass = "testPhrase";
        private const string ApimKey = "key";
        private const string ApimKid = "kid";

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

        internal void MockSecondaryCareHttpClientGetSummaryReturnsSuccessfulResponseWithData(string data)
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareSummaryUrl)
                .WithHeaders(Data.RequestHeaders)
                .Respond("application/json", data);
        }

        internal void MockSecondaryCareHttpClientGetSummaryReturnsUnsuccessfulResponse()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareSummaryUrl)
                .WithHeaders(Data.RequestHeaders)
                .Respond(HttpStatusCode.BadRequest);
        }

        internal void MockSecondaryCareHttpClientGetSummaryTimesOut()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareSummaryUrl)
                .WithHeaders(Data.RequestHeaders)
                .Throw(new OperationCanceledException());
        }

        internal sealed class TestData
        {
            public static string OAuthAccessToken => "qwertyhgfdsaswedrfghgfds";
            private const string NhsNumber = "1111111111";

            public Dictionary<string, string> RequestHeaders { get; } = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                {"X-NHS-Number", NhsNumber},
                {"Authorization", $"Bearer {OAuthAccessToken}"}
            };

            public P9UserSession P9UserSession { get; } = new P9UserSession("csrfToken", NhsNumber, new CitizenIdUserSession(), "im1ConnectionToken");

            public SummaryResponse SummaryResponse { get; } = new SummaryResponse
            {
                Referrals = new[]
                {
                    new Referral
                    {
                        ReferralId = "521379702987",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 03, 08, 11, 48, 56, TimeSpan.Zero),
                        ReferrerOrganisation = "Birch GP Surgery",
                        ServiceSpecialty = "Neurology",
                        Status = ReferralStatus.Bookable.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=521379702987",
                    },
                    new Referral
                    {
                        ReferralId = "521379702986",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        ReferredDateTime = new DateTimeOffset(2022, 03, 09, 11, 48, 56, TimeSpan.Zero),
                        ReferrerOrganisation = "Willow GP Surgery",
                        ReviewDueDate = new DateTimeOffset(2022, 03, 12, 0, 0, 0, 0, TimeSpan.Zero),
                        ServiceSpecialty = "Cardiology",
                        Status = ReferralStatus.InReview.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/referrals?ubrn=521379702986",
                    },
                },
                UpcomingAppointments = new[]
                {
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.ToString(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.Netcall.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/netcall/upcoming-appointments?ubrn=276830555005",
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.ToString(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.eRS.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/ers/upcoming-appointments?ubrn=276830555004",
                        AppointmentDateTime = new DateTimeOffset(2022, 01, 08, 11, 48, 56, TimeSpan.Zero),
                    },
                    new UpcomingAppointment
                    {
                        AppointmentStatus = Appointment.AppointmentStatus.Booked.ToString(),
                        LocationDescription = "The Royal Victoria Hospital, Belfast, BT1",
                        Provider = WayfinderServiceProvider.PKB.ToString(),
                        DeepLinkUrl = "http://stubs.local.bitraft.io:8080/pkb/upcoming-appointments?ubrn=276830555003",
                        AppointmentDateTime = new DateTimeOffset(2022, 01, 09, 11, 48, 56, TimeSpan.Zero),
                    },
                },
            };
        }

        internal sealed class TestMocks : IDisposable
        {
            internal Mock<IConfiguration> Configuration { get; }
                = new Mock<IConfiguration>();

            internal Mock<ILogger<SecondaryCareController>> ControllerLogger { get; }
                = new Mock<ILogger<SecondaryCareController>>();

            internal Mock<ILogger<ISecondaryCareSummaryMapper>> SummaryMapperLogger { get; }
                = new Mock<ILogger<ISecondaryCareSummaryMapper>>();
            internal Mock<IAuditor> Auditor { get; }
                = new Mock<IAuditor>();

            internal MockHttpMessageHandler MockHttpMessageHandler { get; }
                = new MockHttpMessageHandler();
            internal Mock<IHttpTimeoutConfigurationSettings> HttpTimeoutConfigurationSettings { get; }
                = new Mock<IHttpTimeoutConfigurationSettings>();

            internal Mock<IApimJwtHelper> ApimJwtHelper { get; }
            = new Mock<IApimJwtHelper>();

            public TestMocks()
            {
                Configuration
                    .SetupGet(x => x["SECONDARY_CARE_BASE_URL"])
                    .Returns(SecondaryCareApiBaseUrl);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_BASE_URL"])
                    .Returns(ApimOathBaseUrl);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_PFX"])
                    .Returns(ApimOathBaseUrl);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_PFX_PASSPHRASE"])
                    .Returns(ApimOathBaseUrl);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_KEY"])
                    .Returns(ApimOathBaseUrl);

                Configuration
                    .SetupGet(x => x["NHSAPP_APIM_KID"])
                    .Returns(ApimOathBaseUrl);

                HttpTimeoutConfigurationSettings
                    .Setup(x => x.DefaultHttpTimeoutSeconds).Returns(10);

                ApimJwtHelper
                    .Setup(x => x.CreateApimJwt(
                        new Uri(ApimOathUrl),
                        ApimCertPath,
                        ApimCertPass,
                        ApimKey,
                        ApimKid))
                    .Returns("qwerthygfd");

                MockHttpMessageHandler
                    .When(HttpMethod.Post, ApimOathUrl)
                    .Respond("application/json", JsonConvert.SerializeObject(new ApimAccessToken
                    {
                        ExpiresIn = "123",
                        IssuedAt = "123",
                        AccessToken = TestData.OAuthAccessToken,
                        TokenType = "Bearer"
                    }));
            }

            public void ConfigureServices(ServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(Auditor.Object)
                    .AddSingleton(ControllerLogger.Object)
                    .AddSingleton(SummaryMapperLogger.Object)
                    .AddSingleton(Configuration.Object)
                    .AddSingleton(HttpTimeoutConfigurationSettings.Object)
                    .AddSingleton(ApimJwtHelper.Object)
                    .AddSingleton(MockHttpMessageHandler)
                    .AddMockLoggers();
            }

            public void Dispose() => MockHttpMessageHandler?.Dispose();
        }

        public void Dispose() => Mocks?.Dispose();
    }
}
