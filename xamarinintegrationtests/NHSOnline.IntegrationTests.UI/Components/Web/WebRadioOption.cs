using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebRadioOption
    {
        private readonly IWebInteractor _interactor;
        private readonly string _legend;
        private readonly string _option;

        private WebRadioOption(IWebInteractor interactor, string legend, string option)
        {
            _interactor = interactor;
            _legend = legend;
            _option = option;
        }

        public static WebRadioOption InFieldsetLegendWithLabel(IWebInteractor interactor, string legend, string option)
            => new WebRadioOption(interactor, legend, option);

        public void Click()
            => ActOnLabelElement(e => e.Click());

        private void ActOnLabelElement(Action<IWebElement> action)
            => _interactor.ActOnElement(LabelFindBy, action);

        private By LabelFindBy
            => By.XPath($"//fieldset[legend[normalize-space(text()) = {_legend.QuoteXPathLiteral()}]]//label[normalize-space(text()) = {_option.QuoteXPathLiteral()}]");
    }
}