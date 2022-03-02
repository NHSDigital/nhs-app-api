using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.Errors.Models;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Areas.WebIntegration.Models;
using NHSOnline.App.Config;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
using NHSOnline.App.Services.FIDO;
using NHSOnline.App.Services.Media;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Presenters
{
    internal sealed class NhsLoginUpliftPresenter : IAuthReturnCheckResultVisitor<Task>
    {
        private readonly ILogger _logger;
        private readonly INhsLoginUpliftView _view;
        private readonly NhsLoginUpliftModel _model;
        private readonly IPageFactory _pageFactory;
        private readonly INhsLoginService _nhsLoginService;
        private readonly INhsLoginConfiguration _nhsLoginConfiguration;
        private readonly IBrowserOverlay _browserOverlay;
        private readonly ISelectMediaService _selectMediaService;
        private readonly IBiometricAuthenticationService _biometricAuthenticationService;

        private readonly LoginState _loginState;

        public NhsLoginUpliftPresenter(
            ILogger<NhsLoginUpliftPresenter> logger,
            INhsLoginUpliftView view,
            NhsLoginUpliftModel model,
            IPageFactory pageFactory,
            INhsLoginService nhsLoginService,
            INhsLoginConfiguration nhsLoginConfiguration,
            IBrowserOverlay browserOverlay,
            ISelectMediaService selectMediaService,
            IBiometricAuthenticationService biometricAuthenticationService)
        {
            _logger = logger;
            _view = view;
            _model = model;
            _pageFactory = pageFactory;
            _nhsLoginService = nhsLoginService;
            _nhsLoginConfiguration = nhsLoginConfiguration;
            _browserOverlay = browserOverlay;
            _selectMediaService = selectMediaService;
            _biometricAuthenticationService = biometricAuthenticationService;

            _view.AppNavigation
                .RegisterHandler<WebNavigatingEventArgs>(ViewOnNavigating, (view, handler) => view.Navigating = handler)
                .RegisterHandler(ViewOnNavigationFailed, (view, handler) => view.NavigationFailed = handler)
                .RegisterHandler(BackRequested, (view, handler) => view.BackRequested = handler)
                .RegisterHandler<ISelectMediaRequest>(SelectMediaRequested,
                    (view, handler) => view.SelectMediaRequested = handler);

            _loginState = nhsLoginService.CreateNhsLoginUpliftSession(model.PkceCodes, model.AssertedLoginIdentity);
            _view.LoadUrlAndNotifyOnRedirect(_loginState.AuthoriseUri, IsRedirect, OnRedirect);
        }

        private async Task SelectMediaRequested(ISelectMediaRequest request)
        {
            await _selectMediaService.SelectMedia(request).PreserveThreadContext();
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

        private Task ViewOnNavigationFailed()
        {
            var model = new CloseSlimBackToHomeNetworkErrorModel(_model.NavigationHandler.HomeRequested);
            var page = _pageFactory.CreatePageFor(model);
            return _view.AppNavigation.Push(page);
        }

        private bool IsRedirect(Uri uri) => _loginState.IsAuthReturn(uri);

        private async void OnRedirect(Uri redirectUri)
        {
            var result = _loginState.CheckAuthReturn(redirectUri);
            await result.Accept(this).PreserveThreadContext();
        }

        public async Task Visit(AuthReturnCheckResult.Authorised authorised)
        {
            _logger.LogInformation("NHS Login Uplift Authorised, Code: {AuthCode}", authorised.AuthCode);

            var createSessionModel = _model.AuthReturn(authorised.RedirectUri, authorised.AuthCode);
            var createSessionPage = _pageFactory.CreatePageFor(createSessionModel);

            await _view.AppNavigation.ReplaceCurrentPage(createSessionPage).PreserveThreadContext();
        }

        public async Task Visit(AuthReturnCheckResult.TermsAndConditionsDeclined termsDeclined)
        {
            _logger.LogInformation("NHS Login Uplift Terms and Conditions declined");

            var termsAndConditionsDeclinedModel = new NhsLoginTermsAndConditionsDeclinedModel();
            var termsAndConditionsDeclinedPage = _pageFactory.CreatePageFor(termsAndConditionsDeclinedModel);

            await _view.AppNavigation.ReplaceCurrentPage(termsAndConditionsDeclinedPage).PreserveThreadContext();
        }

        public async Task Visit(AuthReturnCheckResult.SignatureInvalid signatureInvalid)
        {
            _logger.LogInformation("NHS Login No FIDO record");

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
            _logger.LogError("NHS Login Uplift failed, error reference {ServiceDeskReference} generated",
                nhsLoginErrorModel.ServiceDeskReference);

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