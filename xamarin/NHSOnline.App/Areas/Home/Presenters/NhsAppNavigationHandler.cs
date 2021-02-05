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

        public Task SettingsRequested()
        {
            _view.NavigateToSettings();
            return Task.CompletedTask;
        }

        public Task HomeRequested()
        {
            _view.NavigateToHome();
            return Task.CompletedTask;
        }

        public Task SymptomsRequested()
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

        public Task RecordRequested()
        {
            _view.NavigateToYourHealth();
            return Task.CompletedTask;
        }

        public Task MoreRequested()
        {
            _view.NavigateToMessages();
            return Task.CompletedTask;
        }

        public Task RedirectToNhsAppPageRequested(string page)
        {
            // TODO deal with homepage mapping to route as part of NHSO-10645
            _view.NavigateToRedirectedPathWithinApp(page == "homePage" ? "/": page);
            return Task.CompletedTask;
        }
    }
}
