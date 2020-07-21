using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Login failed");

        private AndroidLabel WalesTitle => AndroidLabel
            .WithText(
                _driver,
                "If your GP surgery is in Wales")
            .ScrollIntoView();
        private AndroidLabel WalesNotAvailableText => AndroidLabel
            .WithText(
                _driver,
                "The NHS App is not available in Wales because health services are managed separately from England.")
            .ScrollIntoView();
        private AndroidLabel WalesIfYouNeedText => AndroidLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, go to My Health Online or contact your GP surgery directly. " +
                "For urgent medical advice, go to 111.wales.nhs.uk or call 111.")
            .ScrollIntoView();

        private AndroidLabel EnglandTitle => AndroidLabel
            .WithText(
                _driver,
                "If your GP surgery is in England")
            .ScrollIntoView();
        private AndroidLabel EnglandNoSurgeryText => AndroidLabel
            .WithText(
                _driver,
                "Either we cannot connect to your GP surgery, or we cannot match your NHS number to a GP surgery.")
            .ScrollIntoView();
        private AndroidLabel EnglandIfYouNeedText => AndroidLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, contact your GP surgery directly. " +
                "For urgent medical advice, go to 111.nhs.uk or call 111.")
            .ScrollIntoView();
        private AndroidLabel EnglandIfYouNeedAppHelpText => AndroidLabel
            .WithText(
                _driver,
                "If you still need help to access the app, contact us.")
            .ScrollIntoView();

        private AndroidLabel NorthernIrelandOrScotlandTitle => AndroidLabel
            .WithText(
                _driver,
                "If your GP surgery is in Northern Ireland or Scotland")
            .ScrollIntoView();
        private AndroidLabel NorthernIrelandOrScotlandNotAvailableText => AndroidLabel
            .WithText(
                _driver,
                "The NHS App is not available in Northern Ireland or Scotland because health services are managed separately from England.")
            .ScrollIntoView();
        private AndroidLabel NorthernIrelandOrScotlandIfYouNeedText => AndroidLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, contact your GP surgery directly." +
                " For urgent medical advice, call 111.")
            .ScrollIntoView();

        private AndroidLabel ErrorCodeText => AndroidLabel.WhichMatches(_driver, "Reference: 3f[0-9a-z]{4}");

        public static AndroidCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage AssertPageElements()
        {
            WalesTitle.AssertVisible();
            WalesNotAvailableText.AssertVisible();
            WalesIfYouNeedText.AssertVisible();

            EnglandTitle.AssertVisible();
            EnglandNoSurgeryText.AssertVisible();
            EnglandIfYouNeedText.AssertVisible();
            EnglandIfYouNeedAppHelpText.AssertVisible();

            NorthernIrelandOrScotlandTitle.AssertVisible();
            NorthernIrelandOrScotlandNotAvailableText.AssertVisible();
            NorthernIrelandOrScotlandIfYouNeedText.AssertVisible();

            ErrorCodeText.AssertVisible();

            return this;
        }
    }
}