using System;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class CreateSessionErrorPresenter
    {
        private readonly ICreateSessionErrorView _view;
        private readonly CreateSessionErrorModel _model;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public CreateSessionErrorPresenter(
            ICreateSessionErrorView view,
            CreateSessionErrorModel model,
            IAppBrowserTab appBrowserTab,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
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
            var contactUsUri = _externalServicesConfiguration.NhsUkContactUsUrlWithErrorCode(_model.ServiceDeskReference);
            await _appBrowserTab
                .OpenAppBrowserTab(contactUsUri)
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