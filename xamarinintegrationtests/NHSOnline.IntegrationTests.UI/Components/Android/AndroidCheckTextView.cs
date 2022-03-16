using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidCheckedTextView
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _text;

        private AndroidCheckedTextView(IAndroidInteractor interactor, string text)
        {
            _interactor = interactor;
            _text = text;
        }

        public static AndroidCheckedTextView WithText(IAndroidInteractor interactor, string text)
            => new (interactor, text);

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($".//android.widget.CheckedTextView[normalize-space(@text)={_text.QuoteXPathLiteral()}]");

        public void Click()
            => ActOnElement(e => e.Click());

    }
}