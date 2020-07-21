using System;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorUpstreamSystemTimeoutPresenter
    {
        private readonly ICreateSessionErrorUpstreamSystemTimeoutView _view;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorUpstreamSystemTimeoutPresenter(
            ICreateSessionErrorUpstreamSystemTimeoutView view,
            CreateSessionErrorUpstreamSystemTimeoutModel model,
            IAppBrowserTab appBrowserTab,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _appBrowserTab = appBrowserTab;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.ServiceDeskReference = model.ServiceDeskReference;

            _view.OneOneOneRequested += ViewOnOneOneOneRequested;
            _view.ContactUsRequested += ViewOnContactUsRequested;
            _view.BackHomeRequested += ViewOnBackHomeRequested;
        }

        private async void ViewOnOneOneOneRequested(object sender, EventArgs e)
        {
            await _appBrowserTab
                .OpenAppBrowserTab(_externalServicesConfiguration.OneOneOneUrl)
                .PreserveThreadContext();
        }

        private async void ViewOnContactUsRequested(object sender, EventArgs e)
        {
            await _appBrowserTab
                .OpenAppBrowserTab(_externalServicesConfiguration.NhsUkContactUsUrl)
                .PreserveThreadContext();
        }

        private async void ViewOnBackHomeRequested(object sender, EventArgs e)
        {
            await _view.Navigation
                .PopToRootAsync()
                .PreserveThreadContext();
        }
    }
}