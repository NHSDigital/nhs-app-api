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
        private WebMenuItem GpSurgeryMessages => WebMenuItem.WithTitle(_interactor, "GP surgery messages");
        private WebMenuItem ConsultationsEventsAndMessages => WebMenuItem.WithTitle(_interactor, "Consultation, events and messages");
        private WebMenuItem TestProvider => WebMenuItem.WithTitle(_interactor, "Test Provider");
        private WebMenuItem HealthInfoAndUpdates => WebMenuItem.WithTitle(_interactor, "Health information and updates");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GpSurgeryMessages,
            ConsultationsEventsAndMessages,
            TestProvider,
            HealthInfoAndUpdates
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
            TestProvider.Click();
        }

        public void KeyboardNavigateToTestProvider(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(TestProvider, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(GpSurgeryMessages, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
