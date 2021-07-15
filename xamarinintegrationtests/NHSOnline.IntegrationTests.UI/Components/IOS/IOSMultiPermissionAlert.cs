using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSMultiPermissionAlert
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _text;

        private By AllowOnceFindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeButton' AND label == 'Allow Once'");

        private readonly string DoNotAllowText = "Don’t Allow";

        private By DenyFindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeButton' AND label == {DoNotAllowText.QuotePredicateLiteral()}");

        private IOSMultiPermissionAlert(IIOSInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static IOSMultiPermissionAlert WithMatchingText(IIOSInteractor interactor, string matchingText) => new(interactor, matchingText);

        public void AssertText() => _interactor.ActOnDriver(driver => driver.SwitchTo().Alert().Text.Should().Be(_text));

        public string Text()
        {
            var text = "";
            _interactor.ActOnDriver(driver => text = driver.SwitchTo().Alert().Text);
            return text;
        }

        public void AcceptAllowOnce() => _interactor.ActOnElement(AllowOnceFindBy, e => e.Click());

        public void DoNotAllow() => _interactor.ActOnElement(DenyFindBy, e => e.Click());
    }
}
