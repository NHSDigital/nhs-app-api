using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class OrderARepeatPrescriptionPageContent
    {
        private readonly IWebInteractor _interactor;

        internal OrderARepeatPrescriptionPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText ErrorTitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Sorry, there is a problem getting your repeat prescription information");

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebButton TryAgainButton => WebButton.WithText(_interactor, "Try Again");

        internal void AssertOnPage() => ErrorTitleText.AssertVisible();

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            BackBreadcrumb,
            TryAgainButton
        };

        public void KeyboardNavigateBack(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivate(BackBreadcrumb, navigation);

        private void KeyboardNavigateToAndActivate(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBack(menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}