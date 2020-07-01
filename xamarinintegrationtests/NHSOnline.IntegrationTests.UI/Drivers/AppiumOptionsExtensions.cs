using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal static class AppiumOptionsExtensions
    {
        internal static void AddAdditionalCapabilityIf(
            this AppiumOptions appiumOptions,
            bool condition,
            string capabilityName,
            string capabilityValue)
        {
            if (condition)
            {
                appiumOptions.AddAdditionalCapability(capabilityName, capabilityValue);
            }
        }
    }
}