using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidSubstraktPage
    {
        private AndroidSubstraktPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new SubstraktPageContent(driver.Web(WebViewContext.SubstraktWebIntegration));
        }

        private AndroidFullNavigation Navigation { get; }

        private SubstraktPageContent PageContent { get; }

        public static AndroidSubstraktPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidSubstraktPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidSubstraktPage AssertNativeHeader()
        {
            Navigation.AssertNavigationPresent();
            return this;
        }
    }
}