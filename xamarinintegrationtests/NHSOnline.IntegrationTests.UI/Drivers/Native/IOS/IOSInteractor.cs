using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    internal sealed class IOSInteractor : IIOSInteractor
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly Interactor<IIOSBrowserStackDriver, IOSElement> _interactor;

        public IOSInteractor(
            NativeDriverContext nativeDriverContext,
            Interactor<IIOSBrowserStackDriver, IOSElement> createContainedInteractor)
        {
            _nativeDriverContext = nativeDriverContext;
            _interactor = createContainedInteractor;
        }

        IIOSInteractor IIOSInteractor.CreateContainedInteractor(By findContainerBy)
        {
            return new IOSInteractor(_nativeDriverContext, _interactor.CreateContainedInteractor(findContainerBy));
        }

        void IIOSInteractor.AssertElementCannotBeFound(By by, string because)
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.AssertElementCannotBeFound(by, because);
        }

        void IInteractor<IIOSBrowserStackDriver, IOSElement>.ActOnDriver(
            ActOnDriverAction<IIOSBrowserStackDriver, IOSElement> action)
        {
            _nativeDriverContext.SwitchToNativeContext();
            _interactor.ActOnDriver(action);
        }
    }
}