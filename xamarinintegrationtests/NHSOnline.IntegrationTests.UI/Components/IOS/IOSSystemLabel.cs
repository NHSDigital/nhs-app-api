using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSSystemLabel
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _text;

        private IOSSystemLabel(IIOSInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static IOSSystemLabel WithText(IIOSInteractor interactor, string text)
            => new IOSSystemLabel(interactor, text);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a system label with text {0} should be displayed", _text));

        public void Click() => ActOnElement(element => element.Click());

        public bool IsPresent()
        {
            var isPresent = false;
            _interactor.ActOnDriver((_, findBy) =>
            {
                try
                {
                    var element = findBy(FindBy);
                    isPresent = element != null;
                }
                catch (NoSuchElementException)
                {
                    isPresent =  false;
                }
            });

            return isPresent;
        }

        private By FindBy => MobileBy.IosNSPredicate($"type == 'XCUIElementTypeStaticText' AND value == {_text.QuotePredicateLiteral()}");

        private void ActOnElement(Action<IOSElement> action)
            => _interactor.ActOnElement(FindBy, action);
    }
}