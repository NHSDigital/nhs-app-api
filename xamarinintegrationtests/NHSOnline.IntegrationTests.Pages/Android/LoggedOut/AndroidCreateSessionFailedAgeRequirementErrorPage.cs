using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidCreateSessionFailedAgeRequirementErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionFailedAgeRequirementErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Login failed");

        private AndroidLabel DueToLegalRestrictionsText => AndroidLabel.WithText(_driver, "Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. You can still contact your GP surgery to access your NHS services. For urgent medical advice, go to 111.nhs.uk or call 111.");

        public static AndroidCreateSessionFailedAgeRequirementErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionFailedAgeRequirementErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidCreateSessionFailedAgeRequirementErrorPage AssertPageElements()
        {
            DueToLegalRestrictionsText.AssertVisible();
            return this;
        }
    }
}