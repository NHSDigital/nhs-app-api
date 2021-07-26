using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal class NhsLoginOnDemandGpSessionPresenter : IOnGpDemandReturnCheckResultVisitor<Task>
    {
        private readonly INhsLoginOnDemandGpSessionView _view;
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
            _view = view;
            _logger = logger;
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _browserOverlay = browserOverlay;

            view.SetNavigationFooterItem(model.FooterItem);

            view.AppNavigation
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler);

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
            // TODO: When navigating failed, what do? Show error screen with a code? Just return to the home page?

            return Task.CompletedTask;
        }

        private bool IsRedirect(Uri uri) => _createOnDemandGpSessionState.IsOnGpDemandReturn(uri);

        private async void OnRedirect(Uri redirectUri)
        {
            var result = _createOnDemandGpSessionState.CheckOnGpDemandReturn(redirectUri);
            await result.Accept(this).PreserveThreadContext();
        }

        public Task Visit(OnGpDemandReturnCheckResult.Complete complete)
        {
            _logger.LogInformation($"Completed on-demand gp session creation. Returning to web to complete journey. DeepLinkUrl: {_deeplinkUrl}");

            // TODO: Return to NhsAppWebView and pass on DeepLinkUrl

            return _view.AppNavigation.Pop();
        }

        private Task BackRequested()
        {
            _logger.LogInformation("Back Requested");

            // TODO: Back requested, what do?

            return Task.CompletedTask;
        }
    }
}