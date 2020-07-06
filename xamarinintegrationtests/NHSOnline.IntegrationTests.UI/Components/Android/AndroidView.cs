using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public class AndroidView
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        public AndroidView(
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
                e => e.Displayed.Should().BeTrue("a view with text {1} should be displayed", _text));
        }

        public void AssertNotVisible()
        {
            _interactor.AssertElementDoesntExist(
                XPath);
        }

        private By XPath => By.XPath($"//android.view.View[normalize-space(@text)={_text.QuoteXPathLiteral()}]");
    }
}