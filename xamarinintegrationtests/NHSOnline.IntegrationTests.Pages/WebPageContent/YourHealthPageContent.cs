using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Your health");
        private WebMenuItem TestResultsMenuItem => WebMenuItem.WithTitle(_interactor, "Test results");
        private WebMenuItem CarePlansMenuItem => WebMenuItem.WithTitle(_interactor, "Care plans");
        private WebMenuItem TrackYourHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Track your health");
        private WebMenuItem SharedHealthMenuItem => WebMenuItem.WithTitle(_interactor, "Shared health links");
        private WebMenuItem RecordSharingMenuItem => WebMenuItem.WithTitle(_interactor, "Record sharing");

        private WebText SharedHealthMenuItemTitle => WebText.WithTagAndText(_interactor, "h2", "Shared health links");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public YourHealthPageContent AssertPageElements()
        {
            Title.AssertVisible();
            return this;
        }

        public void NavigateToTestResults()
        {
            TestResultsMenuItem.Click();
        }

        public void NavigateToCarePlans()
        {
            CarePlansMenuItem.Click();
        }

        public void NavigateToTrackYourHealth()
        {
            TrackYourHealthMenuItem.Click();
        }

        public void NavigateToSharedHealth()
        {
            SharedHealthMenuItemTitle.ScrollTo();
            SharedHealthMenuItemTitle.AssertVisible();
            SharedHealthMenuItem.Click();
        }

        public void NavigateToRecordSharing()
        {
            RecordSharingMenuItem.Click();
        }
    }
}
