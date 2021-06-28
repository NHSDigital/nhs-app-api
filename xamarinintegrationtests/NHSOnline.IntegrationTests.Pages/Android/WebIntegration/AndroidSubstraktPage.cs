using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidSubstraktPage
    {
        public AndroidFullNavigation Navigation { get; }
        public SubstraktPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidSubstraktPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new SubstraktPageContent(driver.Web(WebViewContext.SubstraktWebIntegration));
        }

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