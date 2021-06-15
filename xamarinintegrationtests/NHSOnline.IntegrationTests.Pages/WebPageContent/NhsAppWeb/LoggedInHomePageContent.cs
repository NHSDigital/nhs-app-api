using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class LoggedInHomePageContent
    {
        private readonly IWebInteractor _interactor;

        internal LoggedInHomePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Home");

        private WebDefinitionTerm NameDefinitionTerm => WebDefinitionTerm.WithTerm(_interactor, "Name:");

        private WebButton OpenSettingsButton => WebButton.WithText(_interactor, "Open Settings");

        private WebLink DismissBiometricsBanner => WebLink.WithText(_interactor, "Dismiss");

        private WebMenuItem GetYourCovidPassMenuItem => WebMenuItem.WithTitle(_interactor, "Get your NHS COVID Pass");

        private WebMenuItem MessagesMenuItem => WebMenuItem.WithTitle(_interactor, "View your messages");

        private WebMenuItem LinkedProfilesMenuItem => WebMenuItem.WithTitle(_interactor, "Linked profiles");

        public WebMenuItem GpHealthMenuItem => WebMenuItem.WithTitle(_interactor, "View your GP health record");

        public WebMenuItem PrescriptionsMenuItem => WebMenuItem.WithTitle(_interactor, "Order a repeat prescription");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            OpenSettingsButton,
            DismissBiometricsBanner,
            GetYourCovidPassMenuItem,
            MessagesMenuItem,
            LinkedProfilesMenuItem,
            GpHealthMenuItem,
            PrescriptionsMenuItem
        };

        private WebPanel UpliftPanel => WebPanel.WithTitle(_interactor, "Prove your identity to get full access");

        private WebButton Continue => UpliftPanel.ContainingButtonWithText("Continue");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public void AssertNameDisplayedFor(string patientName) => NameDefinitionTerm.AssertValue(patientName);

        public void AssertUserAgent(Platform platform) =>
            Assert.IsTrue(_interactor.GetUserAgent().Contains(platform.UserAgentDeviceTypePrefix(), StringComparison.InvariantCulture));

        public void ProveYourIdentityContinue() => Continue.Click();
    }
}