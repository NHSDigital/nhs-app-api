using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NHSOnline.App.Api.Client.Cookies;
using NHSOnline.App.Api.Session;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Areas.PreHome.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
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
        private readonly IInstallReferrer _installReferrer;
        private Uri? _deeplinkUrl;

        private Uri? ResolveDeeplinkUrl => _deeplinkUrl ?? _model.DeeplinkUrl;

        public CreateSessionPresenter(
            ICreateSessionView view,
            CreateSessionModel model,
            ILogger<CreateSessionPresenter> logger,
            IPageFactory pageFactory,
            ISessionService sessionService,
            IBackgroundExecutionService backgroundExecutionService,
            INhsAppWebConfiguration config,
            IInstallReferrer installReferrer)
        {
            _view = view;
            _model = model;
            _logger = logger;
            _pageFactory = pageFactory;
            _sessionService = sessionService;
            _backgroundExecutionService = backgroundExecutionService;
            _config = config;
            _installReferrer = installReferrer;

            view.AppNavigation
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler);

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
                .CreateSession(_model.AuthCode, _model.PkceCodes.Verifier, _installReferrer.Referrer, _model.RedirectUri, _model.IntegrationReferrer)
                .ResumeOnThreadPool();
        }

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.Created created)
            => await NavigateToPreHomeScreenPages(created.UserSession, created.Cookies).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.Failed failed)
            => await NavigateToFailedPage().PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.BadRequest badRequest)
            => await NavigateToBadRequestPage(badRequest.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.OdsCodeNotSupported odsCodeNotSupported)
            => await NavigateToOdsCodeNotSupportedPage(odsCodeNotSupported.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.OdsCodeNotFound odsCodeNotFound)
            => await NavigateToOdsCodeNotFoundPage(odsCodeNotFound.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.NoNhsNumber noNhsNumber)
            => await NavigateToNoNhsNumberPage(noNhsNumber.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.FailedAgeRequirement failedAgeRequirement)
            => await NavigateToFailedAgeRequirementPage().PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.BadResponseFromUpstreamSystem badResponseFromUpstreamSystem)
            => await NavigateToBadResponseFromUpstreamSystemPage(badResponseFromUpstreamSystem.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.UpstreamSystemTimeout upstreamSystemTimeout)
            => await NavigateToUpstreamSystemTimeoutPage(upstreamSystemTimeout.ServiceDeskReference).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.InternalServerError internalServerError)
            => await NavigateToInternalServerErrorPage(internalServerError.ServiceDeskReference).PreserveThreadContext();

        private async Task NavigateToPreHomeScreenPages(UserSession userSession, CookieJar cookies)
        {
            var sessionCookies = BuildSessionCookieContainer(userSession, cookies);
            var preHomeScreenModel = new NhsAppPreHomeScreenWebModel(sessionCookies, ResolveDeeplinkUrl);
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

        private async Task NavigateToOdsCodeNotSupportedPage(string serviceDeskReference)
        {
            var errorModel = _model.OdsCodeNotSupportedError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToOdsCodeNotFoundPage(string serviceDeskReference)
        {
            var errorModel = _model.OdsCodeNotFoundError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToNoNhsNumberPage(string serviceDeskReference)
        {
            var errorModel = _model.NoNhsNumberError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToFailedAgeRequirementPage()
        {
            var errorModel = _model.FailedAgeRequirementError();
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

        private async Task NavigateToInternalServerErrorPage(string serviceDeskReference)
        {
            var errorModel = _model.InternalServerError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.AppNavigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private CookieJar BuildSessionCookieContainer(UserSession userSession, CookieJar cookies)
        {
            cookies.Add(CreateNhsOnlineSessionCookie(_config.BaseAddress, userSession));
            return cookies;
        }

        private ApiCookie CreateNhsOnlineSessionCookie(Uri homeUri, UserSession userSession)
        {
            var sessionCookieJson = JsonConvert.SerializeObject(
                new NhsOnlineSessionCookie(userSession),
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            var sessionCookieEscaped = Uri.EscapeDataString(sessionCookieJson);

            _logger.LogTrace("Creating cookie {CookieName}: {CookieValue}",
                NhsOnlineSessionCookieName,
                sessionCookieEscaped);

            return new ApiCookie(homeUri,
                NhsOnlineSessionCookieName,
                sessionCookieEscaped,
                homeUri.Host,
                "/",
                false,
                _config.NhsOnlineSessionCookieSecure,
                SameSiteMode.Lax);
        }

        private Task DeeplinkRequested(Uri deeplinkUrl)
        {
            _deeplinkUrl = deeplinkUrl;
            return Task.CompletedTask;
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

            public string? User => _userSession.Name;
            public int DurationSeconds => _userSession.SessionTimeout;
            public string? GpOdsCode => _userSession.OdsCode;
            public string? CsrfToken => _userSession.Token;
            public string LastCalledAt => _userSession.LastCalledAt;
            public string? NhsNumber => _userSession.NhsNumber;
            public string? DateOfBirth => _userSession.DateOfBirth;
            public string? AccessToken => _userSession.AccessToken;
            public string? ProofLevel => _userSession.ProofLevel;
        }
    }
}