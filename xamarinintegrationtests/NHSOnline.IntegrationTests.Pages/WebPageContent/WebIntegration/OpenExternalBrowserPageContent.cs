using System.Collections.Generic;
using AngleSharp;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class OpenExternalBrowserPageContent
    {
        private readonly IWebInteractor _interactor;

        internal OpenExternalBrowserPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Web Integration Functionality - Open External Browser");

        private WebButton OpenExternalBrowserButton => WebButton.WithText(_interactor, "Open External Browser");

        private WebInputText UrlOverlayText => WebInputText.WithLabel(_interactor, "Enter URL");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            OpenExternalBrowserButton,
        };

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void OpenExternalBrowser() => OpenExternalBrowserButton.Click();

        public void EnterExternalUrl(Url url) => UrlOverlayText.EnterText(url.ToString());
    }
}