using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS
{
    internal sealed class IOSBeforeYouStartPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSBeforeYouStartPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Before You Start");
        private IOSLabel CovidLink => IOSLabel.WithText(_driver, "Check if you have coronavirus symptoms");
        private IOSLabel ConditionsLink => IOSLabel.WithText(_driver, "Search conditions and treatments");
        private IOSLabel OneOneOneLink => IOSLabel.WithText(_driver, "Use NHS 111 online to check if you need urgent help");

        private IOSButton ContinueButton => new IOSButton(_driver, "Continue");

        private IOSLabel ExpanderHeader => IOSLabel.WithText(_driver, "What to do if you're aged 13 to 15");
        private IOSLabel ExpanderBody => IOSLabel.WithText(_driver, "You'll need to contact your GP surgery first and request access to GP online services.");

        internal static IOSBeforeYouStartPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSBeforeYouStartPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        internal void AssertPageElements()
        {
            CovidLink.AssertVisible();
            ConditionsLink.AssertVisible();
            OneOneOneLink.AssertVisible();
            ExpanderHeader.AssertVisible();
        }

        internal void CheckCoronavirusSymptoms()
        {
            CovidLink.Click();
        }

        internal void SearchConditionsAndTreatments()
        {
            ConditionsLink.Click();
        }

        internal void UseNhs111Online()
        {
            OneOneOneLink.Click();
        }

        internal void AssertExpanderPresent()
        {
            ExpanderHeader.AssertVisible();
            ExpanderBody.AssertNotVisible();
            ExpanderHeader.Click();
            ExpanderBody.AssertVisible();
        }

        internal void Continue()
        {
            ContinueButton.Click();
        }
    }
}
