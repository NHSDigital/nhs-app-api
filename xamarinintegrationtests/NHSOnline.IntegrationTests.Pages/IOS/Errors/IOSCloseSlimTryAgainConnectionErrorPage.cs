using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Errors
{
    public class IOSCloseSlimTryAgainConnectionErrorPage
    {
        private readonly IIOSDriverWrapper _driver;
        public IOSSlimCloseNavigation Navigation { get; }

        private IOSCloseSlimTryAgainConnectionErrorPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSSlimCloseNavigation(driver);
        }

        public static IOSCloseSlimTryAgainConnectionErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCloseSlimTryAgainConnectionErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        private IOSLabel Title => IOSLabel.WithText(_driver, "Connection error");

        private IOSLink GoTo111Link => IOSLink.WithText(_driver, "Go to 111.nhs.uk");

        private IOSLink TryAgainLink => IOSLink.WithText(_driver, "Try again");

        public void GoTo111() => GoTo111Link.Touch();

        public void TryAgain() => TryAgainLink.Touch();
    }
}