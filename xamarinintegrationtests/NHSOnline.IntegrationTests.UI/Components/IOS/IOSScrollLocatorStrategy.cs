using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    internal sealed class IOSScrollLocatorStrategy : IIOSLocatorStrategy
    {
        private readonly IIOSInteractor _interactor;
        private readonly IIOSLocatorStrategy _wrappedStrategy;

        public IOSScrollLocatorStrategy(IIOSInteractor interactor, IIOSLocatorStrategy wrappedStrategy)
        {
            _interactor = interactor;
            _wrappedStrategy = wrappedStrategy;
        }

        public string Description => _wrappedStrategy.Description;

        public By FindBy => throw new NotSupportedException("Cannot FindBy an element that needs scrolling to");

        void IIOSLocatorStrategy.ActOnElementContext(Action<ElementContext<IOSDriver<IOSElement>, IOSElement>> action)
        {
            _interactor.ActOnElementContext(
                _wrappedStrategy.FindBy,
                context =>
                {
                    if (!context.Element.Displayed)
                    {
                        _interactor.ActOnElementContext(
                            MobileBy.IosNSPredicate("type == 'XCUIElementTypeScrollView'"),
                            scrollContext => scrollContext.SwipeUp());
                    }

                    action(context);
                });
        }

        void IIOSLocatorStrategy.AssertCannotBeFound(string because) => _interactor.AssertElementCannotBeFound(_wrappedStrategy.FindBy, because);
    }
}