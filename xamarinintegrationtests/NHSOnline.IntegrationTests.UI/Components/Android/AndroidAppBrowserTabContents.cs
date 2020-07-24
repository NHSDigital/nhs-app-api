using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public class AndroidAppBrowserTabContents
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidAppBrowserTabContents(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidAppBrowserTabContents WithText(IAndroidInteractor interactor, string text)
            => new AndroidAppBrowserTabContents(interactor, text);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a view with text {1} should be displayed", _text));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//android.view.View[normalize-space(@text)={_text.QuoteXPathLiteral()}]");
    }
}