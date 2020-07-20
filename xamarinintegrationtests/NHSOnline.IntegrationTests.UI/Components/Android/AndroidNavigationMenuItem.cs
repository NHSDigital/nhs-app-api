using System;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;
using NHSOnline.IntegrationTests.UI.Drivers.Native.Android;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidNavigationMenuItem
    {

        private readonly IAndroidInteractor _interactor;
        private readonly string _iconDescription;
        private readonly string _text;
        private readonly IAndroidInteractor _containedInteractor;

        public AndroidNavigationMenuItem(IAndroidInteractor interactor, string iconDescription, string text)
        {
            _interactor = interactor;
            _text = text;
            _containedInteractor = _interactor.CreateContainedInteractor(ContainerXPath);
            _iconDescription = iconDescription;
        }

        private AndroidIcon Icon => new AndroidIcon(_containedInteractor, _iconDescription);
        private AndroidLabel Label => AndroidLabel.WithText(_containedInteractor, _text);

        public void Click() => ActOnElement(e => e.Click());

        public void AssertVisible()
        {
            ActOnElement(e => e.Displayed.Should().BeTrue("a button with text {1} should be displayed", _text));
            Icon.AssertVisible();
            Label.AssertVisible();
        }

        private void ActOnElement(Action<AndroidElement> action) => _interactor.ActOnElement(ContainerXPath, action);

        private By ContainerXPath => By.XPath($"//android.view.ViewGroup[normalize-space(@content-desc)={_text.QuoteXPathLiteral()}]");
    }
}