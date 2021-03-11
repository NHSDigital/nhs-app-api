using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public class WebFormLabel
    {
        private readonly IWebInteractor _interactor;
        private readonly string _text;

        private WebFormLabel(IWebInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static WebFormLabel WithText(IWebInteractor interactor, string text)
            => new WebFormLabel(interactor, text);


        public void Click()
            => _interactor.ActOnElementContext(
                FindBy,
                c =>
                {
                    new Actions(c.Driver).MoveToElement(c.Element);
                    c.Element.Click();
                });

        private By FindBy
            => By.XPath($".//label[normalize-space(text())={_text.QuoteXPathLiteral()}]");
    }
}