using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionInternalServerErrorPage
    {
        private readonly IIOSDriverWrapper _driver;
        private IOSCreateSessionInternalServerErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "The service is unavailable");
        private IOSLabel ThisMightBeATemporaryProblemText => IOSLabel.WithText(_driver, "This might be a temporary problem.");
        private IOSLink GoBackAndTryLoggingInAgainLink => IOSLink.WithText(_driver, "Go back and try logging in again");
        private IOSLabel IfYouStillCannotLoginText => IOSLabel.WithText(_driver, "If you still cannot log in, try again later.");
        private IOSLabel OtherServicesYouCanUseText => IOSLabel.WithText(_driver, "Other services you can use");
        private IOSLink GetYourNhsCovidPassOnlineLink => IOSLink.WhichMatches(_driver, "Get your NHS COVID Pass online");
        private IOSLabel GetMedicalAdviceText => IOSLabel.WithText(_driver, "Get medical advice");
        private IOSLabel IfYouNeedText => IOSLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, use your GP surgery's website or call the surgery directly.");
        private IOSLink ForUrgentMedicalAdviceLink => IOSLink.WhichMatches(_driver, "For urgent medical advice, go to 111.nhs.uk or call 111");

        public static IOSCreateSessionInternalServerErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionInternalServerErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSCreateSessionInternalServerErrorPage AssertPageElements()
        {
            ThisMightBeATemporaryProblemText.AssertVisible();
            GoBackAndTryLoggingInAgainLink.AssertVisible();
            IfYouStillCannotLoginText.AssertVisible();
            OtherServicesYouCanUseText.AssertVisible();
            GetYourNhsCovidPassOnlineLink.AssertVisible();
            GetMedicalAdviceText.AssertVisible();
            IfYouNeedText.AssertVisible();
            ForUrgentMedicalAdviceLink.AssertVisible();
            return this;
        }

        public void GetUrgentMedicalAdvice() => ForUrgentMedicalAdviceLink.Touch();
        public void GetYourNhsCovidPassOnline() => GetYourNhsCovidPassOnlineLink.Touch();
        public void BackToLogin() => GoBackAndTryLoggingInAgainLink.Touch();
    }
}