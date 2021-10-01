using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Navigation;
using NHSOnline.App.Threading;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.LoggedOut.Views
{
    [DesignTimeVisible(false)]
    public partial class CreateSessionPage: ICreateSessionView, ICreateSessionView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<ICreateSessionView.IEvents> _appNavigation;

        public CreateSessionPage(ILogger<CreateSessionPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<ICreateSessionView.IEvents>(this, Navigation);

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public Func<Uri, Task>? DeeplinkRequested { get; set; }

        IAppNavigation<ICreateSessionView.IEvents> INavigationView<ICreateSessionView.IEvents>.AppNavigation => _appNavigation;

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();
            Spinner.AccessibilityFocus();
        }

        protected override void OnDisappearing()
        {
            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();
        }

        public async Task HandleDeeplink(Uri deeplinkUrl)
        {
            if (DeeplinkRequested != null)
            {
                await DeeplinkRequested(deeplinkUrl).PreserveThreadContext();
            }
        }
    }
}
