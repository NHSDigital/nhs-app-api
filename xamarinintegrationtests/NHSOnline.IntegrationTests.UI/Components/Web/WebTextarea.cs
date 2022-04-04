using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

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
            => ActOnElement(e => e.SendKeys(text));

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(TextAreaFindBy, action);

        public void ScrollTo() => _interactor.ActOnElementContext(
            TextAreaFindBy, c => new Actions(c.Driver).MoveToElement(c.Element).Perform());

        private By TextAreaFindBy
            => _by.Id;
    }
}
