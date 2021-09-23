using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public sealed class AndroidStubbedLoginUpliftPage
    {
        private AndroidStubbedLoginUpliftPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidSlimCloseNavigation(driver);
            PageContent = new StubbedLoginUpliftPageContent(driver.Web.WebIntegrationWebView());
        }

        public AndroidSlimCloseNavigation Navigation { get; }

        private StubbedLoginUpliftPageContent PageContent { get; }

        public static AndroidStubbedLoginUpliftPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidStubbedLoginUpliftPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void NavigateToInternalPage() => PageContent.NavigateToInternalPage();

        public void AssertFileSelected() => PageContent.AssertFileSelected();

        public void UploadFile() => PageContent.UploadFile();

        public void OpenCamera() => PageContent.OpenCamera();

        public void AssertPhotoCaptured() => PageContent.AssertPhotoCaptured();

        public void AssertPhotoNotCaptured() => PageContent.AssertPhotoNotCaptured();
    }
}