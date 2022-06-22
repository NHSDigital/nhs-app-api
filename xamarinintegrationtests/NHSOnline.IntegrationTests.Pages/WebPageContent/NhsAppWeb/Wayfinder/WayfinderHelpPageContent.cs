using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Wayfinder
{
    public sealed class WayfinderHelpPageContent
    {
        private readonly IWebInteractor _interactor;
        private IEnumerable<IFocusable>? _focusableElements;

        internal WayfinderHelpPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebLink BackButton => WebLink.WithText(_interactor, "Back");

        public IEnumerable<IFocusable> FocusableElements
        {
            get
            {
                _focusableElements = new IFocusable[]
                {
                    BackButton,
                };

                return _focusableElements;
            }
        }

        public void NavigateViaBackButton() =>
            BackButton.Click();

        public void NavigateViaBackButton(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(BackButton, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(BackButton, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}