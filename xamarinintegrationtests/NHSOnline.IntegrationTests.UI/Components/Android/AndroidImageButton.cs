using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidImageButton
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _description;

        private AndroidImageButton(IAndroidInteractor interactor, string description)
        {
            _interactor = interactor;
            _description = description;
        }

        public static AndroidImageButton WithDescription(IAndroidInteractor interactor, string description)
            => new AndroidImageButton(interactor, description);

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//android.widget.ImageButton[normalize-space(@content-desc)={_description.QuoteXPathLiteral()}]");
    }
}
