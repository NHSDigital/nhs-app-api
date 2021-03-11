using System;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    internal static class InteractorExtensions
    {
        internal static void ActOnElement<TDriver, TElement>(
            this IInteractor<TDriver, TElement> interactor,
            By by,
            Action<TElement> action)
        {
            interactor.ActOnDriver((_, findElement) =>
            {
                var element = findElement(by);
                action(element);
            });
        }

        internal static void ActOnElementContext<TDriver, TElement>(
            this IInteractor<TDriver, TElement> interactor,
            By by,
            Action<ElementContext<TDriver, TElement>> action)
        {
            interactor.ActOnDriver((driver, findElement) =>
            {
                var element = findElement(by);
                action(new ElementContext<TDriver, TElement>(driver, element));
            });
        }

        internal static void ActOnDriver<TDriver, TElement>(
            this IInteractor<TDriver, TElement> interactor,
            Action<TDriver> action)
        {
            interactor.ActOnDriver((driver, _) => action(driver));
        }
    }
}