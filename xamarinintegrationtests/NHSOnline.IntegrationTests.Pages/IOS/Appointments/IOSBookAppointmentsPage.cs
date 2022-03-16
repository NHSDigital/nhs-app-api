using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Appointments;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Appointments
{
    public class IOSBookAppointmentsPage
    {

        private readonly IIOSDriverWrapper _driver;
        public BookAppointmentsPageContent PageContent { get; }
        private IOSPickerWheel SelectType => new IOSPickerWheel(_driver);

        private IOSButton PickerDoneButton => IOSButton.WithText(_driver, "Done");

        private IOSBookAppointmentsPage(IIOSDriverWrapper driver)
        {
            PageContent = new BookAppointmentsPageContent(driver.Web.NhsAppLoggedInWebView());
            _driver = driver;
        }

        public static IOSBookAppointmentsPage AssertOnPage(IIOSDriverWrapper driver, bool screenshot = false)
        {
            var page = new IOSBookAppointmentsPage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSBookAppointmentsPage));
            }

            return page;
        }

        public void SetPickerValue(string value)
        {
            SelectType.SetValue(value);
            SelectType.Click();
            PickerDoneButton.Click();
        }

        public void ScrollAndScreenshot()
        {
            PageContent.AppointmentBookingText.ScrollTo();
            _driver.Screenshot($"{nameof(IOSBookAppointmentsPage)}_scrolled");
        }
    }
}