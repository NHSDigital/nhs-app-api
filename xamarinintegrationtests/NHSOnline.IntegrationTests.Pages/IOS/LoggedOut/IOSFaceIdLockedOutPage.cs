using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public class IOSFaceIdLockedOutPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFaceIdLockedOutPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Turn on Face ID again");

        public static IOSFaceIdLockedOutPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSFaceIdLockedOutPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSFaceIdLockedOutPage AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }
    }
}