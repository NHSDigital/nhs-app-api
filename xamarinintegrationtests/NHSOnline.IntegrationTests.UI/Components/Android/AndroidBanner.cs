using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidBanner
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidBanner(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidBanner WithText(IAndroidInteractor interactor, string text)
            => new AndroidBanner(interactor, text);

        public void Click()
            => ActOnElement(e => e.Click());

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a banner with text {1} should be displayed", _text));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => MobileBy.AndroidUIAutomator($"new UiSelector().className(\"android.widget.TextView\").text({_text.QuoteUiAutomatorLiteral()})");
    }
}