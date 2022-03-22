using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class OrderARepeatPrescriptionPageContent
    {
        private readonly IWebInteractor _interactor;

        internal OrderARepeatPrescriptionPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "What type of prescription do you want to order?");

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage()
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(15);

            TitleText.AssertVisible();
        }

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            BackBreadcrumb
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