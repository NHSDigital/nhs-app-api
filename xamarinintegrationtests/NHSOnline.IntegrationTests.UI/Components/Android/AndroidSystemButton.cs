using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidSystemButton
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _pattern;

        private AndroidSystemButton(IAndroidInteractor interactor, string pattern)
        {
            _interactor = interactor;
            _pattern = pattern;
        }

        public static AndroidSystemButton WhichMatches(IAndroidInteractor interactor, string pattern)
            => new(interactor, pattern);

        public void Click()
            => ActOnElement(e => e.Click());

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a button with matches {0} should be displayed", _pattern));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy => MobileBy.AndroidUIAutomator($"new UiSelector().className(\"android.widget.Button\").textMatches({_pattern.QuoteUiAutomatorLiteral()})");
    }
}