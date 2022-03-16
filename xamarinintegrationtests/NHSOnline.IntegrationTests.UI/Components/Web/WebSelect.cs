using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebSelect
    {
        private readonly IWebInteractor _interactor;
        private readonly SelectBy _by;

        private WebSelect(IWebInteractor interactor, string id)
        {
            _interactor = interactor;
            _by = new SelectBy(id);
        }

        public static WebSelect WithId(IWebInteractor interactor, string id)
            => new(interactor, id);

        public void Click()
            => ActOnSelectElement(e => e.Click());

        private void ActOnSelectElement(Action<IWebElement> action)
            => _interactor.ActOnElement(CheckboxFindBy, action);

        private By CheckboxFindBy
            => _by.Id;
    }
}
