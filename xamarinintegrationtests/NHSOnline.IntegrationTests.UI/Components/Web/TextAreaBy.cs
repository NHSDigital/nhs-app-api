using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    internal sealed class TextAreaBy
    {
        private readonly string _id;

        internal TextAreaBy(string id)
        {
            _id = id;
        }

        internal By Id => By.XPath($"//textarea[@id={_id.QuoteXPathLiteral()}]");
    }
}