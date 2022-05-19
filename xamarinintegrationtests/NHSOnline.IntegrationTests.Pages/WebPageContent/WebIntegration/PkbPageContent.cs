using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class PkbPageContent
    {
        private readonly IWebInteractor _interactor;
        private readonly string _phrPath;

        internal PkbPageContent(IWebInteractor interactor, string phrPath)
        {
            _interactor = interactor;
            _phrPath = phrPath;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Pkb Internal Page");

        private WebText PhrPathText => WebText.WithTagAndText(_interactor, "li", $"phrPath: {_phrPath}");

        private WebLink CalendarLink => WebLink.WithText(_interactor, "Calendar");

        private WebLink GoToPageLink => WebLink.WithText(_interactor, "Go to page");

        private WebLink OpenBrowserOverlayLink => WebLink.WithText(_interactor, "Open Browser Overlay");

        private WebLink OpenExternalBrowserLink => WebLink.WithText(_interactor, "Open External Browser");
        
        private WebLink NativeBackActionLink => WebLink.WithText(_interactor, "Native Back Action");

        private WebLink FileUploadLink => WebLink.WithText(_interactor, "File upload");

        private WebLink DocumentDownloadLink => WebLink.WithText(_interactor, "Download document");

        private WebLink LocationServicesLink => WebLink.WithText(_interactor, "Location services");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            CalendarLink,
            GoToPageLink,
            OpenBrowserOverlayLink,
            FileUploadLink,
            DocumentDownloadLink,
            LocationServicesLink
        };

        internal void AssertOnPage()
        {
            TitleText.AssertVisible();
            PhrPathText.AssertVisible();
        }

        public void NavigateToCalendar() => CalendarLink.Click();

        public void NavigateToGoToPage() => GoToPageLink.Click();

        public void NavigateToOpenBrowserOverlay() => OpenBrowserOverlayLink.Click();

        public void NavigateToOpenExternalBrowser() => OpenExternalBrowserLink.Click();
        
        public void NavigateToNativeBackAction() => NativeBackActionLink.Click();

        public void NavigateToFileUpload() => FileUploadLink.Click();

        public void NavigateToDocumentDownload() => DocumentDownloadLink.Click();

        public void NavigateToLocationServices() => LocationServicesLink.Click();
    }
}