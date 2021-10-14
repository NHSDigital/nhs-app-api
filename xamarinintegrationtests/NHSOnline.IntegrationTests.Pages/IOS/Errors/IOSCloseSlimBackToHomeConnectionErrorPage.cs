using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Errors
{
    public class IOSCloseSlimBackToHomeConnectionErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        public IOSSlimCloseNavigation Navigation { get; set; }

        private IOSCloseSlimBackToHomeConnectionErrorPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSSlimCloseNavigation(driver);
            _driver = driver;
        }

        public static IOSCloseSlimBackToHomeConnectionErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCloseSlimBackToHomeConnectionErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        private IOSLabel Title => IOSLabel.WithText(_driver, "Connection error");

        private IOSLink GoTo111Link => IOSLink.WithText(_driver, "Go to 111.nhs.uk");

        private IOSLink BackToHomeLink => IOSLink.WithText(_driver, "Back to home");

        public void GoTo111() => GoTo111Link.Touch();

        public void BackToHome() => BackToHomeLink.Touch();
    }
}