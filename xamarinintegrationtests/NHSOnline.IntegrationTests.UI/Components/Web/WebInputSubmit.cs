using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebInputSubmit
    {
        private readonly IWebInteractor _interactor;
        private readonly string _text;

        private WebInputSubmit(IWebInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static WebInputSubmit WithText(IWebInteractor interactor, string text)
            => new WebInputSubmit(interactor, text);

        public void Click()
            => _interactor.ActOnElementContext(
                LabelFindBy,
                c =>
                {
                    ScrollTo();
                    c.Element.Click();
                });

        public void ScrollTo() => _interactor.ActOnElementContext(
            LabelFindBy, c => new Actions(c.Driver).MoveToElement(c.Element).Perform());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(LabelFindBy, action);

        private By LabelFindBy
            => By.XPath($"//input[normalize-space(@value)={_text.QuoteXPathLiteral()}]");
    }
}