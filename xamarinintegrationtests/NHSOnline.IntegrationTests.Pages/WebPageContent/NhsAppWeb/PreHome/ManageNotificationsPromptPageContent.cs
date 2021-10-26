using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.PreHome
{
    public sealed class ManageNotificationsPromptPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ManageNotificationsPromptPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Turn on notifications");

        private WebText WeUseNotificationsText => WebText.WithTagAndText(
            _interactor,
            "p",
            "We use notifications to tell you when you get a new message.");

        private WebText NhsMaySendYouMessagesText => WebText.WithTagAndText(
            _interactor,
            "p",
            "The NHS and connected healthcare providers, like your GP surgery, may send you messages using the NHS App.");

        private WebText MoreInformationIsAvailableText => WebText.WithTagAndText(
            _interactor,
            "p",
            "More information is available in the NHS App privacy policy");

        private WebLink PrivacyPolicyLink => WebLink.WithText(_interactor, "NHS App privacy policy");

        private WebText AcceptNotificationsText => WebText.WithTagAndText(
            _interactor,
            "span",
            "Tell me about new messages");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        private WebToggle NotificationsToggle => WebToggle.WithLabel(
            _interactor,
            "Turn on notifications on this deviceTell me about new messages");

        internal void AssertOnPage()
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);
            TitleText.AssertVisible();
        }

        public ManageNotificationsPromptPageContent AssertPageContent()
        {
            WeUseNotificationsText.AssertVisible();
            NhsMaySendYouMessagesText.AssertVisible();
            MoreInformationIsAvailableText.AssertVisible();
            PrivacyPolicyLink.AssertVisible();
            AcceptNotificationsText.AssertVisible();
            ContinueButton.AssertVisible();
            return this;
        }

        public void Continue() => ContinueButton.Click();

        public ManageNotificationsPromptPageContent ToggleOnNotifications()
        {
            NotificationsToggle.ToggleOn();
            using var timeout = ExtendedTimeout.FromSeconds(10);
            NotificationsToggle.AssertToggledOn();
            return this;
        }
    }
}
