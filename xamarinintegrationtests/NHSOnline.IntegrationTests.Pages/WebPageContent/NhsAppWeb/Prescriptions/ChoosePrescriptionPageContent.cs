using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class ChoosePrescriptionPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ChoosePrescriptionPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Order a repeat prescription");

        private WebLink BackBreadcrumb => WebLink.WithText(_interactor, "Back");

        private WebCheckbox PrescriptionCheckbox => WebCheckbox.WithLabel(_interactor, "Tablet");

        private WebTextarea SpecialRequestText => WebTextarea.WithId(_interactor, "specialRequest");
        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        public ChoosePrescriptionPageContent ChooseRepeat()
        {
            PrescriptionCheckbox.Click();
            return this;
        }

        public ChoosePrescriptionPageContent ClickPrescription()
        {
            PrescriptionCheckbox.Click();
            return this;
        }

        public void Continue() => ContinueButton.Click();

        public ChoosePrescriptionPageContent InsertSpecialRequest()
        {
            SpecialRequestText.InsertText("I need this medication");
            return this;
        }

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