using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public sealed class IOSPaycassoPassportPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSPaycassoPassportPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSLabel Title => IOSLabel.WithText(_driver, "Getting a picture of your Passport");

        private IOSLabel OpenYourPassportLabel => IOSLabel.WithText(
            _driver,
            @"Open your Passport at the page with your photo on it. Hold it as shown in the on-screen prompts making sure your fingers and thumbs aren’t covering up any information.\n\nWhen the frame turns green, we’ve grabbed a picture.");

        private IOSLabel PressNextLabel => IOSLabel.WithText(_driver, "Press ‘Next’ when you’re ready.");

        private IOSButton NextButton => IOSButton.WithText(_driver, "Next");

        public static IOSPaycassoPassportPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSPaycassoPassportPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void AssertPageContent()
        {
            OpenYourPassportLabel.AssertVisible();
            PressNextLabel.AssertVisible();
            NextButton.AssertVisible();
        }
    }
}