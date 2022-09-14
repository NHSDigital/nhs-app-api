using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidImageFrame
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string? _description;

        private AndroidImageFrame(IAndroidInteractor interactor, string description)
        {
            _interactor = interactor;
            _description = description;
        }

        public static AndroidImageFrame WithContentDescription(IAndroidInteractor interactor, string description) =>
            new(interactor, description);

        public void ClickByDesc()
            => ActOnElementByDesc(e => e.Click());

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                FindByDescription,
                e => e.Displayed.Should().BeTrue("a frame with description {0} should be displayed", _description));
        }

        private void ActOnElementByDesc(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindByDescription, action);

        private By FindByDescription
            =>  By.XPath($"//android.widget.LinearLayout[contains(@content-desc, {_description!.QuoteXPathLiteral()})]");
    }
}
