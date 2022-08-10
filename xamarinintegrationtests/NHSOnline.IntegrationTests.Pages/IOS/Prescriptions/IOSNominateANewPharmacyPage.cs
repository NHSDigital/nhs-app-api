using NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.Prescriptions
{
    public sealed class IOSNominateANewPharmacyPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSNominateANewPharmacyPage(IIOSDriverWrapper driver)
        {
            _driver = driver;
            Navigation = new IOSFullNavigation(driver);
            PageContent = new NominateANewPharmacyPageContent(driver.Web.NhsAppLoggedInWebView());
        }

        private IOSFullNavigation Navigation { get; }

        public NominateANewPharmacyPageContent PageContent { get; }

        private IOSRadioButton StepChoosePharmacyHighStreetRadioButton =>
            IOSRadioButton.StartsWith(_driver, "High street pharmacies");

        private IOSRadioButton StepChoosePharmacyOnlineRadioButton =>
            IOSRadioButton.StartsWith(_driver, "Online-only pharmacies");

        public void TakeScreenshot() => _driver.Screenshot(nameof(IOSNominateANewPharmacyPage));

        public static IOSNominateANewPharmacyPage StepStart(IIOSDriverWrapper driver, bool continueClick)
        {
            var page = new IOSNominateANewPharmacyPage(driver);

            page.PageContent.StepStartHeading.AssertVisible();
            page.TakeScreenshot();

            if (continueClick)
            {
                page.PageContent.ContinueButton.Click();
            }

            return page;
        }

        public IOSNominateANewPharmacyPage StepChoosePharmacy(bool isOnlinePharmacy = false)
        {
            PageContent.StepChoosePharmacyHeading.AssertVisible();

            if (isOnlinePharmacy)
            {
                StepChoosePharmacyOnlineRadioButton.Click();
            }
            else
            {
                StepChoosePharmacyHighStreetRadioButton.Click();
            }

            TakeScreenshot();
            PageContent.ContinueButton.Click();

            return this;
        }

        public IOSNominateANewPharmacyPage StepFindAHighStreetPharmacy()
        {
            PageContent.StepFindAHighStreetPharmacyHeading.AssertVisible();
            PageContent.StepFindAHighStreetPharmacyPostcodeInputText.EnterText("LS1 1AB", labelLevel: 2);
            TakeScreenshot();
            PageContent.SearchButton.Click();

            return this;
        }

        public IOSNominateANewPharmacyPage StepSelectAHighStreetPharmacy()
        {
            PageContent.StepSelectAHighStreetPharmacyHeading("LS1 1AB").AssertVisible();
            TakeScreenshot();
            PageContent.StepSelectAHighStreetPharmacy.Click(findByContains: true);

            return this;
        }

        public IOSNominateANewPharmacyPage StepConfirmNominatedPharmacy()
        {
            PageContent.StepConfirmNominatedPharmacyHeading().AssertVisible();
            TakeScreenshot();
            PageContent.ConfirmButton.Click();

            return this;
        }

        public IOSNominateANewPharmacyPage StepHighStreetPharmacyFinalConfirmation()
        {
            // API calls required to load this page
            using var timeout = ExtendedTimeout.FromSeconds(20);

            PageContent.StepHighStreetPharmacyFinalConfirmationHeading().AssertVisible();
            PageContent.StepHighStreetPharmacyFinalPharmacyNameConfirmed().AssertVisible();
            TakeScreenshot();

            return this;
        }

        public IOSNominateANewPharmacyPage StepOnlinePharmacyFinalShutter()
        {
            PageContent.StepOnlinePharmacyFinalShutterHeading().AssertVisible();
            PageContent.StepOnlinePharmacyFinalShutterText().AssertVisible();
            TakeScreenshot();

            return this;
        }
    }
}

