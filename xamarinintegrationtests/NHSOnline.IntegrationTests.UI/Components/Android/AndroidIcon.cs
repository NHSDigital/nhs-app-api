using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public class AndroidIcon
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _description;

        private AndroidIcon(IAndroidInteractor interactor, string description)
        {
            _interactor = interactor;
            _description = description;
        }

        public static AndroidIcon WithDescription(IAndroidInteractor interactor, string description)
            => new AndroidIcon(interactor, description);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("an icon with description {1} should be displayed", _description));
        
        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//android.view.ViewGroup[normalize-space(@content-desc)={_description.QuoteXPathLiteral()}]");
    }
}