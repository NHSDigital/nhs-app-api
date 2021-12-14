using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.DependencyServices.Navigation;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class UnsupportedPlatformVersionPage: IUnsupportedPlatformVersionView, IUnsupportedPlatformVersionView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IUnsupportedPlatformVersionView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public UnsupportedPlatformVersionPage(ILogger<UnsupportedPlatformVersionPage> logger, IAccessibilityService accessibilityService, INavigationService navigationService): base(accessibilityService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<IUnsupportedPlatformVersionView.IEvents>(this, _navigationService);

            InitializeComponent();
        }

        IAppNavigation<IUnsupportedPlatformVersionView.IEvents> INavigationView<IUnsupportedPlatformVersionView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Func<Task>? CovidPassRequested { get; set; }
        public ICommand CovidPassCommand => new AsyncCommand(() => CovidPassRequested);

        public Func<Task>? NhsAppOnlineLoginRequested { get; set; }
        public ICommand NhsAppOnlineLoginCommand => new AsyncCommand(() => NhsAppOnlineLoginRequested);

        public static readonly BindableProperty MinimumPlatformVersionProperty
            = BindableProperty.Create(nameof(MinimumPlatformVersion), typeof(string), typeof(UnsupportedPlatformVersionPage));

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
            _logger.LogInformation("{ClassName} is not required to handle deeplinks", nameof(UnsupportedPlatformVersionPage));
            return Task.CompletedTask;
        }

        public string MinimumPlatformVersion
        {
            get => (string) GetValue(MinimumPlatformVersionProperty);
            set => SetValue(MinimumPlatformVersionProperty, value);
        }
    }
}
