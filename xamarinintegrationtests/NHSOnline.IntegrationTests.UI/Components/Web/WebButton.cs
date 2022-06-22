using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NHSOnline.IntegrationTests.UI.Components.Web
{
    public sealed class WebButton: IFocusable
    {
        private readonly IWebInteractor _interactor;
        private readonly string _text;

        private WebButton(IWebInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static WebButton WithText(IWebInteractor interactor, string text)
            => new WebButton(interactor, text);

        public void Click()
            => _interactor.ActOnElementContext(
                FindBy,
                c =>
                {
                    ScrollTo();
                    c.Element.Click();
                });

        public void ScrollTo() => _interactor.ActOnElementContext(
            FindBy, c => new Actions(c.Driver).MoveToElement(c.Element).Perform());

        private void ActOnElement(Action<IWebElement> action)
            => _interactor.ActOnElement(FindBy, action);

        //query will get only the first matching element if more than one found
        private By FindBy
            => By.XPath($"(.//button[normalize-space(text())={_text.QuoteXPathLiteral()}])[1]");

        string IFocusable.ElementDescription
            => new FocusableDescriptionBuilder {Tag = "Button", Text = _text}.Description;

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("A button with text {0} should be displayed", _text));

        public void AssertNotVisible() => _interactor.IsPresent(FindBy).Should().BeFalse("A button with text {0} should not be displayed", _text);

        public bool IsVisible() => _interactor.IsPresent(FindBy);
    }
}