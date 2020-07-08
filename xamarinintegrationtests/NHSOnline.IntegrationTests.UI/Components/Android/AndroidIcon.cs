using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public class AndroidIcon
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        public AndroidIcon(
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
                e => e.Displayed.Should().BeTrue("a view group with content description {1} should be displayed", _text));
        }

        public void Click()
        {
            _interactor.ActOnElement(
                XPath,
                e => e.Click());
        }

        private By XPath => By.XPath($"//android.view.ViewGroup[normalize-space(@content-desc)={_text.QuoteXPathLiteral()}]");
    }
}