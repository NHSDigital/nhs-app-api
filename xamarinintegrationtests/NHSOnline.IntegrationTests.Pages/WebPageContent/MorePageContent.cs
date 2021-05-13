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

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            LinkedProfilesMenuItem,
            CookiesMenuItem,
            BiometricMenuItem,
            NhsLoginMenuItem,
            NotificationsMenuItem
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            AssertOnPage();
            CookiesMenuItem.AssertVisible();
            NhsLoginMenuItem.AssertVisible();
            NotificationsMenuItem.AssertVisible();
        }

        public void NavigateToNotifications() => NotificationsMenuItem.Click();

        public void NavigateToNhsLogin() => NhsLoginMenuItem.Click();

        public void KeyboardNavigateToNhsLoginSettings(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(NhsLoginMenuItem, navigation);

        public void KeyboardNavigateToNotificationSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateMenuItem(NotificationsMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(LinkedProfilesMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
