using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public class IOSAcceptNhsTermsOfUsePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSAcceptNhsTermsOfUsePage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(
            _driver,
            "You need to accept NHS login terms of use to continue");

        private IOSLabel YouCantUseTheNhsApp => IOSLabel.WithText(
            _driver,
            "You cannot use the NHS App if you have not accepted NHS login terms of use.");

        private IOSLabel ContactGpSurgery => IOSLabel.WithText(
            _driver,
            "If you need to book an appointment or get a prescription now, contact your GP surgery directly.");

        private IOSLabel UrgentMedicalAdvice => IOSLabel.WithText(
            _driver,
            "For urgent medical advice, visit 111.nhs.uk or call 111.");

        public static IOSAcceptNhsTermsOfUsePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAcceptNhsTermsOfUsePage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSAcceptNhsTermsOfUsePage AssertPageContent()
        {
            Title.AssertVisible();
            YouCantUseTheNhsApp.AssertVisible();
            ContactGpSurgery.AssertVisible();
            UrgentMedicalAdvice.AssertVisible();

            return this;
        }

        public void BackToHome() => BackToHomeLink.Touch();

        private IOSLink BackToHomeLink => IOSLink.WithText(_driver, "Back to home");
    }
}