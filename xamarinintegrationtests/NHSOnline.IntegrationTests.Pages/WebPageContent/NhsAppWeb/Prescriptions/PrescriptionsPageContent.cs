using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb.Prescriptions
{
    public class PrescriptionsPageContent
    {
        private readonly IWebInteractor _interactor;

        internal PrescriptionsPageContent(IWebInteractor interactor) => _interactor = interactor;

        private WebText TitleText => WebText.WithTagAndText(
            _interactor,
            "h1",
            "Prescriptions");

        private WebButton OrderARepeatPrescriptionButton =>  WebButton.WithText(
            _interactor,
            "Order a prescription");

        private WebMenuItem ViewYourOrdersMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "View your orders",
            "view-orders");

        private WebText ViewYourOrdersText => WebText.WithTagAndText(_interactor,
            "p",
            "See repeat prescriptions you have ordered" );

        private WebMenuItem PkbHospitalAndOtherPrescriptionsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Hospital and other medicines",
            "btn_pkb_medicines");

        private WebText PkbHospitalAndOtherPrescriptionsText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View your current and past medicines or add a record of your own");

        private WebMenuItem YourNominatedPharmacyItem => WebMenuItem.WithTitle(
            _interactor,
            "Your nominated pharmacy",
            "Pharmacy");

        private WebMenuItem NominateANewPharmacyItem => WebMenuItem.WithTitle(
            _interactor,
            "Nominate a pharmacy");

        internal void AssertOnPage() => TitleText.AssertVisible();

        public PrescriptionsPageContent AssertPageElements()
        {
            TitleText.AssertVisible();
            OrderARepeatPrescriptionButton.AssertVisible();
            ViewYourOrdersMenuItem.AssertVisible();
            ViewYourOrdersText.AssertVisible();
            return this;
        }

        public PrescriptionsPageContent AssertPkbElements()
        {
            PkbHospitalAndOtherPrescriptionsMenuItem.AssertVisible();
            PkbHospitalAndOtherPrescriptionsText.AssertVisible();
            return this;
        }

        public PrescriptionsPageContent AssertNominateANewPharmacy()
        {
            // API call required to load this link
            using var timeout = ExtendedTimeout.FromSeconds(10);

            NominateANewPharmacyItem.AssertVisible();
            return this;
        }

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            OrderARepeatPrescriptionButton,
            ViewYourOrdersMenuItem,
            PkbHospitalAndOtherPrescriptionsMenuItem
        };

        public void NavigateToOrderARepeatPrescription() => OrderARepeatPrescriptionButton.Click();

        public void NavigateToHospitalAndOtherPrescriptions() => PkbHospitalAndOtherPrescriptionsMenuItem.Click();

        public void NavigateToNominatedPharmacy() => YourNominatedPharmacyItem.Click();

        public void NavigateToNominateANewPharmacy() => NominateANewPharmacyItem.Click();

        public void NavigateToViewYourOrders() => ViewYourOrdersMenuItem.Click();

        public void KeyboardNavigateToOrderARepeatPrescription(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(OrderARepeatPrescriptionButton, navigation);

        public void KeyboardNavigateToViewYourOrders(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(ViewYourOrdersMenuItem, navigation);

        public void KeyboardNavigateToPkbHospitalAndOtherPrescriptions(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(PkbHospitalAndOtherPrescriptionsMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(OrderARepeatPrescriptionButton, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
