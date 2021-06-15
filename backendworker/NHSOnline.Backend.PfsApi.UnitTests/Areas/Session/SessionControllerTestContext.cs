using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auditing.UnitTestsSupport;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    internal sealed class SessionControllerTestContext
    {
        internal TestMocks Mocks { get; }
        private TestData Data { get; }
        private ServiceProvider ServiceProvider { get; set; }

        public SessionControllerTestContext()
        {
            Data = new TestData();
            Mocks = new TestMocks();
            InitializeServiceProvider();
        }

        private void InitializeServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<SessionController>()
                .AddSingleton(Data.ConfigurationSettings);

            Mocks.ConfigureServices(serviceCollection);

            new PfsApi.Session.ServiceConfigurationModule().ConfigureServices(serviceCollection, null);
            new PfsApi.GpSession.ServiceConfigurationModule().ConfigureServices(serviceCollection, null);

            ServiceProvider = serviceCollection.BuildServiceProvider();
            Mocks.HttpContext.Setup(x => x.RequestServices).Returns(() => ServiceProvider);
        }

        internal SessionController CreateSystemUnderTest()
        {
            var systemUnderTest = ServiceProvider.GetRequiredService<SessionController>();

            systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = Mocks.HttpContext.Object
            };

            return systemUnderTest;
        }

        internal AuditBuilderStub ArrangeAudit()
        {
            var auditBuilderStub = new AuditBuilderStub();
            Mocks.Auditor.Setup(x => x.Audit()).Returns(auditBuilderStub);
            return auditBuilderStub;
        }

        private sealed class TestData
        {
            internal ConfigurationSettings ConfigurationSettings { get; }

            internal TestData()
            {
                ConfigurationSettings = new ConfigurationSettings
                {
                    DefaultSessionExpiryMinutes = 10
                };
            }
        }

        internal sealed class TestMocks
        {
            internal Mock<IAuditor> Auditor { get; } = new Mock<IAuditor>();
            internal Mock<IGpSessionManager> GpSessionManager { get; } = new Mock<IGpSessionManager>();
            internal Mock<HttpContext> HttpContext { get; } = new Mock<HttpContext>();
            internal Mock<ISessionCacheService> SessionCacheService { get; } = new Mock<ISessionCacheService>();

            public void ConfigureServices(IServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(Auditor.Object)
                    .AddSingleton(GpSessionManager.Object)
                    .AddSingleton(SessionCacheService.Object)
                    .AddSingleton(new Mock<ICitizenIdSessionService>().Object)
                    .AddSingleton(new Mock<IGpSystemFactory>().Object)
                    .AddSingleton(new Mock<ILogger<SessionController>>().Object)
                    .AddSingleton(new Mock<IIm1CacheService>().Object)
                    .AddSingleton(new Mock<IOdsCodeMassager>().Object)
                    .AddSingleton(new Mock<IServiceJourneyRulesService>().Object)
                    .AddSingleton(new Mock<IErrorReferenceGenerator>().Object)
                    .AddSingleton(new Mock<IUserInfoService>().Object)
                    .AddSingleton(new Mock<IAntiforgery>().Object)
                    .AddSingleton(new Mock<IAuthenticationService>().Object)
                    .AddSingleton(new Mock<IMetricLogger>().Object)
                    .AddSingleton(new Mock<ISigning>().Object)
                    .AddSingleton(new AuthSigningConfig(new Mock<IConfiguration>().Object, new Mock<ILogger<AuthSigningConfig>>().Object))
                    .AddSingleton(new Mock<ICurrentDateTimeProvider>().Object)
                    .AddSingleton(new Mock<IJwtTokenGenerator>().Object)
                    .AddSingleton(new Mock<IWebHostEnvironment>().Object)
                    .AddMockLoggers();
            }
        }
    }
}