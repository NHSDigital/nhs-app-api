using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Remote;

namespace NHSOnline.IntegrationTests.UI.Drivers.BrowserStack
{
    public sealed class AndroidBrowserStackDriver : BrowserStackDriver<AndroidDriver<AndroidElement>, AndroidElement>, IAndroidBrowserStackDriver
    {
        private readonly TestLogs _logs;
        private string? _lastSetContext;
        private string? _lastSetWindow;

        internal AndroidBrowserStackDriver(Uri remoteAddress, AppiumOptions driverOptions, TestLogs logs) : base(
            new AndroidDriver<AndroidElement>(remoteAddress, driverOptions))
        {
            _logs = logs;
        }

        public override string Context
        {
            get => Driver.Context;
            set
            {
                _lastSetContext = value;
                _lastSetWindow = null;
                Driver.Context = value;
            }
        }

        public override ITargetLocator SwitchTo()
        {
            return new WrappedTargetLocator(this, Driver.SwitchTo());
        }

        private class WrappedTargetLocator : ITargetLocator
        {
            private readonly AndroidBrowserStackDriver _driver;
            private readonly ITargetLocator _targetLocatorImplementation;

            public WrappedTargetLocator(AndroidBrowserStackDriver driver, ITargetLocator targetLocatorImplementation)
            {
                _driver = driver;
                _targetLocatorImplementation = targetLocatorImplementation;
            }

            public IWebDriver Frame(int frameIndex)
            {
                return _targetLocatorImplementation.Frame(frameIndex);
            }

            public IWebDriver Frame(string frameName)
            {
                return _targetLocatorImplementation.Frame(frameName);
            }

            public IWebDriver Frame(IWebElement frameElement)
            {
                return _targetLocatorImplementation.Frame(frameElement);
            }

            public IWebDriver ParentFrame()
            {
                return _targetLocatorImplementation.ParentFrame();
            }

            public IWebDriver Window(string windowName)
            {
                _driver._lastSetWindow = windowName;
                return _targetLocatorImplementation.Window(windowName);
            }

            public IWebDriver DefaultContent()
            {
                return _targetLocatorImplementation.DefaultContent();
            }

            public IWebElement ActiveElement()
            {
                return _targetLocatorImplementation.ActiveElement();
            }

            public IAlert Alert()
            {
                return _targetLocatorImplementation.Alert();
            }
        }

        private void DoWithRecovery(Action action)
        {
            var _ = DoWithRecovery<object?>(() =>
            {
                action();
                return null;
            });
        }

        private T DoWithRecovery<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex) when (ex.AnyMessageContains("unable to connect to renderer"))
            {
                const string nativeAppContext = "NATIVE_APP";

                _logs.Info(
                    "Attempting to recover from an 'unable to connect to renderer' error by switching to native context and then back again");

                _logs.Info($"Setting context to '{nativeAppContext}'");
                Driver.Context = nativeAppContext;

                _logs.Info($"Setting context to '{_lastSetContext}'");
                Driver.Context = _lastSetContext;

                if (_lastSetWindow != null)
                {
                    _logs.Info($"Setting window to '{_lastSetWindow}'");
                    Driver.SwitchTo().Window(_lastSetWindow);
                }

                _logs.Info("Recovery attempt complete, retrying action");
                return action();
            }
            catch (Exception e)
            {
                _logs.Info($"Error received from BrowserStack: {e}");
                throw;
            }
        }

        public override IWebElement FindElement(By by) => DoWithRecovery(() => ((RemoteWebDriver) Driver).FindElement(by));

        public override string Url
        {
            get => DoWithRecovery(() => Driver.Url);
            set => DoWithRecovery(() => Driver.Url = value);
        }

        public override Response Execute(string commandName, Dictionary<string, object> parameters) => DoWithRecovery(() => ((IExecuteMethod) Driver).Execute(commandName, parameters));
        public override Response Execute(string driverCommand) => DoWithRecovery(() => ((IExecuteMethod) Driver).Execute(driverCommand));

        public override object ExecuteScript(string script, params object[] args) => DoWithRecovery(() => Driver.ExecuteScript(script, args));
        public override object ExecuteAsyncScript(string script, params object[] args) => DoWithRecovery(() => Driver.ExecuteAsyncScript(script, args));

        public void PressKeyCode(int keyCode, int metastate = 0) => Driver.PressKeyCode(keyCode, metastate);
        public void LongPressKeyCode(int keyCode, int metastate = 0) => Driver.LongPressKeyCode(keyCode, metastate);
        public void PressKeyCode(KeyEvent keyEvent) => Driver.PressKeyCode(keyEvent);
        public void LongPressKeyCode(KeyEvent keyEvent) => Driver.LongPressKeyCode(keyEvent);

        public void StartActivity(string appPackage, string appActivity, string appWaitPackage = "", string appWaitActivity = "",
            bool stopApp = true) =>
            Driver.StartActivity(appPackage, appActivity, appWaitPackage, appWaitActivity, stopApp);

        public void StartActivityWithIntent(string appPackage, string appActivity, string intentAction, string appWaitPackage = "",
            string appWaitActivity = "", string intentCategory = "", string intentFlags = "", string intentOptionalArgs = "",
            bool stopApp = true) =>
            Driver.StartActivityWithIntent(appPackage, appActivity, intentAction, appWaitPackage, appWaitActivity, intentCategory, intentFlags, intentOptionalArgs, stopApp);

        public string CurrentActivity => Driver.CurrentActivity;
    }
}