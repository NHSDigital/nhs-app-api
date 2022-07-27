using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidGLButton
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidGLButton(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidGLButton WithText(IAndroidInteractor interactor, string description)
            => new AndroidGLButton(interactor, description);

        public void Click()
            => ActOnElement(e => e.Click());

        public void AssertVisible() =>
            ActOnElement(e =>
                e.Displayed.Should().BeTrue("a GL Button {0} should be displayed", _text));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//GLButton[normalize-space(@text)={_text.QuoteXPathLiteral()}]");
    }
}
