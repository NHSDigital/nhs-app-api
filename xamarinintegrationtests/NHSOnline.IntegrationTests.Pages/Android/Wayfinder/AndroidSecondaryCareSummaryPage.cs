using System.Collections.Generic;
using System.Linq;
using NHSOnline.HttpMocks.SecondaryCare;
using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.Pages.Android.Wayfinder
{
    public class AndroidSecondaryCareSummaryPage
    {
        private AndroidFullNavigation Navigation { get; }
        private SecondaryCareSummaryPageContent PageContent { get; }

        private readonly IAndroidDriverWrapper _driver;

        private AndroidSecondaryCareSummaryPage(IAndroidDriverWrapper driver, WayfinderErrorType errorType)
        {
            _driver = driver;
            Navigation = new AndroidFullNavigation(driver);
            PageContent = new SecondaryCareSummaryPageContent(driver.Web.NhsAppLoggedInWebView(),
                errorType);
        }

        public static AndroidSecondaryCareSummaryPage AssertOnPage(IAndroidDriverWrapper driver,
            bool screenshot = false,
            WayfinderErrorType errorType = WayfinderErrorType.none)
        {
            var page = new AndroidSecondaryCareSummaryPage(driver, errorType);
            page.PageContent.AssertPageElements();

            if (screenshot)
            {
                driver.Screenshot(nameof(AndroidSecondaryCareSummaryPage));
            }

            return page;
        }

        public void ScrollToMissingOrIncorrectReferralsAppointmentsHelpPageLinkAndScreenshotThenClick()
        {
            PageContent.MissingOrIncorrectReferralsOrAppointmentsMenuItem.ScrollTo();
            _driver.Screenshot($"{nameof(AndroidSecondaryCareSummaryPage)}_scrolled");
            PageContent.MissingOrIncorrectReferralsOrAppointmentsMenuItem.Click();
        }

        public void ScrollToConfirmedAppointmentsHelpPageLinkAndScreenshotThenClick()
        {
            //need to scroll to the item below to get a clear screenshot here
            PageContent.InReviewReferralsMenuItem.ScrollTo();
            _driver.Screenshot($"{nameof(AndroidSecondaryCareSummaryPage)}_scrolled");
            PageContent.ConfirmedAppointmentsMenuItem.Click();
        }

        public void ScrollToReferralsInReviewHelpPageLinkAndScreenshotThenClick()
        {
            PageContent.InReviewReferralsMenuItem.ScrollTo();
            _driver.Screenshot($"{nameof(AndroidSecondaryCareSummaryPage)}_scrolled");
            PageContent.InReviewReferralsMenuItem.Click();
        }

        public void ScrollToReadyToConfirmAppointmentDeepLinkButtonAndScreenshotThenClick()
        {
            //need to go past the element to select first to get a good screenshot
            PageContent.ReadyToConfirmAppointmentDeepLinkButton.ScrollTo();
            _driver.Screenshot($"{nameof(AndroidSecondaryCareSummaryPage)}_scrolled");
            PageContent.ReadyToConfirmAppointmentDeepLinkButton.Click();
        }

        public void ScrollToCancelledAppointmentDeepLinkButtonAndScreenshotThenClick()
        {
            PageContent.ScrollToConfirmedAppointmentsHeader();
            PageContent.CancelledAppointmentDeepLinkButton.ScrollTo();
            _driver.Screenshot($"{nameof(AndroidSecondaryCareSummaryPage)}_scrolled");
            PageContent.CancelledAppointmentDeepLinkButton.Click();
        }

        private AndroidKeyboardNavigation KeyboardPageContentNavigation => AndroidKeyboardNavigation
            .WithExpectedFocusableElements(_driver, GetAllKeyboardHomeNavigationFocusableElements());

        private IEnumerable<IFocusable> GetAllKeyboardHomeNavigationFocusableElements()
        {
            var headerFocusableList = Navigation.KeyboardHeaderNavigation.GetFocusableElements();
            var footerFocusableList = Navigation.KeyboardFooterNavigation.GetFocusableElements();
            var pageFocusableList = PageContent.FocusableElements;

            return pageFocusableList.Concat(footerFocusableList).Concat(headerFocusableList);
        }
    }
}

