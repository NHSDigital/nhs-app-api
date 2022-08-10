using System.Threading;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class NominatedPharmacyPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NominatedPharmacyPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Your nominated pharmacy");

        private WebText nominatedPharmacyInstructionsText => WebText.WithTagAndText(_interactor,
            "p",
            "If you order prescriptions using the NHS App, this pharmacy is where they will be sent." );

        private WebButton ChangeNominatedPharmacyButton =>  WebButton.WithText(
            _interactor,
            "Change your nominated pharmacy");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public NominatedPharmacyPageContent AssertPageElements()
        {
            TitleText.AssertVisible();
            nominatedPharmacyInstructionsText.AssertVisible();
            ChangeNominatedPharmacyButton.AssertVisible();

            return this;
        }

        public void NavigateToChangeNominatedPharmacy() => ChangeNominatedPharmacyButton.Click();
    }
}