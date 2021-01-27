using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebText
    {
        private readonly IWebInteractor _interactor;
        private readonly string _tag;
        private readonly string _text;

        private WebText(IWebInteractor interactor, string tag, string text)
        {
            _interactor = interactor;
            _tag = tag;
            _text = text;
        }

        public static WebText WithTagAndText(IWebInteractor interactor, string tag, string text)
            => new WebText(interactor, tag, text);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("A {0} tag with text {1} should be displayed", _tag, _text));

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//{_tag}[normalize-space(text())={_text.QuoteXPathLiteral()}]");
    }
}