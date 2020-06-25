using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BeforeYouStartPresenter
    {
        private readonly IBeforeYouStartView _view;
        private readonly ILogger<BeforeYouStartPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;
        private readonly INhsLoginService _nhsLoginService;

        public BeforeYouStartPresenter(
            IBeforeYouStartView view,
            ILogger<BeforeYouStartPresenter> logger,
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

            _userPreferencesService.ShowBeforeYouStart = false;

            var pkceCodes = _nhsLoginService.GeneratePkceCodes();
            var loginModel = new NhsLoginModel(pkceCodes);

            var loginPage = _pageFactory.CreatePageFor(loginModel);
            await _view.Navigation.ReplaceCurrentPageAsync(loginPage).PreserveThreadContext();
        }
    }
}