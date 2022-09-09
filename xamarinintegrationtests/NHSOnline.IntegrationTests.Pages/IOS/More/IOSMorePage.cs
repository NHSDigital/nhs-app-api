using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.More;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.More
{
    public sealed class IOSMorePage
    {
        private readonly IIOSInteractor _interactor;
        IOSLink AccountAndSettingsMenuItem => IOSLink.WithText(_interactor, "Account and settings");

        IOSMenuItem LinkedProfilesMenuItem => IOSMenuItem.StartsWith(_interactor, "Linked profiles");

        private IOSMorePage(IIOSDriverWrapper driver)
        {
            _interactor = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new MorePageContent(driver.Web.NhsAppLoggedInWebView());
        }

        public IOSFullNavigation Navigation { get; }

        public MorePageContent PageContent { get; }

        public static IOSMorePage AssertOnPage(IIOSDriverWrapper driver,  bool screenshot = false)
        {
            var page = new IOSMorePage(driver);
            page.PageContent.AssertOnPage();

            if (screenshot)
            {
                driver.Screenshot(nameof(IOSMorePage));
            }

            return page;
        }

        public IOSMorePage AssertPageElements()
        {
            Navigation.AssertNavigationIconsArePresent();
            PageContent.AssertPageElements();

            return this;
        }

        public void NavigateToAccountAndSettings() => AccountAndSettingsMenuItem.Touch();

        public void NavigateToLinkedProfiles() => LinkedProfilesMenuItem.Click();
    }
}
