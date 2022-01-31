using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSPreIOS13GettingStartedPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSPreIOS13GettingStartedPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOS11HeaderLabel Title => IOS11HeaderLabel.WithText(_driver, "Getting started");

        private IOSButton ContinueButton => IOSButton
            .WithText(_driver, "Continue")
            .ScrollIntoView();

        public static IOSPreIOS13GettingStartedPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSPreIOS13GettingStartedPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void Continue()
        {
            ContinueButton.Click();
        }
    }
}
