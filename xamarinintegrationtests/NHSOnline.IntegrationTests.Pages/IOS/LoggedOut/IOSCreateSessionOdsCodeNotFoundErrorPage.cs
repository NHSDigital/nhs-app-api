using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionOdsCodeNotFoundErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionOdsCodeNotFoundErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Login failed");

        private IOSLabel WeCannotConnectToYourGpSurgeryText => IOSLabel
            .WithText(
                _driver,
                "We cannot connect to your GP surgery.")
            .ScrollIntoView();

        private IOSLink FindOutHowToGetCovidPassLink => IOSLink
            .WithText(
                _driver,
                "Find out how to get an NHS COVID Pass using the online service");
        private IOSLabel ContactYourGpSurgeryText => IOSLabel
            .WithText(
                _driver,
                "Contact your surgery and ask them to check you're registered correctly with them, so you can log in to the NHS App.")
            .ScrollIntoView();
        private IOSLabel SurgeryMayNeedToResubmitRegistrationText => IOSLabel
            .WithText(
                _driver,
                "Your surgery may need to resubmit your registration.")
            .ScrollIntoView();
        private IOSLabel IfYouNeedToBookAnAppointmentOrPrescriptionText => IOSLabel
            .WithText(
                _driver,
                "If you need to book an appointment or get a prescription now, contact your GP surgery directly.")
            .ScrollIntoView();
        private IOSLabel ForUrgentMedicalAdviceText => IOSLabel
            .WithText(
                _driver,
                "For urgent medical advice, use NHS 111 online or call 111.")
            .ScrollIntoView();

        private IOSLink GoToOneOneOneWalesLink => IOSLink
            .WithText(
                _driver,
                "Go to 111.nhs.uk");
        private IOSLabel ContactUsIfText => IOSLabel
            .WithText(
                _driver,
                "Contact us if you have checked with your GP surgery about this issue and are still unable to access the NHS App.")
            .ScrollIntoView();

        private IOSLink ErrorCodeLink => IOSLink
            .WhichMatches(_driver, "Contact us, quoting error code 3 r ([0-9a-z] ){4}")
            .ScrollIntoView();

        public static IOSCreateSessionOdsCodeNotFoundErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionOdsCodeNotFoundErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSCreateSessionOdsCodeNotFoundErrorPage AssertPageElements()
        {
            WeCannotConnectToYourGpSurgeryText.AssertVisible();
            FindOutHowToGetCovidPassLink.AssertVisible();
            ContactYourGpSurgeryText.AssertVisible();
            SurgeryMayNeedToResubmitRegistrationText.AssertVisible();
            IfYouNeedToBookAnAppointmentOrPrescriptionText.AssertVisible();
            ForUrgentMedicalAdviceText.AssertVisible();
            GoToOneOneOneWalesLink.AssertVisible();
            ContactUsIfText.AssertVisible();
            ErrorCodeLink.AssertVisible();

            return this;
        }
    }
}