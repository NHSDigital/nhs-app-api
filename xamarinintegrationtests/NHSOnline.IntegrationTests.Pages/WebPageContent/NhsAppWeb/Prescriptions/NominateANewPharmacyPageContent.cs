using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class NominateANewPharmacyPageContent
    {
        private readonly IWebInteractor _interactor;

        internal NominateANewPharmacyPageContent(IWebInteractor interactor) => _interactor = interactor;

        public WebText StepStartHeading => WebText.WithTagAndText(
            _interactor,
            "h1",
            "The pharmacy you choose is where your prescriptions will be sent");

        public WebText StepChoosePharmacyHeading => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Choose a type of pharmacy to search for");

        public WebText StepFindAHighStreetPharmacyHeading => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Find a high street pharmacy");

        public WebInputText StepFindAHighStreetPharmacyPostcodeInputText =>
            WebInputText.WithLabel(_interactor, "Enter a full postcode in England");

        public WebText StepSelectAHighStreetPharmacyHeading(string postcode)
        {
            return WebText.WithTagAndText(
                _interactor,
                "h1",
                $"High street pharmacies near \"{postcode}\"");
        }

        public WebText StepConfirmNominatedPharmacyHeading()
        {
            return WebText.WithTagAndText(
                _interactor,
                "h1",
                "Check your nominated pharmacy details");
        }

        public WebText StepHighStreetPharmacyFinalConfirmationHeading()
        {
            return WebText.WithTagAndText(
                _interactor,
                "h1",
                "You have nominated a pharmacy");
        }

        public WebText StepHighStreetPharmacyFinalPharmacyNameConfirmed()
        {
            return WebText.WithTagAndText(
                _interactor,
                "p",
                DefaultNominatedPharmacyName);
        }

        public WebText StepOnlinePharmacyFinalShutterHeading()
        {
            return WebText.WithTagAndText(
                _interactor,
                "h1",
                "Register with the online-only pharmacy directly");
        }

        public WebText StepOnlinePharmacyFinalShutterText()
        {
            return WebText.WithTagAndText(
                _interactor,
                "p",
                "To nominate an online-only pharmacy, you must register with the pharmacy through their website or contact them.");
        }

        public WebLink StepSelectAHighStreetPharmacy => WebLink.WithText(_interactor, DefaultNominatedPharmacyName);

        private static string DefaultNominatedPharmacyName => "Walter White Chemists";

        public WebButton ContinueButton => WebButton.WithText(_interactor, "Continue");
        public WebButton SearchButton => WebButton.WithText(_interactor, "Search");
        public WebButton ConfirmButton => WebButton.WithText(_interactor, "Confirm");
    }
}

