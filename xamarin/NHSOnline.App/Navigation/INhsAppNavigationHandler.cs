using System;

namespace NHSOnline.App.Navigation
{
    internal interface INhsAppNavigationHandler
    {
        void SettingsRequested(object sender, EventArgs e);
        void HomeRequested(object sender, EventArgs e);
        void SymptomsRequested(object sender, EventArgs e);
        void AppointmentsRequested(object sender, EventArgs e);
        void PrescriptionsRequested(object sender, EventArgs e);
        void RecordRequested(object sender, EventArgs e);
        void MoreRequested(object sender, EventArgs e);
    }
}