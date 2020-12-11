using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auditing.UnitTestsSupport;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Im1Connection.Cache;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Emis;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.PfsApi.Areas.Session;
using NHSOnline.Backend.PfsApi.Areas.Session.Models;
using NHSOnline.Backend.PfsApi.CitizenId;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.PfsApi.UserInfo;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;
using NHSOnline.Backend.Support.Settings;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests.Areas.Session
{
    internal sealed class SessionControllerTestContext
    {
        internal const int SessionTimeoutSeconds = 600;
        internal const string ServiceDeskReference = "ServiceDeskReference";
        internal const string CsrfRequestToken = "dskhfakserhhvjcgbfdsh";

        private const int SessionTimeoutMinutes = 10;
        private const string ApiSessionId = "ApiSessionId";
        private const string AccessToken = "AccessToken";
        private const string RefreshToken = "RefreshToken";

        internal TestMocks Mocks { get; }
        internal TestData Data { get; }
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
            Mocks.HttpContext.Setup(x => x.Request.Headers)
                .Returns(new HeaderDictionary{ { "User-Agent", "userAgent" } });
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

        internal void ArrangeAntiforgery()
        {
            Mocks.Antiforgery
                .Setup(x => x.GetTokens(Mocks.HttpContext.Object))
                .Returns(new AntiforgeryTokenSet(CsrfRequestToken, "cookieToken", "formFieldName", "headerName"));
        }

        internal void ArrangeCitizenIdService()
        {
            Mocks.CitizenIdSessionService
                .Setup(x => x.Create(
                    Data.UserSessionRequest.AuthCode,
                    Data.UserSessionRequest.CodeVerifier,
                    new Uri(Data.UserSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(Data.CitizenIdSessionResult));
        }

        internal void ArrangeOdsCodeMassager()
        {
            Mocks.OdsCodeMassager
                .Setup(x => x.CheckOdsCode(Data.UserProfile.OdsCode))
                .Returns(Data.UserProfile.OdsCode);
        }

        internal void ArrangeGpSystemFactory()
        {
            Mocks.GpSystemFactory.Setup(x => x.CreateGpSystem(Supplier.Emis)).Returns(Mocks.GpSystem.Object);
        }

        internal void ArrangeSessionCacheService()
        {
            Mocks.SessionCacheService
                .Setup(x => x.CreateUserSession(It.IsAny<UserSession>()))
                .Callback<UserSession>(userSession => userSession.Key = ApiSessionId)
                .Returns(Task.FromResult(ApiSessionId));
        }

        internal void ArrangeServiceJourneyRulesService()
        {
            Mocks.ServiceJourneyRulesService
                .Setup(x => x.GetServiceJourneyRulesForOdsCode(Data.UserProfile.OdsCode))
                .Returns(Task.FromResult(Data.ServiceJourneyRulesConfigResult));
        }

        internal sealed class TestData
        {
            internal EmisConnectionToken ConnectionToken { get; } = new EmisConnectionToken();

            internal UserSessionRequest UserSessionRequest { get; } = new UserSessionRequest
            {
                AuthCode = "AuthCode",
                CodeVerifier = "CodeVerifier",
                RedirectUrl = "http://RedirectUrl/"
            };

            internal Auth.CitizenId.Models.UserInfo UserInfo { get; }
            internal UserProfile UserProfile { get; }
            internal EmisUserSession EmisUserSession { get; }
            internal CitizenIdUserSession CitizenIdUserSession { get; }
            internal CitizenIdSessionResult CitizenIdSessionResult { get; }
            internal ServiceJourneyRulesResponse ServiceJourneyRulesResponse { get; }
            internal SessionConfigurationSettings SessionConfigSettings { get; }
            internal P9UserSession UserSession { get; }
            internal ServiceJourneyRulesConfigResult ServiceJourneyRulesConfigResult { get; }
            internal ConfigurationSettings ConfigurationSettings { get; }

            internal TestData()
            {
                UserInfo = new Auth.CitizenId.Models.UserInfo
                {
                    Birthdate = "1980-01-02",
                    Im1ConnectionToken = ConnectionToken.SerializeJson(),
                    NhsNumber = "012 345 6789",
                    GivenName = "Given",
                    FamilyName = "Family",
                    GpIntegrationCredentials = { OdsCode = "OdsCode" }
                };
                UserProfile = new UserProfile(UserInfo, AccessToken, RefreshToken);

                EmisUserSession = new EmisUserSession
                {
                    OdsCode = UserProfile.OdsCode,
                    NhsNumber = UserProfile.NhsNumber,
                    Name = $"{UserProfile.GivenName}{UserProfile.FamilyName}"
                };

                CitizenIdUserSession = new CitizenIdUserSession
                {
                    AccessToken = AccessToken,
                    ProofLevel = ProofLevel.P9,
                    OdsCode = UserProfile.OdsCode,
                    RefreshToken = RefreshToken,
                    GivenName = "Given",
                    FamilyName = "Family"
                };

                CitizenIdSessionResult = new CitizenIdSessionResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    DateOfBirth = new DateTime(1980, 01, 02, 0, 0, 0, DateTimeKind.Utc),
                    Im1ConnectionToken = UserProfile.Im1ConnectionToken,
                    NhsNumber = UserProfile.NhsNumber,
                    Session = CitizenIdUserSession
                };

                ServiceJourneyRulesResponse = new ServiceJourneyRulesResponse { Journeys = new Journeys { Supplier = Supplier.Emis } };
                ServiceJourneyRulesConfigResult = new ServiceJourneyRulesConfigResult.Success(ServiceJourneyRulesResponse);

                SessionConfigSettings = new SessionConfigurationSettings(false);

                UserSession = new P9UserSession(CsrfRequestToken, "nhsNumber", CitizenIdUserSession, EmisUserSession, string.Empty);

                ConfigurationSettings = new ConfigurationSettings
                {
                    DefaultSessionExpiryMinutes = SessionTimeoutMinutes,
                };
            }
        }

        internal sealed class TestMocks
        {
            internal Mock<ICitizenIdSessionService> CitizenIdSessionService { get; } = new Mock<ICitizenIdSessionService>();
            internal Mock<IGpSystem> GpSystem { get; } = new Mock<IGpSystem>();
            internal Mock<IGpSystemFactory> GpSystemFactory { get; } = new Mock<IGpSystemFactory>();
            internal Mock<ILogger<SessionController>> SessionControllerLogger { get; } = new Mock<ILogger<SessionController>>();
            internal Mock<IAuditor> Auditor { get; } = new Mock<IAuditor>();
            internal Mock<IIm1CacheService> Im1CacheService { get; } = new Mock<IIm1CacheService>();
            internal Mock<IOdsCodeMassager> OdsCodeMassager { get; } = new Mock<IOdsCodeMassager>();
            internal Mock<IServiceJourneyRulesService> ServiceJourneyRulesService { get; } = new Mock<IServiceJourneyRulesService>();
            internal Mock<IErrorReferenceGenerator> ErrorReferenceGenerator { get; } = new Mock<IErrorReferenceGenerator>();
            internal Mock<IGpSessionManager> GpSessionManager { get; } = new Mock<IGpSessionManager>();
            internal Mock<HttpContext> HttpContext { get; } = new Mock<HttpContext>();
            internal Mock<IAntiforgery> Antiforgery { get; } = new Mock<IAntiforgery>();
            internal Mock<ISessionCacheService> SessionCacheService { get; } = new Mock<ISessionCacheService>();
            internal Mock<IAuthenticationService> AuthenticationService { get; } = new Mock<IAuthenticationService>();
            internal Mock<IMetricLogger> MetricLogger { get; } = new Mock<IMetricLogger>();

            public void ConfigureServices(IServiceCollection serviceCollection)
            {
                serviceCollection
                    .AddSingleton(CitizenIdSessionService.Object)
                    .AddSingleton(GpSystemFactory.Object)
                    .AddSingleton(SessionControllerLogger.Object)
                    .AddSingleton(Auditor.Object)
                    .AddSingleton(Im1CacheService.Object)
                    .AddSingleton(OdsCodeMassager.Object)
                    .AddSingleton(ServiceJourneyRulesService.Object)
                    .AddSingleton(ErrorReferenceGenerator.Object)
                    .AddSingleton(new Mock<IUserInfoService>().Object)
                    .AddSingleton(GpSessionManager.Object)
                    .AddSingleton(Antiforgery.Object)
                    .AddSingleton(SessionCacheService.Object)
                    .AddSingleton(AuthenticationService.Object)
                    .AddSingleton(MetricLogger.Object)
                    .AddMockLoggers();
            }
        }
    }
}