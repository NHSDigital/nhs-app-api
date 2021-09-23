using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings.LegalAndCookies
{
    public class LegalAndCookiesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal LegalAndCookiesPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Legal and cookies");

        private WebMenuItem ManageCookiesMenuItem => WebMenuItem.WithTitle(_interactor, "Manage cookies");

        private WebMenuItem TermsOfUseMenuItem => WebMenuItem.WithTitle(_interactor, "Terms of use");

        private WebMenuItem PrivacyPolicyMenuItem => WebMenuItem.WithTitle(_interactor, "Privacy policy");

        private WebMenuItem AccessibilityStatementMenuItem => WebMenuItem.WithTitle(_interactor, "Accessibility statement");

        private WebMenuItem OpenSourceLicencesMenuItem => WebMenuItem.WithTitle(_interactor, "Open source licences");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            ManageCookiesMenuItem,
            TermsOfUseMenuItem,
            PrivacyPolicyMenuItem,
            AccessibilityStatementMenuItem,
            OpenSourceLicencesMenuItem
        };

        internal void AssertOnPage()
        {
            TitleText.ScrollTo();
            TitleText.AssertVisible();
        }

        public void KeyboardNavigateToManageCookies(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateFocusable(ManageCookiesMenuItem, navigation);

        private void KeyboardNavigateToAndActivateFocusable(IFocusable focusable, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(ManageCookiesMenuItem, focusable);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}