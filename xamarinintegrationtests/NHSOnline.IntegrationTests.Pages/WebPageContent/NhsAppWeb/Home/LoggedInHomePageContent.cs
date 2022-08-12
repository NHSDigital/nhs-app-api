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
        private readonly IWebInteractor _interactor;

        internal LoggedInHomePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText MobileTitleText => WebText.WithTagAndText(_interactor, "h1", @"We're here for you");

        private WebText TabletTitleText => WebText.WithTagAndText(
            _interactor, "h1", "Access your NHS services any time, day or night");

        private WebDefinitionTerm NhsNumberTerm => WebDefinitionTerm.WithTerm(_interactor, "NHS number:");

        private WebText YellowProxyUserBannerText(string linkedPatientName) =>
            WebText.WithTagAndText(_interactor, "p", $"Acting on behalf of {linkedPatientName}");

        private WebMenuItem GetYourCovidPassMenuItem => WebMenuItem.WithTitle(_interactor, "Get your NHS COVID Pass");

        private WebMenuItem BookYourCovidVaccinationMenuItem => WebMenuItem.WithTitle(_interactor, "Book or manage a coronavirus (COVID-19) vaccination");

        private WebMenuItem MessagesMenuItem => WebMenuItem.WithTitle(_interactor, "View your messages");

        private WebMenuItem LinkedProfilesMenuItem => WebMenuItem.WithTitle(_interactor, "Linked profiles");

        public WebMenuItem GpHealthMenuItem => WebMenuItem.WithTitle(_interactor, "View your GP health record");

        public WebMenuItem PrescriptionsMenuItem => WebMenuItem.WithTitle(_interactor, "Order a repeat prescription");

        private WebButton FindNhsServicesNearYouButton => WebButton.WithText(_interactor,"Find NHS services near you");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            MessagesMenuItem,
            GpHealthMenuItem,
            PrescriptionsMenuItem,
            GetYourCovidPassMenuItem,
            BookYourCovidVaccinationMenuItem,
            LinkedProfilesMenuItem,
            FindNhsServicesNearYouButton
        };

        public void AssertNameDisplayedFor(string patientName) => WebText.WithText(_interactor, patientName);

        public void AssertUserAgent(Platform platform) =>
            Assert.IsTrue(_interactor.GetUserAgent().Contains(platform.UserAgentDeviceTypePrefix(), StringComparison.InvariantCulture));

        public void AssertLinkedProfileYellowBannerVisible(string linkedPatientName) =>
            YellowProxyUserBannerText(linkedPatientName).AssertVisible();

        public void AssertUpliftPanelNotVisible() => UpliftPanel.AssertNotVisible();

        public void AssertNhsNumberNotVisible() => NhsNumberTerm.AssertNotVisible();

        public void ProveYourIdentityContinue() => Continue.Click();

        public void GetYourCovidPass() => GetYourCovidPassMenuItem.Click();

        private WebPanel UpliftPanel => WebPanel.WithTitle(_interactor, "Prove your identity to get full access");

        private WebButton Continue => UpliftPanel.ContainingButtonWithText("Continue");

        internal void AssertOnPage() => MobileTitleText.AssertVisible();

        internal void AssertOnTabletPage() => TabletTitleText.AssertVisible();
    }
}