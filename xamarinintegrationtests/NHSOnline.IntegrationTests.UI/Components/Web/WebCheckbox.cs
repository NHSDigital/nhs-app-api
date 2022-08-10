using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

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

        public WebLink WithChildLink(string linkText)
            => WebLink.WithText(_interactor, linkText);

        public void Click()
            => ActOnCheckboxElement(e => e.Click());

        public void VerifyClick()
            => ActOnCheckboxElement(e => e.Selected.Should().BeTrue("Checkbox should be selected"));

        public void ScrollTo() => _interactor.ActOnElementContext(
            CheckboxFindBy, c => new Actions(c.Driver).MoveToElement(c.Element).Perform());

        private void ActOnCheckboxElement(Action<IWebElement> action)
            => _interactor.ActOnElement(CheckboxFindBy, action);

        private By CheckboxFindBy
            => _by.Input(1);
    }
}
