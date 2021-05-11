using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSSubstraktPage
    {
        private IOSSubstraktPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new SubstraktPageContent(driver.Web(WebViewContext.SubstraktWebIntegration));
        }

        private IOSFullNavigation Navigation { get; }

        private SubstraktPageContent PageContent { get; }

        public static IOSSubstraktPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSSubstraktPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
        }
    }
}