using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Controls;
using NHSOnline.App.Controls.WebViews;
using Xamarin.Forms;

namespace NHSOnline.App.Areas.WebIntegration.Views
{
    [DesignTimeVisible(false)]
    public partial class NhsLoginUpliftPage : INhsLoginUpliftView
    {
        private readonly ILogger _logger;

        public NhsLoginUpliftPage(ILogger<NhsLoginUpliftPage> logger)
        {
            _logger = logger;

            InitializeComponent();

            AddEventHandlers();
        }

        Func<Task>? INhsLoginUpliftView.Appearing { get; set; }
        private AsyncCommand AppearingCommand => new AsyncCommand(() => View.Appearing);

        Func<WebNavigatingEventArgs, Task>? INhsLoginUpliftView.Navigating { get; set; }
        private AsyncCommand<WebNavigatingEventArgs> NavigatingCommand => new AsyncCommand<WebNavigatingEventArgs>(() => View.Navigating);

        Func<Task>? INhsLoginUpliftView.BackRequested { get; set; }
        private AsyncCommand BackRequestedCommand => new AsyncCommand(() => View.BackRequested);

        Func<ISelectMediaRequest, Task>? INhsLoginUpliftView.SelectMediaRequested { get; set; }
        public AsyncCommand<ISelectMediaRequest> SelectMediaCommand => new AsyncCommand<ISelectMediaRequest>(() => View.SelectMediaRequested);

        private INhsLoginUpliftView View => this;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RemoveEventHandlers();
            AddEventHandlers();

            AppearingCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

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
