using System;
using System.ComponentModel;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class LoggedOutHomeScreenPage: ILoggedOutHomeScreenView
    {
        public event EventHandler<EventArgs>? LoginRequested;

        public LoggedOutHomeScreenPage()
        {
            InitializeComponent();
        }

        private void LoginButton_OnClicked(object sender, EventArgs e)
        {
            LoginRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
