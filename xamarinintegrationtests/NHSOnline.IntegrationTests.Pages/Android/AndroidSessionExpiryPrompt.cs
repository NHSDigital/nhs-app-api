using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidSessionExpiryPrompt
    {
        private readonly AndroidPrompt _androidPrompt;

        private const string PromptLabelText = "For security reasons, we'll log you out of the NHS App in 1 minute.";
        private const string AcceptButtonText = "Stay logged in";
        private const string CancelButtonText = "Log out";

        private AndroidSessionExpiryPrompt(AndroidPrompt androidPrompt)
        {
            _androidPrompt = androidPrompt;
        }

        public static AndroidSessionExpiryPrompt AssertDisplayed(IAndroidDriverWrapper driver)
        {
            var prompt = AndroidPrompt.AssertDisplayed(driver, PromptLabelText, AcceptButtonText, CancelButtonText);
            return new AndroidSessionExpiryPrompt(prompt);
        }

        public static void ExtendIfDisplayed(IAndroidDriverWrapper driver)
        {
            try
            {
                var prompt = AndroidPrompt.AssertDisplayed(driver, PromptLabelText, AcceptButtonText, CancelButtonText);
                prompt.Accept();
            }
            catch (AssertFailedException)
            {
                Console.WriteLine("Dialog is not shown");
            }
        }

        public void ExtendSession() => _androidPrompt.Accept();

        public void Logout() => _androidPrompt.Cancel();
    }
}