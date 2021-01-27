using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidExpander
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _headerText;
        private readonly string _bodyText;
        private AndroidExpander(IAndroidInteractor interactor, string headerText, string bodyText)
        {
            _interactor = interactor;
            _headerText = headerText;
            _bodyText = bodyText;
        }

        public static AndroidExpander WithHeaderAndBodyText(IAndroidInteractor interactor, string headerText, string bodyText)
            => new AndroidExpander(interactor, headerText, bodyText);

        public void AssertVisible() => ActOnHeaderElement(e => e.Displayed.Should().BeTrue("an expander with header '{1}' should be displayed", _headerText));

        public void AssertCollapsed()
        {
            ActOnContainerElement(
                containerElement =>
                {
                    var children = containerElement
                        .FindElements(By.ClassName("android.widget.TextView"));
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
                        .FindElements(By.ClassName("android.widget.TextView"));
                    children
                        .Select(child => child.Text)
                        .Should()
                        .Equal(new[] { _headerText, _bodyText }, "only the header and body text should be visible");
                });
        }

        public void Toggle() => ActOnHeaderElement(e => e.Click());

        private void ActOnHeaderElement(Action<AndroidElement> action) => _interactor.ActOnElement(FindHeaderBy, action);
        private void ActOnContainerElement(Action<AndroidElement> action) => _interactor.ActOnElement(FindContainerBy, action);

        private By FindHeaderBy => By.XPath(HeaderSelector);
        private By FindContainerBy => By.XPath(ContainerSelector);

        // The common parent of the header and body is two ancestors up from the header text
        private string ContainerSelector => $"{HeaderSelector}/../..";
        private string HeaderSelector => $"//android.widget.TextView[normalize-space(@text)={_headerText.QuoteXPathLiteral()}]";
    }
}