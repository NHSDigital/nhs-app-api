using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings
{
    public class AccountSettingsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal AccountSettingsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Account and settings");

        private WebMenuItem BiometricMenuItem => WebMenuItem.WithTitle(_interactor, "Fingerprint");

        private WebMenuItem ManageNhsLoginMenuItem => WebMenuItem.WithTitle(_interactor, "Manage NHS login account");

        private WebMenuItem ManageNotificationsMenuItem => WebMenuItem.WithTitle(_interactor, "Manage notifications");

        private WebMenuItem CookiesMenuItem => WebMenuItem.WithTitle(_interactor, "Legal and cookies");

        private WebButton CookiesOkButton => WebButton.WithText(_interactor, "I'm OK with analytics cookies");

        private WebButton CookiesDenyButton => WebButton.WithText(_interactor, "Do not use analytics cookies");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            BiometricMenuItem,
            ManageNhsLoginMenuItem,
            ManageNotificationsMenuItem,
            CookiesMenuItem
        };

        internal void AssertOnPage()
        {
            TitleText.ScrollTo();
            TitleText.AssertVisible();
        }

        public void AssertPageElements()
        {
            AssertOnPage();
            BiometricMenuItem.AssertVisible();
            ManageNhsLoginMenuItem.AssertVisible();
            ManageNotificationsMenuItem.AssertVisible();
            CookiesMenuItem.AssertVisible();
        }

        public void NavigateToNotifications()
        {
            EnsureAnalyticsCookieAccepted();
            ManageNotificationsMenuItem.Click();
        }

        public void NavigateToNhsLogin()
        {
            EnsureAnalyticsCookieAccepted();
            ManageNhsLoginMenuItem.Click();
        }

        public void KeyboardNavigateToBiometricsSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(BiometricMenuItem, navigation);

        public void KeyboardNavigateToNhsLoginSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(ManageNhsLoginMenuItem, navigation);

        public void KeyboardNavigateToNotificationsSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(ManageNotificationsMenuItem, navigation);

        public void KeyboardNavigateToCookiesSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(CookiesMenuItem, navigation);

        private void KeyboardNavigateToAndActivateFocusable(IFocusable focusable, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(BiometricMenuItem, focusable);
            keyboardPageContentNavigation.PressEnterKey();
        }

        private void EnsureAnalyticsCookieAccepted()
        {
            if(CookiesOkButton.IsVisible())
                CookiesOkButton.Click();
        }
    }
}
