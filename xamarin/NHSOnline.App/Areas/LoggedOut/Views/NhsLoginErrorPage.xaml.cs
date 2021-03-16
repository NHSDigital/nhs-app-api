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
    public partial class NhsLoginErrorPage : INhsLoginErrorView, INhsLoginErrorView.IEvents
    {
        public static readonly BindableProperty ServiceDeskReferenceProperty
            = BindableProperty.Create(nameof(ServiceDeskReference), typeof(string), typeof(NhsLoginErrorPage), "3w");

        private readonly ILogger _logger;
        private readonly AppNavigation<INhsLoginErrorView.IEvents> _appNavigation;

        public NhsLoginErrorPage(ILogger<NhsLoginErrorPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsLoginErrorView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<INhsLoginErrorView.IEvents> INavigationView<INhsLoginErrorView.IEvents>.AppNavigation => _appNavigation;

        public string ServiceDeskReference
        {
            get => (string)GetValue(ServiceDeskReferenceProperty);
            set => SetValue(ServiceDeskReferenceProperty, value);
        }

        public Func<Task>? BackHomeRequested { get; set; }
        public ICommand BackHomeCommand => new AsyncCommand(() => BackHomeRequested);

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
