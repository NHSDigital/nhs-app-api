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

        internal By Input(int labelLevel)
        {
            var labelLevelXpath = string.Empty;
            for (int i = 0; i < labelLevel; i++)
            {
                labelLevelXpath += "../";
            }
            return By.XPath($"//input[@type={_type.QuoteXPathLiteral()} and {labelLevelXpath}{LabelXPath}/@for=@id]");
        }

        private string LabelXPath => $"label[contains(normalize-space(string()), {_label.QuoteXPathLiteral()})]";
    }
}