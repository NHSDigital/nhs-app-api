using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using NHSOnline.App.Controls.WebViews.Payloads;
using NHSOnline.App.Navigation;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginUpliftPage : INhsLoginUpliftView, INhsLoginUpliftView.IEvents
    {
        private readonly ILogger _logger;
        private readonly AppNavigation<INhsLoginUpliftView.IEvents> _appNavigation;

        public NhsLoginUpliftPage(ILogger<NhsLoginUpliftPage> logger)
        {
            _logger = logger;
            _appNavigation = new AppNavigation<INhsLoginUpliftView.IEvents>(this, Navigation);

            InitializeComponent();

            AddEventHandlers();
        }

        IAppNavigation<INhsLoginUpliftView.IEvents> INavigationView<INhsLoginUpliftView.IEvents>.AppNavigation => _appNavigation;

        Func<Task>? INhsLoginUpliftView.IEvents.Appearing { get; set; }
        private AsyncCommand AppearingCommand
            => new AsyncCommand(() => ((INhsLoginUpliftView.IEvents)this).Appearing);

        public Func<WebNavigatingEventArgs, Task>? Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand
            => new AsyncCommand<WebNavigatingEventArgs>(() => Navigating);

        public Func<Task>? BackRequested { get; set; }
        private AsyncCommand BackRequestedCommand => new AsyncCommand(() => BackRequested);

        public Func<ISelectMediaRequest, Task>? SelectMediaRequested { get; set; }
        public AsyncCommand<ISelectMediaRequest> SelectMediaCommand
            => new AsyncCommand<ISelectMediaRequest>(() => SelectMediaRequested);

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _logger.LogInformation("{Method}", nameof(OnAppearing));
            _appNavigation.EnableHandlers();

            RemoveEventHandlers();
            AddEventHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _logger.LogInformation("{Method}", nameof(OnDisappearing));
            _appNavigation.SuppressHandlers();

            RemoveEventHandlers();
        }

        private void AddEventHandlers()
        {
            WebView.Navigating += WebViewOnNavigating;
            WebView.Navigated += WebViewOnNavigated;
        }

        private void RemoveEventHandlers()
        {
            WebView.Navigating -= WebViewOnNavigating;
            WebView.Navigated -= WebViewOnNavigated;
        }

        private void WebViewOnNavigating(object sender, WebNavigatingEventArgs args)
        {
            _logger.LogInformation("Navigating: {Uri}", args.Url);
            NavigatingCommand.Execute(args);
        }

        private void WebViewOnNavigated(object sender, WebNavigatedEventArgs args)
        {
            _logger.LogInformation("Navigated ({Result}): {Uri}", args.Result, args.Url);
        }

        protected override bool OnBackButtonPressed()
        {
            BackRequestedCommand.Execute(null);
            return true;
        }

        public void GoToUri(Uri uri) => WebView.GoToUri(uri);
    }
}
