using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebLink
    {
        private readonly IWebInteractor _interactor;
        private readonly string _text;

        private WebLink(IWebInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static WebLink WithText(IWebInteractor interactor, string text)
            => new WebLink(interactor, text);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a link with text {1} should be displayed", _text));

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//a[normalize-space(text())={_text.QuoteXPathLiteral()}]");
    }
}