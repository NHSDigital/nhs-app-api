using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSPrompt
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly string _promptText;
        private readonly string _acceptButtonText;
        private readonly string _cancelButtonText;

        private IOSPrompt(
            IIOSDriverWrapper driver,
            string promptText,
            string acceptButtonText,
            string cancelButtonText)
        {
            _driver = driver;
            _promptText = promptText;
            _acceptButtonText = acceptButtonText;
            _cancelButtonText = cancelButtonText;
        }

        private IOSLabel PromptText => IOSLabel.WithText(_driver, _promptText);

        private IOSButton AcceptButton => IOSButton.WithText(_driver, _acceptButtonText);

        private IOSButton CancelButton => IOSButton.WithText(_driver, _cancelButtonText);

        public static IOSPrompt AssertDisplayed(IIOSDriverWrapper driver, string promptText, string acceptButtonText, string cancelButtonText)
        {
            var prompt = new IOSPrompt(driver, promptText, acceptButtonText, cancelButtonText);
            prompt.PromptText.AssertVisible();
            return prompt;
        }

        public void Accept() => AcceptButton.Click();

        public void Cancel() => CancelButton.Click();
    }
}