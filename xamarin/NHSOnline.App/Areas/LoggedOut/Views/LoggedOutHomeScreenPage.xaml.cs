using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using NHSOnline.App.Controls;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class LoggedOutHomeScreenPage: ILoggedOutHomeScreenView, IRootPage
    {
        public event EventHandler<EventArgs>? LoginRequested;
        public event EventHandler<EventArgs>? NhsUkCovidConditionsServicePageRequested;
        public event EventHandler<EventArgs>? NhsUkLoginHelpServicePageRequested;

        public Func<Task>? ResetAndShowErrorRequested { get; set; }

        public LoggedOutHomeScreenPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public ICommand LoginCommand => new Command(()=>LoginRequested?.Invoke(this, EventArgs.Empty));
        public ICommand NhsUkCovidConditionsServiceCommand => new Command(() => NhsUkCovidConditionsServicePageRequested?.Invoke(this, EventArgs.Empty));
        public ICommand NhsUkLoginHelpServiceCommand => new Command(() => NhsUkLoginHelpServicePageRequested?.Invoke(this, EventArgs.Empty));

        public async Task ResetAndShowError()
        {
            await (ResetAndShowErrorRequested?.Invoke() ?? Task.CompletedTask).PreserveThreadContext();
        }
    }
}
