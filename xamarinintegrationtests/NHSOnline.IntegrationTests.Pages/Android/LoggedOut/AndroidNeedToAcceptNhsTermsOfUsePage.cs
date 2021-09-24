using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public class AndroidNeedToAcceptNhsTermsOfUsePage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidNeedToAcceptNhsTermsOfUsePage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "You need to accept NHS login terms of use to continue");
        private AndroidLabel YouCantUseTheNhsApp => AndroidLabel.WithText(_driver, "You cannot use the NHS App if you have not accepted NHS login terms of use.");
        private AndroidLabel ContactGpSurgery => AndroidLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly.");
        private AndroidLabel UrgentMedicalAdvice => AndroidLabel.WithText(_driver, "For urgent medical advice, use NHS 111 online or call 111.");
        private AndroidLink GoTo111Link => AndroidLink.WithContentDescription(_driver, "Go to 111.nhs.uk");
        private AndroidLink BackToLogInLink => AndroidLink.WithContentDescription(_driver, "Back to log in").ScrollIntoView();

        public static AndroidNeedToAcceptNhsTermsOfUsePage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNeedToAcceptNhsTermsOfUsePage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidNeedToAcceptNhsTermsOfUsePage AssertPageContent()
        {
            Title.AssertVisible();
            YouCantUseTheNhsApp.AssertVisible();
            ContactGpSurgery.AssertVisible();
            UrgentMedicalAdvice.AssertVisible();
            GoTo111Link.AssertVisible();
            BackToLogInLink.AssertVisible();
            return this;
        }

        public void BackToLogin() => BackToLogInLink.Touch();
    }
}