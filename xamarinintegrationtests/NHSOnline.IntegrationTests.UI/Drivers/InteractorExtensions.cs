using System;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public static class InteractorExtensions
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

        internal static bool IsPresent<TDriver, TElement>(
        this IInteractor<TDriver, TElement> interactor, By by)
        {
            var isPresent = false;
            interactor.ActOnDriver((_, findBy) =>
            {
                try
                {
                    var element = findBy(by);
                    isPresent = element != null;
                }
                catch (NoSuchElementException)
                {
                    isPresent = false;
                }
            });
            return isPresent;
        }
    }
}