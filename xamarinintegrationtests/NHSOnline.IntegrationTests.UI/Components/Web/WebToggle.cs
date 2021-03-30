using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebToggle
    {
        private readonly IWebInteractor _interactor;
        private readonly InputByText _by;

        private WebToggle(IWebInteractor interactor, string label)
        {
            _interactor = interactor;
            _by = new InputByText("checkbox", label);
        }

        public static WebToggle WithLabel(IWebInteractor interactor, string label)
            => new(interactor, label);

        public void AssertToggledOn()
            => _interactor.ActOnElement(CheckboxFindBy("true"),
                e => e.GetAttribute("aria-checked").Should().Be("true"));

        public void AssertToggledOff()
            => _interactor.ActOnElement(CheckboxFindByNotToggled(),
                e => e.GetAttribute("aria-checked").Should().Be(null));

        public void ToggleOn() => _interactor.ActOnElementContext(
            CheckboxFindBy(), c => new Actions(c.Driver).MoveToElement(c.Element).Click().Perform());

        public void ToggleOff() => _interactor.ActOnElementContext(
            CheckboxFindBy("true"), c => new Actions(c.Driver).MoveToElement(c.Element).Click().Perform());

        private By CheckboxFindBy(string checkedValue)
            => _by.LabelAndChecked(checkedValue);

        private By CheckboxFindBy()
            => _by.Input;

        private By CheckboxFindByNotToggled()
            => _by.LabelAndNotChecked();
    }
}
