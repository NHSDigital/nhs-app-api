using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
{
    public class YourHealthGncrPageContent : YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthGncrPageContent(IWebInteractor interactor) : base(interactor) => _interactor = interactor;

        private WebMenuItem GncrHospitalAndHealthcareLettersMenuItem => WebMenuItem.WithTitle(_interactor, "Hospital and other healthcare letters");

        private WebText GncrHospitalAndHealthcareLettersText => WebText.WithTagAndText(_interactor, "p", "This includes your hospital, mental health and social care letters and documents");

        public void AssertElements()
        {
            GncrHospitalAndHealthcareLettersMenuItem.AssertVisible();
            GncrHospitalAndHealthcareLettersText.AssertVisible();
        }

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            CovidPassMenuItem,
            VaccineRecordMenuItem,
            GpHeathRecordMenuItem,
            GncrHospitalAndHealthcareLettersMenuItem,
            OrganDonationMenuItem,
            NdopMenuItem
        };

        public void KeyboardNavigateToGncr(AndroidKeyboardNavigation navigation) =>
            KeyboardNavigateToAndActivateMenuItem(GncrHospitalAndHealthcareLettersMenuItem, navigation);
    }
}