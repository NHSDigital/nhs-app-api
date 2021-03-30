using System;
using FluentAssertions;
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

        public void AssertVisible() =>
            ActOnElement(e =>
                e.Displayed.Should().BeTrue("a label {0} should be displayed", _description));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//android.widget.ImageButton[normalize-space(@content-desc)={_description.QuoteXPathLiteral()}]");
    }
}
