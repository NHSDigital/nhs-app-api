using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public class AndroidFullNavigationFooter
    {
        private readonly IAndroidInteractor _interactor;
        private const string Name = "NHS App Full Navigation Footer";

        public AndroidFullNavigationFooter(IAndroidInteractor interactor)
        {
            _interactor = interactor;
        }

        public void AssertVisible()
        {
            ActOnElement(e => e.Displayed.Should().BeTrue("a navigation bar should be displayed"));
        }

        private void ActOnElement(Action<AndroidElement> action) => _interactor.ActOnElement(FindBy, action);

        private static By FindBy => By.XPath($"//android.view.ViewGroup[normalize-space(@content-desc)={Name.QuoteXPathLiteral()}]");
    }
}
