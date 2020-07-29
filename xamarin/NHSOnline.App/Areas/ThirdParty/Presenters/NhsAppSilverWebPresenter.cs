using System;
using NHSOnline.App.Areas.ThirdParty.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;

namespace NHSOnline.App.Areas.ThirdParty.Presenters
{
    internal sealed class NhsAppSilverWebPresenter
    {

        private readonly INhsAppSilverWebView _view;
        private readonly NhsAppSilverWebModel _model;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IAppBrowserTab _appBrowserTab;

        public NhsAppSilverWebPresenter(
            INhsAppSilverWebView view,
            NhsAppSilverWebModel model,
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
            DisplayNhsAppSilverWeb(_model.SilverUrl);
        }

        private async void HelpRequested(object sender, EventArgs e)
        {
            await _appBrowserTab.OpenAppBrowserTab(
                _nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private void DisplayNhsAppSilverWeb(string url)
        {
            _view.GoToUri(new Uri(url));
        }
    }
}