using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.IOS;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class ChoosePrescriptionErrorPageContent
    {
        private readonly IWebInteractor _interactor;

        internal ChoosePrescriptionErrorPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "No repeat prescriptions available to order");

        private WebText NoRepeatPrescriptionsText => WebText.WithTagAndText(
            _interactor,
            "p",
            "Contact your GP surgery to get medication or set up a repeat prescription.");

        internal void AssertOnPage()
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(15);

            TitleText.AssertVisible();
            NoRepeatPrescriptionsText.AssertVisible();
        }
    }
}