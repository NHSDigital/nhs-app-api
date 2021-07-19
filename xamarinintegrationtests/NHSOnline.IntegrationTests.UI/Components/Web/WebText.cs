using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebText : IActOnElementContext
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

        public void AssertNotVisible() => _interactor.IsPresent(FindBy).Should().BeFalse("A {0} tag with text {1} should not be displayed", _tag, _text);

        public WebLink WithChildLink(string linkText)
            => WebLink.WithText(_interactor, linkText, WholeElementSelector );

        public void ScrollTo() => _interactor.ActOnElementContext(
            FindBy, c => new Actions(c.Driver).MoveToElement(c.Element).Perform());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath(WholeElementSelector);

        private string WholeElementSelector => $"//{ _tag}[normalize-space()={_text.QuoteXPathLiteral()}]";
        void IActOnElementContext.ActOnElementContext(Action<ElementContext<IWebDriver, IWebElement>> action)
            => _interactor.ActOnElementContext(FindBy, action);

    }
}