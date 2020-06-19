using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebText
    {
        private readonly IWebInteractor _driver;
        private readonly string _tag;
        private readonly string _text;

        public WebText(IWebInteractor driver, string tag, string text)
        {
            _driver = driver;
            _tag = tag;
            _text = text;
        }

        public void AssertVisible()
        {
            _driver.ActOnElement(
                By.XPath($"//{_tag}[normalize-space(text())={_text.QuoteXPathLiteral()}]"),
                e => e.Displayed.Should().BeTrue("A {0} tag with text {1} should be displayed", _tag, _text));
        }
    }
}