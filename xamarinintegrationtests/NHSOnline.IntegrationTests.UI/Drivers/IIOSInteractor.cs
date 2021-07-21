using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IIOSInteractor: IInteractor<IIOSBrowserStackDriver, IOSElement>
    {
        internal IIOSInteractor CreateContainedInteractor(By findContainerBy);
        void AssertElementCannotBeFound(By by, string because);
    }
}