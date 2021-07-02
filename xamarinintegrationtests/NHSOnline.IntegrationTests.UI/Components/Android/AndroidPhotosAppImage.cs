using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidPhotosAppImage
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _imageDescription;

        private AndroidPhotosAppImage(IAndroidInteractor interactor, string imageDescription)
        {
            _interactor = interactor;
            _imageDescription = imageDescription;
        }

        public static AndroidPhotosAppImage WithImageViewDescription(IAndroidInteractor interactor, string imageDescription)
            => new AndroidPhotosAppImage(interactor, imageDescription);

        public void AssertVisible()
            => ActOnElement(e
                => e.Displayed.Should().BeTrue("a image group view with content desc containing {0} should be displayed", _imageDescription));

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($"//{Xpath}");

        private string Xpath
             => $"android.view.ViewGroup[contains(normalize-space(@content-desc), {_imageDescription.QuoteXPathLiteral()})]";
    }
}
