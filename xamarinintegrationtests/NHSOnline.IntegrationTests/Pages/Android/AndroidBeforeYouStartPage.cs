using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidBeforeYouStartPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidBeforeYouStartPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Before You Start");
        private AndroidLabel CovidLink => AndroidLabel.WithText(_driver, "Check if you have coronavirus symptoms");
        private AndroidLabel ConditionsLink => AndroidLabel.WithText(_driver, "Search conditions and treatments");
        private AndroidLabel OneOneOneLink => AndroidLabel.WithText(_driver, "Use NHS 111 online to check if you need urgent help");
        private AndroidLabel ExpanderHeader => AndroidLabel.WithText(_driver, "What to do if you're aged 13 to 15");

        private AndroidLabel ExpanderBody => AndroidLabel.WithText(_driver,
            "You'll need to contact your GP surgery first and request access to GP online services.");


        private AndroidButton ContinueButton => new AndroidButton(_driver, "Continue");

        internal static AndroidBeforeYouStartPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidBeforeYouStartPage(driver);
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
