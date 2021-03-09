using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public interface IActOnElementContext
    {
        internal void ActOnElementContext(Action<ElementContext<IWebDriver, IWebElement>> action);
    }
}