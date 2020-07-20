using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public class IOSNavigationMenuItem
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _iconDescription;
        private readonly string _text;
        private readonly IIOSInteractor _containedInteractor;

        public IOSNavigationMenuItem(IIOSInteractor interactor, string iconDescription, string text)
        {
            _interactor = interactor;
            _text = text;
            _containedInteractor = _interactor.CreateContainedInteractor(ContainerFindBy);
            _iconDescription = iconDescription;
        }

        private IOSIcon Icon => new IOSIcon(_containedInteractor, _iconDescription);
        private IOSLabel Label => IOSLabel.WithText(_containedInteractor, _text);

        public void Click() => ActOnElement(e => e.Click());

        public void AssertVisible()
        {
            ActOnElement(e => e.Displayed.Should().BeTrue("a button with text {1} should be displayed", _text));
            Icon.AssertVisible();
            Label.AssertVisible();
        }

        private void ActOnElement(Action<IOSElement> action) => _interactor.ActOnElement(ContainerFindBy, action);

        private By ContainerFindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeOther' AND name == {_text.QuotePredicateLiteral()}");
    }
}