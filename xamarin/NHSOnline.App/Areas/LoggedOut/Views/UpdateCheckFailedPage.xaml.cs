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
    public partial class UpdateCheckFailedPage: IUpdateCheckFailedView, IUpdateCheckFailedView.IEvents, ISwipeablePage
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IUpdateCheckFailedView.IEvents> _appNavigation;

        public UpdateCheckFailedPage(ILogger<UpdateCheckFailedPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IUpdateCheckFailedView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IUpdateCheckFailedView.IEvents> INavigationView<IUpdateCheckFailedView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Func<Task>? BackToLoginRequested { get; set; }
        public ICommand BackToLoginCommand => new AsyncCommand(() => BackToLoginRequested);

        public Func<Task>? BackRequested { get; set; }
        private ICommand BackCommand => new AsyncCommand(() => BackRequested);

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            Heading.AccessibilityFocus();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        protected override bool OnBackButtonPressed()
        {
            BackCommand.Execute(null);
            return true;
        }

        public bool OnSwipeBack()
        {
            BackCommand.Execute(null);
            return true;
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{ClassName} is not required to handle deeplinks", nameof(UpdateCheckFailedPage));
            return Task.CompletedTask;
        }
    }
}
