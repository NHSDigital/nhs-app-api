using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidInteractor: IInteractor<AndroidDriver<AndroidElement>, AndroidElement>
    {
        internal IAndroidInteractor CreateContainedInteractor(By findContainerBy);

        internal void PressTabKey();
    }
}