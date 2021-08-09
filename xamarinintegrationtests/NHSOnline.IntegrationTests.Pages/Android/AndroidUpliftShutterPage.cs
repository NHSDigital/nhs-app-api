using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidUpliftShutterPage
    {
        private AndroidUpliftShutterPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new UpliftShutterPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        private UpliftShutterPageContent PageContent { get; }

        public static AndroidUpliftShutterPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidUpliftShutterPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void Continue()
        {
            PageContent.ProveYourIdentityContinue();
        }

        public void NavigateToAppointments() => Navigation.NavigateToAppointments();
    }
}