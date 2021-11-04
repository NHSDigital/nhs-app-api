using NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.WebIntegration
{
    public class IOSCalendarPage
    {

        private IOSCalendarPage(IIOSDriverWrapper driver)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new CalendarPageContent(driver.Web.WebIntegrationWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public CalendarPageContent PageContent { get; }

        public static IOSCalendarPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCalendarPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSCalendarPage AssertNativeHeader()
        {
            Navigation.AssertNavigationIconsArePresent();
            return this;
        }

        public IOSCalendarPage AddCalendarDetails(decimal startTime, decimal endTime)
        {
            PageContent.AddToCalendar(startTime, endTime);
            return this;
        }
    }
}