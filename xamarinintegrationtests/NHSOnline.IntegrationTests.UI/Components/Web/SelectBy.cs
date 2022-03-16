using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    internal sealed class SelectBy
    {
        private readonly string _id;

        internal SelectBy(string id)
        {
            _id = id;
        }

        internal By Id => By.XPath($"//select[@id={_id.QuoteXPathLiteral()}]");
    }
}