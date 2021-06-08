using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidIcon: IFocusable
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _name;

        private AndroidIcon(IAndroidInteractor interactor, string name)
        {
            _interactor = interactor;
            _name = name;
        }

        public static AndroidIcon WithName(IAndroidInteractor interactor, string name)
            => new AndroidIcon(interactor, name);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("an icon with description {1} should be displayed", _name));

        public void Click()
            => ActOnElement(e => e.Click());

        public void AssertSelected()
            => ActOnElement(e => e.GetAttribute("selected").Should().Be("true",
                    "a selected icon with description {1} should be displayed"));

        public void AssertNotSelected()
            => ActOnElement(e => e.GetAttribute("selected").Should().Be("false",
                "a selected icon with description {1} should be displayed"));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($".//android.view.ViewGroup[normalize-space(@content-desc)={_name.QuoteXPathLiteral()}]");

        string IFocusable.ElementDescription
            => new FocusableDescriptionBuilder {Tag = "android.view.ViewGroup", ContentDesc = _name}.ViewGroupDescription;
    }
}