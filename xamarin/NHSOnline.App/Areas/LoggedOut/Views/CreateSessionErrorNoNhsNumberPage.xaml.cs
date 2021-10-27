using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionErrorNoNhsNumberPage : ICreateSessionErrorNoNhsNumberView, ICreateSessionErrorNoNhsNumberView.IEvents
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorNoNhsNumberPage), "3r");

        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionErrorNoNhsNumberView.IEvents> _appNavigation;

        public CreateSessionErrorNoNhsNumberPage(ILogger<CreateSessionErrorNoNhsNumberPage> logger, IAccessibilityService accessibilityService): base(accessibilityService)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICreateSessionErrorNoNhsNumberView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<ICreateSessionErrorNoNhsNumberView.IEvents> INavigationView<ICreateSessionErrorNoNhsNumberView.IEvents>.AppNavigation => _appNavigation;

        public string ServiceDeskReference
        {
            get => (string) GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }
        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Func<Task>? ContactUsRequested { get; set; }
        public ICommand ContactUsCommand => new AsyncCommand(() => ContactUsRequested);

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
            _logger.LogInformation("{className} is not required to handle deeplinks", nameof(CreateSessionErrorNoNhsNumberPage));
            return Task.CompletedTask;
        }
    }
}
