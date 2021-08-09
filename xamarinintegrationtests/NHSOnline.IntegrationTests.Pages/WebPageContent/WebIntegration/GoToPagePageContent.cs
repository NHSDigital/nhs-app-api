using System.Collections.Generic;
using System.Linq;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class GoToPagePageContent
    {
        private readonly IWebInteractor _interactor;

        internal GoToPagePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Web Integration Functionality - Go To Page");

        private WebButton GoToHomeButton => WebButton.WithText(_interactor, "Go to Home");
        private WebButton GoToAdviceButton => WebButton.WithText(_interactor, "Go to Advice");
        private WebButton GoToAppointmentsButton => WebButton.WithText(_interactor, "Go to Appointments");
        private WebButton GoToPrescriptionsButton => WebButton.WithText(_interactor, "Go to Prescriptions");
        private WebButton GoToHealthRecordsButton => WebButton.WithText(_interactor, "Go to Health Records");
        private WebButton GoToMessagesButton => WebButton.WithText(_interactor, "Go to Messages");
        private WebButton GoToMoreButton => WebButton.WithText(_interactor, "Go to More");
        private WebButton GoToSettingsButton => WebButton.WithText(_interactor, "Go to Settings");
        private WebButton GoToUpliftButton => WebButton.WithText(_interactor, "Go to Uplift");
        private WebButton GoToInvalidPageButton => WebButton.WithText(_interactor, "Go to Invalid Page");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GoToHomeButton,
            GoToAdviceButton,
            GoToAppointmentsButton,
            GoToPrescriptionsButton,
            GoToHealthRecordsButton,
            GoToMessagesButton,
            GoToMoreButton,
            GoToSettingsButton,
            GoToUpliftButton,
            GoToInvalidPageButton
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void GoToHome() => GoToHomeButton.Click();
        public void GoToAdvice() => GoToAdviceButton.Click();
        public void GoToAppointments() => GoToAppointmentsButton.Click();
        public void GoToPrescriptions() => GoToPrescriptionsButton.Click();
        public void GoToHealthRecords() => GoToHealthRecordsButton.Click();
        public void GoToMore() => GoToMoreButton.Click();
        public void GoToSettings() => GoToSettingsButton.Click();
        public void GoToUplift() => GoToUpliftButton.Click();
        public void GoToInvalidPage() => GoToInvalidPageButton.Click();

        public void KeyboardNavigateToGoToHome(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToHomeButton, navigation);
        public void KeyboardNavigateToGoToAdvice(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToAdviceButton, navigation);
        public void KeyboardNavigateToGoToAppointments(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToAppointmentsButton, navigation);
        public void KeyboardNavigateToGoToPrescriptions(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToPrescriptionsButton, navigation);
        public void KeyboardNavigateToGoToHealthRecords(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToHealthRecordsButton, navigation);
        public void KeyboardNavigateToGoToMore(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToMoreButton, navigation);
        public void KeyboardNavigateToGoToSettings(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToSettingsButton, navigation);
        public void KeyboardNavigateToGoToUplift(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToUpliftButton, navigation);
        public void KeyboardNavigateToGoToInvalidPage(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToInvalidPageButton, navigation);

        private void KeyboardNavigateToAndActivateButton(IFocusable button, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(FocusableElements.First(), button);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}