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

        private AndroidFrameLayoutWithImageView NextStandard =>
            AndroidFrameLayoutWithImageView.WithImageViewDescription(_driver, "next page");

        private AndroidImageButton NextAlternative =>
            AndroidImageButton.WithDescription(_driver, "next page");

        private AndroidButton GotItButton => AndroidButton.WithText(_driver, "GOT IT");

        private AndroidEditText SubjectText => AndroidEditText.WithText(_driver, "Test Subject");

        private AndroidEditText BodyText => AndroidEditText.WithText(_driver, "Test Body");

        private AndroidEditText LocationText => AndroidEditText.WithText(_driver, "Test Location");

        private AndroidLabel StartDateText => AndroidLabel.WithContentDescription(_driver, "Start date: Wednesday, Jan 2, 2030");

        private AndroidLabel StartTimeText => AndroidLabel.WithContentDescription(_driver, "Start time: 1:00 PM");

        private AndroidLabel EndDateText => AndroidLabel.WithContentDescription(_driver, "End date: Wednesday, Jan 2, 2030");

        private AndroidLabel EndTimeText => AndroidLabel.WithContentDescription(_driver, "End time: 1:10 PM");

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
                NextStandard.Click();
                NextStandard.Click();
                NextStandard.Click();
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

            StartDateText.AssertVisible();
            StartTimeText.AssertVisible();
            EndDateText.AssertVisible();
            EndTimeText.AssertVisible();
        }
    }
}