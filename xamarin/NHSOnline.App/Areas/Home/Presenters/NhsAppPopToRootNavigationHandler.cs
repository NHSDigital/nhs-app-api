using System;
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

        public async void SettingsRequested(object sender, EventArgs e)
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            _rootHandler.SettingsRequested(sender, e);
        }

        public async void HomeRequested(object sender, EventArgs e)
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            _rootHandler.HomeRequested(sender, e);
        }

        public async void SymptomsRequested(object sender, EventArgs e)
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            _rootHandler.SymptomsRequested(sender, e);
        }

        public async void AppointmentsRequested(object sender, EventArgs e)
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            _rootHandler.AppointmentsRequested(sender, e);
        }

        public async void PrescriptionsRequested(object sender, EventArgs e)
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            _rootHandler.PrescriptionsRequested(sender, e);
        }

        public async void RecordRequested(object sender, EventArgs e)
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            _rootHandler.RecordRequested(sender, e);
        }

        public async void MoreRequested(object sender, EventArgs e)
        {
            await _navigation.PopToRootAsync(true).PreserveThreadContext();
            _rootHandler.MoreRequested(sender, e);
        }
    }
}