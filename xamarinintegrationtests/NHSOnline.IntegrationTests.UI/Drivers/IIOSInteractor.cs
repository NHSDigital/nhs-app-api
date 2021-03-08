using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IIOSInteractor: IInteractor<IOSDriver<IOSElement>, IOSElement>
    {
        internal IIOSInteractor CreateContainedInteractor(By findContainerBy);
        void AssertElementCannotBeFound(By by, string because);
    }
}