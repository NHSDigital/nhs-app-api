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
            _view.NavigateWithinApp("account");
            return Task.CompletedTask;
        }

        public Task HomeRequested()
        {
            _view.NavigateWithinApp("/");
            return Task.CompletedTask;
        }

        public Task SymptomsRequested()
        {
            _view.NavigateWithinApp("symptoms");
            return Task.CompletedTask;
        }

        public Task AppointmentsRequested()
        {
            _view.NavigateWithinApp("appointments");
            return Task.CompletedTask;
        }

        public Task PrescriptionsRequested()
        {
            _view.NavigateWithinApp("prescriptions");
            return Task.CompletedTask;
        }

        public Task RecordRequested()
        {
            _view.NavigateWithinApp("health-records");
            return Task.CompletedTask;
        }

        public Task MoreRequested()
        {
            _view.NavigateWithinApp("more");
            return Task.CompletedTask;
        }

        public Task RedirectToNhsAppPageRequested(string page)
        {
            // TODO deal with homepage mapping to route as part of NHSO-10645
            _view.NavigateWithinApp(page == "homePage" ? "/": page);
            return Task.CompletedTask;
        }
    }
}
