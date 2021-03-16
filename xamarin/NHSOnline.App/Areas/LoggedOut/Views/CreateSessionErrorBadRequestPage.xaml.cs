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
    public partial class CreateSessionErrorBadRequestPage : ICreateSessionErrorBadRequestView, ICreateSessionErrorBadRequestView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionErrorBadRequestView.IEvents> _appNavigation;

        public CreateSessionErrorBadRequestPage(ILogger<CreateSessionErrorBadRequestPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICreateSessionErrorBadRequestView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<ICreateSessionErrorBadRequestView.IEvents> INavigationView<ICreateSessionErrorBadRequestView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Func<Task>? ContactUsRequested { get; set; }
        public ICommand ContactUsCommand => new AsyncCommand(() => ContactUsRequested);

        public Func<Task>? BackHomeRequested { get; set; }
        public ICommand BackHomeCommand => new AsyncCommand(() => BackHomeRequested);

        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(CreateSessionErrorBadRequestPage), "3c");

        public string ServiceDeskReference
        {
            get => (string) GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
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

    }
}
