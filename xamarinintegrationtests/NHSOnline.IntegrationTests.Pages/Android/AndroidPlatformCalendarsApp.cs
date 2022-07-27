using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidPlatformCalendarsApp
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidPlatformCalendarsApp(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidEditText SubjectText => AndroidEditText.WithText(_driver, "Test Subject");

        private AndroidLabel StartLabel => AndroidLabel.WithText(_driver, "Start");

        private AndroidLabel EndLabel => AndroidLabel.WithText(_driver, "End");

        private AndroidLabel TimeLabel => AndroidLabel.WithText(_driver, "Time");

        private AndroidLabel StartText => AndroidLabel.WithText(_driver, "Wed, Jan 2, 2030   13:00");

        private AndroidLabel EndText => AndroidLabel.WithText(_driver, "Wed, Jan 2, 2030   13:10");

        public static AndroidPlatformCalendarsApp AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidPlatformCalendarsApp(driver);
            page.StartLabel.AssertVisible();
            page.EndLabel.AssertVisible();
            page.TimeLabel.AssertVisible();
            return page;
        }

        public void AssertDetailsArePassed()
        {
            SubjectText.AssertVisible();
            StartText.AssertVisible();
            EndText.AssertVisible();
        }
    }
}