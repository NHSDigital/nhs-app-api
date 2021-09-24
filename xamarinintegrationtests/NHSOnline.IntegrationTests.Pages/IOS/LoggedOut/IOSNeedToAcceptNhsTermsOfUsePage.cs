using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public class IOSNeedToAcceptNhsTermsOfUsePage
    {
        private readonly IIOSDriverWrapper _driver;
        private IOSNeedToAcceptNhsTermsOfUsePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "You need to accept NHS login terms of use to continue");
        private IOSLabel YouCantUseTheNhsApp => IOSLabel.WithText(_driver, "You cannot use the NHS App if you have not accepted NHS login terms of use.");
        private IOSLabel ContactGpSurgery => IOSLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly.");
        private IOSLabel UrgentMedicalAdvice => IOSLabel.WithText(_driver, "For urgent medical advice, use NHS 111 online or call 111.");
        private IOSLink GoTo111Link => IOSLink.WithText(_driver, "Go to 111.nhs.uk");
        private IOSLink BackToLoginLink => IOSLink.WithText(_driver, "Back to log in");

        public static IOSNeedToAcceptNhsTermsOfUsePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSNeedToAcceptNhsTermsOfUsePage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSNeedToAcceptNhsTermsOfUsePage AssertPageContent()
        {
            Title.AssertVisible();
            YouCantUseTheNhsApp.AssertVisible();
            ContactGpSurgery.AssertVisible();
            UrgentMedicalAdvice.AssertVisible();
            GoTo111Link.AssertVisible();
            BackToLoginLink.AssertVisible();
            return this;
        }

        public void BackToLogin() => BackToLoginLink.Touch();
    }
}