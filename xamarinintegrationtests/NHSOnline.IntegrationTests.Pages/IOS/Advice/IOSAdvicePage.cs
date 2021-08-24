using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Advice;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Advice
{
    public sealed class IOSAdvicePage
    {
        private IOSAdvicePage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AdvicePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public AdvicePageContent PageContent { get; }

        public static IOSAdvicePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAdvicePage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();
        }
    }
}