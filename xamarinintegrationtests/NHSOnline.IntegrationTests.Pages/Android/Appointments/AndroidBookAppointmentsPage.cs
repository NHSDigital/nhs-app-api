using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.Appointments
{
    public class AndroidBookAppointmentsPage
    {

        private readonly IAndroidDriverWrapper _driver;

        public BookAppointmentsPageContent PageContent { get; }

        private AndroidCheckedTextView SelectType => AndroidCheckedTextView.WithText(_driver, "Practice");

        private AndroidBookAppointmentsPage(IAndroidDriverWrapper driver)
        {
            PageContent = new BookAppointmentsPageContent(driver.Web.NhsAppLoggedInWebView());
            _driver = driver;
        }

        public static AndroidBookAppointmentsPage AssertOnPage(IAndroidDriverWrapper driver, bool screenshot = false)
        {
            var page = new AndroidBookAppointmentsPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidBookAppointmentsPage));
            }

            return page;
        }

        public void ClickType() => SelectType.Click();

        public void ScrollToBookingTextAndScreenshot()
        {
            PageContent.AppointmentBookingText.ScrollTo();
            _driver.Screenshot($"{nameof(AndroidBookAppointmentsPage)}_scrolled");
        }
    }
}