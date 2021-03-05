using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebButton
    {
        private readonly IWebInteractor _interactor;
        private readonly string _text;

        private WebButton(IWebInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static WebButton WithText(IWebInteractor interactor, string text)
            => new WebButton(interactor, text);

        public void Click()
            => _interactor.ActOnElementContext(
                FindBy,
                c =>
                {
                    ScrollTo();
                    c.Element.Click();
                });

        public void ScrollTo() => _interactor.ActOnElementContext(
            FindBy, c => new Actions(c.Driver).MoveToElement(c.Element).Perform());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($".//button[normalize-space(text())={_text.QuoteXPathLiteral()}]");
    }
}