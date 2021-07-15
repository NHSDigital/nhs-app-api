using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidPrompt
    {
        private readonly IAndroidDriverWrapper _driver;
        private readonly string _promptText;
        private readonly string _acceptButtonText;
        private readonly string _cancelButtonText;

        private AndroidPrompt(
            IAndroidDriverWrapper driver,
            string promptText,
            string acceptButtonText,
            string cancelButtonText)
        {
            _driver = driver;
            _promptText = promptText;
            _acceptButtonText = acceptButtonText;
            _cancelButtonText = cancelButtonText;
        }

        private AndroidLabel PromptText => AndroidLabel.WithText(_driver, _promptText);

        private AndroidSystemButton AcceptButton => AndroidSystemButton.WhichMatches(_driver, _acceptButtonText);

        private AndroidSystemButton CancelButton => AndroidSystemButton.WhichMatches(_driver, _cancelButtonText);

        public static AndroidPrompt AssertDisplayed(IAndroidDriverWrapper driver, string promptText, string acceptButtonText, string cancelButtonText)
        {
            var prompt = new AndroidPrompt(driver, promptText, acceptButtonText, cancelButtonText);
            prompt.PromptText.AssertVisible();
            return prompt;
        }

        public void Accept() => AcceptButton.Click();

        public void Cancel() => CancelButton.Click();
    }
}