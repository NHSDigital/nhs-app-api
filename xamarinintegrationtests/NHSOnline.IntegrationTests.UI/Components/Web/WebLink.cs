using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebLink: IFocusable
    {
        private readonly IWebInteractor _interactor;
        private readonly string _text;
        private readonly string _searchPrefix;

        private WebLink(IWebInteractor interactor, string text, string searchPrefix)
        {
            _interactor = interactor;
            _text = text;
            _searchPrefix = searchPrefix;
        }

        public static WebLink WithText(IWebInteractor interactor, string text)
            => new (interactor, text, string.Empty);

        internal static WebLink WithText(IWebInteractor interactor, string text, string searchPrefix)
            => new (interactor, text, searchPrefix);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("A link with text {0} should be displayed", _text));

        public void AssertNotVisible() => _interactor.IsPresent(FindBy).Should().BeFalse("A link with text {0} should not be displayed", _text);

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        public WebLink ScrollTo()
        {
            _interactor.ActOnElementContext(
                FindBy, c => new Actions(c.Driver).MoveToElement(c.Element).Perform());
            return this;
        }

        private By FindBy
            => By.XPath($"{_searchPrefix}//a[normalize-space(string())={_text.QuoteXPathLiteral()}]");

        string IFocusable.ElementDescription
            => new FocusableDescriptionBuilder {Tag = "a", Text = _text}.Description;
    }
}