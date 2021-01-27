using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

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
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(LabelFindBy, action);

        private By LabelFindBy
            => By.XPath($"//input[normalize-space(@value)={_text.QuoteXPathLiteral()}]");
    }
}