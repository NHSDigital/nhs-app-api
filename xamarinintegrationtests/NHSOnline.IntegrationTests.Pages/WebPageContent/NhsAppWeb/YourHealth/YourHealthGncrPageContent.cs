using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class YourHealthGncrPageContent : YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthGncrPageContent(IWebInteractor interactor) : base(interactor) => _interactor = interactor;

        private WebMenuItem GncrHospitalAndHealthcareDocumentsMenuItem => WebMenuItem.WithTitle(_interactor, "Hospital and other healthcare documents");

        private WebText GncrHospitalAndHealthcareLettersAndDocumentsText => WebText.WithTagAndText(_interactor, "p", "View letters and documents from your hospital, mental health or social care teams");

        public void AssertElements()
        {
            GncrHospitalAndHealthcareDocumentsMenuItem.AssertVisible();
            GncrHospitalAndHealthcareLettersAndDocumentsText.AssertVisible();
        }

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            CovidPassMenuItem,
            VaccineRecordMenuItem,
            GpHeathRecordMenuItem,
            GncrHospitalAndHealthcareDocumentsMenuItem,
            OrganDonationMenuItem,
            NdopMenuItem
        };

        public void KeyboardNavigateToGncr(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateMenuItem(GncrHospitalAndHealthcareDocumentsMenuItem, navigation);
    }
}