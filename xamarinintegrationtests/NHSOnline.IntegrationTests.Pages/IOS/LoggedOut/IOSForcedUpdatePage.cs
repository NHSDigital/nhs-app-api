using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public class IOSForcedUpdatePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSForcedUpdatePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "You need to update the NHS App");

        private IOSLabel YouWillNotBeAbleToUseItLabel => IOSLabel.WithText(_driver, "You will not be able to use it until you do.");

        private IOSLink UpdateLink => IOSLink.WithText(_driver, "Update the NHS App now");

        public static IOSForcedUpdatePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSForcedUpdatePage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSForcedUpdatePage AssertPageElements()
        {
            YouWillNotBeAbleToUseItLabel.AssertVisible();
            return this;
        }

        public void Upgrade() => UpdateLink.Touch();
    }
}