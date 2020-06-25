using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class LoggedOutHomeScreenPresenter
    {
        private readonly ILoggedOutHomeScreenView _view;
        private readonly ILogger _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly INhsLoginService _nhsLoginService;

        public LoggedOutHomeScreenPresenter(
            ILoggedOutHomeScreenView view,
            ILogger<LoggedOutHomeScreenPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService,
            INhsLoginService nhsLoginService)
        {
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;
            _nhsLoginService = nhsLoginService;

            view.LoginRequested += ViewOnLoginRequested;
        }

        private async void ViewOnLoginRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Login Requested");

            if (_userPreferencesService.ShowBeforeYouStart)
            {
                await ShowBeforeYouStartPage().PreserveThreadContext();
            }
            else
            {
                await ShowNhsLoginPage().PreserveThreadContext();
            }
        }

        private async Task ShowBeforeYouStartPage()
        {
            var beforeYouStart = new BeforeYouStartModel();
            var beforeYouStartPage = _pageFactory.CreatePageFor(beforeYouStart);
            await _view.Navigation.PushAsync(beforeYouStartPage).PreserveThreadContext();
        }

        private async Task ShowNhsLoginPage()
        {
            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes);
            var loginView = _pageFactory.CreatePageFor(loginModel);
            await _view.Navigation.PushAsync(loginView).PreserveThreadContext();
        }
    }
}
