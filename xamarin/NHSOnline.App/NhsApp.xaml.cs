using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.Areas.LoggedOut.Models;
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

            var loggerFactory = NhsAppLogging.Init();

            var serviceProvider = NhsAppDependencyInjection.Init(Startup.ConfigureServices, loggerFactory);

            var pageFactory = serviceProvider.GetRequiredService<IPageFactory>();
            var loggedOutHomeScreenPage = pageFactory.CreatePageFor(new LoggedOutHomeScreenModel());

            MainPage = new NavigationPage(loggedOutHomeScreenPage);
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
