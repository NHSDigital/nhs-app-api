using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings
{
    public sealed class IOSAccountSettingsPage
    {
        private readonly IIOSDriverWrapper _driver;
        private readonly IIOSInteractor _interactor;

        private IOSAccountSettingsPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            _interactor = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new AccountSettingsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        IOSLink NotificationsSettingsMenuItem => IOSLink.WithText(_interactor, "Manage notifications");

        IOSLink NhsLoginSettingsMenuItem => IOSLink.WithText(_interactor, "Manage NHS login account");

        public IOSFullNavigation Navigation { get; }

        public AccountSettingsPageContent PageContent { get; }

        public static IOSAccountSettingsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAccountSettingsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSAccountSettingsPage AssertPageElements()
        {
            Navigation.AssertNavigationPresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void NavigateToNotificationsSettings() => NotificationsSettingsMenuItem.Touch();

        public void NavigateToNhsLoginSettings() => NhsLoginSettingsMenuItem.Touch();
    }
}
