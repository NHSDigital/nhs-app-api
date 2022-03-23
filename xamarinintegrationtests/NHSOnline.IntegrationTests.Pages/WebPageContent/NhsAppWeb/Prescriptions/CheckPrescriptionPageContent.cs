using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class CheckPrescriptionPageContent
    {
        private readonly IWebInteractor _interactor;

        internal CheckPrescriptionPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Check your prescription details before you order");

        private WebText PrescriptionToCheck => WebText.WithTagAndText(_interactor,
            "p", "Tablet");

        private WebButton ContinueButton => WebButton.WithText(_interactor, "Confirm and order prescriptions");

        public void Continue() => ContinueButton.Click();

        internal void AssertOnPage()
        {
            TitleText.AssertVisible();
            PrescriptionToCheck.AssertVisible();
        }
    }
}