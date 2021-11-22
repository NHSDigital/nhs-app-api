using System.Collections.Generic;
using System.Linq;
using AngleSharp;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class OpenBrowserOverlayPageContent
    {
        private readonly IWebInteractor _interactor;

        internal OpenBrowserOverlayPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Web Integration Functionality - Open Browser Overlay");

        private WebButton OpenBrowserOverlayButton => WebButton.WithText(_interactor, "Open Browser Overlay");

        private WebInputText UrlOverlayText => WebInputText.WithLabel(_interactor, "Enter URL");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            OpenBrowserOverlayButton,
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void OpenBrowserOverlay() => OpenBrowserOverlayButton.Click();

        public void EnterOverlayUrl(Url url) => UrlOverlayText.EnterText(url.ToString());
    }
}