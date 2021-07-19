using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidLeavePrompt
    {
        private readonly AndroidPrompt _androidPrompt;

        private const string PromptLabelText = "Leave this page?";
        private const string AcceptButtonText = "Leave page";
        private const string CancelButtonText = "Cancel";

        private AndroidLeavePrompt(AndroidPrompt androidPrompt)
        {
            _androidPrompt = androidPrompt;
        }

        public static AndroidLeavePrompt AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var prompt = AndroidPrompt.AssertDisplayed(driver, PromptLabelText, AcceptButtonText, CancelButtonText);
            var leavePrompt = new AndroidLeavePrompt(prompt);
            return leavePrompt;
        }

        public void Leave() => _androidPrompt.Accept();

        public void Cancel() => _androidPrompt.Cancel();
    }
}