using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Messages
{
    public class MessagesPageContent
    {
        private readonly IWebInteractor _interactor;

        internal MessagesPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Messages");

        private WebMenuItem GpSurgeryMessagingMenuItem => WebMenuItem.WithTitle(_interactor, "GP surgery messaging");

        private WebText GpSurgeryMessagesText => WebText.WithTagAndText(_interactor, "p", "Send and view messages from staff at your GP surgery");

        private WebMenuItem AskYourGpSurgeryAQuestionMenuItem => WebMenuItem.WithTitle(_interactor, "Ask your GP surgery a question");

        private WebText AskYourGpSurgeryAQuestionText => WebText.WithTagAndText(_interactor,"p", "Fill out a form to send a request, get advice or ask a question");

        private WebMenuItem OnlineConsultationsMenuItem => WebMenuItem.WithTitle(_interactor, "Online consultations");

        private WebText OnlineConsultationsText => WebText.WithTagAndText(_interactor, "p", "View your online consultations and any responses from your GP surgery");

        private WebMenuItem ConsultationsEventsAndMessagesPkbMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages", "btn_pkb_messages_and_consultations");

        private WebText ConsultationsEventsAndMessagesPkbText => WebText.WithTagAndText(_interactor, "p", "See details of your visits and treatments, view clinical documents, message your health team, or fill in a consultation form");

        private WebMenuItem ConsultationsEventsAndMessagesCieMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages", "btn_pkb_cie_messages_and_consultations");

        private WebText ConsultationsEventsAndMessagesCieText => WebText.WithTagAndText(_interactor, "p", "See details of your visits and treatments, view clinical documents, message your health team, or fill in a consultation form");

        private WebMenuItem ConsultationsEventsAndMessagesMyCareViewMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages", "btn_pkb_my_care_view_messages_and_consultations");

        private WebText ConsultationsEventsAndMessagesMyCareViewText => WebText.WithTagAndText(_interactor, "p", "See details of your visits and treatments, view clinical documents, message your health team, or fill in a consultation form");

        private WebMenuItem ConsultationsEventsAndMessagesSecondaryCareViewMenuItem => WebMenuItem.WithTitle(_interactor, "Consultations, events and messages", "btn_pkb_secondary_care_messages_and_consultations");

        private WebText ConsultationsEventsAndMessagesSecondaryCareViewText => WebText.WithTagAndText(_interactor, "p", "See details of your visits and treatments, view clinical documents, message your health team, or fill in a consultation form");

        private WebMenuItem TestProviderMenuItem => WebMenuItem.WithTitle(_interactor, "Test Provider");

        private WebMenuItem YourHealthServiceMessagesMenuItem => WebMenuItem.WithTitle(_interactor, "Your health service messages");

        private WebText YourHealthServiceMessagesText => WebText.WithTagAndText(_interactor, "p", "View messages from the NHS App and connected healthcare providers, like your GP surgery");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            GpSurgeryMessagingMenuItem,
            AskYourGpSurgeryAQuestionMenuItem,
            OnlineConsultationsMenuItem,
            ConsultationsEventsAndMessagesPkbMenuItem,
            ConsultationsEventsAndMessagesCieMenuItem,
            ConsultationsEventsAndMessagesMyCareViewMenuItem,
            ConsultationsEventsAndMessagesSecondaryCareViewMenuItem,
            TestProviderMenuItem,
            YourHealthServiceMessagesMenuItem
        };

        internal void AssertOnPage()
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(15);

            TitleText.AssertVisible();
        }

        public MessagesPageContent AssertSubstraktElements()
        {
            AskYourGpSurgeryAQuestionMenuItem.AssertVisible();
            AskYourGpSurgeryAQuestionText.AssertVisible();
            return this;
        }

        public MessagesPageContent AssertEngageElements()
        {
            OnlineConsultationsMenuItem.AssertVisible();
            OnlineConsultationsText.AssertVisible();
            return this;
        }

        public MessagesPageContent AssertPkbElements()
        {
            ConsultationsEventsAndMessagesPkbMenuItem.AssertVisible();
            ConsultationsEventsAndMessagesPkbText.AssertVisible();
            return this;
        }

        public MessagesPageContent AssertCieElements()
        {
            ConsultationsEventsAndMessagesCieMenuItem.AssertVisible();
            ConsultationsEventsAndMessagesCieText.AssertVisible();
            return this;
        }

        public MessagesPageContent AssertMyCareViewElements()
        {
            ConsultationsEventsAndMessagesMyCareViewMenuItem.AssertVisible();
            ConsultationsEventsAndMessagesMyCareViewText.AssertVisible();
            return this;
        }

        public MessagesPageContent AssertSecondaryCareViewElements()
        {
            ConsultationsEventsAndMessagesSecondaryCareViewMenuItem.AssertVisible();
            ConsultationsEventsAndMessagesSecondaryCareViewText.AssertVisible();
            return this;
        }

        public MessagesPageContent AssertPageElements()
        {
            GpSurgeryMessagingMenuItem.AssertVisible();
            GpSurgeryMessagesText.AssertVisible();
            YourHealthServiceMessagesMenuItem.AssertVisible();
            YourHealthServiceMessagesText.AssertVisible();
            return this;
        }

        public void NavigateToTestProvider() => TestProviderMenuItem.Click();

        public void NavigateToConsultationsEventsAndMessagesPkb() => ConsultationsEventsAndMessagesPkbMenuItem.Click();

        public void KeyboardNavigateToGpSurgeryMessages(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(GpSurgeryMessagingMenuItem, navigation);

        public void KeyboardNavigateToSubstrakt(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(AskYourGpSurgeryAQuestionMenuItem, navigation);

        public void KeyboardNavigateToGncr(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(OnlineConsultationsMenuItem, navigation);

        public void KeyboardNavigateToPkb(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(ConsultationsEventsAndMessagesPkbMenuItem, navigation);

        public void KeyboardNavigateToCie(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(ConsultationsEventsAndMessagesCieMenuItem, navigation);

        public void KeyboardNavigateToMyCareView(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(ConsultationsEventsAndMessagesMyCareViewMenuItem, navigation);

        public void KeyboardNavigateToSecondaryCareView(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(ConsultationsEventsAndMessagesSecondaryCareViewMenuItem, navigation);

        public void KeyboardNavigateToYourHealthServiceMessages(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(YourHealthServiceMessagesMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(GpSurgeryMessagingMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
