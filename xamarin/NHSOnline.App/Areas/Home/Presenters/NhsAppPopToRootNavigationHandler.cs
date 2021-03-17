using System.Threading.Tasks;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppPopToRootNavigationHandler : INhsAppNavigationHandler
    {
        private readonly IAppNavigation<INhsAppWebView.IEvents> _appNavigation;
        private readonly INhsAppNavigationHandler _rootHandler;

        public NhsAppPopToRootNavigationHandler(
            INhsAppNavigationHandler rootHandler,
            IAppNavigation<INhsAppWebView.IEvents> navigation)
        {
            _rootHandler = rootHandler;
            _appNavigation = navigation;
        }

        public async Task SettingsRequested()
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.SettingsRequested().PreserveThreadContext();
        }

        public async Task HomeRequested()
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.HomeRequested().PreserveThreadContext();
        }

        public async Task SymptomsRequested()
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.SymptomsRequested().PreserveThreadContext();
        }

        public async Task AppointmentsRequested()
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.AppointmentsRequested().PreserveThreadContext();
        }

        public async Task PrescriptionsRequested()
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.PrescriptionsRequested().PreserveThreadContext();
        }

        public async Task RecordRequested()
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.RecordRequested().PreserveThreadContext();
        }

        public async Task MoreRequested()
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.MoreRequested().PreserveThreadContext();
        }

        public async Task RedirectToNhsAppPageRequested(string page)
        {
            await _appNavigation.PopToRootAnimated().PreserveThreadContext();
            await _rootHandler.RedirectToNhsAppPageRequested(page).PreserveThreadContext();
        }
    }
}
