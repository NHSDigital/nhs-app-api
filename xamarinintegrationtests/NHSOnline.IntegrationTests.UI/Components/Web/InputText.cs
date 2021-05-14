using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    internal sealed class InputByText
    {
        private readonly string _type;
        private readonly string _text;

        internal InputByText(string type, string text)
        {
            _type = type;
            _text = text;
        }

        internal By Input => By.XPath($"//input[@type={_type.QuoteXPathLiteral()} and //{TextXPath}/@for=@id]");

        internal By LabelAndNotChecked() => By.XPath($"//input[@type={_type.QuoteXPathLiteral()} and not(@aria-checked) and //{TextXPath}/@for=@id]");

        internal By LabelAndChecked(string checkedValue) => By.XPath($"//input[@type={_type.QuoteXPathLiteral()} and @aria-checked={checkedValue.QuoteXPathLiteral()} and //{TextXPath}/@for=@id]");

        private string TextXPath => $"label[normalize-space(string())={_text.QuoteXPathLiteral()}]";

        public string Description => _text;
    }
}