using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Wayfinder
{
    public static class AndroidBlueScreenInterruptPage
    {
        public static void AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidBlueScreenInterruptPage));
            }
        }
    }
}