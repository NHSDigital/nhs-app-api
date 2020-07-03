using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.NhsLogin;

namespace NHSOnline.App.Areas.LoggedOut.Presenters
{
    internal sealed class NhsLoginPresenter: IAuthReturnCheckResultVisitor<Task>
    {
        private readonly NhsLoginModel _model;
        private readonly INhsLoginView _view;
        private readonly ILogger<NhsLoginPresenter> _logger;
        private readonly IPageFactory _pageFactory;
        private readonly LoginState _loginState;

        public NhsLoginPresenter(
            NhsLoginModel model,
            INhsLoginView view,
            ILogger<NhsLoginPresenter> logger,
            ICookies cookies,
            IPageFactory pageFactory,
            INhsLoginService nhsLoginService)
        {
            _model = model;
            _view = view;
            _logger = logger;
            _pageFactory = pageFactory;

            _view.NavigationFailed += ViewOnNavigationFailed;

            // TODO: NHSO-10323 addresses cookie management in web views
            cookies.Clear();

            _loginState = nhsLoginService.BeginLogin(_model.PkceCodes);
            _view.LoadUrlAndNotifyOnRedirect(_loginState.AuthoriseUri, IsRedirect, OnRedirect);
        }

        private async void ViewOnNavigationFailed(object sender, EventArgs e)
        {
            _logger.LogWarning("NHS login navigation failed");

            _view.NavigationFailed -= ViewOnNavigationFailed;

            await NavigateToLoginErrorPage().PreserveThreadContext();
        }

        private bool IsRedirect(Uri uri) => _loginState.IsAuthReturn(uri);

        private async void OnRedirect(Uri redirectUri)
        {
            _view.NavigationFailed -= ViewOnNavigationFailed;

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

            await NavigateToLoginErrorPage().PreserveThreadContext();
        }

        private async Task NavigateToLoginErrorPage()
        {
            var nhsLoginErrorModel = _model.NhsLoginFailed();
            var nhsLoginErrorPage = _pageFactory.CreatePageFor(nhsLoginErrorModel);

            await _view.Navigation.ReplaceCurrentPage(nhsLoginErrorPage).PreserveThreadContext();
        }
    }
}
