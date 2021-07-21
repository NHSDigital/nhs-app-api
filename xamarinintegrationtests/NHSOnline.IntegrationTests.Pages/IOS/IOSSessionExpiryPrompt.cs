using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSSessionExpiryPrompt
    {
        private readonly IOSPrompt _iosPrompt;

        private const string PromptLabelText = "For security reasons, we'll log you out of the NHS App in 1 minute.";
        private const string AcceptButtonText = "Stay logged in";
        private const string CancelButtonText = "Log out";

        private IOSSessionExpiryPrompt(IOSPrompt iosPrompt)
        {
            _iosPrompt = iosPrompt;
        }

        public static IOSSessionExpiryPrompt AssertDisplayed(IIOSDriverWrapper driver)
        {
            var prompt = IOSPrompt.AssertDisplayed(driver, PromptLabelText, AcceptButtonText, CancelButtonText);
            var leavePrompt = new IOSSessionExpiryPrompt(prompt);
            return leavePrompt;
        }

        public void ExtendSession() => _iosPrompt.Accept();

        public void Logout() => _iosPrompt.Cancel();
    }
}