using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.YourHealth
{
    public class YourHealthPageContent
    {
        private readonly IWebInteractor _interactor;

        internal YourHealthPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(_interactor, "h1", "Your health");

        internal WebMenuItem CovidPassMenuItem => WebMenuItem.WithTitle(_interactor, "NHS COVID Pass");

        private WebText CovidPassText => WebText.WithTagAndText(_interactor, "p", "View and download your COVID Pass for travel abroad");

        internal WebMenuItem VaccineRecordMenuItem => WebMenuItem.WithTitle(_interactor, "Check your COVID-19 vaccine record");

        private WebText VaccineRecordText => WebText.WithTagAndText(_interactor, "p", "View your vaccination details, like the name and batch number, and report any side effects you have experienced");

        internal WebMenuItem RecordSharingMenuItem => WebMenuItem.WithTitle(_interactor, "Record sharing");

        private WebText RecordSharingText => WebText.WithText(_interactor, "Choose and manage information you share with your health teams");

        internal WebMenuItem GpHeathRecordMenuItem => WebMenuItem.WithTitle(_interactor, "GP health record");

        private WebText GpHealthRecordText => WebText.WithTagAndText(_interactor, "p", "View allergies, medicines, test results and more in your GP health record");

        internal WebMenuItem OrganDonationMenuItem => WebMenuItem.WithTitle(_interactor, "Manage your organ donation decision");

        private WebText OrganDonationText => WebText.WithTagAndText(_interactor, "p", "Help save thousands of lives in the UK every year by signing up to become a donor on the NHS Organ Donor Register");

        internal WebMenuItem NdopMenuItem => WebMenuItem.WithTitle(_interactor, "Choose if data from your health records is shared for research and planning");

        private WebText NdopText => WebText.WithTagAndText(_interactor, "p", "Find out how the NHS uses your confidential patient information and choose whether or not it can be used for research and planning");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public YourHealthPageContent AssertPageElements()
        {
            GpHeathRecordMenuItem.AssertVisible();
            GpHealthRecordText.AssertVisible();
            OrganDonationMenuItem.AssertVisible();
            OrganDonationText.AssertVisible();
            NdopMenuItem.AssertVisible();
            NdopText.AssertVisible();
            return this;
        }

        public YourHealthPageContent AssertCovidPassElements()
        {
            CovidPassMenuItem.AssertVisible();
            CovidPassText.AssertVisible();
            return this;
        }

        public YourHealthPageContent AssertVaccineRecordElements()
        {
            VaccineRecordMenuItem.AssertVisible();
            VaccineRecordText.AssertVisible();
            return this;
        }

        public void NavigateToVaccineRecord() => VaccineRecordMenuItem.Click();

        public void NavigateToNdop() => NdopMenuItem.Click();

        public void NavigateToGPHealthRecord() => GpHeathRecordMenuItem.Click();

        internal void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(CovidPassMenuItem, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
