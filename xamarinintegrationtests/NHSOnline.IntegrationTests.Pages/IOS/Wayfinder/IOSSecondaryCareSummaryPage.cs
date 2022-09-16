using NHSOnline.HttpMocks.SecondaryCare;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Wayfinder
{
    public class IOSSecondaryCareSummaryPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSSecondaryCareSummaryPage(IIOSDriverWrapper driver, WayfinderErrorType errorType)
        {
            Navigation = new IOSFullNavigation(driver);
            PageContent = new SecondaryCareSummaryPageContent(driver.Web.NhsAppLoggedInWebView(), errorType);
            _driver = driver;
        }

        private IOSFullNavigation Navigation { get; }

        private SecondaryCareSummaryPageContent PageContent { get; }

        private IOSButton ReadyToConfirmAppointmentDeepLinkButton => IOSButton.WithText(
            _driver,
            "Contact the clinic to confirm"
        );

        private IOSLink CancelledAppointmentDeepLink => IOSLink.WithText(
            _driver,
            "View this appointment"
        );

        public static IOSSecondaryCareSummaryPage AssertOnPage(IIOSDriverWrapper driver,
            bool screenshot = false,
            WayfinderErrorType errorType = WayfinderErrorType.none)
        {
            var page = new IOSSecondaryCareSummaryPage(driver, errorType);
            page.PageContent.AssertPageElements();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSSecondaryCareSummaryPage));
            }

            return page;
        }

        public void ScrollToMissingOrIncorrectReferralsAppointmentsHelpPageLinkAndScreenshotThenClick()
        {
            PageContent.MissingOrIncorrectReferralsOrAppointmentsMenuItem.ScrollTo();
            _driver.Screenshot($"{nameof(IOSSecondaryCareSummaryPage)}_scrolled");
            PageContent.MissingOrIncorrectReferralsOrAppointmentsMenuItem.Click();
        }

        public void ScrollToAppointmentsHelpPageLinkAndScreenshotThenClick()
        {
            //need to scroll to the item below to get a clear screenshot here
            PageContent.InReviewReferralsMenuItem.ScrollTo();
            _driver.Screenshot($"{nameof(IOSSecondaryCareSummaryPage)}_scrolled");
            PageContent.AppointmentsMenuItem.Click();
        }

        public void ScrollToReferralsInReviewHelpPageLinkAndScreenshotThenClick()
        {
            PageContent.InReviewReferralsMenuItem.ScrollTo();
            _driver.Screenshot($"{nameof(IOSSecondaryCareSummaryPage)}_scrolled");
            PageContent.InReviewReferralsMenuItem.Click();
        }

        public void ScrollToReadyToConfirmAppointmentDeepLinkButtonAndScreenshotThenClick()
        {
            ReadyToConfirmAppointmentDeepLinkButton.ScrollIntoView();
            _driver.Screenshot($"{nameof(IOSSecondaryCareSummaryPage)}_scrolled");
            ReadyToConfirmAppointmentDeepLinkButton.Click();
        }

        public void ScrollToCancelledAppointmentDeepLinkButtonAndScreenshotThenClick()
        {
            PageContent.ScrollToAppointmentsHeader();
            CancelledAppointmentDeepLink.ScrollIntoView();
            _driver.Screenshot($"{nameof(IOSSecondaryCareSummaryPage)}_scrolled");
            CancelledAppointmentDeepLink.Touch();
        }
    }
}

