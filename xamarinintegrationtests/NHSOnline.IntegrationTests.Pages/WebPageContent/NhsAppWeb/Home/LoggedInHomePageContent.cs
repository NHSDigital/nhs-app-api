using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.DeviceProperties;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Home
{
    public class LoggedInHomePageContent
    {
        public const string BioFingerprint = "Set up fingerprint, face or iris";
        public const string BoiFaceId = "Set up Face ID";

        private readonly string _biometricsButtonText;
        private readonly IWebInteractor _interactor;

        internal LoggedInHomePageContent(IWebInteractor interactor, string biometricsButtonText)
        {
            _interactor = interactor;
            _biometricsButtonText = biometricsButtonText;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Home");

        private WebDefinitionTerm NameDefinitionTerm => WebDefinitionTerm.WithTerm(_interactor, "Name:");

        private WebDefinitionTerm NhsNumberTerm => WebDefinitionTerm.WithTerm(_interactor, "NHS number:");

        private WebText BiometricsPanelTitle => WebText.WithTagAndText(_interactor, "h2", "Login options");

        private WebText BiometricsPanelText => WebText.WithTagAndText(
            _interactor,
            "p",
            "If your mobile device supports fingerprint, face or iris recognition, " +
            "you can use it to log in to the NHS App instead of a password and security code.");

        private WebButton OpenSettingsButton => WebButton.WithText(_interactor, _biometricsButtonText);

        private WebLink DismissBiometricsBanner => WebLink.WithText(_interactor, "Dismiss");

        private WebMenuItem GetYourCovidPassMenuItem => WebMenuItem.WithTitle(_interactor, "Get your NHS COVID Pass");

        private WebMenuItem MessagesMenuItem => WebMenuItem.WithTitle(_interactor, "View your messages");

        private WebMenuItem LinkedProfilesMenuItem => WebMenuItem.WithTitle(_interactor, "Linked profiles");

        public WebMenuItem GpHealthMenuItem => WebMenuItem.WithTitle(_interactor, "View your GP health record");

        public WebMenuItem PrescriptionsMenuItem => WebMenuItem.WithTitle(_interactor, "Order a prescription");

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

        public void AssertNameDisplayedFor(string patientName) => NameDefinitionTerm.AssertValue(patientName);

        public void AssertUserAgent(Platform platform) =>
            Assert.IsTrue(_interactor.GetUserAgent().Contains(platform.UserAgentDeviceTypePrefix(), StringComparison.InvariantCulture));

        public LoggedInHomePageContent AssertBiometricPanelVisible()
        {
            BiometricsPanelTitle.AssertVisible();
            BiometricsPanelText.AssertVisible();
            OpenSettingsButton.AssertVisible();
            DismissBiometricsBanner.AssertVisible();

            return this;
        }

        public void AssertBiometricPanelNotVisible()
        {
            BiometricsPanelTitle.AssertNotVisible();
            BiometricsPanelText.AssertNotVisible();
            OpenSettingsButton.AssertNotVisible();
            DismissBiometricsBanner.AssertNotVisible();
        }

        public void AssertUpliftPanelNotVisible() => UpliftPanel.AssertNotVisible();

        public void AssertNhsNumberNotVisible() => NhsNumberTerm.AssertNotVisible();

        public void ProveYourIdentityContinue() => Continue.Click();

        public void DismissBiometricPanel() => DismissBiometricsBanner.Click();

        public void GetYourCovidPass() => GetYourCovidPassMenuItem.Click();

        private WebPanel UpliftPanel => WebPanel.WithTitle(_interactor, "Prove your identity to get full access");

        private WebButton Continue => UpliftPanel.ContainingButtonWithText("Continue");

        internal void AssertOnPage() => TitleText.AssertVisible();
    }
}