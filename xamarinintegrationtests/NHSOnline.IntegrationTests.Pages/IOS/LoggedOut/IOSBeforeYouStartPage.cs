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

        private IOSButton ContinueButton => IOSButton.WithText(_driver, "Continue");

        private IOSExpander GuidanceForAges13To15Expander => IOSExpander.WithHeaderAndBodyText(
            _driver,
            "What to do if you're aged 13 to 15",
            "You'll need to contact your GP surgery first and request access to GP online services.");

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
            GuidanceForAges13To15Expander.AssertVisible();
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

        public void AssertCanShowAndHideGuidanceForAges13To15()
        {
            GuidanceForAges13To15Expander.AssertCollapsed();
            GuidanceForAges13To15Expander.Toggle();
            GuidanceForAges13To15Expander.AssertExpanded();
            GuidanceForAges13To15Expander.Toggle();
            GuidanceForAges13To15Expander.AssertCollapsed();
        }

        public void Continue()
        {
            ContinueButton.Click();
        }
    }
}
