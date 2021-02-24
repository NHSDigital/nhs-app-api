using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidBeforeYouStartPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidBeforeYouStartPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Before You Start");
        private AndroidLink CovidLink => AndroidLink
            .WithText(_driver, "Check if you have coronavirus symptoms")
            .ScrollIntoView();
        private AndroidLink ConditionsLink => AndroidLink
            .WithText(_driver, "Search conditions and treatments")
            .ScrollIntoView();
        private AndroidLink OneOneOneLink => AndroidLink.
            WithText(_driver, "Use NHS 111 online to check if you need urgent help")
            .ScrollIntoView();

        private AndroidExpander GuidanceForAges13To15Expander => AndroidExpander.WithHeaderAndBodyText(
            _driver,
            "What to do if you're aged 13 to 15",
            "You'll need to contact your GP surgery first and request access to GP online services.");

        private AndroidButton ContinueButton => AndroidButton
            .WithText(_driver, "Continue")
            .ScrollIntoView();

        private AndroidKeyboardNavigation KeyboardNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            CovidLink,
            ConditionsLink,
            OneOneOneLink,
            ContinueButton);

        public static AndroidBeforeYouStartPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidBeforeYouStartPage(driver);
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

        public AndroidBeforeYouStartPage TabToCheckCoronavirusSymptoms()
        {
            KeyboardNavigation.TabTo(CovidLink);
            return this;
        }

        public void SearchConditionsAndTreatments()
        {
            ConditionsLink.Touch();
        }

        public AndroidBeforeYouStartPage TabToSearchConditionsAndTreatments()
        {
            KeyboardNavigation.TabTo(ConditionsLink);
            return this;
        }

        public void UseNhs111Online()
        {
            OneOneOneLink.Touch();
        }

        public AndroidBeforeYouStartPage TabToUseNhs111Online()
        {
            KeyboardNavigation.TabTo(OneOneOneLink);
            return this;
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

        public void PressEnterKey()
        {
            KeyboardNavigation.PressEnterKey();
        }

        public void AssertTabFocusOrder()
        {
            KeyboardNavigation.AssertFocusOrder();
        }
    }
}
