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

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    internal class SessionCreateResultVisitorTestContext
    {
        internal const string CookieDomain = "cookie.domain";

        internal TestMocks Mocks { get; }
        internal TestData Data { get; }
        private ServiceProvider ServiceProvider { get; set; }

        public SessionCreateResultVisitorTestContext()
        {
            Mocks = new TestMocks();
            Data = new TestData();
            InitializeServiceProvider();
        }

        private void InitializeServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(Data.ConfigurationSettings)
                .AddTransient<UserSessionService>()
                .AddTransient<ISessionExpiryCookieCreator, SessionExpiryCookieCreator>()
                .AddTransient<SessionCreateResultVisitor>();

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

        internal SessionCreateResultVisitor CreateSystemUnderTest() =>
            ServiceProvider.GetRequiredService<SessionCreateResultVisitor>();

        internal class TestData
        {
            internal CreateSessionResult.Success SuccessResult { get; }
            internal ConfigurationSettings ConfigurationSettings { get; }

            internal TestData()
            {
                var odsCode = "A123456";
                var nhsNumber = "012 345 6789";

                var emisUserSession = new EmisUserSession
                {
                    OdsCode = odsCode,
                    NhsNumber = nhsNumber,
                    Name = "Given Family"
                };

                var citizenIdUserSession = new CitizenIdUserSession
                {
                    AccessToken = "AccessToken",
                    ProofLevel = ProofLevel.P9,
                    OdsCode = odsCode,
                    RefreshToken = "RefreshToken",
                    GivenName = "Given",
                    FamilyName = "Family"
                };
                var serviceJourneyRulesResponse = new ServiceJourneyRulesResponse { Journeys = new Journeys { Supplier = Supplier.Emis } };
                var userSession = new P9UserSession("CsrfRequestToken", nhsNumber,
                    citizenIdUserSession, emisUserSession, "im1ConnectionToken")
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

            public void ConfigureServices(IServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(new Mock<ISigning>().Object)
                    .AddSingleton(new Mock<ICurrentDateTimeProvider>().Object)
                    .AddSingleton(new Mock<IJwtTokenGenerator>().Object)
                    .AddSingleton(new Mock<IWebHostEnvironment>().Object)
                    .AddSingleton(new Mock<IMetricLogger>().Object)
                    .AddSingleton(new Mock<ISessionErrorResultBuilder>().Object)
                    .AddSingleton(new Mock<IAuthenticationService>().Object)
                    .AddSingleton(new AuthSigningConfig(new Mock<IConfiguration>().Object, new Mock<ILogger<AuthSigningConfig>>().Object))
                    .AddMockLoggers();
            }
        }
    }
}