using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidButton
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        public AndroidButton(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public void Click()
        {
            _interactor.ActOnElement(
                By.XPath($"//android.widget.Button[normalize-space(@text)={_text.QuoteXPathLiteral()}]"),
                e => e.Click());
        }
    }
}