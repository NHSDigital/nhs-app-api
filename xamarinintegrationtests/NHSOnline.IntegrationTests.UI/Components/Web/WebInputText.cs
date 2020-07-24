using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebInputText
    {
        private readonly IWebInteractor _interactor;
        private readonly InputBy _by;

        private WebInputText(IWebInteractor interactor, string label)
        {
            _interactor = interactor;
            _by = new InputBy("text", label);
        }

        public static WebInputText WithLabel(IWebInteractor interactor, string label)
            => new WebInputText(interactor, label);

        public void EnterText(string text)
            => ActOnInputElement(e => e.SendKeys(text));

        private void ActOnInputElement(Action<IWebElement> action)
            => _interactor.ActOnElement(InputFindBy, action);

        private By InputFindBy
            => _by.Input;
    }
}