using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidCreateSessionOdsCodeNotFoundErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionOdsCodeNotFoundErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Cannot log in");

        private AndroidLabel FindOutHowToGetCovidPassText => AndroidLabel
            .WithText(
                _driver,
                "Find out how to get an NHS COVID Pass using the online service")
            .ScrollIntoView();
        private AndroidLabel ContactYourGpSurgeryText => AndroidLabel
            .WithText(
                _driver,
                "Contact your surgery and ask them to check you're registered correctly with them, so you can log in to the NHS App.")
            .ScrollIntoView();
        private AndroidLabel SurgeryMayNeedToResubmitRegistrationText => AndroidLabel
            .WithText(
                _driver,
                "Your surgery may need to resubmit your registration.")
            .ScrollIntoView();
        private AndroidLabel IfYouNeedToBookAnAppointmentOrPrescriptionText => AndroidLabel
            .WithText(
                _driver,
                "If you need to book an appointment or get a prescription now, contact your GP surgery directly.")
            .ScrollIntoView();
        private AndroidLabel ForUrgentMedicalAdviceText => AndroidLabel
            .WithText(
                _driver,
                "For urgent medical advice, use NHS 111 online or call 111.")
            .ScrollIntoView();
        private AndroidLink GoToOneOneOneWalesLink => AndroidLink
            .WithContentDescription(
                _driver,
                "Go to 111.nhs.uk")
            .ScrollIntoView();
        private AndroidLabel ContactUsIfText => AndroidLabel
            .WithText(
                _driver,
                "Contact us if you have checked with your GP surgery about this issue and are still unable to access the NHS App.")
            .ScrollIntoView();
        private AndroidLink ErrorCodeLink => AndroidLink
            .WhichMatches(_driver, "Contact us, quoting error code 3r[0-9a-z]{4}")
            .ScrollIntoView();

        public static AndroidCreateSessionOdsCodeNotFoundErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionOdsCodeNotFoundErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidCreateSessionOdsCodeNotFoundErrorPage AssertPageElements()
        {
            FindOutHowToGetCovidPassText.AssertVisible();
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