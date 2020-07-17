using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Appium.MultiTouch;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal static class ElementContextExtensions
    {
        internal static void Tap<TDriver, TElement>(this ElementContext<TDriver, TElement> interactor)
            where TDriver : IPerformsTouchActions
            where TElement: IWebElement
        {
            interactor.PerformTouchAction(action => action.Press(interactor.Element, 5, 5).Wait(100).Release());
        }

        private static void PerformTouchAction<TDriver, TElement>(
            this ElementContext<TDriver, TElement> interactor,
            Func<ITouchAction, ITouchAction> configure)
            where TDriver: IPerformsTouchActions
        {
            configure(new TouchAction(interactor.Driver)).Perform();
        }
    }
}