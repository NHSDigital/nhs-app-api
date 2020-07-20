using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Symptoms
{
    public sealed class AndroidSymptomsPage
    {
        private AndroidSymptomsPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new SymptomsPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public AndroidFullNavigation Navigation { get; }

        private SymptomsPageContent PageContent { get; }

        public static AndroidSymptomsPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidSymptomsPage(driver);
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
