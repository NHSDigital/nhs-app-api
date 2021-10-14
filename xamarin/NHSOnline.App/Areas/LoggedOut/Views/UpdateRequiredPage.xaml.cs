using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Navigation;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class UpdateRequiredPage: IUpdateRequiredView, IUpdateRequiredView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IUpdateRequiredView.IEvents> _appNavigation;

        public UpdateRequiredPage(ILogger<UpdateRequiredPage> logger, IAccessibilityService accessibilityService): base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IUpdateRequiredView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<IUpdateRequiredView.IEvents> INavigationView<IUpdateRequiredView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? OpenAppStoreUrlRequested { get; set; }
        public ICommand OpenAppStoreUrlCommand => new AsyncCommand(() => OpenAppStoreUrlRequested);

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        public Task HandleDeeplink(Uri deeplinkUrl)
        {
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(UpdateRequiredPage));
            return Task.CompletedTask;
        }
    }
}
