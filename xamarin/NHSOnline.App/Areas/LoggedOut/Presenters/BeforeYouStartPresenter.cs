using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class BeforeYouStartPresenter
    {
        private readonly IBeforeYouStartView _view;
        private readonly ILogger<BeforeYouStartPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly IUserPreferencesService _userPreferencesService;

        public BeforeYouStartPresenter(
            IBeforeYouStartView view,
            ILogger<BeforeYouStartPresenter> logger,
            IPageFactory pageFactory,
            IUserPreferencesService userPreferencesService)
        {
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _userPreferencesService = userPreferencesService;

            view.LoginRequested += ViewOnLoginRequested;
        }

        private async void ViewOnLoginRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Login Requested");

            _userPreferencesService.ShowBeforeYouStart = false;

            await _view.Navigation.PopToRootAsync(true).PreserveThreadContext();
        }
    }
}