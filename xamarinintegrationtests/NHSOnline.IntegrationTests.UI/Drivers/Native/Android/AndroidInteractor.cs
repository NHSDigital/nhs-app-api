using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.Android
{
    internal sealed class AndroidInteractor : IAndroidInteractor
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly Interactor<AndroidDriver<AndroidElement>, AndroidElement> _interactor;

        public AndroidInteractor(
            NativeDriverContext nativeDriverContext,
            Interactor<AndroidDriver<AndroidElement>, AndroidElement> createContainedInteractor)
        {
            _nativeDriverContext = nativeDriverContext;
            _interactor = createContainedInteractor;
        }

        void IInteractor<AndroidDriver<AndroidElement>, AndroidElement>.ActOnDriver(
            ActOnDriverAction<AndroidDriver<AndroidElement>, AndroidElement> action)
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
    }
}