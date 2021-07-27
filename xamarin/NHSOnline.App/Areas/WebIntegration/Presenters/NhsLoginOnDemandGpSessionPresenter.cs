using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal class NhsLoginOnDemandGpSessionPresenter : IOnDemandGpReturnCheckResultVisitor<Task>
    {
        private readonly NhsLoginOnDemandGpSessionModel _model;
        private readonly ILogger<NhsLoginOnDemandGpSessionPresenter> _logger;
        private readonly INhsLoginConfiguration _nhsLoginConfiguration;
        private readonly IBrowserOverlay _browserOverlay;

        private readonly CreateOnDemandGpSessionState _createOnDemandGpSessionState;

        private Uri? _deeplinkUrl;

        public NhsLoginOnDemandGpSessionPresenter(
            INhsLoginOnDemandGpSessionView view,
            NhsLoginOnDemandGpSessionModel model,
            ILogger<NhsLoginOnDemandGpSessionPresenter> logger,
            INhsLoginService nhsLoginService,
            INhsLoginConfiguration nhsLoginConfiguration,
            IBrowserOverlay browserOverlay)
        {
            _model = model;
            _logger = logger;
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _browserOverlay = browserOverlay;

            view.SetNavigationFooterItem(model.FooterItem);

            view.AppNavigation
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler)
                .RegisterHandler(model.NavigationHandler.HomeRequested, (view, handler) => view.HomeRequested = handler)
                .RegisterHandler(model.NavigationHandler.MoreRequested, (view, handler) => view.MoreRequested = handler)
                .RegisterHandler(model.NavigationHandler.AdviceRequested, (view, handler) => view.AdviceRequested = handler)
                .RegisterHandler(model.NavigationHandler.AppointmentsRequested, (view, handler) => view.AppointmentsRequested = handler)
                .RegisterHandler(model.NavigationHandler.PrescriptionsRequested, (view, handler) => view.PrescriptionsRequested = handler)
                .RegisterHandler(model.NavigationHandler.YourHealthRequested, (view, handler) => view.YourHealthRequested = handler)
                .RegisterHandler(model.NavigationHandler.MessagesRequested, (view, handler) => view.MessagesRequested = handler);

            _createOnDemandGpSessionState = nhsLoginService.CreateOnDemandGpSession(model.AssertedLoginIdentity, model.RedirectTo);
            view.LoadUrlAndNotifyOnRedirect(_createOnDemandGpSessionState.AuthoriseUri, IsRedirect, OnRedirect);
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (ShouldOpenInBrowserOverlay(url))
            {
                await OpenInBrowserOverlay(webNavigatingEventArgs, url).PreserveThreadContext();
            }
        }

        private bool ShouldOpenInBrowserOverlay(Uri url)
        {
            if (IsNhsLoginHost(url))
            {
                return false;
            }

            return true;
        }

        private bool IsNhsLoginHost(Uri url)
        {
            return url.Host.EndsWith(_nhsLoginConfiguration.BaseHost, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task OpenInBrowserOverlay(WebNavigatingEventArgs webNavigatingEventArgs, Uri url)
        {
            webNavigatingEventArgs.Cancel = true;
            await _browserOverlay.OpenBrowserOverlay(url).PreserveThreadContext();
        }

        private Task DeeplinkRequested(Uri deeplinkUrl)
        {
            _deeplinkUrl = deeplinkUrl;
            return Task.CompletedTask;
        }

        private Task ViewOnNavigationFailed()
        {
            _logger.LogInformation("Navigation failed. Returning to on-demand gp return");

            return _model.NavigationHandler.NavigateToOnDemandGpReturn(new Dictionary<string, string>
            {
                {"state", _model.RedirectTo}
            });
        }

        private bool IsRedirect(Uri uri) => _createOnDemandGpSessionState.IsOnDemandGpReturn(uri);

        private async void OnRedirect(Uri redirectUri)
        {
            var result = _createOnDemandGpSessionState.CheckOnDemandGpReturn(redirectUri);
            await result.Accept(this).PreserveThreadContext();
        }

        public Task Visit(OnDemandGpReturnCheckResult.Complete complete)
        {
            _logger.LogInformation($"Completed on-demand gp session creation. Returning to web to complete journey. DeepLinkUrl: {_deeplinkUrl}");

            if (_deeplinkUrl != null)
            {
                complete.Parameters.Add("deepLinkUrl", _deeplinkUrl.ToString());
            }

            return _model.NavigationHandler.NavigateToOnDemandGpReturn(complete.Parameters);
        }

        private Task BackRequested()
        {
            _logger.LogInformation("Back Requested. Doing nothing");
            return Task.CompletedTask;
        }
    }
}