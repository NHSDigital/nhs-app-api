using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public sealed class AndroidFrameLayoutWithImageView
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _imageViewDescription;

        private AndroidFrameLayoutWithImageView(IAndroidInteractor interactor, string imageViewDescription)
        {
            _interactor = interactor;
            _imageViewDescription = imageViewDescription;
        }

        public static AndroidFrameLayoutWithImageView WithImageViewDescription(IAndroidInteractor interactor, string imageViewDescription)
            => new AndroidFrameLayoutWithImageView(interactor, imageViewDescription);

        public void Click()
            => ActOnElement(e => e.Click());

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            =>  By.XPath($"//android.widget.FrameLayout/android.widget.ImageView[normalize-space(@content-desc)={_imageViewDescription.QuoteXPathLiteral()}]");
    }
}
