using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class  AndroidCreateSessionOdsCodeNotSupportedErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionOdsCodeNotSupportedErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Login failed");

        private AndroidLabel IfNotRegisteredOrArmedForceText => AndroidLabel
            .WithText(
                _driver,
                "If you’re not registered with a GP surgery in England, or you’re a member of the armed forces, you may still be able to get an NHS COVID Pass or proof of vaccination status.")
            .ScrollIntoView();

        private AndroidLink FindOutHowToGetCovidPassLink => AndroidLink
            .WithContentDescription(
                _driver,
                "Find out how to get your NHS COVID Pass or proof of vaccination status")
            .ScrollIntoView();
        private AndroidLabel IfYourGpSurgeryIsInWalesText => AndroidLabel
            .WithText(
                _driver,
                "If your GP surgery is in Wales")
            .ScrollIntoView();

        private AndroidLabel NhsAppNotAvailableInWalesText => AndroidLabel
            .WithText(
                _driver,
                "The NHS App is not available in Wales because health services are managed separately from England.")
            .ScrollIntoView();
        private AndroidLabel IfYouNeedText => AndroidLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, use the My Health Online service or contact your GP surgery directly.")
            .ScrollIntoView();

        private AndroidLink GoToMyHealthOnlineLink => AndroidLink
            .WithContentDescription(
                _driver,
                "Go to My Health Online")
            .ScrollIntoView();
        private AndroidLabel ForUrgentMedicalAdviceText => AndroidLabel
            .WithText(
                _driver,
                "For urgent medical advice use NHS 111 Wales online or call 111.")
            .ScrollIntoView();

        private AndroidLink OneOneOneWalesLink => AndroidLink
            .WithContentDescription(
                _driver,
                "Go to 111.wales.uk")
            .ScrollIntoView();

        private AndroidLabel IfRegisteredWithArmedForcesSurgeryText => AndroidLabel
            .WithText(
                _driver,
                "If you're registered with an armed forces GP surgery")
            .ScrollIntoView();
        private AndroidLabel NotAvailableToArmedForcesText => AndroidLabel
            .WithText(
                _driver,
                "The NHS App is not available to members of the armed forces because these surgeries are managed separately.")
            .ScrollIntoView();
        private AndroidLabel IfSurgeryInScotlandOrNorthernIrelandText => AndroidLabel
            .WithText(
                _driver,
                "If your GP surgery is in Scotland or Northern Ireland")
            .ScrollIntoView();
        private AndroidLabel NotAvailableInNorthernIrelandOrScotlandText => AndroidLabel
            .WithText(
                _driver,
                "The NHS App is not available in Scotland or Northern Ireland because health services are managed separately from England.")
            .ScrollIntoView();
        private AndroidLabel IfYouNeedAnAppointmentOrPrescriptionText => AndroidLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, contact your GP surgery directly.")
            .ScrollIntoView();
        private AndroidLabel IfInScotlandText => AndroidLabel
            .WithText(
                _driver,
                "If you’re in Scotland, for urgent medical advice call 111.")
            .ScrollIntoView();
        private AndroidLabel IfInNorthernIrelandText => AndroidLabel
            .WithText(
                _driver,
                "If you’re in Northern Ireland, for urgent medical advice contact your GP out of hours service.")
            .ScrollIntoView();

        private AndroidLink GoToGpOutOfHoursServiceLink => AndroidLink
            .WithContentDescription(
                _driver,
                "Go to the GP out of hours service for Northern Ireland")
            .ScrollIntoView();
        private AndroidLabel IfSurgeryInEnglandNotWithArmedForcesText => AndroidLabel
            .WithText(
                _driver,
                "If you’re registered with a GP surgery in England that is not with the armed forces")
            .ScrollIntoView();
        private AndroidLabel WeCannotContactYourGpSurgeryText => AndroidLabel
            .WithText(
                _driver,
                "We cannot connect to your GP surgery.")
            .ScrollIntoView();

        private AndroidLink ErrorCodeLink => AndroidLink
            .WhichMatches(_driver, "Contact us, quoting error code 3f[0-9a-z]{4}")
            .ScrollIntoView();

        public static AndroidCreateSessionOdsCodeNotSupportedErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionOdsCodeNotSupportedErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidCreateSessionOdsCodeNotSupportedErrorPage AssertPageElements()
        {
            IfNotRegisteredOrArmedForceText.AssertVisible();
            FindOutHowToGetCovidPassLink.AssertVisible();
            IfYourGpSurgeryIsInWalesText.AssertVisible();

            NhsAppNotAvailableInWalesText.AssertVisible();
            IfYouNeedText.AssertVisible();
            GoToMyHealthOnlineLink.AssertVisible();
            ForUrgentMedicalAdviceText.AssertVisible();

            OneOneOneWalesLink.AssertVisible();
            IfRegisteredWithArmedForcesSurgeryText.AssertVisible();
            NotAvailableToArmedForcesText.AssertVisible();

            IfSurgeryInScotlandOrNorthernIrelandText.AssertVisible();
            NotAvailableInNorthernIrelandOrScotlandText.AssertVisible();
            IfYouNeedAnAppointmentOrPrescriptionText.AssertVisible();
            IfInScotlandText.AssertVisible();
            IfInNorthernIrelandText.AssertVisible();
            GoToGpOutOfHoursServiceLink.AssertVisible();

            IfSurgeryInEnglandNotWithArmedForcesText.AssertVisible();
            WeCannotContactYourGpSurgeryText.AssertVisible();

            ErrorCodeLink.AssertVisible();

            return this;
        }
    }
}