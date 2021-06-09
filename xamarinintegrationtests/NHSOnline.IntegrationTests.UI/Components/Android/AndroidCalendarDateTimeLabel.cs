using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidCalendarDateTimeLabel
    {
        private readonly IAndroidInteractor _interactor;
        private readonly IAndroidLocatorStrategy _locatorStrategy;

        private AndroidCalendarDateTimeLabel(IAndroidInteractor interactor, IAndroidLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static AndroidCalendarDateTimeLabel WithContentDescription(IAndroidInteractor interactor, string description)
            => new(interactor, new DescriptionLocatorStrategy(description));

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue("a label {0} should be displayed", Description));
        }

        public void Click()
        {
            _interactor.ActOnElementContext(FindBy, context=>context.Element.Click());
        }

        private By FindBy => MobileBy.AndroidUIAutomator(_locatorStrategy.Selector);
        private string Description => _locatorStrategy.Description;

        private sealed class DescriptionLocatorStrategy : IAndroidLocatorStrategy
        {
            private readonly string _description;

            public DescriptionLocatorStrategy(string description)
                => _description = description;

            public string Selector => $"new UiSelector().descriptionContains({_description.QuoteUiAutomatorLiteral()})";

            public string Description => $"with description '{_description}'";
        }
    }
}
