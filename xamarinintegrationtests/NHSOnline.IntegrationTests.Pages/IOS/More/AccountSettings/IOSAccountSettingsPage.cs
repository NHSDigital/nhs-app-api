using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More.AccountSettings;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More.AccountSettings
{
    public sealed class IOSAccountSettingsPage
    {
        private readonly IIOSInteractor _interactor;

        private AccountSettingsPageContent PageContent { get; }

        private IOSAccountSettingsPage(IIOSDriverWrapper driver)
        {
            _interactor = driver;

            PageContent = new AccountSettingsPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public static IOSAccountSettingsPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSAccountSettingsPage(driver);
            page.PageContent.AssertOnPage();
            return page;
        }

        public IOSAccountSettingsPage AssertTouchIdMenuItemElements()
        {
            TouchIdMenuItem.AssertVisible();
            return this;
        }

        public IOSAccountSettingsPage AssertFaceIdMenuItemElements()
        {
            FaceIdMenuItem.AssertVisible();
            return this;
        }

        private IOSLink LegalAndCookiesMenuItem => IOSLink.WithText(_interactor, "Legal and cookies");

        private IOSLink NhsLoginSettingsMenuItem => IOSLink.WithText(_interactor, "Manage NHS account");

        private IOSLink NotificationsSettingsMenuItem => IOSLink.WithText(_interactor, "Manage notifications");

        private IOSLink FaceIdMenuItem => IOSLink.WithText(_interactor, "Face ID");

        private IOSLink TouchIdMenuItem => IOSLink.WithText(_interactor, "Touch ID");

        public void NavigateToLegalAndCookies() => LegalAndCookiesMenuItem.Touch();

        public void NavigateToNhsLoginSettings() => NhsLoginSettingsMenuItem.Touch();

        public void NavigateToNotificationsSettings() => NotificationsSettingsMenuItem.Touch();

        public void NavigateToFaceIdBiometrics() => FaceIdMenuItem.Touch();

        public void NavigateToTouchIdBiometrics() => TouchIdMenuItem.Touch();
    }
}
