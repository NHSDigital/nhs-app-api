using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.Android.LoggedOut
{
    public sealed class AndroidGettingStartedPage
    {
        private readonly IAndroidDriverWrapper _driver;

        private AndroidGettingStartedPage(IAndroidDriverWrapper driver) => _driver = driver;

        private AndroidLabel Title => AndroidLabel.WithText(_driver, "Getting started");
        private AndroidLabel CannotLogInTitle => AndroidLabel.WithText(_driver, "Cannot log in");

        private AndroidLabel UseTheNhsAppToLabel => AndroidLabel
            .WithText(_driver, "Use the NHS App to:")
            .ScrollIntoView();

        private AndroidLabel PrescriptionsLabel => AndroidLabel
            .WithText(_driver, "order repeat prescriptions")
            .ScrollIntoView();

        private AndroidLabel AppointmentsLabel => AndroidLabel
            .WithText(_driver, "book and manage appointments")
            .ScrollIntoView();

        private AndroidLabel HealthInformationLabel => AndroidLabel
            .WithText(_driver, "get health information and advice")
            .ScrollIntoView();

        private AndroidLabel HealthRecordLabel => AndroidLabel
            .WithText(_driver, "view your health record securely")
            .ScrollIntoView();

        private AndroidLabel OrganDonationLabel => AndroidLabel
            .WithText(_driver, "manage your organ donation decision")
            .ScrollIntoView();

        private AndroidLabel NhsNumberLabel => AndroidLabel
            .WithText(_driver, "view your NHS number")
            .ScrollIntoView();

        private AndroidLabel LegalAgeLabel => AndroidLabel
            .WithText(_driver, "You must be aged 13 and over to use the app, and registered at a GP surgery in England.")
            .ScrollIntoView();

        private AndroidLabel GetStartedLabel => AndroidLabel
            .WithText(_driver, "To get started you'll need to create your NHS login.")
            .ScrollIntoView();

        private AndroidLink WhatToDoThirteenToFifteenLink => AndroidLink
            .WithContentDescription(_driver, "What to do if you're aged 13 to 15")
            .ScrollIntoView();

        private AndroidButton ContinueButton => AndroidButton
            .WithText(_driver, "Continue")
            .ScrollIntoView();

        private AndroidIcon CloseIcon => AndroidIcon
            .WithName(_driver, "Close");

        private AndroidKeyboardNavigation KeyboardNavigation => AndroidKeyboardNavigation.WithExpectedFocusableElements(
            _driver,
            CloseIcon,
            WhatToDoThirteenToFifteenLink,
            ContinueButton);

        public static AndroidGettingStartedPage AssertOnPage(IAndroidDriverWrapper driver)
        {
            // Extending timeout as its been shown this page causes failures by not taking enough time to wait for it to load
            using var extendedTimeout = ExtendedTimeout.FromSeconds(20);

            var page = new AndroidGettingStartedPage(driver);

            KnownIssue.BrowserStackUnableToVerifyAppVersionFailure()
                .ShouldExpect(() =>
                {
                    page.Title.AssertVisible();
                })
                .OrIfKnownIssueOccuredExpect(() =>
                {
                    page.CannotLogInTitle.AssertVisible();
                });

            return page;
        }

        public AndroidGettingStartedPage AssertPageElements()
        {
            UseTheNhsAppToLabel.AssertVisible();
            PrescriptionsLabel.AssertVisible();
            AppointmentsLabel.AssertVisible();
            HealthInformationLabel.AssertVisible();
            HealthRecordLabel.AssertVisible();
            OrganDonationLabel.AssertVisible();
            NhsNumberLabel.AssertVisible();
            WhatToDoThirteenToFifteenLink.AssertVisible();
            LegalAgeLabel.AssertVisible();
            GetStartedLabel.AssertVisible();

            return this;
        }

        public void Continue() => ContinueButton.Click();

        public void GoToWhatToDoThirteenToFifteenLink() => WhatToDoThirteenToFifteenLink.Touch();

        public void PressEnterKey() => KeyboardNavigation.PressEnterKey();

        public void AssertTabFocusOrder() => KeyboardNavigation.AssertFocusOrder();
    }
}
