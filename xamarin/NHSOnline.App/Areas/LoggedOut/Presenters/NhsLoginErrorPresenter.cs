using System;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginErrorPresenter
    {
        private readonly INhsLoginErrorView _view;
        private readonly NhsLoginErrorModel _model;
        private readonly ILogger<NhsLoginPresenter> _logger;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public NhsLoginErrorPresenter(
            INhsLoginErrorView view,
            NhsLoginErrorModel model,
            ILogger<NhsLoginPresenter> logger,
            IAppBrowserTab appBrowserTab,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _logger = logger;
            _appBrowserTab = appBrowserTab;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.BackHomeRequested += ViewOnBackHomeRequested;
            _view.ContactUsRequested += ViewOnContactUsRequested;
        }

        private async void ViewOnBackHomeRequested(object sender, EventArgs e)
        {
            await _view.Navigation.PopToRootAsync().PreserveThreadContext();
        }

        private async void ViewOnContactUsRequested(object sender, EventArgs e)
        {
            await _appBrowserTab.OpenAppBrowserTab(_externalServicesConfiguration.NhsUkContactUsUrl)
                .PreserveThreadContext();
        }
    }
}