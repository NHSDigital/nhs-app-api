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
            Navigation = new IOSSlimCloseNavigation(driver);
            PageContent = new StubbedLoginUpliftPageContent(driver.Web(WebViewContext.NhsLoginUplift));
            _driver = driver;
        }

        public IOSSlimCloseNavigation Navigation { get; }

        public StubbedLoginUpliftPageContent PageContent { get; }

        /*
            IOS is looking for a native button instead of a HTML button
            Bug raised to investigate: NHSO-13694
        */
        private IOSButton FileUploadButton => IOSButton.WithText(_driver, "Open photo library Choose File");

        public static IOSStubbedLoginUpliftPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStubbedLoginUpliftPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public void UploadFile() => FileUploadButton.Click();
    }
}