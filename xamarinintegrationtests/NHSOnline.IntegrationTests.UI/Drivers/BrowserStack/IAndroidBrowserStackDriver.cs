using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Interfaces;

namespace NHSOnline.IntegrationTests.UI.Drivers.BrowserStack
{
    public interface IAndroidBrowserStackDriver :
        IBrowserStackDriver,
        ISendsKeyEvents,
        IStartsActivity
    { }
}