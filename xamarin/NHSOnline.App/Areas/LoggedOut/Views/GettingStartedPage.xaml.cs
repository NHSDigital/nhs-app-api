using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class GettingStartedPage: IGettingStartedView, IGettingStartedView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IGettingStartedView.IEvents> _appNavigation;

        public GettingStartedPage(ILogger<GettingStartedPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IGettingStartedView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IGettingStartedView.IEvents> INavigationView<IGettingStartedView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? LoginRequested { get; set; }
        public ICommand LoginCommand => new AsyncCommand(() => LoginRequested);

        public Func<Task>? NhsUkCovidAppPageRequested { get; set; }
        public ICommand NhsUkCovidAppCommand => new AsyncCommand(() => NhsUkCovidAppPageRequested);

        public Func<Task>? BackRequested { get; set; }
        private ICommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

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
