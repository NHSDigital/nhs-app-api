using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings
{
    public class NotificationsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NotificationsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;

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

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Manage notifications");

        private WebText WeUseNotifications => WebText.WithTagAndText(_interactor, "p",
            "We use notifications to tell you when you get a new message.");

        private WebText TheNhsAndConnected => WebText.WithTagAndText(_interactor, "p",
        "The NHS and connected healthcare providers, like your GP surgery, may send you messages using the NHS App.");

        private WebLinkExpander AboutNotificationsExpander { get; }

        private WebText IfYouShareThisDeviceText { get; }

        private WebText IfYouWantToGetNotificationsText { get; }

        private WebLink PrivacyLink => WebLink.WithText(_interactor, "NHS App privacy policy");

        private WebText MoreInfoText => WebText.WithTagAndText(_interactor, "p",
            "More information is available in the NHS App privacy policy.");

        private WebToggle NotificationsToggle => WebToggle.WithLabel(_interactor,
            "Turn on notifications on this deviceWhen off, you may not be told about new messages unless you log in");

        private WebLink NotificationSettingsLink => WebLink.WithText(_interactor,
            "Choose how notifications are shown on this device (opens your device settings)");

        private WebText NotificationTurnedOffOnDeviceErrorTitleText => WebText.WithTagAndText(_interactor, "h1", "Notifications error");

        private WebText ErrorTitleText => WebText.WithTagAndText(_interactor, "h1", "Cannot update notification preferences");

        private WebText NotificationsTurnedOffText => WebText.WithTagAndText(
            _interactor,
            "p",
            "Notifications are turned off on your device");

        private WebText TurnOnNotificationsText => WebText.WithTagAndText(
            _interactor,
            "p",
            "To turn on notifications, go to your device settings and allow notifications. Then return to the app and try again.");

        private WebButton TryAgainButton => WebButton.WithText(_interactor, "Try again");

        private WebText ErrorCannotChangeChoiceTitleText => WebText.WithTagAndText(_interactor, "h1", "Cannot change notifications choice");

        private WebText NotificationsMayBeTurnedOffText => WebText.WithTagAndText(
            _interactor,
            "p",
            "This may be because notifications are turned off in your device settings.");

        private WebText GoToDeviceSettingsText => WebText.WithTagAndText(
            _interactor,
            "p",
            "Go to your device settings and check notifications are turned on, then try again.");

        private WebLink DeviceSettingsLink => WebLink.WithText(_interactor, "Go to device settings");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            PrivacyLink,
            NotificationsToggle,
            AboutNotificationsExpander,
            NotificationSettingsLink
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        internal void AssertErrorOnPage() => ErrorTitleText.AssertVisible();

        internal void AssertNotificationsTurnedOffOnDeviceErrorOnPage() => NotificationTurnedOffOnDeviceErrorTitleText.AssertVisible();

        internal void AssertNotificationsChoiceErrorOnPage() => ErrorCannotChangeChoiceTitleText.AssertVisible();

        public void AssertPageElements()
        {
            TitleText.AssertVisible();
            WeUseNotifications.AssertVisible();
            TheNhsAndConnected.AssertVisible();

            AboutNotificationsExpander.AssertVisible();
            AboutNotificationsExpander.AssertCollapsed();
            AboutNotificationsExpander.Toggle();
            AboutNotificationsExpander.AssertExpanded();
            IfYouWantToGetNotificationsText.AssertVisible();
            IfYouShareThisDeviceText.AssertVisible();

            MoreInfoText.AssertVisible();
            NotificationSettingsLink.AssertVisible();
        }

        public void AssertNotificationsTurnedOffErrorPageElements()
        {
            NotificationTurnedOffOnDeviceErrorTitleText.AssertVisible();
            NotificationsTurnedOffText.AssertVisible();
            TurnOnNotificationsText.AssertVisible();
            TryAgainButton.AssertVisible();
        }

        public NotificationsPageContent AssertNotificationsChoiceErrorOnPageElements()
        {
            ErrorCannotChangeChoiceTitleText.AssertVisible();
            NotificationsMayBeTurnedOffText.AssertVisible();
            GoToDeviceSettingsText.AssertVisible();
            DeviceSettingsLink.AssertVisible();
            return this;
        }

        public void ClickDeviceSettings()
        {
            DeviceSettingsLink.Click();
        }

        public void AssertNotificationsEnabled()
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);
            NotificationsToggle.AssertToggledOn();
        }

        public void AssertNotificationsDisabled()
        {
            using var timeout = ExtendedTimeout.FromSeconds(10);
            NotificationsToggle.AssertToggledOff();
        }

        public NotificationsPageContent ToggleOnNotifications()
        {
            NotificationsToggle.ToggleOn();
            using var timeout = ExtendedTimeout.FromSeconds(10);
            NotificationsToggle.AssertToggledOn();
            return this;
        }

        public NotificationsPageContent ToggleOffNotifications()
        {
            NotificationsToggle.ToggleOff();
            using var timeout = ExtendedTimeout.FromSeconds(10);
            NotificationsToggle.AssertToggledOff();
            return this;
        }

        public void ToggleOffNotificationsAndDoNotAssert()
        {
            NotificationsToggle.ToggleOff();
        }

        public void OpenNotificationSettings() => NotificationSettingsLink.Click();

        public void KeyboardNavigateToDeviceSettings(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateMenuItem(NotificationSettingsLink, navigation);

        private static void KeyboardNavigateToAndActivateMenuItem(IFocusable linkItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabTo(linkItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}