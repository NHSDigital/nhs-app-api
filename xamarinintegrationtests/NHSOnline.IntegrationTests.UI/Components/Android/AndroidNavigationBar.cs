using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidNavigationBar: IAndroidContainer
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _name;

        private AndroidNavigationBar(IAndroidInteractor interactor, string name)
        {
            _interactor = interactor;
            _name = name;
        }

        public static AndroidNavigationBar WithName(IAndroidInteractor interactor, string name)
            => new (interactor, name);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a {0} should be displayed", _name));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//android.view.ViewGroup[normalize-space(@content-desc)={_name.QuoteXPathLiteral()}]");

        IAndroidInteractor IAndroidContainer.ContainerInteractor => _interactor.CreateContainedInteractor(FindBy);
    }
}