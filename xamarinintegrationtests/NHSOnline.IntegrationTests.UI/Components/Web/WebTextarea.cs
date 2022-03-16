using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebTextarea
    {
        private readonly IWebInteractor _interactor;
        private readonly TextAreaBy _by;

        private WebTextarea(IWebInteractor interactor, string id)
        {
            _interactor = interactor;
            _by = new TextAreaBy(id);
        }

        public static WebTextarea WithId(IWebInteractor interactor, string id)
            => new(interactor, id);

        public void InsertText(string text)
            => ActOnSelectElement(e => e.SendKeys(text));

        private void ActOnSelectElement(Action<IWebElement> action)
            => _interactor.ActOnElement(CheckboxFindBy, action);

        private By CheckboxFindBy
            => _by.Id;
    }
}
