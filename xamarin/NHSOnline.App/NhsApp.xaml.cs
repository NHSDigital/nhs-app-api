using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.App.Areas.LoggedOut.Models;
using NHSOnline.App.Controls;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Logging;
using Xamarin.Forms;

namespace NHSOnline.App
{
    public partial class NhsApp
    {
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
                        loggerFactory),
                    loggerFactory);

                NhsAppLogging.AddProvidersFromServiceProvider(loggerFactory, serviceProvider);

                var pageFactory = serviceProvider.GetRequiredService<IPageFactory>();
                var loggedOutHomeScreenPage = pageFactory.CreatePageFor(new LoggedOutHomeScreenModel());

                var navigationPage = new NavigationPage(loggedOutHomeScreenPage);

                NhsAppResilience.Init(navigationPage.Navigation, Dispatcher);

                MainPage = navigationPage;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("App Startup Failed: {0}", e);
                TryLogFailure(e);
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

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
