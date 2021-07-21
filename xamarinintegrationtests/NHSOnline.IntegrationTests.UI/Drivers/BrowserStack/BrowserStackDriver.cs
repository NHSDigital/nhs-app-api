using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;

namespace NHSOnline.IntegrationTests.UI.Drivers.BrowserStack
{
    public abstract class BrowserStackDriver<TDriver, TElement> : IBrowserStackDriver
        where TDriver : AppiumDriver<TElement>
        where TElement : IWebElement
    {
        protected TDriver Driver { get; }

        internal BrowserStackDriver(TDriver driver)
        {
            Driver = driver;
        }

        public virtual string Context
        {
            get => Driver.Context;
            set => Driver.Context = value;
        }

        public ReadOnlyCollection<string> Contexts => Driver.Contexts;
        public virtual IWebElement FindElement(By by) => ((RemoteWebDriver) Driver).FindElement(by);
        public ReadOnlyCollection<IWebElement> FindElements(By by) => ((RemoteWebDriver) Driver).FindElements(by);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Driver.Dispose();
        }

        public void Close() => Driver.Close();
        public void Quit() => Driver.Quit();
        public IOptions Manage() => Driver.Manage();
        public INavigation Navigate() => Driver.Navigate();
        public virtual ITargetLocator SwitchTo() => Driver.SwitchTo();

        public virtual string Url
        {
            get => Driver.Url;
            set => Driver.Url = value;
        }

        public string Title => Driver.Title;
        public string PageSource => Driver.PageSource;
        public string CurrentWindowHandle => Driver.CurrentWindowHandle;
        public ReadOnlyCollection<string> WindowHandles => Driver.WindowHandles;
        public SessionId SessionId => Driver.SessionId;

        public virtual Response Execute(string commandName, Dictionary<string, object> parameters) =>
            ((IExecuteMethod) Driver).Execute(commandName, parameters);

        public virtual Response Execute(string driverCommand) => ((IExecuteMethod) Driver).Execute(driverCommand);
        public void InstallApp(string appPath) => Driver.InstallApp(appPath);
        public void LaunchApp() => Driver.LaunchApp();
        public bool IsAppInstalled(string bundleId) => Driver.IsAppInstalled(bundleId);
        public void ResetApp() => Driver.ResetApp();
        public void BackgroundApp() => Driver.BackgroundApp();

        [Obsolete("The underlying method is deprecated, but we need to implement this")]
        public void BackgroundApp(int seconds) => Driver.BackgroundApp(seconds);

        public void BackgroundApp(TimeSpan timepSpan) => Driver.BackgroundApp(timepSpan);
        public void RemoveApp(string appId) => Driver.RemoveApp(appId);
        public void ActivateApp(string appId) => Driver.ActivateApp(appId);
        public bool TerminateApp(string appId) => Driver.TerminateApp(appId);
        public bool TerminateApp(string appId, TimeSpan timeout) => Driver.TerminateApp(appId, timeout);
        public void CloseApp() => Driver.CloseApp();
        public AppState GetAppState(string appId) => Driver.GetAppState(appId);

        public virtual object ExecuteScript(string script, params object[] args) => Driver.ExecuteScript(script, args);

        public virtual object ExecuteAsyncScript(string script, params object[] args) =>
            Driver.ExecuteAsyncScript(script, args);

        public byte[] PullFile(string pathOnDevice) => Driver.PullFile(pathOnDevice);
        public byte[] PullFolder(string remotePath) => Driver.PullFolder(remotePath);
        public void PushFile(string pathOnDevice, string base64Data) => Driver.PushFile(pathOnDevice, base64Data);
        public void PushFile(string pathOnDevice, byte[] base64Data) => Driver.PushFile(pathOnDevice, base64Data);
        public void PushFile(string pathOnDevice, FileInfo file) => Driver.PushFile(pathOnDevice, file);
        public void PerformMultiAction(IMultiAction multiAction) => Driver.PerformMultiAction(multiAction);
        public void PerformTouchAction(ITouchAction touchAction) => Driver.PerformTouchAction(touchAction);

        [Obsolete("The underlying property is deprecated but we need to implement this to be complete")]
        public IKeyboard Keyboard => Driver.Keyboard;

        [Obsolete("The underlying property is deprecated but we need to implement this to be complete")]
        public IMouse Mouse => Driver.Mouse;

        public void PerformActions(IList<ActionSequence> actionSequenceList) =>
            ((RemoteWebDriver) Driver).PerformActions(actionSequenceList);

        public void ResetInputState() => Driver.ResetInputState();
        public bool IsActionExecutor => Driver.IsActionExecutor;
        public void HideKeyboard() => Driver.HideKeyboard();

        public void HideKeyboard(string key) => Driver.HideKeyboard(key);

        public void HideKeyboard(string strategy, string key) => Driver.HideKeyboard(strategy, key);

        public bool IsKeyboardShown() => Driver.IsKeyboardShown();

        public Screenshot GetScreenshot() => Driver.GetScreenshot();
    }
}