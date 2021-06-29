using System;
using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppNavigationHandler : INhsAppNavigationHandler
    {
        private readonly INhsAppWebView _view;

        public NhsAppNavigationHandler(INhsAppWebView view)
        {
            _view = view;
        }

        public Task HomeRequested()
        {
            _view.NavigateToHome();
            return Task.CompletedTask;
        }

        public Task AdviceRequested()
        {
            _view.NavigateToAdvice();
            return Task.CompletedTask;
        }

        public Task AppointmentsRequested()
        {
            _view.NavigateToAppointments();
            return Task.CompletedTask;
        }

        public Task PrescriptionsRequested()
        {
            _view.NavigateToPrescriptions();
            return Task.CompletedTask;
        }

        public Task YourHealthRequested()
        {
            _view.NavigateToYourHealth();
            return Task.CompletedTask;
        }

        public Task MoreRequested()
        {
            _view.NavigateToMore();
            return Task.CompletedTask;
        }

        public Task MessagesRequested()
        {
            _view.NavigateToMessages();
            return Task.CompletedTask;
        }

        public Task GoToNhsAppPageRequested(string page)
        {
            _view.NavigateToAppPage(page);
            return Task.CompletedTask;
        }

        public Task RedirectToDeepLinkRequested(Uri deeplinkUrl)
        {
            _view.HandleDeeplink(deeplinkUrl);
            return Task.CompletedTask;
        }
    }
}
