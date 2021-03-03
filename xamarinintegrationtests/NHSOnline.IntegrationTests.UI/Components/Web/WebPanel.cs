using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebPanel: IWebContainer
    {
        private readonly IWebInteractor _interactor;
        private readonly string _text;
        private readonly IWebInteractor _containerInteractor;

        private WebPanel(IWebInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
            _containerInteractor = _interactor.CreateContainedInteractor(FindBy);
        }

        public static WebPanel WithTitle(IWebInteractor interactor, string text)
            => new (interactor, text);
        
        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);
        
        private By FindBy
            => By.XPath($"//div[h2[normalize-space()={_text.QuoteXPathLiteral()}]]");

        IWebInteractor IWebContainer.ContainerInteractor => _containerInteractor;
    }
}
