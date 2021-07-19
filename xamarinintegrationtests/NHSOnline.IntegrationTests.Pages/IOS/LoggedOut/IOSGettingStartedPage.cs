using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.IOS.LoggedOut
{
    public sealed class IOSGettingStartedPage
    {
        private readonly IIOSDriverWrapper _driver;

        private IOSGettingStartedPage(IIOSDriverWrapper driver) => _driver = driver;

        private IOSLabel Title => IOSLabel.WithText(_driver, "Getting started");

        private IOSLabel PrescriptionsLabel => IOSLabel
            .WithText(_driver, "order repeat prescriptions")
            .ScrollIntoView();
        private IOSLabel AppointmentsLabel => IOSLabel
            .WithText(_driver, "book and manage appointments")
            .ScrollIntoView();
        private IOSLabel HealthInformationLabel => IOSLabel
            .WithText(_driver, "get health information and advice")
            .ScrollIntoView();
        private IOSLabel OrganDonationLabel => IOSLabel
            .WithText(_driver, "manage your organ donation decision")
            .ScrollIntoView();
        private IOSLabel NhsNumberLabel => IOSLabel
            .WithText(_driver, "view your NHS number")
            .ScrollIntoView();
        private IOSLabel LegalAgeLabel => IOSLabel
            .WithText(_driver, "You must be aged 13 and over to use the app, and registered at a GP surgery in England.")
            .ScrollIntoView();
        private IOSLabel GetStartedLabel => IOSLabel
            .WithText(_driver, "To get started you'll need to create your NHS login.")
            .ScrollIntoView();

        private IOSButton ContinueButton => IOSButton
            .WithText(_driver, "Continue")
            .ScrollIntoView();

        public static IOSGettingStartedPage AssertOnPage(IIOSDriverWrapper driver)
        {
            var page = new IOSGettingStartedPage(driver);
            page.Title.AssertVisible();
            return page;
        }

        public void AssertPageElements()
        {
            PrescriptionsLabel.AssertVisible();
            AppointmentsLabel.AssertVisible();
            HealthInformationLabel.AssertVisible();
            OrganDonationLabel.AssertVisible();
            NhsNumberLabel.AssertVisible();
            LegalAgeLabel.AssertVisible();
            GetStartedLabel.AssertVisible();
            ContinueButton.AssertVisible();
        }

        public void Continue()
        {
            ContinueButton.Click();
        }
    }
}
