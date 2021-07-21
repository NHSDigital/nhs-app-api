using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android.Interfaces;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;

namespace NHSOnline.IntegrationTests.UI.Drivers.BrowserStack
{
    public interface IBrowserStackDriver :
        IWebDriver,
        IContextAware,
        IHasSessionId,
        IInteractsWithApps,
        IPushesFiles,
        IJavaScriptExecutor,
        IPerformsTouchActions,
#pragma warning disable 0618
        // The underlying interface is deprecated to ensure people use the ActionBuilder class,
        // the interface is still valid for us to implement so that ActionBuilder can be used with our driver
        IHasInputDevices,
#pragma warning restore 0618
        IActionExecutor,
        IHidesKeyboard,
        ITakesScreenshot
    { }
}