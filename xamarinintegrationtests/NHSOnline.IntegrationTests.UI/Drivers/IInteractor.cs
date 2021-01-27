using System;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Drivers
{
    public interface IInteractor<TDriver, TElement>
    {
        internal void ActOnElementContext(By by, Action<ElementContext<TDriver, TElement>> action);
    }
}