using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent
{
    public class LoggedInHomePageContent
    {
        private readonly IWebInteractor _interactor;

        internal LoggedInHomePageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText Title => WebText.WithTagAndText(_interactor, "h1", "Home");

        private WebDefinitionTerm Name => WebDefinitionTerm.WithTerm(_interactor, "Name:");

        private WebButton OpenSettings => WebButton.WithText(_interactor, "Open Settings");

        private WebLink DismissBiometricsBanner => WebLink.WithText(_interactor, "Dismiss");

        private WebMenuItem CheckCovidVaccineMenuItem => WebMenuItem.WithTitle(_interactor, "Check your COVID-19 vaccine record");

        private WebMenuItem MessagesMenuItem => WebMenuItem.WithTitle(_interactor, "View your messages");

        private WebMenuItem LinkedProfilesMenuItem => WebMenuItem.WithTitle(_interactor, "Linked profiles");

        private WebMenuItem GpHealthMenuItem => WebMenuItem.WithTitle(_interactor, "View your GP health record");

        private WebMenuItem PrescriptionsMenuItem => WebMenuItem.WithTitle(_interactor, "Order a repeat prescriptions");

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            OpenSettings,
            DismissBiometricsBanner,
            CheckCovidVaccineMenuItem,
            MessagesMenuItem,
            LinkedProfilesMenuItem,
            GpHealthMenuItem,
            PrescriptionsMenuItem
        };

        private WebPanel UpliftPanel => WebPanel.WithTitle(_interactor, "Prove your identity to get full access");
        private WebButton Continue => UpliftPanel.ContainingButtonWithText("Continue");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public void AssertNameDisplayedFor(string patientName)
        {
            Name.AssertValue(patientName);
        }

        public void ProveYourIdentityContinue() => Continue.Click();
    }
}