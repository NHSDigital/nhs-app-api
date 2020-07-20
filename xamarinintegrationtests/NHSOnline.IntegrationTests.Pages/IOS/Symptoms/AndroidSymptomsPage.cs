using NHSOnline.IntegrationTests.Pages.IOS;
using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Symptoms
{
    public sealed class IOSSymptomsPage
    {
        private IOSSymptomsPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new SymptomsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public IOSFullNavigation Navigation { get; }

        private SymptomsPageContent PageContent { get; }

        public static IOSSymptomsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSSymptomsPage(driver);
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
