using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidLabel
    {
        private readonly IAndroidInteractor _interactor;
        private readonly IAndroidLocatorStrategy _locatorStrategy;

        private AndroidLabel(IAndroidInteractor interactor, IAndroidLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static AndroidLabel WithText(IAndroidInteractor interactor, string text)
            => new AndroidLabel(interactor, new TextLocatorStrategy(text));

        public static AndroidLabel WhichMatches(IAndroidInteractor interactor, string pattern)
            => new AndroidLabel(interactor, new MatchesLocatorStrategy(pattern));

        public AndroidLabel ScrollIntoView()
            => new AndroidLabel(_interactor, new AndroidScrollLocatorStrategy(_locatorStrategy));

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue("a label {1} should be displayed", Description));
        }

        public void Click()
        {
            _interactor.ActOnElementContext(FindBy, context=>context.Element.Click());
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

        private sealed class MatchesLocatorStrategy : IAndroidLocatorStrategy
        {
            private readonly string _pattern;

            public MatchesLocatorStrategy(string pattern)
                => _pattern = pattern;
            
            public string Selector => $"new UiSelector().className(\"android.widget.TextView\").textMatches({_pattern.QuoteUiAutomatorLiteral()})";
            public string Description => $"which matches '{_pattern}'";
        }
    }
}
