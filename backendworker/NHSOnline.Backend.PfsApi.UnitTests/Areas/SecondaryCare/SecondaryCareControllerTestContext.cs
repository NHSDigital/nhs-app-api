extern alias r4;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using CorrelationId;
using CorrelationId.Abstractions;
using CorrelationId.DependencyInjection;
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
using NHSOnline.Backend.PfsApi.UnitTests.Extensions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Settings;
using RichardSzalay.MockHttp;
using UnitTestHelper;
using ServiceProvider = Microsoft.Extensions.DependencyInjection.ServiceProvider;

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
        private const string SecondaryCareAggregatorWaitTimesPath = "patient-care-aggregator-api/aggregator/waittimes";
        private static string SecondaryCareAggregatorEventsUrl => $"{SecondaryCareAggregatorBaseUrl}/{SecondaryCareAggregatorEventsPath}";
        private static string SecondaryCareAggregatorWaitTimesUrl => $"{SecondaryCareAggregatorBaseUrl}/{SecondaryCareAggregatorWaitTimesPath}";

        // Aggregator Headers
        private const string OAuthAccessToken = "oauth-access-token";
        private const string NHSDTargetIdentifierHeaderValue =
            "ewrCoCDCoCAic3lzdGVtIjogInVybjppZXRmOnJmYzozOTg2IiwKwqAgwqAgInZh" +
            "bHVlIjogImRiNzE2OThiLWNkN2MtNGRkNS05NWM0LTBhYTk3NzY1OTVmNSIKfQ==";
        private static readonly Guid CorrelationId = Guid.Parse("64fc48ff-af19-43f9-a92c-9374970b7e85");
        private static readonly Guid CorrelationIdNotFromRequest = Guid.Parse("8b2d8fc1-2bc3-4421-9287-5dfe5734a3e7");
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
            ServiceCollection.AddDefaultCorrelationId();

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

        internal void MockSecondaryCareHttpClientGetSummaryCorrelationIdDoesNotMatch(string data)
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareAggregatorEventsUrl)
                .WithHeaders(Data.RequestHeaders)
                .WithHeaders(
                    Constants.SecondaryCareConstants.CorrelationIdHeaderKey,
                    CorrelationIdNotFromRequest.ToString())
                .Respond("application/json", data);
        }

        internal void MockSecondaryCareHttpClientGetWaitTimesReturnsSuccessfulResponseWithData(string data)
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareAggregatorWaitTimesUrl)
                .WithHeaders(Data.RequestHeaders)
                .Respond("application/json", data);
        }

        internal void MockNhsApimHttpClientGetTokenReturnsUnsuccessfulResponse()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Post, ApimOauthUrl)
                .WithContent(ApimRequestContent)
                .WithHeaders(Data.APIMRequestHeaders)
                .Respond(HttpStatusCode.BadRequest);
        }

        internal void MockNhsApimHttpClientGetTokenReturnsSuccessfulResponseWithAuthToken()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Post, ApimOauthUrl)
                .WithContent(ApimRequestContent)
                .WithHeaders(Data.APIMRequestHeaders)
                .Respond("application/json", JsonConvert.SerializeObject(new ApimAccessToken
                    {
                        ExpiresIn = "123",
                        IssuedTokenType = "urn:ietf:params:oauth:token-type:access_token",
                        AccessToken = OAuthAccessToken,
                        TokenType = "Bearer"
                    }
                ));
        }

        internal void MockNhsApimHttpClientGetTokenReturnsSuccessfulResponseButMismatchCorrelationId()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Post, ApimOauthUrl)
                .WithContent(ApimRequestContent)
                .WithHeaders(Data.APIMRequestHeaders)
                .WithHeaders(
                    Constants.SecondaryCareConstants.CorrelationIdHeaderKey,
                    CorrelationIdNotFromRequest.ToString())
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

        internal void MockSecondaryCareHttpClientGetWaitTimesReturnsUnsuccessfulResponse()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareAggregatorWaitTimesUrl)
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

        internal void MockSecondaryCareHttpClientGetWaitTimesTimesOut()
        {
            Mocks.MockHttpMessageHandler
                .When(HttpMethod.Get, SecondaryCareAggregatorWaitTimesUrl)
                .WithHeaders(Data.RequestHeaders)
                .Throw(new OperationCanceledException());
        }

        internal sealed class TestData
        {
            internal Dictionary<string, string> RequestHeaders { get; } =
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    { "Authorization", $"Bearer {OAuthAccessToken}" },
                    { "NHSD-Target-Identifier", NHSDTargetIdentifierHeaderValue },
                    { "X-Request-Id", RequestId.ToString() },
                    { Constants.SecondaryCareConstants.CorrelationIdHeaderKey, CorrelationId.ToString() }
                };

            public Dictionary<string, string> APIMRequestHeaders { get; } =
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    { Constants.SecondaryCareConstants.CorrelationIdHeaderKey, CorrelationId.ToString() }
                };

            public P9UserSession P9UserSession { get; } = new P9UserSession(
                "csrfToken",
                NhsNumber,
                new CitizenIdUserSession { NhsLoginIdToken = NhsLoginIdToken },
                "im1ConnectionToken");
        }

        internal sealed class TestMocks : IDisposable
        {
            internal Mock<IConfiguration> Configuration { get; }
                = new Mock<IConfiguration>();

            internal Mock<ILogger<SecondaryCareController>> ControllerLogger { get; }
                = new Mock<ILogger<SecondaryCareController>>();

            internal Mock<ILogger<SecondaryCareSummaryService>> ServiceLogger { get; }
                = new Mock<ILogger<SecondaryCareSummaryService>>();

            internal Mock<ILogger<SecondaryCareWaitTimesService>> WaitTimesServiceLogger { get; }
                = new Mock<ILogger<SecondaryCareWaitTimesService>>();

            internal Mock<ILogger<SecondaryCareSummaryMapper>> SummaryMapperLogger { get; }
                = new Mock<ILogger<SecondaryCareSummaryMapper>>();

            internal Mock<ILogger<SecondaryCareWaitTimesMapper>> WaitTimesMapperLogger { get; }
                = new Mock<ILogger<SecondaryCareWaitTimesMapper>>();

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

            internal Mock<ICorrelationContextAccessor> CorrelationIdContextAccessor { get; }
                = new Mock<ICorrelationContextAccessor>();

            public TestMocks()
            {
                Configuration
                    .SetupGet(x => x["SECONDARY_CARE_AGGREGATOR_BASE_URL"])
                    .Returns(SecondaryCareAggregatorBaseUrl);

                Configuration
                    .SetupGet(x => x["SECONDARY_CARE_AGGREGATOR_EVENTS_PATH"])
                    .Returns(SecondaryCareAggregatorEventsPath);

                Configuration
                    .SetupGet(x => x["SECONDARY_CARE_AGGREGATOR_WAIT_TIMES_PATH"])
                    .Returns(SecondaryCareAggregatorWaitTimesPath);

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

                Configuration
                    .SetupGet(x => x["SECONDARY_CARE_WAIT_TIMES_ENABLED"])
                    .Returns("true");

                HttpTimeoutConfigurationSettings
                    .Setup(x => x.DefaultHttpTimeoutSeconds).Returns(10);

                GuidCreator
                    .SetupSequence(c => c.CreateGuid())
                    .Returns(RequestId);

                CorrelationIdContextAccessor
                    .Setup(x => x.CorrelationContext).Returns(
                    new CorrelationContext(
                        CorrelationId.ToString(),
                        Constants.SecondaryCareConstants.CorrelationIdHeaderKey));

                ApimJwtHelper
                    .Setup(x => x.CreateApimJwt(
                        new Uri(ApimOauthUrl),
                        ApimCertPath,
                        ApimCertPass,
                        ApimKey,
                        ApimKid))
                    .Returns(ClientAssertion);
            }

            public void UpdateConfigurationKeyValue(string key, string value)
            {
                Configuration
                    .SetupGet(x => x[key])
                    .Returns(value);
            }

            public void ConfigureServices(ServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(Auditor.Object)
                    .AddSingleton(ControllerLogger.Object)
                    .AddSingleton(ServiceLogger.Object)
                    .AddSingleton(WaitTimesServiceLogger.Object)
                    .AddSingleton(SummaryMapperLogger.Object)
                    .AddSingleton(WaitTimesMapperLogger.Object)
                    .AddSingleton(Configuration.Object)
                    .AddSingleton(HttpTimeoutConfigurationSettings.Object)
                    .AddSingleton(ApimJwtHelper.Object)
                    .AddSingleton(GuidCreator.Object)
                    .AddSingleton(MockHttpMessageHandler)
                    .AddSingleton(CorrelationIdContextAccessor.Object)
                    .AddMockLoggers();
            }

            public void Dispose() => MockHttpMessageHandler?.Dispose();
        }

        public void Dispose() => Mocks?.Dispose();
    }
}