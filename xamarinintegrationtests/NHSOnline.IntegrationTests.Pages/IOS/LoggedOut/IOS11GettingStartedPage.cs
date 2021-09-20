using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOS11GettingStartedPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOS11GettingStartedPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOS11HeaderLabel Title => IOS11HeaderLabel.WithText(_driver, "Getting started");

        private IOSButton ContinueButton => IOSButton
            .WithText(_driver, "Continue")
            .ScrollIntoView();

        public static IOS11GettingStartedPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOS11GettingStartedPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void Continue()
        {
            ContinueButton.Click();
        }
    }
}
