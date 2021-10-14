using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidBrowserOverlayBrowserChoiceOption
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidBrowserOverlayBrowserChoiceOption(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidBrowserOverlayBrowserChoiceOption WithText(IAndroidInteractor interactor, string text)
            => new AndroidBrowserOverlayBrowserChoiceOption(interactor, text);

        public void Click()
            => _interactor.ActOnElement(FindBy, e => e.Click());

        public void AssertVisible()
            => _interactor.ActOnElement(FindBy, e => e.Displayed.Should().BeTrue("an App browser tab element with text {0} should be displayed", _text));

        private By FindBy => MobileBy.AndroidUIAutomator($"new UiSelector().className(\"android.widget.TextView\").text({_text.QuoteUiAutomatorLiteral()})");
    }
}
