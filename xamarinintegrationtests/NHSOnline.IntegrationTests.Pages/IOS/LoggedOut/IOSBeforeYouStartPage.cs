using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSBeforeYouStartPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSBeforeYouStartPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLink Title => IOSLink.WithText(_driver, "Before You Start");
        private IOSLink CovidLink => IOSLink.WithText(_driver, "Check if you have coronavirus symptoms");
        private IOSLink ConditionsLink => IOSLink.WithText(_driver, "Search conditions and treatments");
        private IOSLink OneOneOneLink => IOSLink.WithText(_driver, "Use NHS 111 online to check if you need urgent help");

        private IOSButton ContinueButton => new IOSButton(_driver, "Continue");

        private IOSLabel ExpanderHeader => IOSLabel.WithText(_driver, "What to do if you're aged 13 to 15");
        private IOSLabel ExpanderBody => IOSLabel.WithText(_driver, "You'll need to contact your GP surgery first and request access to GP online services.");

        public static IOSBeforeYouStartPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSBeforeYouStartPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void AssertPageElements()
        {
            CovidLink.AssertVisible();
            ConditionsLink.AssertVisible();
            OneOneOneLink.AssertVisible();
            ExpanderHeader.AssertVisible();
        }

        public void CheckCoronavirusSymptoms()
        {
            CovidLink.Touch();
        }

        public void SearchConditionsAndTreatments()
        {
            ConditionsLink.Touch();
        }

        public void UseNhs111Online()
        {
            OneOneOneLink.Touch();
        }

        public void AssertExpanderPresent()
        {
            ExpanderHeader.AssertVisible();
            ExpanderBody.AssertNotVisible();
            ExpanderHeader.Click();
            ExpanderBody.AssertVisible();
        }

        public void Continue()
        {
            ContinueButton.Click();
        }
    }
}
