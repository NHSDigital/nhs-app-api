using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Drivers.Native.IOS
{
    class IOSInteractor : IIOSInteractor
    {
        private readonly NativeDriverContext _nativeDriverContext;
        private readonly Interactor<IOSDriver<IOSElement>, IOSElement> _interactor;

        public IOSInteractor(NativeDriverContext nativeDriverContext,
            Interactor<IOSDriver<IOSElement>, IOSElement> createContainedInteractor)
        {
            _nativeDriverContext = nativeDriverContext;
            _interactor = createContainedInteractor;
        }

        void IIOSInteractor.AssertElementDoesntExist(By @by)
        {
            _interactor.AssertElementDoesntExist(by);
        }

        IIOSInteractor IIOSInteractor.CreateContainedInteractor(By findContainerBy)
        {
            return new IOSInteractor(_nativeDriverContext, _interactor.CreateContainedInteractor(findContainerBy));
        }

        void IInteractor<IOSDriver<IOSElement>, IOSElement>.ActOnElementContext(By by,
            Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action) =>
            _interactor.ActOnElementContext(by, action);
    }
}