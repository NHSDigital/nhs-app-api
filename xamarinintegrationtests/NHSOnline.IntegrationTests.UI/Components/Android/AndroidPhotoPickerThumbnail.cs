using System;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace NHSOnline.IntegrationTests.UI.Components.Android
{
    public class AndroidPhotoPickerThumbnail
    {
        private readonly IAndroidInteractor _interactor;
        private readonly string _description;

        private AndroidPhotoPickerThumbnail(IAndroidInteractor interactor, string description)
        {
            _description = description;
            _interactor = interactor;
        }

        public static AndroidPhotoPickerThumbnail WithDescription(IAndroidInteractor interactor, string description)
        => new AndroidPhotoPickerThumbnail(interactor, description);

        public void AssertVisible()
            => ActOnElement(e => e.Displayed.Should().BeTrue("a thumbnail with description {1} should be displayed", _description));

        public void Click() => ActOnElement(e => e.Click());

        private void ActOnElement(Action<AndroidElement> action)
            => _interactor.ActOnElement(FindBy, action);

        private By FindBy
            => By.XPath($".//android.view.ViewGroup[starts-with(normalize-space(@content-desc), {_description.QuoteXPathLiteral()})]");
    }
}