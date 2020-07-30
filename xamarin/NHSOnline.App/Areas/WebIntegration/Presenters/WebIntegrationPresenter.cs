using System;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class WebIntegrationPresenter
    {
        private readonly IWebIntegrationView _view;
        private readonly WebIntegrationModel _model;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IAppBrowserTab _appBrowserTab;

        public WebIntegrationPresenter(
            IWebIntegrationView view,
            WebIntegrationModel model,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IAppBrowserTab appBrowserTab)
        {
            _view = view;
            _model = model;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _appBrowserTab = appBrowserTab;

            _view.Appearing += ViewOnAppearing;
            _view.HelpRequested += HelpRequested;

            _view.SettingsRequested += model.NavigationHandler.SettingsRequested;
            _view.HomeRequested += model.NavigationHandler.HomeRequested;
            _view.SymptomsRequested += model.NavigationHandler.SymptomsRequested;
            _view.AppointmentsRequested += model.NavigationHandler.AppointmentsRequested;
            _view.PrescriptionsRequested += model.NavigationHandler.PrescriptionsRequested;
            _view.RecordRequested += model.NavigationHandler.RecordRequested;
            _view.MoreRequested += model.NavigationHandler.MoreRequested;
        }

        private void ViewOnAppearing(object sender, EventArgs e)
        {
            _view.Appearing -= ViewOnAppearing;
            _view.GoToUri(_model.Url);
        }

        private async void HelpRequested(object sender, EventArgs e)
        {
            await _appBrowserTab.OpenAppBrowserTab(
                _nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }
    }
}