using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebCheckbox
    {
        private readonly IWebInteractor _interactor;
        private readonly InputBy _by;

        private WebCheckbox(IWebInteractor interactor, string label)
        {
            _interactor = interactor;
            _by = new InputBy("checkbox", label);
        }

        public static WebCheckbox WithLabel(IWebInteractor interactor, string label)
            => new(interactor, label);

        public void Click()
            => ActOnCheckboxElement(e => e.Click());

        private void ActOnCheckboxElement(Action<IWebElement> action)
            => _interactor.ActOnElement(CheckboxFindBy, action);
        
        private By CheckboxFindBy
            => _by.Input;
    }
}
