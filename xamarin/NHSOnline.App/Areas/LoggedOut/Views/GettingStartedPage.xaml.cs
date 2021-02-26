using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class GettingStartedPage: IGettingStartedView
    {
        public event EventHandler<EventArgs>? LoginRequested;
        public event EventHandler<EventArgs>? NhsUkCovidServicePageRequested;
        public event EventHandler<EventArgs>? BackRequested;

        public GettingStartedPage()
        {
            InitializeComponent();
        }

        public ICommand LoginCommand => new Command(() => LoginRequested?.Invoke(this, EventArgs.Empty));
        public ICommand NhsUkCovidServiceCommand => new Command(() => NhsUkCovidServicePageRequested?.Invoke(this, EventArgs.Empty));

        protected override bool OnBackButtonPressed()
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
