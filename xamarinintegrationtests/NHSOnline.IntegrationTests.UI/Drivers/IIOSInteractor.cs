using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IIOSInteractor: IInteractor<IOSDriver<IOSElement>, IOSElement>
    {
        internal void AssertElementNotVisible(By by);
        internal IIOSInteractor CreateContainedInteractor(By findContainerBy);
    }
}