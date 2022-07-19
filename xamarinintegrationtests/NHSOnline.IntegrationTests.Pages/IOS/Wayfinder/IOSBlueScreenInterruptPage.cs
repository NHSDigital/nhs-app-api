using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Wayfinder
{
    public static class IOSBlueScreenInterruptPage
    {
        public static void AssertOnPage(IIOSDriverWrapper driver,
            bool screenshot = false)
        {
            if (screenshot)
            {
                driver.Screenshot(nameof(IOSSecondaryCareSummaryPage));
            }
        }
    }
}

