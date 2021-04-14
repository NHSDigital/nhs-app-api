using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class MorePageContent
    {
        private readonly IWebInteractor _interactor;

        internal MorePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "More");
        private WebMenuItem NotificationsMenuItem => WebMenuItem.WithTitle(_interactor, "Notifications");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public MorePageContent AssertPageElements()
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
