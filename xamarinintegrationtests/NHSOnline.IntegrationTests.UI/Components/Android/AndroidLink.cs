using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidLink
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidLink(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidLink WithText(IAndroidInteractor interactor, string text)
            => new AndroidLink(interactor, text);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a link with text {1} should be displayed", _text));

        public void Touch()
            => ActOnElementContext(context => context.Tap());

        private void ActOnElementContext(Action<ElementContext<AndroidDriver<AndroidElement>, AndroidElement>> action)
            => _interactor.ActOnElementContext(FindBy, action);

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => MobileBy.AndroidUIAutomator($"new UiSelector().className(\"android.widget.TextView\").text({_text.QuoteUiAutomatorLiteral()})");
    }
}