using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionNoNhsNumberErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionNoNhsNumberErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Login failed");

        private IOSLabel CannotMatchNhsNumberText => IOSLabel
            .WithText(
                _driver,
                "We cannot match your NHS number to a GP surgery.")
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

        private IOSLink GoToOneOneOneWales => IOSLink
            .WithText(
                _driver,
                "Go to 111.nhs.uk");

        private IOSLink ErrorCodeLink => IOSLink
            .WhichMatches(_driver, "Contact us, quoting error code 3 u ([0-9a-z] ){4}");

        public static IOSCreateSessionNoNhsNumberErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionNoNhsNumberErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSCreateSessionNoNhsNumberErrorPage AssertPageElements()
        {
            CannotMatchNhsNumberText.AssertVisible();
            IfYouNeedToBookAnAppointmentOrPrescriptionText.AssertVisible();
            ForUrgentMedicalAdviceText.AssertVisible();
            GoToOneOneOneWales.AssertVisible();
            ErrorCodeLink.AssertVisible();

            return this;
        }
    }
}