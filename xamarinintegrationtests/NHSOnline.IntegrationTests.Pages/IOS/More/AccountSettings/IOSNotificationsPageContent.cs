using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings
{
    public class IOSNotificationsPageContent
    {
        private readonly IIOSInteractor _interactor;

        public IOSNotificationsPageContent(IIOSInteractor interactor)
        {
            _interactor = interactor;
        }

        private IOSLabel NotificationsToggleLabel =>
            IOSLabel.WithText(_interactor, "Tell me when I get new messages from my GP surgery and other healthcare services");

        public void ToggleOnNotifications()
        {
            NotificationsToggleLabel.Click();
        }
    }
}