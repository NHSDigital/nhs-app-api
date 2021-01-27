using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidButton
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidButton(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidButton WithText(IAndroidInteractor interactor, string text)
            => new AndroidButton(interactor, text);

        public void Click()
            => ActOnElement(e => e.Click());

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a button with text {1} should be displayed", _text));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//android.widget.Button[normalize-space(@text)={_text.QuoteXPathLiteral()}]");
    }
}
