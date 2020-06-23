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

        private async void LoginButton_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync(true).PreserveThreadContext();
            LoginRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
