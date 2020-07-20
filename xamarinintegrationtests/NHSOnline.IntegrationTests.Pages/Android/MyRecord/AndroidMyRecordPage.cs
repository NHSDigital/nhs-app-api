using NHSOnline.IntegrationTests.Pages.WebPageContent;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.MyRecord
{
    public sealed class AndroidMyRecordPage
    {
        private AndroidMyRecordPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new MyRecordPageContent(driver.Web(WebViewContext.NhsApp));
        }

        public AndroidFullNavigation Navigation { get; }

        private MyRecordPageContent PageContent { get; }

        public static AndroidMyRecordPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidMyRecordPage(driver);
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
