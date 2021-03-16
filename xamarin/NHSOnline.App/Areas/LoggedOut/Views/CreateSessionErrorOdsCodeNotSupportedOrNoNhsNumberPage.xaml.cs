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
    public partial class CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPage : ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView, ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView.IEvents
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPage), "3f");

        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView.IEvents> _appNavigation;

        public CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPage(ILogger<CreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView.IEvents> INavigationView<ICreateSessionErrorOdsCodeNotSupportedOrNoNhsNumberView.IEvents>.AppNavigation => _appNavigation;

        public string ServiceDeskReference
        {
            get => (string) GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }

        public Func<Task>? MyHealthOnlineRequested { get; set; }
        public ICommand MyHealthOnlineCommand => new AsyncCommand(() => MyHealthOnlineRequested);

        public Func<Task>? OneOneOneWalesRequested { get; set; }
        public ICommand OneOneOneWalesCommand => new AsyncCommand(() => OneOneOneWalesRequested);

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Func<Task>? ContactUsRequested { get; set; }
        public ICommand ContactUsCommand => new AsyncCommand(() => ContactUsRequested);

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
    }
}
