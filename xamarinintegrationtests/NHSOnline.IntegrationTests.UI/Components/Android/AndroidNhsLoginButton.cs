using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidNhsLoginButton
    {
        private readonly IAndroidInteractor _interactor;
        private readonly IAndroidLocatorStrategy _locatorStrategy;

        private AndroidNhsLoginButton(IAndroidInteractor interactor, IAndroidLocatorStrategy locatorStrategy)
        {
            _interactor = interactor;
            _locatorStrategy = locatorStrategy;
        }

        public static AndroidNhsLoginButton WithContentDescription(IAndroidInteractor interactor, string description)
            => new(interactor, new DescriptionLocatorStrategy(description));

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindBy,
                e => e.Displayed.Should().BeTrue("a button with description {0} should be displayed", Description));
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

            public string Selector => $"new UiSelector().className(\"android.view.ViewGroup\").descriptionContains({_description.QuoteUiAutomatorLiteral()})";

            public string Description => $"with description '{_description}'";
        }
    }
}
