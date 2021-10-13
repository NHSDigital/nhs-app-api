using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.WebIntegration
{
    public class TestWebIntegrationProviderPageContent
    {
        private readonly IWebInteractor _interactor;

        internal TestWebIntegrationProviderPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Silver Integration Test Provider Internal Page");
        private WebLink BackLink => WebLink.WithText(_interactor, "Back");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertUserAgent(Platform platform) =>
            Assert.IsTrue(_interactor.GetUserAgent().Contains(platform.UserAgentDeviceTypePrefix(), StringComparison.InvariantCulture));

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[] { BackLink };
    }
}