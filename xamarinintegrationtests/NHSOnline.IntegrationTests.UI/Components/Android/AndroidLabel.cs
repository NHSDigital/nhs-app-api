using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidLabel
    {
        private readonly IAndroidInteractor _interactor;
        private readonly ILocatorStrategy _locatorStrategy;

        private AndroidLabel(IAndroidInteractor interactor, ILocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static AndroidLabel WithText(IAndroidInteractor interactor, string text)
            => new AndroidLabel(interactor, new TextLocatorStrategy(text));

        public static AndroidLabel WhichMatches(IAndroidInteractor interactor, string pattern)
            => new AndroidLabel(interactor, new MatchesLocatorStrategy(pattern));

        public AndroidLabel ScrollIntoView()
            => new AndroidLabel(_interactor, new ScrollLocatorStrategy(_locatorStrategy));

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue("a label {1} should be displayed", Description));
        }

        public void AssertNotVisible()
        {
            _interactor.AssertElementDoesntExist(FindBy);
        }

        public void Touch()
        {
            _interactor.ActOnElementContext(FindBy, context => context.Tap());
        }

        public void Click()
        {
            _interactor.ActOnElementContext(FindBy, context=>context.Element.Click());
        }

        private By FindBy => MobileBy.AndroidUIAutomator(_locatorStrategy.Selector);
        private string Description => _locatorStrategy.Description;

        private interface ILocatorStrategy
        {
            string Selector { get; }
            string Description { get; }
        }

        private sealed class TextLocatorStrategy : ILocatorStrategy
        {
            private readonly string _text;

            public TextLocatorStrategy(string text)
                => _text = text;

            public string Selector => $"new UiSelector().className(\"android.widget.TextView\").text({_text.QuoteUiAutomatorLiteral()})";
            public string Description => $"with text '{_text}'";
        }

        private sealed class MatchesLocatorStrategy : ILocatorStrategy
        {
            private readonly string _pattern;

            public MatchesLocatorStrategy(string pattern)
                => _pattern = pattern;
            
            public string Selector => $"new UiSelector().className(\"android.widget.TextView\").textMatches({_pattern.QuoteUiAutomatorLiteral()})";
            public string Description => $"which matches '{_pattern}'";
        }

        private sealed class ScrollLocatorStrategy : ILocatorStrategy
        {
            private readonly ILocatorStrategy _wrappedLocatorStrategy;

            public ScrollLocatorStrategy(ILocatorStrategy wrappedLocatorStrategy)
                => _wrappedLocatorStrategy = wrappedLocatorStrategy;

            public string Selector => $"new UiScrollable(new UiSelector().scrollable(true).instance(0)).scrollIntoView({_wrappedLocatorStrategy.Selector})";
            public string Description => _wrappedLocatorStrategy.Description;
        }
    }
}
