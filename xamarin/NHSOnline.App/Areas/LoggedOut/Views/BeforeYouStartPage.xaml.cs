using System;
using System.ComponentModel;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class BeforeYouStartPage: IBeforeYouStartView
    {
        public event EventHandler<EventArgs>? LoginRequested;

        public BeforeYouStartPage()
        {
            InitializeComponent();
        }

        private void LoginButton_OnClicked(object sender, EventArgs e)
        {
            LoginRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
