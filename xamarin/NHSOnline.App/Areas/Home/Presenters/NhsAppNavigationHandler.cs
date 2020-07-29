using System;
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

        public void SettingsRequested(object sender, EventArgs e)
        {
            _view.NavigateWithinApp("account");
        }

        public void HomeRequested(object sender, EventArgs e)
        {
            _view.NavigateWithinApp("/");
        }

        public void SymptomsRequested(object sender, EventArgs e)
        {
            _view.NavigateWithinApp("symptoms");
        }

        public void AppointmentsRequested(object sender, EventArgs e)
        {
            _view.NavigateWithinApp("appointments");
        }

        public void PrescriptionsRequested(object sender, EventArgs e)
        {
            _view.NavigateWithinApp("prescriptions");
        }

        public void RecordRequested(object sender, EventArgs e)
        {
            _view.NavigateWithinApp("my-record-warning");
        }

        public void MoreRequested(object sender, EventArgs e)
        {
            _view.NavigateWithinApp("more");
        }
    }
}