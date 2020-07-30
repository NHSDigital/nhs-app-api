using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using NHSOnline.App.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Page = Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class LoggedOutHomeScreenPage: ILoggedOutHomeScreenView, IRootPage
    {
        public event EventHandler<EventArgs>? LoginRequested;
        public event EventHandler<EventArgs>? NhsUkCovidConditionsServicePageRequested;
        public event EventHandler<EventArgs>? NhsUkLoginHelpServicePageRequested;

        public event EventHandler<EventArgs>? ResetAndShowErrorRequested;

        public LoggedOutHomeScreenPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public ICommand LoginCommand => new Command(()=>LoginRequested?.Invoke(this, EventArgs.Empty));
        public ICommand NhsUkCovidConditionsServiceCommand => new Command(() => NhsUkCovidConditionsServicePageRequested?.Invoke(this, EventArgs.Empty));
        public ICommand NhsUkLoginHelpServiceCommand => new Command(() => NhsUkLoginHelpServicePageRequested?.Invoke(this, EventArgs.Empty));

        public void ResetAndShowError()
        {
            ResetAndShowErrorRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
