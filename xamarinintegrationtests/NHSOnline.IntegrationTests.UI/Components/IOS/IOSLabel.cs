using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public abstract class IOSLabel
    {
        private readonly IIOSInteractor _interactor;

        private IOSLabel(IIOSInteractor interactor)
        {
            _interactor = interactor;
        }

        public static IOSLabel WithText(IIOSInteractor interactor, string text)
        {
            return new Text(interactor, text);
        }

        public static IOSLabel WhichMatches(IIOSInteractor interactor, string pattern)
        {
            return new Matches(interactor, pattern);
        }

        public void AssertVisible()
            => _interactor.ActOnElement(FindBy, e => e.Displayed.Should().BeTrue("a label {1} should be displayed", Description));

        public void AssertNotVisible() => _interactor.AssertElementDoesntExist(FindBy);

        public void Click() => _interactor.ActOnElementContext(FindBy, context => context.Tap());

        protected abstract By FindBy { get; }
        protected abstract string Description { get; }

        private sealed class Text : IOSLabel
        {
            private readonly string _text;

            public Text(IIOSInteractor interactor, string text) : base(interactor)
            {
                _text = text;
            }

            protected override By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeStaticText' AND value == {_text.QuotePredicateLiteral()} and visible == 1");
            protected override string Description => $"with text '{_text}'";
        }

        private sealed class Matches : IOSLabel
        {
            private readonly string _pattern;

            public Matches(IIOSInteractor interactor, string pattern) : base(interactor)
            {
                _pattern = pattern;
            }

            protected override By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeStaticText' AND value MATCHES {_pattern.QuotePredicateLiteral()}");
            protected override string Description => $"which matches '{_pattern}'";
        }
    }
}