using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.GpSystems;
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
using NHSOnline.Backend.PfsApi.UnitTests.Audit;
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
        internal const string Name = "Name";
        internal const int SessionTimeoutMinutes = 10;
        internal const int SessionTimeoutSeconds = 600;
        internal const string ApiSessionId = "ApiSessionId";
        internal const string ServiceDeskReference = "ServiceDeskReference";
        internal const string AccessToken = "AccessToken";
        internal const string CsrfRequestToken = "dskhfakserhhvjcgbfdsh";

        internal Mock<ICitizenIdSessionService> MockCitizenIdSessionService { get; private set; }
        internal Mock<IGpSystem> MockGpSystem { get; private set; }
        internal Mock<IGpSystemFactory> MockGpSystemFactory { get; private set; }
        internal Mock<ILogger<SessionController>> MockSessionControllerLogger { get; private set; }
        internal Mock<IAuditor> MockAuditor { get; private set; }
        internal Mock<IIm1CacheService> MockIm1CacheService { get; private set; }
        internal Mock<IOdsCodeMassager> MockOdsCodeMassager { get; private set; }
        internal Mock<IServiceJourneyRulesService> MockServiceJourneyRulesService { get; private set; }
        internal Mock<IErrorReferenceGenerator> MockErrorReferenceGenerator { get; private set; }
        internal Mock<IGpSessionManager> MockGpSessionManager { get; private set; }
        internal Mock<HttpContext> MockHttpContext { get; private set; }
        internal Mock<IAntiforgery> MockAntiforgery { get; private set; }
        internal Mock<ISessionCacheService> MockSessionCacheService { get; private set; }
        internal Mock<IAuthenticationService> MockAuthenticationService { get; private set; }
        internal Mock<IMetricLogger> MockMetricLogger { get; private set; }

        internal EmisConnectionToken ConnectionToken { get; private set; }
        internal UserSessionRequest UserSessionRequest { get; private set; }
        internal Auth.CitizenId.Models.UserInfo UserInfo { get; private set; }
        internal UserProfile UserProfile { get; private set; }
        internal EmisUserSession EmisUserSession { get; private set; }
        internal CitizenIdUserSession CitizenIdUserSession { get; private set; }
        internal CitizenIdSessionResult CitizenIdSessionResult { get; private set; }
        internal ServiceJourneyRulesResponse ServiceJourneyRulesResponse { get; private set; }
        internal SessionConfigurationSettings SessionConfigSettings { get; private set; }
        internal P9UserSession UserSession { get; private set; }
        internal ServiceJourneyRulesConfigResult ServiceJourneyRulesConfigResult { get; private set; }
        internal ConfigurationSettings ConfigurationSettings { get; private set; }

        internal ServiceProvider ServiceProvider { get; private set; }

        public SessionControllerTestContext()
        {
            InitializeData();
            InitializeMocks();
            InitializeServiceProvider();
        }

        private void InitializeData()
        {
            ConnectionToken = new EmisConnectionToken();

            UserSessionRequest = new UserSessionRequest
            {
                AuthCode = "AuthCode",
                CodeVerifier = "CodeVerifier",
                RedirectUrl = "http://RedirectUrl/"
            };

            UserInfo = new Auth.CitizenId.Models.UserInfo
            {
                Birthdate = "1980-01-02",
                Im1ConnectionToken = ConnectionToken.SerializeJson(),
                NhsNumber = "012 345 6789",
                GpIntegrationCredentials = { OdsCode = "OdsCode" }
            };
            UserProfile = new UserProfile(UserInfo, AccessToken);

            EmisUserSession = new EmisUserSession
            {
                OdsCode = UserProfile.OdsCode,
                NhsNumber = UserProfile.NhsNumber,
                Name = Name
            };

            CitizenIdUserSession = new CitizenIdUserSession
            {
                AccessToken = AccessToken,
                ProofLevel = ProofLevel.P9,
                OdsCode = UserProfile.OdsCode
            };

            CitizenIdSessionResult = new CitizenIdSessionResult
            {
                StatusCode = (int) HttpStatusCode.OK,
                DateOfBirth = new DateTime(1980, 01, 02, 0, 0, 0, DateTimeKind.Utc),
                Im1ConnectionToken = UserProfile.Im1ConnectionToken,
                NhsNumber = UserProfile.NhsNumber,
                Session = CitizenIdUserSession
            };

            ServiceJourneyRulesResponse = new ServiceJourneyRulesResponse
                { Journeys = new Journeys { Supplier = Supplier.Emis } };
            ServiceJourneyRulesConfigResult = new ServiceJourneyRulesConfigResult.Success(ServiceJourneyRulesResponse);

            SessionConfigSettings = new SessionConfigurationSettings(false);

            UserSession = new P9UserSession(CsrfRequestToken, CitizenIdUserSession, EmisUserSession, string.Empty);

            ConfigurationSettings = new ConfigurationSettings
            {
                DefaultSessionExpiryMinutes = SessionTimeoutMinutes,
            };
        }

        private void InitializeMocks()
        {
            MockCitizenIdSessionService = new Mock<ICitizenIdSessionService>();
            MockCitizenIdSessionService
                .Setup(x => x.Create(UserSessionRequest.AuthCode, UserSessionRequest.CodeVerifier,
                    new Uri(UserSessionRequest.RedirectUrl)))
                .Returns(Task.FromResult(CitizenIdSessionResult));

            MockGpSystem = new Mock<IGpSystem>();
            MockGpSystem.SetupGet(x => x.Supplier).Returns(Supplier.Emis);

            MockGpSystemFactory = new Mock<IGpSystemFactory>();
            MockGpSystemFactory.Setup(x => x.CreateGpSystem(Supplier.Emis)).Returns(MockGpSystem.Object);

            MockSessionControllerLogger = new Mock<ILogger<SessionController>>();
            MockSessionControllerLogger.SetupLogger(LogLevel.Information, $"NhsNumber={UserProfile.NhsNumber}", null);

            MockAuditor = new Mock<IAuditor>();

            MockIm1CacheService = new Mock<IIm1CacheService>();
            MockIm1CacheService
                .Setup(x => x.DeleteIm1ConnectionToken(ConnectionToken.Im1CacheKey))
                .Returns(Task.FromResult(true)).Verifiable();

            MockOdsCodeMassager = new Mock<IOdsCodeMassager>();
            MockOdsCodeMassager.Setup(x => x.CheckOdsCode(UserProfile.OdsCode)).Returns(UserProfile.OdsCode);

            MockServiceJourneyRulesService = new Mock<IServiceJourneyRulesService>();
            MockServiceJourneyRulesService
                .Setup(x => x.GetServiceJourneyRulesForOdsCode(UserProfile.OdsCode))
                .Returns(Task.FromResult(ServiceJourneyRulesConfigResult));

            MockErrorReferenceGenerator = new Mock<IErrorReferenceGenerator>();

            MockGpSessionManager = new Mock<IGpSessionManager>();

            MockHttpContext = new Mock<HttpContext>();
            MockHttpContext.Setup(x => x.RequestServices).Returns(() => ServiceProvider);

            MockAntiforgery = new Mock<IAntiforgery>();
            MockAntiforgery
                .Setup(x => x.GetTokens(MockHttpContext.Object))
                .Returns(new AntiforgeryTokenSet(CsrfRequestToken, "cookieToken", "formFieldName", "headerName"));

            MockSessionCacheService = new Mock<ISessionCacheService>();
            MockSessionCacheService
                .Setup(x => x.CreateUserSession(It.IsAny<UserSession>()))
                .Callback<UserSession>(userSession => userSession.Key = ApiSessionId)
                .Returns(Task.FromResult(ApiSessionId));

            MockAuthenticationService = new Mock<IAuthenticationService>();
            MockAuthenticationService
                .Setup(x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object) null));

            MockMetricLogger = new Mock<IMetricLogger>();
        }

        private void InitializeServiceProvider()
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<SessionController>()
                .AddSingleton(MockCitizenIdSessionService.Object)
                .AddSingleton(MockGpSystemFactory.Object)
                .AddSingleton(ConfigurationSettings)
                .AddSingleton(MockSessionControllerLogger.Object)
                .AddSingleton(MockAuditor.Object)
                .AddSingleton(MockIm1CacheService.Object)
                .AddSingleton(MockOdsCodeMassager.Object)
                .AddSingleton(MockServiceJourneyRulesService.Object)
                .AddSingleton(MockErrorReferenceGenerator.Object)
                .AddSingleton(new Mock<IUserInfoService>().Object)
                .AddSingleton(MockGpSessionManager.Object)
                .AddSingleton(MockAntiforgery.Object)
                .AddSingleton(MockSessionCacheService.Object)
                .AddSingleton(MockAuthenticationService.Object)
                .AddSingleton(MockMetricLogger.Object)
                .AddMockLoggers();

            new PfsApi.Session.ServiceConfigurationModule().ConfigureServices(serviceCollection, null);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        internal SessionController CreateSystemUnderTest()
        {
            var systemUnderTest = ServiceProvider.GetRequiredService<SessionController>();

            systemUnderTest.ControllerContext = new ControllerContext
            {
                HttpContext = MockHttpContext.Object
            };

            return systemUnderTest;
        }

        internal AuditBuilderStub ArrangeAudit()
        {
            var auditBuilderStub = new AuditBuilderStub();
            MockAuditor.Setup(x => x.Audit()).Returns(auditBuilderStub);
            return auditBuilderStub;
        }
    }
}