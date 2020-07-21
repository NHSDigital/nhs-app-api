using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Login failed");

        private IOSLabel WalesTitle => IOSLabel
            .WithText(
                _driver,
                "If your GP surgery is in Wales")
            .ScrollIntoView();
        private IOSLabel WalesNotAvailableText => IOSLabel
            .WithText(
                _driver,
                "The NHS App is not available in Wales because health services are managed separately from England.")
            .ScrollIntoView();
        private IOSLabel WalesIfYouNeedText => IOSLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, go to My Health Online or contact your GP surgery directly. " +
                "For urgent medical advice, go to 111.wales.nhs.uk or call 111.")
            .ScrollIntoView();

        private IOSLabel EnglandTitle => IOSLabel
            .WithText(
                _driver,
                "If your GP surgery is in England")
            .ScrollIntoView();
        private IOSLabel EnglandNoSurgeryText => IOSLabel
            .WithText(
                _driver,
                "Either we cannot connect to your GP surgery, or we cannot match your NHS number to a GP surgery.")
            .ScrollIntoView();
        private IOSLabel EnglandIfYouNeedText => IOSLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, contact your GP surgery directly. " +
                "For urgent medical advice, go to 111.nhs.uk or call 111.")
            .ScrollIntoView();
        private IOSLabel EnglandIfYouNeedAppHelpText => IOSLabel
            .WithText(
                _driver,
                "If you still need help to access the app, contact us.")
            .ScrollIntoView();

        private IOSLabel NorthernIrelandOrScotlandTitle => IOSLabel
            .WithText(
                _driver,
                "If your GP surgery is in Northern Ireland or Scotland")
            .ScrollIntoView();
        private IOSLabel NorthernIrelandOrScotlandNotAvailableText => IOSLabel
            .WithText(
                _driver,
                "The NHS App is not available in Northern Ireland or Scotland because health services are managed separately from England.")
            .ScrollIntoView();
        private IOSLabel NorthernIrelandOrScotlandIfYouNeedText => IOSLabel
            .WithText(
                _driver,
                "If you need an appointment or prescription, contact your GP surgery directly." +
                " For urgent medical advice, call 111.")
            .ScrollIntoView();

        private IOSLabel ErrorCodeText => IOSLabel.WhichMatches(_driver, "Reference: 3f[0-9a-z]{4}");

        public static IOSCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public IOSCreateSessionOdsCodeNotSupportedOrNoNhsNumberErrorPage AssertPageElements()
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