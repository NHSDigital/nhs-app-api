using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidNhsLoginErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidNhsLoginErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => new AndroidLabel(_driver, "Login failed");

        private AndroidLabel CannotGetDetailsText => new AndroidLabel(_driver, "We cannot get your details from your GP surgery.");
        private AndroidLabel GoBackText => new AndroidLabel(_driver, "Go back to the home screen and try logging in again.");
        private AndroidLabel ErrorCodeText => new AndroidLabel(_driver, "If you keep seeing this message, contact us. Quote the error code XXXXX to help us resolve the problem quicker.");
        private AndroidLabel IfYouNeedText => new AndroidLabel(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, visit 111.nhs.uk or call 111.");
        private AndroidLabel ContactUsLink => new AndroidLabel(_driver, "Contact us");
        private AndroidLabel BackHomeLink => new AndroidLabel(_driver, "Back home");

        internal static AndroidNhsLoginErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNhsLoginErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        internal AndroidNhsLoginErrorPage AssertPageElements()
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