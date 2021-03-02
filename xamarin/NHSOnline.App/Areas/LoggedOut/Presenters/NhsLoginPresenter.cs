using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Config;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Navigation;
using NHSOnline.App.NhsLogin;
using NHSOnline.App.Services;
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

        public NhsLoginPresenter(
            NhsLoginModel model,
            INhsLoginView view,
            ILogger<NhsLoginPresenter> logger,
            ICookies cookies,
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

            AttachEventHandlers();

            // TODO: NHSO-10323 addresses cookie management in web views
            cookies.Clear();

            _loginState = nhsLoginService.BeginLogin(_model.PkceCodes);
            _view.LoadUrlAndNotifyOnRedirect(_loginState.AuthoriseUri, IsRedirect, OnRedirect);
            _view.BackRequested += BackRequested;
        }

        private void AttachEventHandlers()
        {
            _view.Navigating = ViewOnNavigating;
            _view.NavigationFailed = ViewOnNavigationFailed;
        }

        private void DetachEventHandlers()
        {
            _view.Navigating = null;
            _view.NavigationFailed = null;
        }

        private async Task ViewOnNavigating(WebNavigatingEventArgs webNavigatingEventArgs)
        {
            var url = new Uri(webNavigatingEventArgs.Url);
            if (ShouldOpenInBrowserOverlay(url))
            {
                await OpenInBrowserOverlay(webNavigatingEventArgs, url).PreserveThreadContext();
            }
        }

        private async Task ViewOnNavigationFailed()
        {
            _logger.LogWarning("NHS login navigation failed");
            var errorReferenceCode =
                $"3w{RandomErrorReferenceGenerator.GenerateString(4, "acefghjkmnorstuwxyz3456789")}";
            _logger.LogError($"NHS login navigation failed, error reference {errorReferenceCode} generated");

            DetachEventHandlers();

            await NavigateToLoginErrorPage(errorReferenceCode).PreserveThreadContext();
        }

        private bool IsRedirect(Uri uri) => _loginState.IsAuthReturn(uri);

        private async void OnRedirect(Uri redirectUri)
        {
            DetachEventHandlers();

            var result = _loginState.CheckAuthReturn(redirectUri);
            await result.Accept(this).PreserveThreadContext();
        }

        public async Task Visit(AuthReturnCheckResult.Authorised authorised)
        {
            _logger.LogInformation("Authorised, Code: {AuthCode}", authorised.AuthCode);

            var createSessionModel = _model.AuthReturn(authorised.RedirectUri, authorised.AuthCode);
            var createSessionPage = _pageFactory.CreatePageFor(createSessionModel);

            await _view.Navigation.ReplaceCurrentPage(createSessionPage).PreserveThreadContext();
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

            await _view.Navigation.ReplaceCurrentPage(nhsLoginErrorPage).PreserveThreadContext();
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

        private async void BackRequested(object sender, EventArgs e)
        {
            _logger.LogInformation("Back Requested");
            await _view.Navigation.PopAsync().PreserveThreadContext();
        }
    }
}
