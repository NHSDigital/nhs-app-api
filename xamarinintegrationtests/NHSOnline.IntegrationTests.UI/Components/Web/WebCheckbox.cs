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
            => new WebCheckbox(interactor, label);

        public void Click()
            => ActOnLabelElement(e => e.Click());

        private void ActOnLabelElement(Action<IWebElement> action)
            => _interactor.ActOnElement(LabelFindBy, action);

        private By LabelFindBy
            => _by.Label;
    }
}
