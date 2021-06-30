using System;
using System.Globalization;
using FluentAssertions;
using NHSOnline.IntegrationTests.UI.Drivers;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;

namespace NHSOnline.IntegrationTests.UI.Components.IOS
{
    public sealed class IOSPhotoCell
    {
        private readonly IIOSInteractor _interactor;
        private readonly DateTime _dateTime;

        private IOSPhotoCell(IIOSInteractor interactor, DateTime dateTime)
        {
            _interactor = interactor;
            _dateTime = dateTime;
        }

        public static IOSPhotoCell WithName(IIOSInteractor interactor, DateTime dateTime)
            => new IOSPhotoCell(interactor, dateTime);

        private void ActOnElement(Action<IOSElement> action, By findBy)
            => _interactor.ActOnElement(findBy, action);

        public void AssertVisible()
        {
            ActOnElement(
                    e => e.Displayed.Should().BeTrue("an icon with Photo, Landscape {1} should be displayed",
                        _dateTime.ToString("HH:mm", new DateTimeFormatInfo())), FindBy(_dateTime));
        }

        private static By FindBy(DateTime datetime)
        {
            return MobileBy.IosNSPredicate(
                $"type == 'XCUIElementTypeImage' AND name == \'Photo, Landscape, {datetime.ToString("HH:mm", new DateTimeFormatInfo())}\'");
        }
    }
}