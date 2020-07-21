using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Appium.MultiTouch;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal static class ElementContextExtensions
    {
        internal static void Tap<TDriver, TElement>(this ElementContext<TDriver, TElement> context)
            where TDriver : IPerformsTouchActions
            where TElement: IWebElement
        {
            context.PerformTouchAction(action => action.Press(context.Element, 5, 5).Wait(5).Release());
        }

        internal static void SwipeUp<TDriver, TElement>(this ElementContext<TDriver, TElement> context)
            where TDriver : IJavaScriptExecutor
            where TElement : AppiumWebElement
        {
            context.Driver.ExecuteScript(
                "mobile:swipe",
                new Dictionary<string, string>
                {
                    { "element", context.Element.Id },
                    { "direction", "up" }
                });
        }

        private static void PerformTouchAction<TDriver, TElement>(
            this ElementContext<TDriver, TElement> context,
            Func<TouchAction, ITouchAction> configure)
            where TDriver: IPerformsTouchActions
        {
            configure(new TouchAction(context.Driver)).Perform();
        }
    }
}