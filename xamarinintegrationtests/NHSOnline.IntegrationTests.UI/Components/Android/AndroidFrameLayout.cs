using System;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidFrameLayout
    {
        private readonly IAndroidInteractor _interactor;
        private readonly ILayoutChild _childElement;

        private AndroidFrameLayout(IAndroidInteractor interactor, ILayoutChild childElement)
        {
            _interactor = interactor;
            _childElement = childElement;
        }

        public static AndroidFrameLayout WithChildElement(IAndroidInteractor interactor, ILayoutChild childElement)
            => new AndroidFrameLayout(interactor, childElement);

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindChildByXpath, action);

        private By FindChildByXpath
            =>  By.XPath($"//android.widget.FrameLayout/{_childElement.Xpath}");
    }
}
