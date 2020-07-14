using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public abstract class AndroidLabel
    {
        private readonly IAndroidInteractor _interactor;

        private AndroidLabel(IAndroidInteractor interactor)
        {
            _interactor = interactor;
        }

        public static AndroidLabel WithText(IAndroidInteractor interactor, string text)
        {
            return new Text(interactor, text);
        }

        public static AndroidLabel WhichMatches(IAndroidInteractor interactor, string pattern)
        {
            return new Matches(interactor, pattern);
        }

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

        public void Click()
        {
            _interactor.ActOnElement(FindBy, e => e.Click());
        }

        protected abstract By FindBy { get; }
        protected abstract string Description { get; }

        private sealed class Text : AndroidLabel
        {
            private readonly string _text;

            public Text(IAndroidInteractor interactor, string text) : base(interactor)
            {
                _text = text;
            }

            protected override By FindBy => By.XPath($"//android.widget.TextView[normalize-space(@text)={_text.QuoteXPathLiteral()}]");
            protected override string Description => $"with text '{_text}'";
        }

        private sealed class Matches : AndroidLabel
        {
            private readonly string _pattern;

            public Matches(IAndroidInteractor interactor, string pattern) : base(interactor)
            {
                _pattern = pattern;
            }

            protected override By FindBy => MobileBy.AndroidUIAutomator($"new UiSelector().className(\"android.widget.TextView\").textMatches({_pattern.QuoteUiAutomatorLiteral()})");
            protected override string Description => $"which matches '{_pattern}'";
        }
    }
}
