using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidImageButton
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        public AndroidImageButton(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public void Click()
        {
            _interactor.ActOnElement(
                XPath,
                e => e.Click());
        }

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                XPath,
                e => e.Displayed.Should().BeTrue("a image button with text {1} should be displayed", _text));
        }

        private By XPath => By.XPath($"//android.widget.ImageButton[normalize-space(@content-desc)={_text.QuoteXPathLiteral()}]");
    }
}
