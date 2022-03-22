using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class ViewYourOrdersPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ViewYourOrdersPageContent(IWebInteractor interactor) =>_interactor = interactor;

        private WebText ErrorTitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Prescription data error");

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage()
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(20);

            // There are no prescriptions mocks yet and so there will always be an error
            ErrorTitleText.AssertVisible();
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