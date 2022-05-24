using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidCreateSessionNoNhsNumberErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionNoNhsNumberErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Cannot log in");

        private AndroidLabel CannotMatchNhsNumberText => AndroidLabel
            .WithText(
                _driver,
                "We cannot match your NHS number to a GP surgery.")
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
        private AndroidLink GoToOneOneOneWales => AndroidLink
            .WithContentDescription(
                _driver,
                "Go to 111.nhs.uk")
            .ScrollIntoView();
        private AndroidLink ErrorCodeLink => AndroidLink
            .WhichMatches(_driver, "Contact us, quoting error code 3u[0-9a-z]{4}")
            .ScrollIntoView();

        public static AndroidCreateSessionNoNhsNumberErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionNoNhsNumberErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidCreateSessionNoNhsNumberErrorPage AssertPageElements()
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