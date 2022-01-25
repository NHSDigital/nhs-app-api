using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSFileChooser
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFileChooser(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSButton FileChooserNavigationButton => IOSButton.WithText(_driver, "Photos");

        private IOSButton ChooseButton => IOSButton.WithText(_driver, "Choose");

        private IOSButton CancelButton => IOSButton.WithText(_driver, "Cancel");

        private IOSSystemImage Photo => IOSSystemImage.WhichMatches(_driver, "Photo,.*");

        public static IOSFileChooser AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSFileChooser(driver);
            page.FileChooserNavigationButton.AssertVisible();
            return page;
        }

        public void ChoosePhoto()
        {
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
            Photo.Click();
        }

        public void ConfirmSelection() => ChooseButton.Click();

        public void CancelSelection() => CancelButton.Click();
    }
}