using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSFileChooser
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSFileChooser(IIOSDriverWrapper driver)
        {
            _driver = driver;
        }

        private IOSSystemLinkLabel AllPhotos => IOSSystemLinkLabel.WithText(_driver, "All Photos");

        private IOSLabel FileChooserTitleText => IOSLabel.WithText(_driver, "Photos");

        private IOSButton Done => IOSButton.WithText(_driver, "Done");

        private IOSSystemLinkLabel Photo => IOSSystemLinkLabel.WhichMatches(_driver, "Photo,.*");

        public static IOSFileChooser AssertDisplayed(IIOSDriverWrapper driver)
        {
            var page = new IOSFileChooser(driver);
            page.FileChooserTitleText.AssertVisible();
            return page;
        }

        public IOSFileChooser SelectFolder()
        {
            AllPhotos.Click();
            return this;
        }

        public IOSFileChooser ChoosePhoto()
        {
            Task.Delay(TimeSpan.FromMilliseconds(100)).Wait();
            Photo.Click();
            return this;
        }

        public void ConfirmSelection()
        {
            Done.Click();
        }
    }
}