using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSNhsLoginErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSNhsLoginErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => new IOSLabel(_driver, "Login failed");

        private IOSLabel CannotGetDetailsText => new IOSLabel(_driver, "We cannot get your details from your GP surgery.");
        private IOSLabel GoBackText => new IOSLabel(_driver, "Go back to the home screen and try logging in again.");
        private IOSLabel ErrorCodeText => new IOSLabel(_driver, "If you keep seeing this message, contact us. Quote the error code XXXXX to help us resolve the problem quicker.");
        private IOSLabel IfYouNeedText => new IOSLabel(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.");
        private IOSLabel ContactUsLink => new IOSLabel(_driver, "Contact us");
        private IOSLabel BackHomeLink => new IOSLabel(_driver, "Back home");

        internal static IOSNhsLoginErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNhsLoginErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        internal IOSNhsLoginErrorPage AssertPageElements()
        {
            CannotGetDetailsText.AssertVisible();
            GoBackText.AssertVisible();
            ErrorCodeText.AssertVisible();
            IfYouNeedText.AssertVisible();
            ContactUsLink.AssertVisible();
            BackHomeLink.AssertVisible();
            return this;
        }

        internal void ContactUs()
        {
            ContactUsLink.Click();
        }

        public void BackHome()
        {
            BackHomeLink.Click();
        }
    }
}