using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using NHSOnline.App.Services.ForcedUpdate;
using NHSOnline.App.Threading;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BeginLoginPresenter
    {
        private readonly IBeginLoginView _view;
        private readonly BeginLoginModel _model;
        private readonly ILogger<BeginLoginPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly INhsLoginService _nhsLoginService;
        private readonly IForcedUpdateCheckService _forcedUpdateCheckService;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly NhsAppCookieService _nhsAppCookieService;
        private readonly INavigationService _navigationService;

        private Uri? _deeplinkUrl;
        private Uri? ResolveDeeplinkUrl => _deeplinkUrl ?? _model.DeeplinkUrl;

        public BeginLoginPresenter(
            IBeginLoginView view,
            BeginLoginModel model,
            ILogger<BeginLoginPresenter> logger,
            IPageFactory pageFactory,
            INhsLoginService nhsLoginService,
            IForcedUpdateCheckService forcedUpdateCheckService,
            IUserPreferencesService userPreferencesService,
            NhsAppCookieService nhsAppCookieService, INavigationService navigationService)
        {
            _view = view;
            _model = model;
            _logger = logger;
            _pageFactory = pageFactory;
            _nhsLoginService = nhsLoginService;
            _forcedUpdateCheckService = forcedUpdateCheckService;
            _userPreferencesService = userPreferencesService;
            _nhsAppCookieService = nhsAppCookieService;
            _navigationService = navigationService;

            view.AppNavigation
                .RegisterHandler(ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler);
        }

        private Task DeeplinkRequested(Uri deeplinkUrl)
        {
            _deeplinkUrl = deeplinkUrl;
            return Task.CompletedTask;
        }

        private async Task ViewOnAppearing()
        {
            var updateCheck = await UpdateRequiredCheck().PreserveThreadContext();
            await ValidateAndUpdateShowGettingStarted().PreserveThreadContext();

            if (updateCheck != UpdateRequired.No)
            {
                await ShowUpdateCheckStatusPage(updateCheck).PreserveThreadContext();
            }
            else if (_userPreferencesService.ShowGettingStarted)
            {
                await ShowGettingStartedPage().PreserveThreadContext();
            }
            else
            {
                await ShowNhsLoginPage(_model.FidoAuthResponse).PreserveThreadContext();
            }
        }

        private async Task ValidateAndUpdateShowGettingStarted()
        {
            if (_userPreferencesService.ShowGettingStarted)
            {
                var hasGettingStartedCookie = await _nhsAppCookieService.HasGettingStartedPageCookie().ResumeOnThreadPool();
                var hasBiometricsEnabled = _userPreferencesService.BiometricsKeyId != null;
                var hasSeenGettingStartedScreen = hasGettingStartedCookie || hasBiometricsEnabled;

                _userPreferencesService.ShowGettingStarted = !hasSeenGettingStartedScreen;
            }
        }

        private async Task<UpdateRequired> UpdateRequiredCheck()
        {
            return await _forcedUpdateCheckService.RequiresForcedUpdate().PreserveThreadContext();
        }

        private async Task ShowUpdateCheckStatusPage(UpdateRequired updateRequired)
        {
            switch (updateRequired)
            {
                case UpdateRequired.Yes:
                    _logger.LogInformation("Update required");
                    await ShowUpdateRequiredPage().PreserveThreadContext();
                    return;
                case UpdateRequired.Failed:
                    _logger.LogInformation("Update check failed");
                    await ShowUpdateCheckFailedPage().PreserveThreadContext();
                    return;
                case UpdateRequired.No:
                default:
                    throw new InvalidOperationException("Attempted to show forced update status screen, but not needed.");
            }
        }

        private async Task ShowGettingStartedPage()
        {
            var gettingStartedModel = new GettingStartedModel(ResolveDeeplinkUrl);
            var gettingStartedPage = _pageFactory.CreatePageFor(gettingStartedModel);
            await _view.AppNavigation.ReplaceCurrentPage(gettingStartedPage).PreserveThreadContext();
        }

        private async Task ShowNhsLoginPage(string? fidoAuthResponse = null)
        {
            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes, fidoAuthResponse, ResolveDeeplinkUrl);
            var loginView = _pageFactory.CreatePageFor(loginModel);
            await _view.AppNavigation.ReplaceCurrentPage(loginView).PreserveThreadContext();
        }

        private async Task ShowUpdateRequiredPage()
        {
            var updateRequiredModel = new UpdateRequiredModel();
            var updateRequiredPage = _pageFactory.CreatePageFor(updateRequiredModel);
            await _view.AppNavigation.ReplaceCurrentPage(updateRequiredPage).PreserveThreadContext();
        }

        private async Task ShowUpdateCheckFailedPage()
        {
            var updateCheckFailedModel = new UpdateCheckFailedModel();
            var updateCheckFailedPage = _pageFactory.CreatePageFor(updateCheckFailedModel);
            await _view.AppNavigation.ReplaceCurrentPage(updateCheckFailedPage).PreserveThreadContext();
        }
    }
}