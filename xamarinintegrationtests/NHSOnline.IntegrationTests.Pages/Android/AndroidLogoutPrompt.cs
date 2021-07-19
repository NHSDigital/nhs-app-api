using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidLogoutPrompt
    {
        private readonly AndroidPrompt _androidPrompt;

        private const string PromptLabelText = "Are you sure you want to log out?";
        private const string ContinueButtonText = "Log out";
        private const string CancelButtonText = "Cancel";

        private AndroidLogoutPrompt(AndroidPrompt androidPrompt)
        {
            _androidPrompt = androidPrompt;
        }

        public static AndroidLogoutPrompt AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var prompt = AndroidPrompt.AssertDisplayed(driver, PromptLabelText, ContinueButtonText, CancelButtonText);
            var leavePrompt = new AndroidLogoutPrompt(prompt);
            return leavePrompt;
        }

        public void Logout() => _androidPrompt.Accept();

        public void Cancel() => _androidPrompt.Cancel();
    }
}