using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidLink
    {
        private readonly IAndroidInteractor _interactor;
        private readonly IAndroidLocatorStrategy _locatorStrategy;

        private AndroidLink(IAndroidInteractor interactor, IAndroidLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static AndroidLink WithText(IAndroidInteractor interactor, string text)
            => new AndroidLink(interactor, new TextLocatorStrategy(text));

        public AndroidLink ScrollIntoView()
            => new AndroidLink(_interactor, new AndroidScrollLocatorStrategy(_locatorStrategy));

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue("a link {1} should be displayed", Description));
        }
        
        public void Touch()
        {
            _interactor.ActOnElementContext(FindBy, context => context.Tap());
        }

        private By FindBy => MobileBy.AndroidUIAutomator(_locatorStrategy.Selector);
        private string Description => _locatorStrategy.Description;

        private sealed class TextLocatorStrategy : IAndroidLocatorStrategy
        {
            private readonly string _text;

            public TextLocatorStrategy(string text)
                => _text = text;

            public string Selector => $"new UiSelector().className(\"android.widget.TextView\").text({_text.QuoteUiAutomatorLiteral()})";
            public string Description => $"with text '{_text}'";
        }
    }
}