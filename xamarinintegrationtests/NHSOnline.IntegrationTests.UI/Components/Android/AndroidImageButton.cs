using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidImageButton
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _description;
        private readonly string _fallbackDescription;

        private AndroidImageButton(IAndroidInteractor interactor, string description, string fallbackDescription)
        {
            _interactor = interactor;
            _description = description;
            _fallbackDescription = fallbackDescription;
        }

        public static AndroidImageButton WithDescription(IAndroidInteractor interactor, string description)
            => new AndroidImageButton(interactor, description, "");

        public static AndroidImageButton WithDescription(IAndroidInteractor interactor, string description, string fallBackDescription)
            => new AndroidImageButton(interactor, description, fallBackDescription);

        public void Click()
            => ActOnElement(e => e.Click());

        public void ClickEither()
            => ActOnEitherElement(e => e.Click());

        public void AssertVisible() =>
            ActOnElement(e =>
                e.Displayed.Should().BeTrue("a label {0} should be displayed", _description));

        public void AssertEitherVisible() =>
            ActOnEitherElement(e =>
                e.Displayed.Should().BeTrue("a label {0} or {1} should be displayed", _description, _fallbackDescription));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private void ActOnEitherElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindByEither, action);

        private By FindBy
            => By.XPath($"//android.widget.ImageButton[normalize-space(@content-desc)={_description.QuoteXPathLiteral()}]");

        private By FindByEither
            => By.XPath($"//android.widget.ImageButton[normalize-space(@content-desc)={_description.QuoteXPathLiteral()} or normalize-space(@content-desc)={_fallbackDescription.QuoteXPathLiteral()}]");
    }
}
