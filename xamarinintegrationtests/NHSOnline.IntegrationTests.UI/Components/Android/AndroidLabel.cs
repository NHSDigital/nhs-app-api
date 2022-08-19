using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android.UiAutomator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            => new(interactor, new TextLocatorStrategy(text));

        public static AndroidLabel WhichMatches(IAndroidInteractor interactor, string pattern)
            => new(interactor, new MatchesLocatorStrategy(pattern));

        public static AndroidLabel WithContentDescription(IAndroidInteractor interactor, string description)
            => new(interactor, new DescriptionLocatorStrategy(description));

        public static AndroidLabel WithResourceId(IAndroidInteractor interactor, string resourceId)
            => new(interactor, new ResourceIdLocatorStrategy(resourceId));

        public AndroidLabel ScrollIntoView()
            => new(_interactor, new AndroidScrollLocatorStrategy(_locatorStrategy));

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue("a label {0} should be displayed", Description));
        }

        public bool IsPresent()
        {
            var isPresent = true;

            try
            {
                _interactor.ActOnElement(
                    FindBy,
                    e => e.Displayed.Should().BeTrue("a label {0} should be displayed", Description));
            }
            catch (AssertFailedException)
            {
                isPresent = false;
            }

            return isPresent;
        }

        public void AssertTextContains(string expectedText)
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Text.Should().Contain(expectedText, "label should contain {0}", expectedText));
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

        private sealed class DescriptionLocatorStrategy : IAndroidLocatorStrategy
        {
            private readonly string _description;

            public DescriptionLocatorStrategy(string description)
                => _description = description;

            public string Selector => $"new UiSelector().className(\"android.widget.TextView\").descriptionContains({_description.QuoteUiAutomatorLiteral()})";

            public string Description => $"with description '{_description}'";
        }

        private sealed class ResourceIdLocatorStrategy : IAndroidLocatorStrategy
        {
            private readonly string _resourceId;

            public ResourceIdLocatorStrategy(string resourceId)
                => _resourceId = resourceId;

            public string Selector => $"new UiSelector().className(\"android.widget.TextView\").resourceIdMatches({_resourceId.QuoteUiAutomatorLiteral()})";

            public string Description => $"with resource-id '{_resourceId}'";
        }
    }
}
