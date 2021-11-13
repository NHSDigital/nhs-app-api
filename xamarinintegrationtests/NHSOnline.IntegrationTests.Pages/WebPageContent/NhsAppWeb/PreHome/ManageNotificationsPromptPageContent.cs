using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome
{
    public sealed class ManageNotificationsPromptPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ManageNotificationsPromptPageContent(IWebInteractor webInteractor)
        {
            _interactor = webInteractor;

            AboutNotificationsExpander = WebLinkExpander.WithText(_interactor, "About notifications on your devices");

            IfYouWantToGetNotificationsText = AboutNotificationsExpander.Contains(interactor => WebText.WithTagAndText(
                interactor,
                "p",
                "If you want to get notifications, you need to turn them on for each device you use to access the NHS App."));

            IfYouShareThisDeviceText = AboutNotificationsExpander.Contains(_interactor => WebText.WithTagAndText(
                _interactor,
                "p",
                "If you share this device with other people, they may see your notifications."));
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Turn on notifications");

        private WebText WeUseNotificationsText => WebText.WithTagAndText(_interactor, "p",
            "We use notifications to tell you when you get a new message.");

        private WebText NhsMaySendYouMessagesText => WebText.WithTagAndText(
            _interactor,
            "p",
            "The NHS and connected healthcare providers, like your GP surgery, may send you messages using the NHS App.");

        private WebRadioOption YesTurnOnNotificationsOption => WebRadioOption.InFieldsetLegendWithLabel(
            _interactor,
            "Do you want to get NHS App notifications?",
            "Yes, turn on notifications on this device");

        private WebText TurnOnNotificationsHint => WebText.WithTagAndText(
            _interactor,
            "div",
            "Tell me about new messages");

        private WebRadioOption NoDontSendNotificationsOption => WebRadioOption.InFieldsetLegendWithLabel(
            _interactor,
            "Do you want to get NHS App notifications?",
            "No, do not send notifications on this device");

        private WebText NoDontSendNotificationsHint => WebText.WithTagAndText(
            _interactor,
            "div",
            "I understand I may not be told about new messages unless I log in");

        private WebLinkExpander AboutNotificationsExpander { get; }

        private WebText IfYouShareThisDeviceText { get; }

        private WebText IfYouWantToGetNotificationsText { get; }

        private WebText MoreInformationIsAvailableText => WebText.WithTagAndText(
            _interactor,
            "p",
            "More information is available in the NHS App privacy policy.");

        private WebLink PrivacyPolicyLink => WebLink.WithText(_interactor, "NHS App privacy policy");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        internal void AssertOnPage()
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);
            TitleText.AssertVisible();
        }

        public ManageNotificationsPromptPageContent AssertPageContent()
        {
            WeUseNotificationsText.AssertVisible();
            NhsMaySendYouMessagesText.AssertVisible();
            YesTurnOnNotificationsOption.AssertVisible();
            TurnOnNotificationsHint.AssertVisible();
            NoDontSendNotificationsOption.AssertVisible();
            NoDontSendNotificationsHint.AssertVisible();

            AboutNotificationsExpander.AssertVisible();
            AboutNotificationsExpander.AssertCollapsed();
            AboutNotificationsExpander.Toggle();
            AboutNotificationsExpander.AssertExpanded();
            IfYouWantToGetNotificationsText.AssertVisible();
            IfYouShareThisDeviceText.AssertVisible();

            MoreInformationIsAvailableText.AssertVisible();
            PrivacyPolicyLink.AssertVisible();
            ContinueButton.AssertVisible();
            return this;
        }

        public void Continue() => ContinueButton.Click();

        public ManageNotificationsPromptPageContent YesTurnOnNotifications()
        {
            YesTurnOnNotificationsOption.Click();
            return this;
        }

        public ManageNotificationsPromptPageContent NoDontSendNotifications()
        {
            NoDontSendNotificationsOption.Click();
            return this;
        }
    }
}
