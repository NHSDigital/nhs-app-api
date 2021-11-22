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
    public partial class CreateSessionErrorOdsCodeNotFoundPage : ICreateSessionErrorOdsCodeNotFoundView, ICreateSessionErrorOdsCodeNotFoundView.IEvents
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorOdsCodeNotFoundPage), "3r");

        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionErrorOdsCodeNotFoundView.IEvents> _appNavigation;
        private readonly INavigationService _navigationService;

        public CreateSessionErrorOdsCodeNotFoundPage(ILogger<CreateSessionErrorOdsCodeNotFoundPage> logger, IAccessibilityService accessibilityService, INavigationService navigationService): base(accessibilityService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _appNavigation = new AppNavigation<ICreateSessionErrorOdsCodeNotFoundView.IEvents>(this, _navigationService);

            InitializeComponent();
        }

        IAppNavigation<ICreateSessionErrorOdsCodeNotFoundView.IEvents> INavigationView<ICreateSessionErrorOdsCodeNotFoundView.IEvents>.AppNavigation => _appNavigation;

        public string ServiceDeskReference
        {
            get => (string) GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Func<Task>? ContactUsRequested { get; set; }
        public ICommand ContactUsCommand => new AsyncCommand(() => ContactUsRequested);

        public Func<Task>? CovidStatusServiceRequested { get; set; }
        public ICommand CovidStatusServiceCommand => new AsyncCommand(() => CovidStatusServiceRequested);

        public Func<Task>? CovidPassRequested { get; set; }
        public ICommand CovidPassCommand => new AsyncCommand(() => CovidPassRequested);

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
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(CreateSessionErrorOdsCodeNotFoundPage));
            return Task.CompletedTask;
        }
    }
}
