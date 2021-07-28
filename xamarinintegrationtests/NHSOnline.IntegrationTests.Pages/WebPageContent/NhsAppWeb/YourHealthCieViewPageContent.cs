using System;
using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class YourHealthCieViewPageContent : YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthCieViewPageContent(IWebInteractor interactor) : base(interactor) => _interactor = interactor;

        private WebMenuItem CieTestResultsMenuItem => WebMenuItem.WithTitle(_interactor, "Test results", "btn_pkb_cie_test_results");

        private WebText CieTestResultsText => WebText.WithTagAndText(_interactor, "p", "View test results from your hospital and other healthcare providers");

        private WebMenuItem CieCarePlansMenuItem => WebMenuItem.WithTitle(_interactor, "Care plans", "btn_pkb_cie_care_plans");

        private WebText CieCarePlansText => WebText.WithTagAndText(_interactor, "p", "View your care plans from your hospital or other care provider, or add your own");

        private WebMenuItem CieTrackYourHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Track your health", "btn_pkb_cie_health_trackers");

        private WebText CieTrackYourHealthText => WebText.WithTagAndText(_interactor, "p", "Record symptoms and add to your health journal");

        private WebMenuItem CieSharedHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Shared health links", "btn_pkb_cie_shared_links");

        private WebText CieSharedHealthText => WebText.WithTagAndText(_interactor, "p", "View links or documents your health team has shared with you, or add your own");

        private WebMenuItem CieRecordSharingMenuItem => WebMenuItem.WithTitle(_interactor, "Record sharing", "btn_pkb_cie_record_sharing");

        private WebText CieRecordSharingText => WebText.WithTagAndText(_interactor, "p", "Choose and manage information you share with your health teams");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            CovidPassMenuItem,
            VaccineRecordMenuItem,
            GpHeathRecordMenuItem,
            CieTestResultsMenuItem,
            CieCarePlansMenuItem,
            CieTrackYourHealthMenuItem,
            CieSharedHealthMenuItem,
            CieRecordSharingMenuItem,
            OrganDonationMenuItem,
            NdopMenuItem
        };

        public void AssertElements()
        {
            CieTestResultsMenuItem.AssertVisible();
            CieTestResultsText.AssertVisible();
            CieCarePlansMenuItem.AssertVisible();
            CieCarePlansText.AssertVisible();
            CieTrackYourHealthMenuItem.AssertVisible();
            CieTrackYourHealthText.AssertVisible();
            CieSharedHealthMenuItem.AssertVisible();
            CieSharedHealthText.AssertVisible();
            CieRecordSharingMenuItem.AssertVisible();
            CieRecordSharingText.AssertVisible();
        }

        public void KeyboardNavigateTo(YourHealthPages location, AndroidKeyboardNavigation navigation)
        {
            switch (location)
            {
                case YourHealthPages.CieTestResults:
                    KeyboardNavigateToAndActivateMenuItem(CieTestResultsMenuItem, navigation);
                    break;
                case YourHealthPages.CieCarePlans:
                    KeyboardNavigateToAndActivateMenuItem(CieCarePlansMenuItem, navigation);
                    break;
                case YourHealthPages.CieTrackYourHealth:
                    KeyboardNavigateToAndActivateMenuItem(CieTrackYourHealthMenuItem, navigation);
                    break;
                case YourHealthPages.CieSharedHealth:
                    KeyboardNavigateToAndActivateMenuItem(CieSharedHealthMenuItem, navigation);
                    break;
                case YourHealthPages.CieRecordSharing:
                    KeyboardNavigateToAndActivateMenuItem(CieRecordSharingMenuItem, navigation);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(location), location, null);
            }
        }
    }
}