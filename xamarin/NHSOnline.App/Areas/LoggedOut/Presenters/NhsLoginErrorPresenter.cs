using System;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginErrorPresenter
    {
        private readonly INhsLoginErrorView _view;
        private readonly NhsLoginErrorModel _model;
        private readonly IAppBrowserTab _appBrowserTab;
        private readonly INhsExternalServicesConfiguration _externalServicesConfiguration;

        public NhsLoginErrorPresenter(
            INhsLoginErrorView view,
            NhsLoginErrorModel model,
            IAppBrowserTab appBrowserTab,
            INhsExternalServicesConfiguration externalServicesConfiguration)
        {
            _view = view;
            _model = model;
            _appBrowserTab = appBrowserTab;
            _externalServicesConfiguration = externalServicesConfiguration;

            _view.BackHomeRequested += ViewOnBackHomeRequested;
            _view.ContactUsRequested += ViewOnContactUsRequested;
            _view.ServiceDeskReference = model.ServiceDeskReference;
        }

        private async void ViewOnBackHomeRequested(object sender, EventArgs e)
        {
            await _view.Navigation
                .PopToRootAsync()
                .PreserveThreadContext();
        }

        private async void ViewOnContactUsRequested(object sender, EventArgs e)
        {
            var contactUsUri = _externalServicesConfiguration.NhsUkContactUsUrlWithErrorCode(_model.ServiceDeskReference);
            await _appBrowserTab
                .OpenAppBrowserTab(contactUsUri)
                .PreserveThreadContext();
        }
    }
}