using System;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorFallbackPresenter
    {
        private readonly ICreateSessionErrorFallbackView _view;
        private readonly CreateSessionErrorFallbackModel _model;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorFallbackPresenter(
            ICreateSessionErrorFallbackView view,
            CreateSessionErrorFallbackModel model,
            IAppBrowserTab appBrowserTab,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _appBrowserTab = appBrowserTab;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.BackHomeRequested += ViewOnBackHomeRequested;
            _view.ContactUsRequested += ViewOnContactUsRequested;
        }

        private async void ViewOnBackHomeRequested(object sender, EventArgs e)
        {
            await _view.Navigation
                .PopToRootAsync()
                .PreserveThreadContext();
        }

        private async void ViewOnContactUsRequested(object sender, EventArgs e)
        {
            await _appBrowserTab
                .OpenAppBrowserTab(_externalServicesConfiguration.NhsUkContactUsUrl)
                .PreserveThreadContext();
        }
    }
}