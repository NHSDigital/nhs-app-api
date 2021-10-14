using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Errors
{
    public class IOSBackToHomeConnectionErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSBackToHomeConnectionErrorPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        public static IOSBackToHomeConnectionErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSBackToHomeConnectionErrorPage(driver);
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