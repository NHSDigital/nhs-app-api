using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidBanner
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidBanner(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidBanner WithText(IAndroidInteractor interactor, string text)
            => new AndroidBanner(interactor, text);

        public void Click()
            => _interactor.ActOnElement(FindBy, e => e.Click());

        public void AssertVisible()
            => _interactor.ActOnElement(FindBy, e => e.Displayed.Should().BeTrue("a banner with text {1} should be displayed", _text));

        private By FindBy
            => MobileBy.AndroidUIAutomator($"new UiSelector().className(\"android.widget.TextView\").text({_text.QuoteUiAutomatorLiteral()})");
    }
}