using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IAndroidInteractor: IInteractor<IAndroidBrowserStackDriver, AndroidElement>
    {
        internal IAndroidInteractor CreateContainedInteractor(By findContainerBy);
        internal void PressTabKey();
        internal void PressEnterKey();
        internal void PressShiftTabKey();
        void TouchScreenCentre();
    }
}