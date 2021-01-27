using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    internal sealed class IOSInteractor : IIOSInteractor
    {
        private readonly IOSDriver<IOSElement> _driver;
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly Interactor<IOSDriver<IOSElement>, IOSElement> _interactor;

        public IOSInteractor(
            IOSDriver<IOSElement> driver,
            NativeDriverContext nativeDriverContext,
            Interactor<IOSDriver<IOSElement>, IOSElement> createContainedInteractor)
        {
            _driver = driver;
            _nativeDriverContext = nativeDriverContext;
            _interactor = createContainedInteractor;
        }
        
        IIOSInteractor IIOSInteractor.CreateContainedInteractor(By findContainerBy)
        {
            return new IOSInteractor(_driver, _nativeDriverContext, _interactor.CreateContainedInteractor(findContainerBy));
        }

        void IInteractor<IOSDriver<IOSElement>, IOSElement>.ActOnElementContext(
            By by,
            Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action)
            => _interactor.ActOnElementContext(by, action);
    }
}