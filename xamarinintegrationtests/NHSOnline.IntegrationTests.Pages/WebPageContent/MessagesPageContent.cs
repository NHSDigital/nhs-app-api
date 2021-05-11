using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class MessagesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal MessagesPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Messages");
        private WebMenuItem GpSurgeryMessagesMenuItem => WebMenuItem.WithTitle(_interactor, "GP surgery messages");
        private WebMenuItem ConsultationsEventsAndMessagesMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages");
        private WebMenuItem TestProviderMenuItem => WebMenuItem.WithTitle(_interactor, "Test Provider");
        private WebMenuItem HealthInfoAndUpdatesMenuItem => WebMenuItem.WithTitle(_interactor, "Health information and updates");
        private WebMenuItem AskYourGpSurgeryMenuItem => WebMenuItem.WithTitle(_interactor, "Ask your GP surgery a question");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GpSurgeryMessagesMenuItem,
            ConsultationsEventsAndMessagesMenuItem,
            TestProviderMenuItem,
            HealthInfoAndUpdatesMenuItem
        };

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public MessagesPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }

        public void NavigateToTestProvider()
        {
            TestProviderMenuItem.Click();
        }

        public void NavigateToConsultationsEventsAndMessages()
        {
            ConsultationsEventsAndMessagesMenuItem.Click();
        }

        public void NavigateToAskYourGpSurgeryAQuestion()
        {
            AskYourGpSurgeryMenuItem.Click();
        }

        public void KeyboardNavigateToTestProvider(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(TestProviderMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(GpSurgeryMessagesMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
