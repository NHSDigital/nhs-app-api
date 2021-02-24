using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidLink : IFocusable
    {
        private readonly IAndroidInteractor _interactor;
        private readonly IAndroidLocatorStrategy _locatorStrategy;

        private AndroidLink(IAndroidInteractor interactor, IAndroidLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static AndroidLink WithText(IAndroidInteractor interactor, string text)
            => new (interactor, new TextLocatorStrategy(text));

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a link with text {1} should be displayed", _locatorStrategy.Description));

        public void Touch()
            => ActOnElementContext(context => context.Tap());

        private void ActOnElementContext(Action<ElementContext<AndroidDriver<AndroidElement>, AndroidElement>> action)
            => _interactor.ActOnElementContext(FindBy, action);

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy => MobileBy.AndroidUIAutomator(_locatorStrategy.Selector);

        public AndroidLink ScrollIntoView()
            => new(_interactor, new AndroidScrollLocatorStrategy(_locatorStrategy));

        string IFocusable.ElementDescription
            => new FocusableDescriptionBuilder {Tag = "android.widget.TextView", Text = _locatorStrategy.Description}.Description;

        private sealed class TextLocatorStrategy : IAndroidLocatorStrategy
        {
            private readonly string _text;

            public TextLocatorStrategy(string text)
                => _text = text;

            public string Selector => $"new UiSelector().className(\"android.widget.TextView\").text({_text.QuoteUiAutomatorLiteral()})";
            public string Description => $"{_text}";
        }
    }
}