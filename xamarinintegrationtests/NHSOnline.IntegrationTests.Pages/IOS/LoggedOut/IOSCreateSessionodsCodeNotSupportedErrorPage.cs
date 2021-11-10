using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionOdsCodeNotSupportedErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionOdsCodeNotSupportedErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Login failed");

        private IOSLabel IfNotRegisteredOrArmedForceText => IOSLabel
            .WithText(
                _driver,
                "If you’re not registered with a GP surgery in England, or you’re a member of the armed forces, you may still be able to get an NHS COVID Pass or proof of vaccination status.")
            .ScrollIntoView();

        private IOSLink FindOutHowToGetCovidPassLink => IOSLink
            .WithText(
                _driver,
                "Find out how to get your NHS COVID Pass or proof of vaccination status");
        private IOSLabel IfYourGpSurgeryIsInWalesText => IOSLabel
            .WithText(
                _driver,
                "If your GP surgery is in Wales")
            .ScrollIntoView();

        private IOSLabel NhsAppNotAvailableInWalesText => IOSLabel
            .WithText(
                _driver,
                "The NHS App is not available in Wales because health services are managed separately from England.")
            .ScrollIntoView();
        private IOSLabel IfYouNeedText => IOSLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, use the My Health Online service or contact your GP surgery directly.")
            .ScrollIntoView();

        private IOSLink GoToMyHealthOnlineLink => IOSLink
            .WithText(
                _driver,
                "Go to My Health Online");
        private IOSLabel ForUrgentMedicalAdviceText => IOSLabel
            .WithText(
                _driver,
                "For urgent medical advice use NHS 111 Wales online or call 111.")
            .ScrollIntoView();

        private IOSLink OneOneOneWalesLink => IOSLink
            .WithText(
                _driver,
                "Go to 111.wales.uk");
        private IOSLabel IfRegisteredWithArmedForcesSurgeryText => IOSLabel
            .WithText(
                _driver,
                "If you're registered with an armed forces GP surgery")
            .ScrollIntoView();
        private IOSLabel NotAvailableToArmedForcesText => IOSLabel
            .WithText(
                _driver,
                "The NHS App is not available to members of the armed forces because these surgeries are managed separately.")
            .ScrollIntoView();
        private IOSLabel IfSurgeryInScotlandOrNorthernIrelandText => IOSLabel
            .WithText(
                _driver,
                "If your GP surgery is in Scotland or Northern Ireland")
            .ScrollIntoView();
        private IOSLabel NotAvailableInNorthernIrelandOrScotlandText => IOSLabel
            .WithText(
                _driver,
                "The NHS App is not available in Scotland or Northern Ireland because health services are managed separately from England.")
            .ScrollIntoView();
        private IOSLabel IfYouNeedAnAppointmentOrPrescriptionText => IOSLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, contact your GP surgery directly.")
            .ScrollIntoView();
        private IOSLabel IfInScotlandText => IOSLabel
            .WithText(
                _driver,
                "If you’re in Scotland, for urgent medical advice call 111.")
            .ScrollIntoView();
        private IOSLabel IfInNorthernIrelandText => IOSLabel
            .WithText(
                _driver,
                "If you’re in Northern Ireland, for urgent medical advice contact your GP out of hours service.")
            .ScrollIntoView();

        private IOSLink GoToGpOutOfHoursServiceLink => IOSLink
            .WithText(
                _driver,
                "Go to the GP out of hours service for Northern Ireland");
        private IOSLabel IfSurgeryInEnglandNotWithArmedForcesText => IOSLabel
            .WithText(
                _driver,
                "If you’re registered with a GP surgery in England that is not with the armed forces")
            .ScrollIntoView();
        private IOSLabel WeCannotContactYourGpSurgeryText => IOSLabel
            .WithText(
                _driver,
                "We cannot connect to your GP surgery.")
            .ScrollIntoView();

        private IOSLink ErrorCodeLink => IOSLink
            .WhichMatches(_driver, "Contact us, quoting error code 3 f ([0-9a-z] ){4}")
            .ScrollIntoView();

        public static IOSCreateSessionOdsCodeNotSupportedErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionOdsCodeNotSupportedErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSCreateSessionOdsCodeNotSupportedErrorPage AssertPageElements()
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