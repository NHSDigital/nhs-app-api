using System;
using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class YourHealthSecondaryCareViewPageContent : YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthSecondaryCareViewPageContent(IWebInteractor interactor) : base(interactor) => _interactor = interactor;

        private WebMenuItem SecondaryCareViewTestResultsMenuItem => WebMenuItem.WithTitle(_interactor, "Test results", "btn_pkb_secondary_care_test_results");

        private WebText SecondaryCareViewTestResultsText => WebText.WithTagAndText(_interactor, "p", "View test results from your hospital and other healthcare providers");

        private WebMenuItem SecondaryCareViewCarePlansMenuItem => WebMenuItem.WithTitle(_interactor, "Care plans", "btn_pkb_secondary_care_care_plans");

        private WebText SecondaryCareViewCarePlansText => WebText.WithTagAndText(_interactor, "p", "View your care plans from your hospital or other care provider, or add your own");

        private WebMenuItem SecondaryCareViewTrackYourHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Track your health", "btn_pkb_secondary_care_health_trackers");

        private WebText SecondaryCareViewTrackYourHealthText => WebText.WithTagAndText(_interactor, "p", "Record symptoms and add to your health journal");

        private WebMenuItem SecondaryCareViewSharedHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Shared health links", "btn_pkb_secondary_care_shared_links");

        private WebText SecondaryCareViewSharedHealthText => WebText.WithTagAndText(_interactor, "p", "View links or documents your health team has shared with you, or add your own");

        private WebMenuItem SecondaryCareViewRecordSharingMenuItem => WebMenuItem.WithTitle(_interactor, "Record sharing", "btn_pkb_secondary_care_record_sharing");

        private WebText SecondaryCareViewRecordSharingText => WebText.WithTagAndText(_interactor, "p", "Choose and manage information you share with your health teams");

        public void AssertElements()
        {
            SecondaryCareViewTestResultsMenuItem.AssertVisible();
            SecondaryCareViewTestResultsText.AssertVisible();
            SecondaryCareViewCarePlansMenuItem.AssertVisible();
            SecondaryCareViewCarePlansText.AssertVisible();
            SecondaryCareViewTrackYourHealthMenuItem.AssertVisible();
            SecondaryCareViewTrackYourHealthText.AssertVisible();
            SecondaryCareViewSharedHealthMenuItem.AssertVisible();
            SecondaryCareViewSharedHealthText.AssertVisible();
            SecondaryCareViewRecordSharingMenuItem.AssertVisible();
            SecondaryCareViewRecordSharingText.AssertVisible();
        }

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            CovidPassMenuItem,
            VaccineRecordMenuItem,
            GpHeathRecordMenuItem,
            SecondaryCareViewTestResultsMenuItem,
            SecondaryCareViewCarePlansMenuItem,
            SecondaryCareViewTrackYourHealthMenuItem,
            SecondaryCareViewSharedHealthMenuItem,
            SecondaryCareViewRecordSharingMenuItem,
            OrganDonationMenuItem,
            NdopMenuItem
        };

        public void KeyboardNavigateToTestResults(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(SecondaryCareViewTestResultsMenuItem, navigation);

        public void KeyboardNavigateToCarePlans(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(SecondaryCareViewCarePlansMenuItem, navigation);

        public void KeyboardNavigateToTrackYourHealth(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(SecondaryCareViewTrackYourHealthMenuItem, navigation);

        public void KeyboardNavigateToSharedHealth(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(SecondaryCareViewSharedHealthMenuItem, navigation);

        public void KeyboardNavigateToRecordSharing(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(SecondaryCareViewRecordSharingMenuItem, navigation);

    }
}