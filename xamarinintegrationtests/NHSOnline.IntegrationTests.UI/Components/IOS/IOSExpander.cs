using System;
using System.Linq;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSExpander
    {
        private readonly IIOSInteractor _interactor;
        private readonly string _headerText;
        private readonly string _bodyText;

        private IOSExpander(IIOSInteractor interactor, string headerText, string bodyText)
        {
            _interactor = interactor;
            _headerText = headerText;
            _bodyText = bodyText;
        }

        public static IOSExpander WithHeaderAndBodyText(IIOSInteractor interactor, string headerText, string bodyText)
            => new IOSExpander(interactor, headerText, bodyText);

        public void AssertVisible() => ActOnHeaderElement(e => e.Displayed.Should().BeTrue("an expander with header '{1}' should be displayed", _headerText));

        public void AssertCollapsed()
        {
            ActOnContainerElement(
                containerElement =>
                {
                    var children = containerElement
                        .FindElements(MobileBy.IosNSPredicate("type != 'XCUIElementTypeOther' AND visible == 1"));

                    children
                        .Should()
                        .ContainSingle("only the header text should be visible")
                        .Which.Text.Should().Be(_headerText, "only the header text should be visible");
                });
        }

        public void AssertExpanded()
        {
            ActOnContainerElement(
                containerElement =>
                {
                    var children = containerElement
                        .FindElements(MobileBy.IosNSPredicate("type != 'XCUIElementTypeOther' AND visible == 1"));

                    children
                        .Select(child => child.Text)
                        .Should()
                        .Equal(new []{_headerText, _bodyText}, "only the header and body text should be visible");
                });
        }

        public void Toggle() => ActOnHeaderElement(e => e.Click());

        private void ActOnHeaderElement(Action<IOSElement> action) => _interactor.ActOnElement(FindHeaderBy, action);
        private void ActOnContainerElement(Action<IOSElement> action) => _interactor.ActOnElement(FindContainerBy, action);

        private By FindHeaderBy => MobileBy.IosNSPredicate(FindHeaderPredicate);
        // The common parent of the header and body is three ancestors up from the header text
        private By FindContainerBy => MobileBy.IosClassChain($"**/XCUIElementTypeAny[${FindHeaderPredicate}$][-4]");

        private string FindHeaderPredicate => $"type == 'XCUIElementTypeStaticText' AND label == {_headerText.QuotePredicateLiteral()}";

    }
}