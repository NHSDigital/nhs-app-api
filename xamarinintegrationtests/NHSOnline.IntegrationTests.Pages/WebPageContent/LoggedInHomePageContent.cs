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


        private WebPanel UpliftPanel => WebPanel.WithTitle(_interactor, "Prove your identity to get full access");
        private WebButton Continue => UpliftPanel.ContainingButtonWithText("Continue");

        internal void AssertOnPage()
        {
            Title.AssertVisible();
        }

        public LoggedInHomePageContent AssertNameDisplayedFor(string patientName)
        {
            Name.AssertValue(patientName);
            return this;
        }

        public void ProveYourIdentityContinue() => Continue.Click();
    }
}