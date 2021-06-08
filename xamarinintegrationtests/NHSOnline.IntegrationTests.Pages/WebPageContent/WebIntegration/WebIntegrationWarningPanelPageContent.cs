using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class WebIntegrationWarningPanelPageContent
    {
        private readonly IWebInteractor _interactor;
        private readonly string _pageTitle;

        internal WebIntegrationWarningPanelPageContent(IWebInteractor interactor, string pageTitle)
        {
            _interactor = interactor;
            _pageTitle = pageTitle;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "p", _pageTitle);

        private WebLink Back => WebLink.WithText(_interactor, "Back");

        private WebLink Continue => WebLink.WithText(_interactor, "Continue");
        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            Back,
            Continue,
        };

        internal void AssertOnPage() => Title.AssertVisible();

        public void NavigateToNextPage() => Continue.Click();

        public void KeyboardNavigateContinue(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(Continue, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(Back, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}