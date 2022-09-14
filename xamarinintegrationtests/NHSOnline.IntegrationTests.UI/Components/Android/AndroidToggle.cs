using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public class AndroidToggle
    {
        private readonly IAndroidInteractor _interactor;
        private readonly IAndroidLocatorStrategy _locatorStrategy;

        private AndroidToggle(IAndroidInteractor interactor, IAndroidLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static AndroidToggle WithTextMatches(IAndroidInteractor interactor, string pattern)
            => new AndroidToggle(interactor, new TextLocatorStrategy(pattern));

        public void Click() => ActOnElement(e => e.Click());

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a button with text {0} should be displayed", _locatorStrategy.Description));

        private void ActOnElement(Action<AndroidElement> action) => _interactor.ActOnElement(FindBy, action);

        private By FindBy => MobileBy.AndroidUIAutomator(_locatorStrategy.Selector);

        private sealed class TextLocatorStrategy : IAndroidLocatorStrategy
        {
            private readonly string _pattern;

            public TextLocatorStrategy(string pattern) => _pattern = pattern;

            public string Selector => $"new UiSelector().className(\"android.widget.Switch\").textMatches({_pattern.QuoteUiAutomatorLiteral()})";

            public string Description => $"{_pattern}";
        }
    }
}