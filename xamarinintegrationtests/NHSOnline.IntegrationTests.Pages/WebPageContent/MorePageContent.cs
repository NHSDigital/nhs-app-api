using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class MorePageContent
    {
        private readonly IWebInteractor _interactor;

        internal MorePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "More");

        private WebMenuItem LinkedProfilesMenuItem => WebMenuItem.WithTitle(_interactor, "Linked profiles");

        private WebMenuItem CookiesMenuItem => WebMenuItem.WithTitle(_interactor, "Cookies");

        private WebMenuItem BiometricMenuItem => WebMenuItem.WithTitle(_interactor, "Fingerprint");

        private WebMenuItem NhsLoginMenuItem => WebMenuItem.WithTitle(_interactor, "NHS login");

        private WebMenuItem NotificationsMenuItem => WebMenuItem.WithTitle(_interactor, "Notifications");

        private WebMenuItem GncrPreferencesMenuItem =>
            WebMenuItem.WithTitle(_interactor, "Great North Care Record service preferences");

        private WebMenuItem PatientParticipationGroupsMenuItem =>
            WebMenuItem.WithTitle(_interactor, "Patient participation groups");

        private WebAnalyticsTag LogoutButtonAnalyticsTag =>
            WebAnalyticsTag.WithTagAndDescription("div", "Log out analytics tag");

        private WebButton LogoutButton => WebButton.WithText(_interactor, "Log out");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            LinkedProfilesMenuItem,
            CookiesMenuItem,
            BiometricMenuItem,
            NhsLoginMenuItem,
            NotificationsMenuItem,
            GncrPreferencesMenuItem,
            PatientParticipationGroupsMenuItem,
            LogoutButtonAnalyticsTag,
            LogoutButton
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            AssertOnPage();
            CookiesMenuItem.AssertVisible();
            NhsLoginMenuItem.AssertVisible();
            NotificationsMenuItem.AssertVisible();
            LogoutButton.AssertVisible();
        }

        public void NavigateToNotifications() => NotificationsMenuItem.Click();

        public void NavigateToNhsLogin() => NhsLoginMenuItem.Click();

        public void Logout() => LogoutButton.Click();

        public void KeyboardNavigateToNhsLoginSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(NhsLoginMenuItem, navigation);

        public void KeyboardNavigateToNotificationSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(NotificationsMenuItem, navigation);

        public void KeyboardNavigateToLogOut(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(LogoutButton, navigation);

        private void KeyboardNavigateToAndActivateFocusable(IFocusable focusable, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(LinkedProfilesMenuItem, focusable);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
