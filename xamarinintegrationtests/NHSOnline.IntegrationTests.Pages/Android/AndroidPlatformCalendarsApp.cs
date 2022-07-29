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

        private AndroidLabel StartText =>
            AndroidLabel.WithResourceId(_driver, "com.samsung.android.calendar:id/start_date_time");

        private AndroidLabel EndText =>
            AndroidLabel.WithResourceId(_driver, "com.samsung.android.calendar:id/end_date_time");

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
            AssertStartText();
            AssertEndText();
        }

        private void AssertStartText()
        {
            StartText.AssertVisible();
            StartText.AssertTextContains("Wed");
            StartText.AssertTextContains("Jan");
            StartText.AssertTextContains("2030");
            StartText.AssertTextContains("13:00");
        }

        private void AssertEndText()
        {
            EndText.AssertVisible();
            EndText.AssertTextContains("Wed");
            EndText.AssertTextContains("Jan");
            EndText.AssertTextContains("2030");
            EndText.AssertTextContains("13:10");
        }
    }
}