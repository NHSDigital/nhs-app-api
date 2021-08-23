using System;
using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class YourHealthMyCareViewPageContent : YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthMyCareViewPageContent(IWebInteractor interactor) : base(interactor) => _interactor = interactor;

        private WebMenuItem MyCareViewTestResultsMenuItem => WebMenuItem.WithTitle(_interactor, "Test results", "btn_pkb_my_care_view_test_results");

        private WebText MyCareViewTestResultsText => WebText.WithTagAndText(_interactor, "p", "View test results from your hospital and other healthcare providers");

        private WebMenuItem MyCareViewCarePlansMenuItem => WebMenuItem.WithTitle(_interactor, "Care plans", "btn_pkb_my_care_view_care_plans");

        private WebText MyCareViewCarePlansText => WebText.WithTagAndText(_interactor, "p", "View your care plans from your hospital or other care provider, or add your own");

        private WebMenuItem MyCareViewTrackYourHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Track your health", "btn_pkb_my_care_view_health_trackers");

        private WebText MyCareViewTrackYourHealthText => WebText.WithTagAndText(_interactor, "p", "Record symptoms and add to your health journal");

        private WebMenuItem MyCareViewSharedHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Shared health links", "btn_pkb_my_care_view_shared_links");

        private WebText MyCareViewSharedHealthText => WebText.WithTagAndText(_interactor, "p", "View links or documents your health team has shared with you, or add your own");

        private WebMenuItem MyCareViewRecordSharingMenuItem => WebMenuItem.WithTitle(_interactor, "Record sharing", "btn_pkb_my_care_view_record_sharing");

        private WebText MyCareViewRecordSharingText => WebText.WithTagAndText(_interactor, "p", "Choose and manage information you share with your health teams");

        public void AssertElements()
        {
            MyCareViewTestResultsMenuItem.AssertVisible();
            MyCareViewTestResultsText.AssertVisible();
            MyCareViewCarePlansMenuItem.AssertVisible();
            MyCareViewCarePlansText.AssertVisible();
            MyCareViewTrackYourHealthMenuItem.AssertVisible();
            MyCareViewTrackYourHealthText.AssertVisible();
            MyCareViewSharedHealthMenuItem.AssertVisible();
            MyCareViewSharedHealthText.AssertVisible();
            MyCareViewRecordSharingMenuItem.AssertVisible();
            MyCareViewRecordSharingText.AssertVisible();
        }

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            CovidPassMenuItem,
            VaccineRecordMenuItem,
            GpHeathRecordMenuItem,
            MyCareViewTestResultsMenuItem,
            MyCareViewCarePlansMenuItem,
            MyCareViewTrackYourHealthMenuItem,
            MyCareViewSharedHealthMenuItem,
            MyCareViewRecordSharingMenuItem,
            OrganDonationMenuItem,
            NdopMenuItem
        };

        public void KeyboardNavigateToTestResults(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(MyCareViewTestResultsMenuItem, navigation);

        public void KeyboardNavigateToCarePlans(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(MyCareViewCarePlansMenuItem, navigation);

        public void KeyboardNavigateToTrackYourHealth(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(MyCareViewTrackYourHealthMenuItem, navigation);

        public void KeyboardNavigateToSharedHealth(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(MyCareViewSharedHealthMenuItem, navigation);

        public void KeyboardNavigateToRecordSharing(AndroidKeyboardNavigation navigation) => KeyboardNavigateToAndActivateMenuItem(MyCareViewRecordSharingMenuItem, navigation);
    }
}