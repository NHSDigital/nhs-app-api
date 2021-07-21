using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.MultiTouch;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidInteractor : IAndroidInteractor
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly Interactor<IAndroidBrowserStackDriver, AndroidElement> _interactor;

        public AndroidInteractor(
            NativeDriverContext nativeDriverContext,
            Interactor<IAndroidBrowserStackDriver, AndroidElement> createContainedInteractor)
        {
            _nativeDriverContext = nativeDriverContext;
            _interactor = createContainedInteractor;
        }

        void IInteractor<IAndroidBrowserStackDriver, AndroidElement>.ActOnDriver(
            ActOnDriverAction<IAndroidBrowserStackDriver, AndroidElement> action)
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.ActOnDriver(action);
        }

        IAndroidInteractor IAndroidInteractor.CreateContainedInteractor(By findContainerBy)
            => new AndroidInteractor(_nativeDriverContext, _interactor.CreateContainedInteractor(findContainerBy));

        void IAndroidInteractor.PressTabKey()
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.ActOnDriver((driver, _) => driver.PressKeyCode(AndroidKeyCode.Keycode_TAB));
        }

        void IAndroidInteractor.PressEnterKey()
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.ActOnDriver((driver, _) => driver.PressKeyCode(AndroidKeyCode.Keycode_ENTER));
        }

        void IAndroidInteractor.PressShiftTabKey()
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.ActOnDriver((driver, _) => driver.PressKeyCode(AndroidKeyCode.Keycode_TAB, AndroidKeyCode.MetaShift_ON));
        }

        void IAndroidInteractor.TouchScreenCentre()
        {
            _nativeDriverContext.SwitchToNativeContext();

            _interactor.ActOnDriver((driver, _) =>
            {
                var x = (double) driver.Manage().Window.Size.Width / 2;
                var y = (double) driver.Manage().Window.Size.Height / 2;
                new TouchAction(driver).Tap(x, y).Perform();
            });
        }
    }
}