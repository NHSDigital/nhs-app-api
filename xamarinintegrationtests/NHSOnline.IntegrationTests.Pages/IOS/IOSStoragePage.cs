using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSStoragePage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSSystemLinkLabel FileLabel => IOSSystemLinkLabel.WithText(_driver, "test, txt");

        private IOSSearchBar SearchBar => IOSSearchBar.WithText(_driver, "Search");

        private IOSButton CancelButton => IOSButton.WithText(_driver, "Cancel");

        private IOSStoragePage(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        public static IOSStoragePage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSStoragePage(driver);
            page.SearchBar.AssertVisible();
            return page;
        }

        public void SelectFile() => FileLabel.Click();

        public void CloseFileSelectorScreen() => CancelButton.Touch();

        public void SearchForText()
        {
            SearchBar.EnterText("test");
        }
    }
}