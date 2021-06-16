using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    public class IOSCalendarsApp
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSCalendarsApp(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "New Event").ScrollIntoView();

        private IOSEditableTextField SubjectText => IOSEditableTextField.WithText(_driver, "Test Subject").ScrollIntoView();

        private IOSLabel LocationText => IOSLabel.WithText(_driver, "Test Location").ScrollIntoView();

        private IOSTextView BodyText => IOSTextView.WithText(_driver, "Test Body").ScrollIntoView();

        private IOSStartDateTime StartDateText => IOSStartDateTime.WithLabel(
            _driver, "Starts", "Jan", "2", "2030", "1:00 PM", "13:00");

        private IOSEndTime EndTimeText => IOSEndTime.WithLabel(_driver, "Ends","1:10 PM", "13:10");

        public static IOSCalendarsApp AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSCalendarsApp(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void AssertDetailsArePassed()
        {
            SubjectText.AssertVisible();
            BodyText.AssertVisible();
            LocationText.AssertVisible();

            StartDateText.AssertVisible();
            EndTimeText.AssertVisible();
        }
    }
}