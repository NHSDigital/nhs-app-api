using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidNhsAppPlayStorePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidNhsAppPlayStorePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel NhsAppLabel => AndroidLabel.WithText(_driver, "NHS App");

        private AndroidLabel NhsDigitalLabel => AndroidLabel.WithText(_driver, "NHS Digital");

        private AndroidButton InstallButton => AndroidButton.WithText(_driver, "Install");

        public static AndroidNhsAppPlayStorePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNhsAppPlayStorePage(driver);
            page.NhsAppLabel.AssertVisible();
            return page;
        }

        public AndroidNhsAppPlayStorePage AssertPageElements()
        {
            NhsDigitalLabel.AssertVisible();
            return this;
        }

        public void InstallAvailable() => InstallButton.AssertVisible();

        public void InstallAvailableFallback()
        {
            var textIdentifier = "Install";

            // Play store app is changing layout and Button no longer has the text property - it is now on a sibling element.
            var isPresent = InstallButton.IsButtonPresentWithinParentElement(
                $".//android.widget.TextView[normalize-space(@text)='{textIdentifier}']/../..");

            isPresent.Should().BeTrue("A button with text: '{0}' should be present.", textIdentifier);
        }
    }
}