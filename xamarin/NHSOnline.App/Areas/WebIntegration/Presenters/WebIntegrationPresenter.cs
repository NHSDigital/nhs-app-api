using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly INhsExternalServicesConfiguration _nhsExternalServicesConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly ILogger _logger;
        private readonly WebIntegrationUriDestination _uriDestination;

        public WebIntegrationPresenter(
            IWebIntegrationView view,
            WebIntegrationModel model,
            INhsLoginConfiguration nhsLoginConfiguration,
            INhsExternalServicesConfiguration nhsExternalServicesConfiguration,
            IBrowserOverlay browserOverlay,
            ILogger<WebIntegrationPresenter> logger)
        {
            _view = view;
            _model = model;
            _nhsExternalServicesConfiguration = nhsExternalServicesConfiguration;
            _browserOverlay = browserOverlay;
            _logger = logger;

            _uriDestination = new WebIntegrationUriDestination(nhsLoginConfiguration, model.Url);

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

            _view.RedirectToNhsAppPageRequested = RedirectToNhsAppPageRequested;
        }

        private async Task RedirectToNhsAppPageRequested(string page)
        {
            _logger.LogInformation("Redirecting to NHS App Page - {page}", page);

            await _model.NavigationHandler.RedirectToNhsAppPageRequested(page).PreserveThreadContext();
        }

        private Task ViewOnAppearing()
        {
            _view.Appearing = null;
            _view.GoToUri(_model.Url);

            return Task.CompletedTask;
        }

        private async Task HelpRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (_uriDestination.ShouldOpenInBrowserOverlay(url))
            {
                await OpenInBrowserOverlay(webNavigatingEventArgs, url).PreserveThreadContext();
            }
        }

        private async Task OpenInBrowserOverlay(WebNavigatingEventArgs webNavigatingEventArgs, Uri url)
        {
            webNavigatingEventArgs.Cancel = true;
            await _browserOverlay.OpenBrowserOverlay(url).PreserveThreadContext();
        }
    }
}
