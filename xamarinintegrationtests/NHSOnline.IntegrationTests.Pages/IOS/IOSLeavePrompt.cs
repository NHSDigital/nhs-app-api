using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSLeavePrompt
    {
        private readonly IOSPrompt _iosPrompt;

        private const string PromptLabelText = "Leave this page?";
        private const string AcceptButtonText = "Leave page";
        private const string CancelButtonText = "Cancel";

        private IOSLeavePrompt(IOSPrompt iosPrompt)
        {
            _iosPrompt = iosPrompt;
        }

        public static IOSLeavePrompt AssertDisplayed(IIOSDriverWrapper driver)
        {
            var prompt = IOSPrompt.AssertDisplayed(driver, PromptLabelText, AcceptButtonText, CancelButtonText);
            var leavePrompt = new IOSLeavePrompt(prompt);
            return leavePrompt;
        }

        public void Leave() => _iosPrompt.Accept();

        public void Cancel() => _iosPrompt.Cancel();
    }
}