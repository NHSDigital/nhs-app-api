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
    public partial class BeginLoginPage: IBeginLoginView, IBeginLoginView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<IBeginLoginView.IEvents> _appNavigation;

        public BeginLoginPage(ILogger<BeginLoginPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<IBeginLoginView.IEvents>(this, Navigation);

            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        Func<Task>? IBeginLoginView.IEvents.Appearing { get; set; }
        private ICommand AppearingCommand => new AsyncCommand(() => Events.Appearing);

        public Func<Uri, Task>? DeeplinkRequested { get; set; }

        private IBeginLoginView.IEvents Events => this;

        IAppNavigation<IBeginLoginView.IEvents> INavigationView<IBeginLoginView.IEvents>.AppNavigation => _appNavigation;

        protected override void OnAppearing()
        {
            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            AppearingCommand.Execute(null);
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
