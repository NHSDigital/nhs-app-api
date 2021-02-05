using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public class WebDefinitionTerm
    {
        private readonly IWebInteractor _interactor;
        private readonly string _term;

        private WebDefinitionTerm(IWebInteractor interactor, string term)
        {
            _interactor = interactor;
            _term = term;
        }

        public static WebDefinitionTerm WithTerm(IWebInteractor interactor, string term)
            => new WebDefinitionTerm(interactor, term);

        public void AssertValue(string value)
            => ActOnElement(e =>
            {
                e.Displayed.Should().BeTrue("A term {0} should be displayed", _term);
                e.Text.Should().Be(value, "A term {0} with text {1} should be displayed", _term, value);
            });

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//dt[normalize-space(text())={_term.QuoteXPathLiteral()}]/following-sibling::dd[1]");
    }
}