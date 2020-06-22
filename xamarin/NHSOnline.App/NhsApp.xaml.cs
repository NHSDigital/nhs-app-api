using Microsoft.Extensions.DependencyInjection;
using NHSOnline.App.DependencyInjection;
using NHSOnline.App.Presenters;
using Xamarin.Forms;

namespace NHSOnline.App
{
    public partial class NhsApp
    {
        public NhsApp()
        {
            InitializeComponent();

            var serviceProvider = NhsAppDependencyInjection.Init(Startup.ConfigureServices);

            var pageFactory = serviceProvider.GetRequiredService<IPageFactory>();
            var mainPage = pageFactory.CreatePageFor(new MainModel());

            MainPage = new NavigationPage(mainPage);
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
