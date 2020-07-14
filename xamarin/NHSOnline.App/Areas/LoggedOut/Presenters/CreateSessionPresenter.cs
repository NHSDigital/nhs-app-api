using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Api.Session;
using NHSOnline.App.Areas.Home.Models;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionPresenter: ICreateSessionResultVisitor<Task>
    {
        private readonly ICreateSessionView _view;
        private readonly CreateSessionModel _model;
        private readonly ILogger<CreateSessionPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly ISessionService _sessionService;
        private readonly IBackgroundExecutionService _backgroundExecutionService;

        public CreateSessionPresenter(
            ICreateSessionView view,
            CreateSessionModel model,
            ILogger<CreateSessionPresenter> logger,
            IPageFactory pageFactory,
            ISessionService sessionService,
            IBackgroundExecutionService backgroundExecutionService)
        {
            _view = view;
            _model = model;
            _logger = logger;
            _pageFactory = pageFactory;
            _sessionService = sessionService;
            _backgroundExecutionService = backgroundExecutionService;

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
            => await NavigateToLoggedInHomePage(created.UserSession, created.Cookies).PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.Failed failed)
            => await NavigateToFailedPage().PreserveThreadContext();

        async Task ICreateSessionResultVisitor<Task>.Visit(CreateSessionResult.Forbidden forbidden)
            => await NavigateToForbiddenPage(forbidden.ServiceDeskReference).PreserveThreadContext();

        private async Task NavigateToLoggedInHomePage(UserSession userSession, CookieContainer cookies)
        {
            var homePageModel = new NhsAppWebModel(userSession, cookies);
            var homePage = _pageFactory.CreatePageFor(homePageModel);

            await _view.Navigation.PopToNewRoot(homePage).PreserveThreadContext();
        }

        private async Task NavigateToFailedPage()
        {
            var errorModel = _model.FallbackError();
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.Navigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }

        private async Task NavigateToForbiddenPage(string serviceDeskReference)
        {
            var errorModel = _model.ForbiddenError(serviceDeskReference);
            var errorPage = _pageFactory.CreatePageFor(errorModel);

            await _view.Navigation.ReplaceCurrentPage(errorPage).PreserveThreadContext();
        }
    }
}