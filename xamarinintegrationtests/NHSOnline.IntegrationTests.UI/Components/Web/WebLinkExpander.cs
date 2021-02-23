using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public class WebLinkExpander
    {
        private readonly IWebInteractor _interactor;
        private readonly string _text;

        private WebLinkExpander(IWebInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static WebLinkExpander WithText(IWebInteractor interactor, string text)
            => new WebLinkExpander(interactor, text);

        public void AssertVisible() => ActOnElement(e => e.Displayed.Should().BeTrue("an expander with text '{1}' should be displayed", _text));

        public void Toggle() => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IWebElement> action) => _interactor.ActOnElement(FindBy, action);

        private By FindBy => By.XPath($"//span[normalize-space()={_text.QuoteXPathLiteral()}]");
    }
}