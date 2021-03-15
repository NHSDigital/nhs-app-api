using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class LoggedOutHomeScreenPage: ILoggedOutHomeScreenView, ILoggedOutHomeScreenView.IEvents, IRootPage
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<ILoggedOutHomeScreenView.IEvents> _appNavigation;

        public LoggedOutHomeScreenPage(ILogger<LoggedOutHomeScreenPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ILoggedOutHomeScreenView.IEvents>(this, Navigation);

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        IAppNavigation<ILoggedOutHomeScreenView.IEvents> INavigationView<ILoggedOutHomeScreenView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? LoginRequested { get; set; }
        public ICommand LoginCommand => new AsyncCommand(() => LoginRequested);

        public Func<Task>? NhsUkCovidConditionsServicePageRequested { get; set; }
        public ICommand NhsUkCovidConditionsServiceCommand => new AsyncCommand(() => NhsUkCovidConditionsServicePageRequested);

        public Func<Task>? NhsUkLoginHelpServicePageRequested { get; set; }
        public ICommand NhsUkLoginHelpServiceCommand => new AsyncCommand(() => NhsUkLoginHelpServicePageRequested);

        public Func<Task>? BackRequested { get; set; }
        private ICommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

        public Func<Task>? ResetAndShowErrorRequested { get; set; }

        public async Task ResetAndShowError()
        {
            if (ResetAndShowErrorRequested != null)
            {
                await ResetAndShowErrorRequested().PreserveThreadContext();
            }
        }

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }
        
        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }
    }
}
