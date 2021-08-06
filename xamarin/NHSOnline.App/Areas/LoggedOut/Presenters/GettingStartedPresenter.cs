using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class GettingStartedPresenter
    {
        private readonly IGettingStartedView _view;
        private readonly ILogger<GettingStartedPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly INhsLoginService _nhsLoginService;
        private readonly GettingStartedModel _model;
        private Uri? _deeplinkUrl;

        private Uri? ResolveDeeplinkUrl => _deeplinkUrl ?? _model.DeeplinkUrl;

        public GettingStartedPresenter(
            GettingStartedModel model,
            IGettingStartedView view,
            ILogger<GettingStartedPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            INhsLoginService nhsLoginService)
        {
            _model = model;
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _nhsLoginService = nhsLoginService;

            view.AppNavigation
                .RegisterHandler(ViewOnLoginRequested, (view, handler) => view.LoginRequested = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler);
        }

        private async Task ViewOnLoginRequested()
        {
            _logger.LogInformation("Login Requested");

            _userPreferencesService.ShowGettingStarted = false;

            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes, null, ResolveDeeplinkUrl);

            var loginPage = _pageFactory.CreatePageFor(loginModel);
            await _view.AppNavigation.ReplaceCurrentPage(loginPage).PreserveThreadContext();
        }

        private Task DeeplinkRequested(Uri deeplinkUrl)
        {
            _deeplinkUrl = deeplinkUrl;
            return Task.CompletedTask;
        }

        private async Task BackRequested()
        {
            _logger.LogInformation("Back Requested");
            await _view.AppNavigation.Pop().PreserveThreadContext();
        }
    }
}