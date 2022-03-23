using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.DependencyServices;
using NHSOnline.App.Logging;
using NHSOnline.App.Navigation;
using NHSOnline.App.Services;
using Xamarin.Forms;

namespace NHSOnline.App
{
    public partial class NhsApp
    {
        private readonly ICookieService? _cookieService;

        private NavigationPage? NavigationPage { get; }


        public NhsApp()
        {
            InitializeComponent();

            try
            {
                var loggerFactory = NhsAppLogging.Init(
                    DependencyService.Get<INativeLog>());

                var serviceProvider = NhsAppDependencyInjection.Init(
                    services => Startup.ConfigureServices(
                        services,
                        loggerFactory,
                        UserAgentService.Instance.NhsAppUserAgent),
                    loggerFactory);

                NhsAppLogging.AddProvidersFromServiceProvider(loggerFactory, serviceProvider);

                _cookieService = serviceProvider.GetRequiredService<ICookieService>();

                var pageFactory = serviceProvider.GetRequiredService<IPageFactory>();
                var loggedOutHomeScreenPage = pageFactory.CreatePageFor(new LoggedOutHomeScreenModel());

                NavigationPage = new NavigationPage(loggedOutHomeScreenPage);

                NavigationPage.BarTextColor = Color.White;

                NhsAppResilience.Init(NavigationPage.Navigation, Dispatcher);

                MainPage = NavigationPage;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("App Startup Failed: {0}", e);
                TryLogFailure(e);
                // TODO: Should we not rethrow here in order to get the crash report?
            }
        }

        private static void TryLogFailure(Exception e)
        {
            try
            {
                DependencyService.Get<INativeLog>().Log(LogLevel.Critical, nameof(NhsApp), $"App Startup Failed: {e}");
            }
            catch
            {
            }
        }

        [SuppressMessage("Design", "CA1054: URI parameters should not be strings",
            Justification = "Parsing at the app level rather than in the individual apps")]
        public void HandleDeeplink(string deeplinkUrl)
        {
            if (Uri.TryCreate(deeplinkUrl, UriKind.Absolute, out var targetUri))
            {
                if (NavigationPage?.CurrentPage is INavigationView currentPage)
                {
                    currentPage.HandleDeeplink(targetUri);
                }
            }
        }

        public void AppClosing()
        {
            _cookieService?.ClearSessionCookies();
        }
    }
}
