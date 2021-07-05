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

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Silver Integration Test Provider Go To Page");

        private WebButton GoToHomePageButton => WebButton.WithText(_interactor, $"Go to Home");
        private WebButton GoToAdvicePageButton => WebButton.WithText(_interactor, $"Go to Advice");
        private WebButton GoToAppointmentsPageButton => WebButton.WithText(_interactor, $"Go to Appointments");
        private WebButton GoToPrescriptionsPageButton => WebButton.WithText(_interactor, $"Go to Prescriptions");
        private WebButton GoToHealthRecordsPageButton => WebButton.WithText(_interactor, $"Go to Health Records");
        private WebButton GoToMessagesPageButton => WebButton.WithText(_interactor, $"Go to Messages");
        private WebButton GoToMorePageButton => WebButton.WithText(_interactor, $"Go to More");
        private WebButton GoToSettingsPageButton => WebButton.WithText(_interactor, $"Go to Settings");
        private WebButton GoToUpliftButton => WebButton.WithText(_interactor, "Go to Uplift");
        private WebButton GoToInvalidPageButton => WebButton.WithText(_interactor, $"Go to Invalid Page");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GoToHomePageButton,
            GoToAdvicePageButton,
            GoToAppointmentsPageButton,
            GoToPrescriptionsPageButton,
            GoToHealthRecordsPageButton,
            GoToMessagesPageButton,
            GoToMorePageButton,
            GoToSettingsPageButton,
            GoToUpliftButton,
            GoToInvalidPageButton
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void GoToYourHealthPage() => GoToHealthRecordsPageButton.Click();

        public void GoToYourAdvicePage() => GoToAdvicePageButton.Click();

        public void GoToSettingsPage() => GoToSettingsPageButton.Click();

        public void GoToMorePage() => GoToMorePageButton.Click();

        public void GoToUpliftPage() => GoToUpliftButton.Click();

        public void KeyboardNavigateToGoToHome(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToHomePageButton, navigation);

        public void KeyboardNavigateToGoToPrescriptions(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToPrescriptionsPageButton, navigation);

        public void KeyboardNavigateToGoToMessages(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToMessagesPageButton, navigation);

        public void KeyboardNavigateToGoToInvalidPage(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToInvalidPageButton, navigation);

        public void KeyboardNavigateToGoToAppointments(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateButton(GoToAppointmentsPageButton, navigation);

        private void KeyboardNavigateToAndActivateButton(IFocusable button, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(FocusableElements.First(), button);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}