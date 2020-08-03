using System;
using System.Threading.Tasks;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Services;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class WebIntegrationPresenter
    {
        private readonly IWebIntegrationView _view;
        private readonly WebIntegrationModel _model;
        private readonly INhsLoginConfiguration _nhsLoginConfiguration;
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IAppBrowserTab _appBrowserTab;

        public WebIntegrationPresenter(
            IWebIntegrationView view,
            WebIntegrationModel model,
            INhsLoginConfiguration nhsLoginConfiguration,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IAppBrowserTab appBrowserTab)
        {
            _view = view;
            _model = model;
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _appBrowserTab = appBrowserTab;

            _view.Appearing = ViewOnAppearing;
            _view.HelpRequested = HelpRequested;
            _view.Navigating = ViewOnNavigating;

            _view.SettingsRequested = model.NavigationHandler.SettingsRequested;
            _view.HomeRequested = model.NavigationHandler.HomeRequested;
            _view.SymptomsRequested = model.NavigationHandler.SymptomsRequested;
            _view.AppointmentsRequested = model.NavigationHandler.AppointmentsRequested;
            _view.PrescriptionsRequested = model.NavigationHandler.PrescriptionsRequested;
            _view.RecordRequested = model.NavigationHandler.RecordRequested;
            _view.MoreRequested = model.NavigationHandler.MoreRequested;
        }

        private Task ViewOnAppearing()
        {
            _view.Appearing = null;
            _view.GoToUri(_model.Url);

            return Task.CompletedTask;
        }

        private async Task HelpRequested()
        {
            await _appBrowserTab.OpenAppBrowserTab(
                _nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (ShouldOpenInAppBrowserTab(url))
            {
                await OpenInAppBrowserTab(webNavigatingEventArgs, url).PreserveThreadContext();
            }
        }

        private bool ShouldOpenInAppBrowserTab(Uri url)
        {
            if (IsSameHost(url))
            {
                return false;
            }

            if (IsNhsLoginHost(url))
            {
                return false;
            }

            return true;
        }

        private bool IsSameHost(Uri url)
        {
            return url.Host == _model.Url.Host;
        }

        private bool IsNhsLoginHost(Uri url)
        {
            return url.Host.EndsWith(_nhsLoginConfiguration.BaseHost, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task OpenInAppBrowserTab(WebNavigatingEventArgs webNavigatingEventArgs, Uri url)
        {
            webNavigatingEventArgs.Cancel = true;
            await _appBrowserTab.OpenAppBrowserTab(url).PreserveThreadContext();
        }
    }
}