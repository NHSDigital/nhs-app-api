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
            interactor.ActOnElementContext(by, context => action(context.Element));
        }
    }
}