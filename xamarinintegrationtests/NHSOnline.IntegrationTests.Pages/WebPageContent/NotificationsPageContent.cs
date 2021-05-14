using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class NotificationsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NotificationsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Manage notifications");

        private WebText TheseMayInclude => WebText.WithTagAndText(_interactor, "p",
            "These may include new features and public health updates.");

        private WebText IfYouShareThisDevice => WebText.WithTagAndText(_interactor, "p",
            "If you share this device with other people, they may see your notifications. The settings will apply to everyone who logs in to the NHS App on this device.");

        private WebLink PrivacyLink => WebLink.WithText(_interactor, "NHS App privacy policy");

        private WebText MoreInfo => WebText.WithTagAndText(_interactor, "p",
            "More information is available in the NHS App privacy policy.");

        private WebToggle NotificationsToggle => WebToggle.WithLabel(
            _interactor,
            "Allow notificationsI accept the NHS App sending notifications on this device");

        private WebLink NotificationSettingsLink => WebLink.WithText(
            _interactor,
            "Manage how notifications are shown on this device (opens your device settings)");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            PrivacyLink,
            NotificationsToggle,
            NotificationSettingsLink
        };

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public NotificationsPageContent AssertPageElements()
        {
            Title.AssertVisible();
            TheseMayInclude.AssertVisible();
            IfYouShareThisDevice.AssertVisible();
            MoreInfo.AssertVisible();
            NotificationSettingsLink.AssertVisible();
            return this;
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

        public void ToggleOffNotifications()
        {
            NotificationsToggle.ToggleOff();
            using var timeout = ExtendedTimeout.FromSeconds(10);
            NotificationsToggle.AssertToggledOff();
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