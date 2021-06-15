using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class MessagesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal MessagesPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Messages");

        private WebMenuItem AskYourGpSurgeryAQuestionMenuItem => WebMenuItem.WithTitle(_interactor, "Ask your GP surgery a question");

        private WebMenuItem OnlineConsultationsMenuItem => WebMenuItem.WithTitle(_interactor, "Online consultations");

        private WebMenuItem ConsultationsEventsAndMessagesMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages");

        private WebMenuItem ConsultationsEventsAndMessagesSecondMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages 2");

        private WebMenuItem ConsultationsEventsAndMessagesThirdMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages 3");

        private WebMenuItem ConsultationsEventsAndMessagesFourthMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages 4");

        private WebMenuItem GpSurgeryMessagesMenuItem => WebMenuItem.WithTitle(_interactor, "GP surgery messages");

        private WebMenuItem TestProviderMenuItem => WebMenuItem.WithTitle(_interactor, "Test Provider");

        private WebMenuItem HealthInfoAndUpdatesMenuItem => WebMenuItem.WithTitle(_interactor, "Health information and updates");

        private WebMenuItem AskYourGpSurgeryMenuItem => WebMenuItem.WithTitle(_interactor, "Ask your GP surgery a question");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            AskYourGpSurgeryAQuestionMenuItem,
            OnlineConsultationsMenuItem,
            ConsultationsEventsAndMessagesMenuItem,
            ConsultationsEventsAndMessagesSecondMenuItem,
            ConsultationsEventsAndMessagesThirdMenuItem,
            ConsultationsEventsAndMessagesFourthMenuItem,
            TestProviderMenuItem,
            HealthInfoAndUpdatesMenuItem
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertPageElements() => TitleText.AssertVisible();

        public void NavigateToTestProvider() => TestProviderMenuItem.Click();

        public void NavigateToConsultationsEventsAndMessages() => ConsultationsEventsAndMessagesMenuItem.Click();

        public void NavigateToAskYourGpSurgeryAQuestion() => AskYourGpSurgeryMenuItem.Click();

        public void KeyboardNavigateToTestProvider(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(TestProviderMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(AskYourGpSurgeryAQuestionMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
