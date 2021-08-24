using System;
using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class YourHealthPkbViewPageContent : YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthPkbViewPageContent(IWebInteractor interactor) : base(interactor) => _interactor = interactor;

        private WebMenuItem PkbTestResultsMenuItem => WebMenuItem.WithTitle(_interactor, "Test results", "btn_pkb_test_results");

        private WebText PkbTestResultsText => WebText.WithTagAndText(_interactor, "p", "View test results from your hospital and other healthcare providers");

        private WebMenuItem PkbCarePlansMenuItem => WebMenuItem.WithTitle(_interactor, "Care plans", "btn_pkb_care_plans");

        private WebText PkbCarePlansText => WebText.WithTagAndText(_interactor, "p", "View your care plans from your hospital or other care provider, or add your own");

        private WebMenuItem PkbTrackYourHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Track your health", "btn_pkb_health_trackers");

        private WebText PkbTrackYourHealthText => WebText.WithTagAndText(_interactor, "p", "Record symptoms and add to your health journal");

        private WebMenuItem PkbSharedHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Shared health links", "btn_pkb_shared_links");

        private WebText PkbSharedHeathText => WebText.WithTagAndText(_interactor, "p", "View links your doctor or health professional has shared with you");

        private WebText PkbSharedHealthMenuItemTitle => WebText.WithTagAndText(_interactor, "h2", "Shared health links");

        private WebMenuItem PkbRecordSharingMenuItem => WebMenuItem.WithTitle(_interactor, "Record sharing", "btn_pkb_record_sharing");

        private WebText PkbRecordSharingText => WebText.WithTagAndText(_interactor, "p", "Choose and manage information you share with your health teams");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            CovidPassMenuItem,
            VaccineRecordMenuItem,
            GpHeathRecordMenuItem,
            PkbTestResultsMenuItem,
            PkbCarePlansMenuItem,
            PkbTrackYourHealthMenuItem,
            PkbSharedHealthMenuItem,
            PkbRecordSharingMenuItem,
            OrganDonationMenuItem,
            NdopMenuItem
        };

        public void AssertElements()
        {
            PkbTestResultsMenuItem.AssertVisible();
            PkbTestResultsText.AssertVisible();
            PkbCarePlansMenuItem.AssertVisible();
            PkbCarePlansText.AssertVisible();
            PkbTrackYourHealthMenuItem.AssertVisible();
            PkbTrackYourHealthText.AssertVisible();
            PkbSharedHealthMenuItem.AssertVisible();
            PkbSharedHeathText.AssertVisible();
            PkbRecordSharingMenuItem.AssertVisible();
            PkbRecordSharingText.AssertVisible();
        }

        public void NavigateToTestResults() => PkbTestResultsMenuItem.Click();

        public void NavigateToCarePlans() => PkbCarePlansMenuItem.Click();

        public void NavigateToTrackYourHealth() => PkbTrackYourHealthMenuItem.Click();

        public void NavigateToRecordSharing() => PkbRecordSharingMenuItem.Click();

        public void NavigateToSharedHealth()
        {
            PkbSharedHealthMenuItemTitle.ScrollTo();
            PkbSharedHealthMenuItemTitle.AssertVisible();
            PkbSharedHealthMenuItem.Click();
        }

        public void KeyboardNavigateTo(YourHealthPages location, AndroidKeyboardNavigation navigation)
        {
            switch (location)
            {
                case YourHealthPages.CovidPass:
                    KeyboardNavigateToAndActivateMenuItem(CovidPassMenuItem, navigation);
                    break;
                case YourHealthPages.VaccineRecord:
                    KeyboardNavigateToAndActivateMenuItem(VaccineRecordMenuItem, navigation);
                    break;
                case YourHealthPages.GpHealthRecord:
                    KeyboardNavigateToAndActivateMenuItem(GpHeathRecordMenuItem, navigation);
                    break;
                case YourHealthPages.PkbTestResults:
                    KeyboardNavigateToAndActivateMenuItem(PkbTestResultsMenuItem, navigation);
                    break;
                case YourHealthPages.PkbCarePlans:
                    KeyboardNavigateToAndActivateMenuItem(PkbCarePlansMenuItem, navigation);
                    break;
                case YourHealthPages.PkbTrackYourHealth:
                    KeyboardNavigateToAndActivateMenuItem(PkbTrackYourHealthMenuItem, navigation);
                    break;
                case YourHealthPages.PkbSharedHealth:
                    KeyboardNavigateToAndActivateMenuItem(PkbSharedHealthMenuItem, navigation);
                    break;
                case YourHealthPages.PkbRecordSharing:
                    KeyboardNavigateToAndActivateMenuItem(PkbRecordSharingMenuItem, navigation);
                    break;
                case YourHealthPages.OrganDonation:
                    KeyboardNavigateToAndActivateMenuItem(OrganDonationMenuItem, navigation);
                    break;
                case YourHealthPages.Ndop:
                    KeyboardNavigateToAndActivateMenuItem(NdopMenuItem, navigation);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }
        }
    }
}