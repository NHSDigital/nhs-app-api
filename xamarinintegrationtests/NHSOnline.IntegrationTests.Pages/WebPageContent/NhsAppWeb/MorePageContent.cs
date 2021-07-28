using System;
using System.Collections.Generic;
using System.Threading;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
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

        private WebMenuItem AccountAndSettingsMenuItem => WebMenuItem.WithTitle(_interactor, "Account and settings");

        private WebMenuItem GncrPreferencesMenuItem =>
            WebMenuItem.WithTitle(_interactor, "Great North Care Record service preferences");

        private WebMenuItem PatientParticipationGroupsMenuItem =>
            WebMenuItem.WithTitle(_interactor, "Join a patient participation group");

        private WebMenuItem HelpAndSupportMenuItem =>
            WebMenuItem.WithTitle(_interactor, "Help and support");

        private WebAnalyticsTag LogoutButtonAnalyticsTag =>
            WebAnalyticsTag.WithTagAndDescription("div", "Log out analytics tag");

        private WebButton LogoutButton => WebButton.WithText(_interactor, "Log out");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            LinkedProfilesMenuItem,
            AccountAndSettingsMenuItem,
            GncrPreferencesMenuItem,
            PatientParticipationGroupsMenuItem,
            HelpAndSupportMenuItem,
            LogoutButtonAnalyticsTag,
            LogoutButton
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements()
        {
            AssertOnPage();
            LogoutButton.AssertVisible();
        }

        public void NavigateToAccountAndSettings()
        {
            AccountAndSettingsMenuItem.Click();
        }

        public void Logout() => LogoutButton.Click();

        public void KeyboardNavigateToAccountAndSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(AccountAndSettingsMenuItem, navigation);

        public void KeyboardNavigateToLogOut(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateFocusable(LogoutButton, navigation);

        private void KeyboardNavigateToAndActivateFocusable(IFocusable focusable, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(LinkedProfilesMenuItem, focusable);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}