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
    public partial class CreateSessionErrorFallbackPage : ICreateSessionErrorFallbackView, ICreateSessionErrorFallbackView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionErrorFallbackView.IEvents> _appNavigation;

        public CreateSessionErrorFallbackPage(ILogger<CreateSessionErrorFallbackPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICreateSessionErrorFallbackView.IEvents>(this, Navigation);

            InitializeComponent();
        }

        IAppNavigation<ICreateSessionErrorFallbackView.IEvents> INavigationView<ICreateSessionErrorFallbackView.IEvents>.AppNavigation => _appNavigation;

        public Func<Task>? OneOneOneRequested { get; set; }
        public ICommand OneOneOneCommand => new AsyncCommand(() => OneOneOneRequested);

        public Func<Task>? ContactUsRequested { get; set; }
        public ICommand ContactUsCommand => new AsyncCommand(() => ContactUsRequested);

        public Func<Task>? BackHomeRequested { get; set; }
        public ICommand BackHomeCommand => new AsyncCommand(() => BackHomeRequested);

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
