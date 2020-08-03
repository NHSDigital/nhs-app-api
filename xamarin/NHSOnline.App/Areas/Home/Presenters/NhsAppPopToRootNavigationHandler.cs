using System.Threading.Tasks;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.Home.Presenters
{
    internal sealed class NhsAppPopToRootNavigationHandler : INhsAppNavigationHandler
    {
        private readonly INavigation _navigation;
        private readonly INhsAppNavigationHandler _rootHandler;

        public NhsAppPopToRootNavigationHandler(
            INhsAppNavigationHandler rootHandler,
            INavigation navigation)
        {
            _rootHandler = rootHandler;
            _navigation = navigation;
        }

        public async Task SettingsRequested()
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            await _rootHandler.SettingsRequested().PreserveThreadContext();
        }

        public async Task HomeRequested()
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            await _rootHandler.HomeRequested().PreserveThreadContext();
        }

        public async Task SymptomsRequested()
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            await _rootHandler.SymptomsRequested().PreserveThreadContext();
        }

        public async Task AppointmentsRequested()
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            await _rootHandler.AppointmentsRequested().PreserveThreadContext();
        }

        public async Task PrescriptionsRequested()
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            await _rootHandler.PrescriptionsRequested().PreserveThreadContext();
        }

        public async Task RecordRequested()
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            await _rootHandler.RecordRequested().PreserveThreadContext();
        }

        public async Task MoreRequested()
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            await _rootHandler.MoreRequested().PreserveThreadContext();
        }
    }
}