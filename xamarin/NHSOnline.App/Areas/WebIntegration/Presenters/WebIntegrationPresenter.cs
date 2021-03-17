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
        private bool _hasAlreadyAppeared;

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

            _view.AppNavigation
                .RegisterHandler(
                    ViewOnAppearing, (view, handler) => view.Appearing = handler)
                .RegisterHandler(
                    HelpRequested, (view, handler) => view.HelpRequested = handler)
                .RegisterHandler<WebNavigatingEventArgs>(
                    ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler(
                    model.NavigationHandler.SettingsRequested, (view, handler) => view.SettingsRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.SymptomsRequested, (view, handler) => view.SymptomsRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.RecordRequested, (view, handler) => view.RecordRequested = handler)
                .RegisterHandler(
                    model.NavigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler<string>(
                    RedirectToNhsAppPageRequested, (view, handler) => view.RedirectToNhsAppPageRequested = handler);
        }

        private async Task RedirectToNhsAppPageRequested(string page)
        {
            _logger.LogInformation("Redirecting to NHS App Page - {page}", page);

            await _model.NavigationHandler.RedirectToNhsAppPageRequested(page).PreserveThreadContext();
        }

        private async Task HelpRequested()
        {
            await _browserOverlay
                .OpenBrowserOverlay(_nhsExternalServicesConfiguration.NhsUkBaseHelpUrl)
                .PreserveThreadContext();
        }

        private Task ViewOnAppearing()
        {
            if (_hasAlreadyAppeared)
            {
                return Task.CompletedTask;
            }

            _hasAlreadyAppeared = true;

            _view.GoToUri(_model.Url);

            return Task.CompletedTask;
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
