using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidNhsLoginErrorPage
    {
        private readonly IAndroidDriverWrapper _driver;
        private AndroidSlimCloseNavigation Navigation { get; }

        private AndroidNhsLoginErrorPage(IAndroidDriverWrapper driver)
        {
            Navigation = new AndroidSlimCloseNavigation(driver);
            _driver = driver;
        }

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Login failed");
        private AndroidLabel CannotGetDetailsText => AndroidLabel.WithText(_driver, "We cannot get your details from your GP surgery.");
        private AndroidLabel GoBackText => AndroidLabel.WithText(_driver, "Go back to the home screen and try logging in again.");
        private AndroidLabel ErrorCodeText => AndroidLabel.WhichMatches(_driver, "If you keep seeing this message, contact us. Quote the error code 3w[0-9a-z]{4} to help us resolve the problem more quickly.");
        private AndroidLabel IfYouNeedText => AndroidLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly. For urgent medical advice, go to 111.nhs.uk or call 111.");
        private AndroidLink ContactUsLink => AndroidLink.WithContentDescription(_driver, "Contact us").ScrollIntoView();
        private AndroidLink BackToHomeLink => AndroidLink.WithContentDescription(_driver, "Back to home").ScrollIntoView();

        private IEnumerable<IFocusable> GetAllKeyboardNavigationFocusableElements()
        {
            var headerList = Navigation.KeyboardNavigation.GetFocusableElements();
            var pageFocusableList = new[] {ContactUsLink, BackToHomeLink};

            return headerList.Concat(pageFocusableList);
        }
        private AndroidKeyboardNavigation KeyboardPageContentNavigation =>
            AndroidKeyboardNavigation.WithExpectedFocusableElements(_driver, GetAllKeyboardNavigationFocusableElements());

        public static AndroidNhsLoginErrorPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidNhsLoginErrorPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public AndroidNhsLoginErrorPage AssertPageElements()
        {
            Title.AssertVisible();
            CannotGetDetailsText.AssertVisible();
            GoBackText.AssertVisible();
            ErrorCodeText.AssertVisible();
            IfYouNeedText.AssertVisible();
            ContactUsLink.AssertVisible();
            BackToHomeLink.AssertVisible();
            return this;
        }

        public void ContactUs() => ContactUsLink.Touch();
        public void BackToHome() => BackToHomeLink.Touch();

        public void KeyboardNavigateToAndActivateContactUs() => KeyboardNavigateToAndActivateFocusable(ContactUsLink);
        public void KeyboardNavigateToAndActivateBackToHome() => KeyboardNavigateToAndActivateFocusable(BackToHomeLink);

        private void KeyboardNavigateToAndActivateFocusable(IFocusable focusable)
        {
            KeyboardPageContentNavigation.TabTo(focusable);
            KeyboardPageContentNavigation.PressEnterKey();
        }

    }
}