using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public sealed class ManageNotificationsPromptPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ManageNotificationsPromptPageContent(IWebInteractor webInteractor) => _interactor = webInteractor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Manage notifications");

        private WebText ThisMayIncludeText => WebText.WithTagAndText(
            _interactor,
            "p",
            "These may include new features and public health updates.");

        private WebText IfYouShareThisDeviceText => WebText.WithTagAndText(
            _interactor,
            "p",
            "If you share this device with other people, they may see your notifications. " +
            "The settings will apply to everyone who logs in to the NHS App on this device.");

        private WebText MoreInformationIsAvailableText => WebText.WithTagAndText(
            _interactor,
            "p",
            "More information is available in the NHS App privacy policy.");

        private WebLink PrivacyPolicyLink => WebLink.WithText(_interactor, "NHS App privacy policy");

        private WebText AcceptNotificationsText => WebText.WithTagAndText(
            _interactor,
            "span",
            "I accept the NHS App sending notifications on this device");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");

        private WebToggle NotificationsToggle => WebToggle.WithLabel(
            _interactor,
            "Allow notificationsI accept the NHS App sending notifications on this device");

        internal void AssertOnPage()
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);
            TitleText.AssertVisible();
        }

        public ManageNotificationsPromptPageContent AssertPageContent()
        {
            ThisMayIncludeText.AssertVisible();
            IfYouShareThisDeviceText.AssertVisible();
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