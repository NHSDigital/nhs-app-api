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
            IOSLabel.WithText(_interactor, "Turn on notifications on this device");

        public void ToggleOnNotifications()
        {
            NotificationsToggleLabel.Click();
        }
    }
}