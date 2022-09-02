using NHSOnline.HttpMocks.GpMedicalRecord;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class GpMedicalRecordListPageContent
    {
        private readonly IWebInteractor _interactor;

        internal GpMedicalRecordListPageContent(IWebInteractor interactor)
        {
            _interactor = interactor;
        }

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Your GP health record");

        private WebLink Allergies => WebLink.WithText(_interactor, "Allergies and adverse reactions");
        private WebLink Medicines => WebLink.WithText(_interactor, "Medicines");

        private WebLink ConsultationsAndEvents => WebLink.WithText(_interactor, "Consultations and events");
        private WebLink Documents => WebLink.WithText(_interactor, "Documents");
        private WebLink TestResults => WebLink.WithText(_interactor, "Test results");

        internal void AssertOnPage(CareRecordLevel careRecordLevel = CareRecordLevel.Summary)
        {
            // Extending timeout to allow SSO to complete
            using var extendedTimeout = ExtendedTimeout.FromSeconds(30);

            TitleText.AssertVisible();

            switch (careRecordLevel)
            {
                case CareRecordLevel.Summary:
                    AssertSummaryCareRecord();
                    break;
                case CareRecordLevel.Detailed:
                    AssertDetailedCareRecord();
                    break;
                default:
                    break;
            }
        }

        private void AssertSummaryCareRecord()
        {
            Allergies.AssertVisibleContains();
            Medicines.AssertVisibleContains();
        }

        private void AssertDetailedCareRecord()
        {
            AssertSummaryCareRecord();

            ConsultationsAndEvents.AssertVisibleContains();
            Documents.AssertVisibleContains();
            TestResults.AssertVisible();
        }
    }
}