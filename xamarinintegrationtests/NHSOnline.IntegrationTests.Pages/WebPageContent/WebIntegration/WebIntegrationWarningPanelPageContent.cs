using System;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public sealed class WebIntegrationWarningPanelPageContent
    {
        private readonly IWebInteractor _interactor;
        private readonly string _pageTitle;

        internal WebIntegrationWarningPanelPageContent(IWebInteractor interactor, string pageTitle)
        {
            _interactor = interactor;
            _pageTitle = pageTitle;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "p", _pageTitle);
        private WebLink Continue => WebLink.WithText(_interactor, "Continue");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public void NavigateToNextPage()
        {
            Continue.Click();
        }
    }
}