using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidAppTabBrowserChoiceOption
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidAppTabBrowserChoiceOption(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidAppTabBrowserChoiceOption WithText(IAndroidInteractor interactor, string text)
            => new AndroidAppTabBrowserChoiceOption(interactor, text);

        public void Click()
            => _interactor.ActOnElement(FindBy, e => e.Click());

        public void AssertVisible()
            => _interactor.ActOnElement(FindBy, e => e.Displayed.Should().BeTrue("an App browser tab element with text {1} should be displayed", _text));

        private By FindBy => MobileBy.AndroidUIAutomator($"new UiSelector().className(\"android.widget.TextView\").text({_text.QuoteUiAutomatorLiteral()})");
    }
}
