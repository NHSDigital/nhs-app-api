using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Events.Models;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginPresenter: IAuthReturnCheckResultVisitor<Task>
    {
        private readonly NhsLoginModel _model;
        private readonly INhsLoginView _view;
        private readonly ILogger<NhsLoginPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly INhsLoginConfiguration _nhsLoginConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly LoginState _loginState;
        private Uri? _deeplinkUrl;

        private Uri? ResolveDeeplinkUrl => _deeplinkUrl ?? _model.DeeplinkUrl;

        public NhsLoginPresenter(
            NhsLoginModel model,
            INhsLoginView view,
            ILogger<NhsLoginPresenter> logger,
            IPageFactory pageFactory,
            INhsLoginConfiguration nhsLoginConfiguration,
            IBrowserOverlay browserOverlay,
            INhsLoginService nhsLoginService)
        {
            _model = model;
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _browserOverlay = browserOverlay;

            _view.AppNavigation
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler<NavigationFailedArgs>(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterPermanentHandler<Uri>(DeeplinkRequested, (view, handler) => view.DeeplinkRequested = handler);

            _loginState = nhsLoginService.BeginLogin(_model.PkceCodes, _model.FidoAuthResponse);
            _view.LoadUrlAndNotifyOnRedirect(_loginState.AuthoriseUri, IsRedirect, OnRedirect);
        }

        private void ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (ShouldOpenInBrowserOverlay(url))
            {
                webNavigatingEventArgs.Cancel = true;
                NhsAppResilience.ExecuteOnMainThread(() =>
                {
                    _browserOverlay.OpenBrowserOverlay(url).PreserveThreadContext();
                });
            }
        }

        private Task ViewOnNavigationFailed(NavigationFailedArgs args)
        {
            if (args.OnInitialNavigation)
            {
                void RetryAction() => _view.GoToUri(args.FailedUrl);

                var model = new CloseSlimTryAgainNetworkErrorModel(RetryAction);
                var page = _pageFactory.CreatePageFor(model);
                return _view.AppNavigation.Push(page);
            }
            else
            {
                var model = new CloseSlimBackToHomeNetworkErrorModel();
                var page = _pageFactory.CreatePageFor(model);
                return _view.AppNavigation.Push(page);
            }
        }

        private Task DeeplinkRequested(Uri deeplinkUrl)
        {
            _deeplinkUrl = deeplinkUrl;
            return Task.CompletedTask;
        }

        private bool IsRedirect(Uri uri) => _loginState.IsAuthReturn(uri);

        private async void OnRedirect(Uri redirectUri)
        {
            var result = _loginState.CheckAuthReturn(redirectUri);
            await result.Accept(this).PreserveThreadContext();
        }

        public async Task Visit(AuthReturnCheckResult.Authorised authorised)
        {
            _logger.LogInformation("Authorised, Code: {AuthCode}", authorised.AuthCode);

            var createSessionModel = _model.AuthReturn(authorised.RedirectUri, authorised.AuthCode, ResolveDeeplinkUrl);
            var createSessionPage = _pageFactory.CreatePageFor(createSessionModel);

            await _view.AppNavigation.ReplaceCurrentPage(createSessionPage).PreserveThreadContext();
        }

        public async Task Visit(AuthReturnCheckResult.TermsAndConditionsDeclined termsDeclined)
        {
            _logger.LogInformation("NHS Login Terms and Conditions declined");

            var termsAndConditionsDeclinedModel = new NhsLoginTermsAndConditionsDeclinedModel();
            var termsAndConditionsDeclinedPage = _pageFactory.CreatePageFor(termsAndConditionsDeclinedModel);

            await _view.AppNavigation.ReplaceCurrentPage(termsAndConditionsDeclinedPage).PreserveThreadContext();
        }

        public async Task Visit(AuthReturnCheckResult.Failed failed)
        {
            _logger.LogWarning("Auth Return Failed");
            var errorReferenceCode =
                $"3w{RandomErrorReferenceGenerator.GenerateString(4, "acefghjkmnorstuwxyz3456789")}";
            _logger.LogError($"Auth Return failed, error reference {errorReferenceCode} generated");

            await NavigateToLoginErrorPage(errorReferenceCode).PreserveThreadContext();
        }

        private async Task NavigateToLoginErrorPage(string errorReferenceCode)
        {
            var nhsLoginErrorModel = _model.NhsLoginFailed(errorReferenceCode);
            var nhsLoginErrorPage = _pageFactory.CreatePageFor(nhsLoginErrorModel);

            await _view.AppNavigation.ReplaceCurrentPage(nhsLoginErrorPage).PreserveThreadContext();
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

        private async Task BackRequested()
        {
            _logger.LogInformation("Back Requested");
            await _view.AppNavigation.Pop().PreserveThreadContext();
        }
    }
}
