using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android
{
    internal sealed class AndroidBeforeYouStartPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidBeforeYouStartPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => new AndroidLabel(_driver, "Before You Start");
        private AndroidLabel CovidLink => new AndroidLabel(_driver, "• Check if you have coronavirus symptoms");
        private AndroidLabel ConditionsLink => new AndroidLabel(_driver, "• Search conditions and treatments");
        private AndroidLabel OneOneOneLink => new AndroidLabel(_driver, "• Use NHS 111 online to check if you need urgent help");
        private AndroidLabel Expander => new AndroidLabel(_driver, "What to do if you're aged 13 to 15");

        private AndroidLabel ExpanderBody => new AndroidLabel(_driver,
            "You'll need to contact your GP surgery first and request access to GP online services.");


        private AndroidButton ContinueButton => new AndroidButton(_driver, "Continue");

        internal static AndroidBeforeYouStartPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            var page = new AndroidBeforeYouStartPage(driver);
            page.Title.AssertVisible();
            page.CovidLink.AssertVisible();
            page.ConditionsLink.AssertVisible();
            page.OneOneOneLink.AssertVisible();
            return page;
        }

        internal void AssertExpanderPresent()
        {
            Expander.AssertVisible();
            ExpanderBody.AssertNotVisible();
            Expander.Click();
            ExpanderBody.AssertVisible();
        }

        internal void TriggerCovidLinkClick()
        {
            CovidLink.Click();
        }

        internal void TriggerConditionsLinkClick()
        {
            ConditionsLink.Click();
        }

        internal void TriggerOneOneOneLinkClick()
        {
            OneOneOneLink.Click();
        }

        internal void Continue()
        {
            ContinueButton.Click();
        }
    }
}