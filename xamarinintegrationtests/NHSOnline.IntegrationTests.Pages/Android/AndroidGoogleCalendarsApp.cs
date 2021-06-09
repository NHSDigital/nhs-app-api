using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    public class AndroidGoogleCalendarsApp
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidGoogleCalendarsApp(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel WelcomeText => AndroidLabel.WithText(_driver, "Google Calendar");

        private AndroidImageView NextStandardImageView =>
            AndroidImageView.WithImageViewDescription(_driver, "next page");

        private AndroidFrameLayout NextFrame =>
            AndroidFrameLayout.WithChildElement(_driver, NextStandardImageView);

        private AndroidImageButton NextAlternative =>
            AndroidImageButton.WithDescription(_driver, "next page");

        private AndroidButton GotItButton => AndroidButton.WithText(_driver, "GOT IT");

        private AndroidEditText SubjectText => AndroidEditText.WithText(_driver, "Test Subject");

        private AndroidEditText BodyText => AndroidEditText.WithText(_driver, "Test Body");

        private AndroidLabel LocationText => AndroidLabel.WithText(_driver, "Test Location");

        private AndroidCalendarDateTimeLabel StartDateTimeText => AndroidCalendarDateTimeLabel.WithContentDescription(_driver, "Start date: Wed, Jan 2, 2030");

        private AndroidCalendarDateTimeLabel StartTimeText => AndroidCalendarDateTimeLabel.WithContentDescription(_driver, "Start time: 1:00 PM");

        private AndroidCalendarDateTimeLabel EndDateTimeText => AndroidCalendarDateTimeLabel.WithContentDescription(_driver, "End date: Wed, Jan 2, 2030");

        private AndroidCalendarDateTimeLabel EndTimeText => AndroidCalendarDateTimeLabel.WithContentDescription(_driver, "End time: 1:10 PM");

        public static AndroidGoogleCalendarsApp AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidGoogleCalendarsApp(driver);
            page.WelcomeText.AssertVisible();
            return page;
        }

        public AndroidGoogleCalendarsApp NavigateThroughOverview()
        {
            try
            {
                NextFrame.Click();
                NextFrame.Click();
                NextFrame.Click();
            }
            catch (AssertFailedException)
            {
                NextAlternative.Click();
                NextAlternative.Click();
            }

            return this;
        }

        public AndroidGoogleCalendarsApp ConfirmGotIt()
        {
            GotItButton.Click();
            return this;
        }

        public void AssertDetailsArePassed()
        {
            SubjectText.AssertVisible();
            BodyText.AssertVisible();
            LocationText.AssertVisible();

            StartDateTimeText.AssertVisible();
            StartTimeText.AssertVisible();
            EndDateTimeText.AssertVisible();
            EndTimeText.AssertVisible();
        }
    }
}