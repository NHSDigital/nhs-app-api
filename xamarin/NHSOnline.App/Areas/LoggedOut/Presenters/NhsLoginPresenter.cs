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
using NHSOnline.App.Services.FIDO;
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
        private readonly IBrowser _browser;
        private readonly LoginState _loginState;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;
        private Uri? _deeplinkUrl;

        private Uri? ResolveDeeplinkUrl => _deeplinkUrl ?? _model.DeeplinkUrl;

        public NhsLoginPresenter(
            NhsLoginModel model,
            INhsLoginView view,
            ILogger<NhsLoginPresenter> logger,
            IPageFactory pageFactory,
            INhsLoginConfiguration nhsLoginConfiguration,
            IBrowser browser,
            INhsLoginService nhsLoginService,
            IBiometricAuthenticationService biometricAuthenticationService)
        {
            _model = model;
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _browser = browser;
            _biometricAuthenticationService = biometricAuthenticationService;

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
                    _browser.OpenBrowserOverlay(url).PreserveThreadContext();
                });
            }
        }

        private Task ViewOnNavigationFailed(NavigationFailedArgs args)
        {
            if (args.OnInitialNavigation)
            {
                void RetryAction() => _view.GoToUri(args.FailedUrl);

                var model = new CloseSlimTryAgainNetworkErrorModel(_view.AppNavigation.PopToRoot, RetryAction);
                var page = _pageFactory.CreatePageFor(model);
                return _view.AppNavigation.Push(page);
            }
            else
            {
                var model = new CloseSlimBackToHomeNetworkErrorModel(_view.AppNavigation.PopToRoot);
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

        public async Task Visit(AuthReturnCheckResult.SignatureInvalid signatureInvalid)
        {
            _logger.LogInformation("NHS Login No FIDO record");

            // iOS doesn't use a fidoUsername and for Android we only care for the type of biometric
            var biometricStatus = await _biometricAuthenticationService.FetchBiometricStatus(string.Empty)
                .PreserveThreadContext();

            Page page = biometricStatus switch
            {
                BiometricStatusResult.FaceId => _pageFactory.CreatePageFor(new BiometricLoginFaceIdLockedOutModel()),
                BiometricStatusResult.TouchId => _pageFactory.CreatePageFor(new BiometricLoginTouchIdLockedOutModel()),
                _ => _pageFactory.CreatePageFor(new BiometricLoginFingerprintLockedOutModel())
            };

            await _view.AppNavigation.ReplaceCurrentPage(page).PreserveThreadContext();
        }

        public async Task Visit(AuthReturnCheckResult.Failed failed)
        {
            var nhsLoginErrorModel = _model.NhsLoginFailed();
            _logger.LogError("Error reference {ServiceDeskReference} generated. {ErrorMessage}", nhsLoginErrorModel.ServiceDeskReference,
                failed.ErrorLogMessage);

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
