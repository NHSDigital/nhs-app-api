using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.WebIntegration
{
    public class AndroidCalendarPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidCalendarPage(IAndroidDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new CalendarPageContent(driver.Web.WebIntegrationWebView());
        }

        private AndroidFullNavigation Navigation { get; }

        private CalendarPageContent PageContent { get; }

        public static AndroidCalendarPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidCalendarPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public AndroidCalendarPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public AndroidCalendarPage AddCalendarDetails(int startTime, int endTime)
        {
            PageContent.AddToCalendar(startTime, endTime);
            return this;
        }

        public void AddCalendarEvent()
        {
            _driver.DismissKeyboard();

            _driver.SendKey(AndroidKeyCode.Keycode_TAB);

            _driver.SendKey(AndroidKeyCode.Keycode_ENTER);
        }
    }
}