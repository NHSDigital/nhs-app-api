using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    internal sealed class InputBy
    {
        private readonly string _type;
        private readonly string _label;

        internal InputBy(string type, string label)
        {
            _type = type;
            _label = label;
        }

        internal By Label => By.XPath($"//{LabelXPath}");

        internal By Input => By.XPath($"//input[@type={_type.QuoteXPathLiteral()} and ../{LabelXPath}/@for=@id]");

        private string LabelXPath => $"label[normalize-space()={_label.QuoteXPathLiteral()}]";
    }
}