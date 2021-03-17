using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Api.Session;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Areas.PreHome.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionPresenter: ICreateSessionResultVisitor<Task>
    {
        private const string NhsOnlineSessionCookieName = "nhso.session";

        private readonly ICreateSessionView _view;
        private readonly CreateSessionModel _model;
        private readonly ILogger<CreateSessionPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly ISessionService _sessionService;
        private readonly IBackgroundExecutionService _backgroundExecutionService;
        private readonly INhsAppWebConfiguration _config;

        public CreateSessionPresenter(
            ICreateSessionView view,
            CreateSessionModel model,
            ILogger<CreateSessionPresenter> logger,
            IPageFactory pageFactory,
            ISessionService sessionService,
            IBackgroundExecutionService backgroundExecutionService,
            INhsAppWebConfiguration config)
        {
            _view = view;
            _model = model;
            _logger = logger;
            _pageFactory = pageFactory;
            _sessionService = sessionService;
            _backgroundExecutionService = backgroundExecutionService;
            _config = config;

            CreateSession();
        }

        private async void CreateSession()
        {
            try
            {
                var createSessionResult = await _backgroundExecutionService.Run(TryCreateSession).PreserveThreadContext();

                await createSessionResult.Accept(this).PreserveThreadContext();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to create session");
                await NavigateToFailedPage().PreserveThreadContext();
            }
        }

        private async Task<CreateSessionResult> TryCreateSession()
        {
            return await _sessionService
                .CreateSession(_model.AuthCode, _model.PkceCodes.Verifier, _model.RedirectUri)
                .ResumeOnThreadPool();
        }

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.Created created)
            => await NavigateToPreHomeScreenPages(created.UserSession, created.Cookies).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.Failed failed)
            => await NavigateToFailedPage().PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.BadRequest badRequest)
            => await NavigateToBadRequestPage(badRequest.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.OdsCodeNotSupportedOrNoNhsNumber odsCodeNotSupportedOrNoNhsNumber)
            => await NavigateToOdsCodeNotSupportedOrNoNhsNumberPage(odsCodeNotSupportedOrNoNhsNumber.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.FailedAgeRequirement failedAgeRequirement)
            => await NavigateToFailedAgeRequirementPage(failedAgeRequirement.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.BadResponseFromUpstreamSystem badResponseFromUpstreamSystem)
            => await NavigateToBadResponseFromUpstreamSystemPage(badResponseFromUpstreamSystem.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.UpstreamSystemTimeout upstreamSystemTimeout)
            => await NavigateToUpstreamSystemTimeoutPage(upstreamSystemTimeout.ServiceDeskReference).PreserveThreadContext();

        private async Task NavigateToPreHomeScreenPages(UserSession userSession, CookieContainer cookies)
        {
            var sessionCookies = BuildSessionCookieContainer(userSession, cookies);
            var preHomeScreenModel = new NhsAppPreHomeScreenWebModel(sessionCookies);
            var preHomeScreenPage = _pageFactory.CreatePageFor(preHomeScreenModel);

            await _view.AppNavigation.PopToNewRoot(preHomeScreenPage).PreserveThreadContext();
        }

        private async Task NavigateToFailedPage()
        {
            var errorModel = _model.FallbackError();
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToBadRequestPage(string serviceDeskReference)
        {
            var errorModel = _model.BadRequestError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToOdsCodeNotSupportedOrNoNhsNumberPage(string serviceDeskReference)
        {
            var errorModel = _model.OdsCodeNotSupportedOrNoNhsNumberError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToFailedAgeRequirementPage(string serviceDeskReference)
        {
            var errorModel = _model.FailedAgeRequirementError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToBadResponseFromUpstreamSystemPage(string serviceDeskReference)
        {
            var errorModel = _model.BadResponseFromUpstreamSystemError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToUpstreamSystemTimeoutPage(string serviceDeskReference)
        {
            var errorModel = _model.UpstreamSystemTimeoutError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private CookieContainer BuildSessionCookieContainer(UserSession userSession, CookieContainer cookies)
        {
            cookies.Add(CreateNhsOnlineSessionCookie(_config.BaseAddress, userSession));
            return cookies;
        }

        private Cookie CreateNhsOnlineSessionCookie(Uri homeUri, UserSession userSession)
        {
            var sessionCookieJson = JsonConvert.SerializeObject(
                new NhsOnlineSessionCookie(userSession),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            var sessionCookieEscaped = Uri.EscapeDataString(sessionCookieJson);

            _logger.LogTrace("Creating cookie {CookieName}: {CookieValue}", NhsOnlineSessionCookieName, sessionCookieEscaped);

            return new Cookie(NhsOnlineSessionCookieName, sessionCookieEscaped, "/", homeUri.Host)
            {

                Secure = _config.NhsOnlineSessionCookieSecure,
                HttpOnly = false
            };
        }

        // Shim to convert the user session object from the API to the slightly
        // different structure the Web stores the same information in its cookie
        private sealed class NhsOnlineSessionCookie
        {
            private readonly UserSession _userSession;

            public NhsOnlineSessionCookie(UserSession userSession)
            {
                _userSession = userSession;
            }

            public string? Name => _userSession.Name;
            public int DurationSeconds => _userSession.SessionTimeout;
            public string? GpOdsCode => _userSession.OdsCode;
            public string? Token => _userSession.Token;
            public string LastCalledAt => _userSession.LastCalledAt;
            public string? NhsNumber => _userSession.NhsNumber;
            public string? DateOfBirth => _userSession.DateOfBirth;
            public string? AccessToken => _userSession.AccessToken;
            public string? ProofLevel => _userSession.ProofLevel;
        }
    }
}