using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class PkbSecondaryCareHospitalAndOtherMedicinesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal PkbSecondaryCareHospitalAndOtherMedicinesPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Hospital and other medicines This service is provided by Patients Know Best");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        private WebLink FindOutMore => WebLink.WithText(_interactor, "Find out more about personal health record services");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            BackBreadcrumb,
            ContinueButton,
            FindOutMore
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