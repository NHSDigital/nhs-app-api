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
        private AndroidLabel GoBackText => AndroidLabel.WithText(_driver, "Go back and try logging in again.");
        private AndroidLabel IfYouNeedText => AndroidLabel.WithText(_driver, "If you need to book an appointment or get a prescription now, contact your GP surgery directly.");
        private AndroidLabel ForUrgentMedicalAdvice => AndroidLabel.WithText(_driver, "For urgent medical advice, use NHS 111 online or call 111.");
        private AndroidLink GoTo111Link => AndroidLink.WithContentDescription(_driver, "Go to 111.nhs.uk");
        private AndroidLink ContactUsLink => AndroidLink.WhichMatches(_driver, "Contact us if you keep seeing this message, quoting error code 3w([0-9a-z]){4}").ScrollIntoView();
        private AndroidLink BackToLoginLink => AndroidLink.WithContentDescription(_driver, "Back to login").ScrollIntoView();

        private IEnumerable<IFocusable> GetAllKeyboardNavigationFocusableElements()
        {
            var headerList = Navigation.KeyboardNavigation.GetFocusableElements();
            var pageFocusableList = new[] {GoTo111Link, ContactUsLink, BackToLoginLink};

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
            CannotGetDetailsText.AssertVisible();
            GoBackText.AssertVisible();
            IfYouNeedText.AssertVisible();
            ForUrgentMedicalAdvice.AssertVisible();
            GoTo111Link.AssertVisible();
            ContactUsLink.AssertVisible();
            BackToLoginLink.AssertVisible();
            return this;
        }

        public void ContactUs() => ContactUsLink.Touch();
        public void BackToLogin() => BackToLoginLink.Touch();

        public void KeyboardNavigateToAndActivateContactUs() => KeyboardNavigateToAndActivateFocusable(ContactUsLink);
        public void KeyboardNavigateToAndActivateBackToLogin() => KeyboardNavigateToAndActivateFocusable(BackToLoginLink);

        private void KeyboardNavigateToAndActivateFocusable(IFocusable focusable)
        {
            KeyboardPageContentNavigation.TabTo(focusable);
            KeyboardPageContentNavigation.PressEnterKey();
        }
    }
}