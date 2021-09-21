using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.BrowserStack;
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

        public static AndroidLink WithContentDescription(IAndroidInteractor interactor, string description)
            => new (interactor, new ContentDescriptionLocatorStrategy(description));

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a link with text {0} should be displayed", _locatorStrategy.Description));

        public void Touch()
            => ActOnElementContext(context => context.Tap());

        private void ActOnElementContext(Action<ElementContext<IAndroidBrowserStackDriver, AndroidElement>> action)
            => _interactor.ActOnElementContext(FindBy, action);

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy => MobileBy.AndroidUIAutomator(_locatorStrategy.Selector);

        public AndroidLink ScrollIntoView()
            => new(_interactor, new AndroidScrollLocatorStrategy(_locatorStrategy));

        string IFocusable.ElementDescription
            => new FocusableDescriptionBuilder {Tag = "android.view.ViewGroup", ContentDesc = _locatorStrategy.Description}.ViewGroupDescription;

        private sealed class ContentDescriptionLocatorStrategy : IAndroidLocatorStrategy
        {
            private readonly string _description;

            public ContentDescriptionLocatorStrategy(string description)
                => _description = description;

            public string Selector => $"new UiSelector().className(\"android.view.ViewGroup\").descriptionContains({_description.QuoteUiAutomatorLiteral()})";

            public string Description => $"{_description}";
        }
    }
}