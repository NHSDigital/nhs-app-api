using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    internal class SessionResultVisitorTestContext
    {
        internal const string CookieDomain = "cookie.domain";

        internal TestMocks Mocks { get; }
        internal TestData Data { get; }
        private ServiceProvider ServiceProvider { get; set; }

        public SessionResultVisitorTestContext(ProofLevel proofLevel = ProofLevel.P9, string nhsNumber = "012 345 6789")
        {
            Mocks = new TestMocks();
            Data = new TestData(proofLevel, nhsNumber);
            InitializeServiceProvider();
        }

        private void InitializeServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(Data.ConfigurationSettings)
                .AddTransient<UserSessionService>()
                .AddTransient<ISessionExpiryCookieCreator, SessionExpiryCookieCreator>()
                .AddTransient<SessionResultVisitor>();

            Mocks.ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
            ArrangeHttpContext();
        }

        private void ArrangeHttpContext()
        {
            Mocks.HttpContext.Setup(x => x.RequestServices).Returns(() => ServiceProvider);
            Mocks.HttpContext.Setup(x => x.Response.Cookies).Returns(Mocks.ResponseCookies.Object);
            Mocks.HttpContext.SetupGet(x => x.TraceIdentifier).Returns("RequestId");
            Mocks.HttpContext.Setup(x => x.Request.Headers).Returns(new HeaderDictionary { { "User-Agent", "userAgent" } });
        }

        internal SessionResultVisitor CreateSystemUnderTest() =>
            ServiceProvider.GetRequiredService<SessionResultVisitor>();

        internal class TestData
        {
            internal CreateSessionResult.Success SuccessResult { get; }
            internal ConfigurationSettings ConfigurationSettings { get; }

            internal TestData(ProofLevel proofLevel = ProofLevel.P9, string nhsNumber = "012 345 6789")
            {
                var odsCode = "A123456";

                var emisUserSession = new EmisUserSession
                {
                    OdsCode = odsCode,
                    NhsNumber = nhsNumber,
                    Name = "Given Family"
                };

                var citizenIdUserSession = new CitizenIdUserSession
                {
                    AccessToken = "AccessToken",
                    ProofLevel = proofLevel,
                    OdsCode = odsCode,
                    RefreshToken = "RefreshToken",
                    GivenName = "Given",
                    FamilyName = "Family"
                };
                var serviceJourneyRulesResponse = new ServiceJourneyRulesResponse { Journeys = new Journeys { Supplier = Supplier.Emis } };
                var userSession = new P9UserSession("CsrfRequestToken", nhsNumber,
                    citizenIdUserSession, "im1ConnectionToken", emisUserSession)
                {
                    Key = "ApiSessionId"
                };

                SuccessResult = new CreateSessionResult.Success(serviceJourneyRulesResponse, userSession);

                ConfigurationSettings = new ConfigurationSettings
                {
                    CookieDomain = CookieDomain,
                };
            }
        }

        internal class TestMocks
        {
            internal Mock<HttpContext> HttpContext { get; } = new Mock<HttpContext>();
            internal Mock<IResponseCookies> ResponseCookies { get; } = new Mock<IResponseCookies>();
            internal Mock<IAuditor> Auditor { get; } = new Mock<IAuditor>();

            public void ConfigureServices(IServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(new Mock<ISigning>().Object)
                    .AddSingleton(new Mock<ICurrentDateTimeProvider>().Object)
                    .AddSingleton(new Mock<IJwtTokenGenerator>().Object)
                    .AddSingleton(new Mock<IWebHostEnvironment>().Object)
                    .AddSingleton(new Mock<IMetricLogger<UserSessionMetricContext>>().Object)
                    .AddSingleton(new Mock<ISessionErrorResultBuilder>().Object)
                    .AddSingleton(new Mock<IAuthenticationService>().Object)
                    .AddSingleton(new AuthSigningConfig(new Mock<IConfiguration>().Object, new Mock<ILogger<AuthSigningConfig>>().Object))
                    .AddSingleton(Auditor.Object)
                    .AddMockLoggers();
            }
        }
    }
}