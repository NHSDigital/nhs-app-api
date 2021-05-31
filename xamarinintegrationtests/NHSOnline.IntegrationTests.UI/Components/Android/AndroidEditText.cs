using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidEditText
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidEditText(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidEditText WithText(IAndroidInteractor interactor, string text)
            => new AndroidEditText(interactor, text);

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue("a editable text field with {0} should be displayed", _text));
        }

        private By FindBy
            => By.XPath($".//android.widget.EditText[normalize-space(@text)={_text.QuoteXPathLiteral()}]");
    }
}