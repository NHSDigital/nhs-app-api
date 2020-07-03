using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidLabel
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        public AndroidLabel(
            IAndroidInteractor interactor,
            string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public void AssertVisible()
        {
            _interactor.ActOnElement(
                XPath,
                e => e.Displayed.Should().BeTrue("a label with text {1} should be displayed", _text));
        }

        public void AssertNotVisible()
        {
            _interactor.AssertElementDoesntExist(
                XPath);
        }

        public void Click()
        {
            _interactor.ActOnElement(
                XPath,
                e => e.Click());
        }

        private By XPath => By.XPath($"//android.widget.TextView[normalize-space(@text)={_text.QuoteXPathLiteral()}]");
    }
}
