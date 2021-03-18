using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class SettingsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal SettingsPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Settings");
        private WebMenuItem NotificationsMenuItem => WebMenuItem.WithTitle(_interactor, "Notifications");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public SettingsPageContent AssertPageElements()
        {
            Title.AssertVisible();
            NotificationsMenuItem.AssertVisible();
            return this;
        }

        public void NavigateToNotifications()
        {
            NotificationsMenuItem.Click();
        }
    }
}