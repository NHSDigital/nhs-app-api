using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSStubbedLoginUpliftPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSStubbedLoginUpliftPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSSlimCloseNavigation(driver);
            PageContent = new StubbedLoginUpliftPageContent(driver.Web.WebIntegrationWebView());
        }

        public IOSSlimCloseNavigation Navigation { get; }

        private StubbedLoginUpliftPageContent PageContent { get; }

        public static IOSStubbedLoginUpliftPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedLoginUpliftPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void NavigateToInternalPage() => PageContent.NavigateToInternalPage();

        public void UploadFile() => PageContent.UploadFile();

        public void AssertNoFileSelected() => PageContent.AssertNoFileSelected();

        public void AssertFileSelected() => PageContent.AssertFileSelected();

        public void OpenCamera() => PageContent.OpenCamera();

        public void AssertPhotoCaptured() => PageContent.AssertPhotoCaptured();

        public void UpliftSuccess() => UpliftButtonClick("Success");

        public void UpliftTsAndCsDeclined() => UpliftButtonClick("TermsAndConditionsDeclined");

        public void UpliftError() => UpliftButtonClick("Error");

        private void UpliftButtonClick(string buttonSuffix)
        {
            IOSButton
                .WithText(_driver, $"Uplift{buttonSuffix}")
                .ScrollIntoView()
                .Click();

            // Clear the web context cache so that revisiting NHS App screens will not assert on cached versions.
            _driver.NhsAppWebViewClosed();
        }
    }
}