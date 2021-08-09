using System.Collections.Generic;
using NHSOnline.IntegrationTests.UI.Components;
using NHSOnline.IntegrationTests.UI.Components.Android;
using NHSOnline.IntegrationTests.UI.Components.Web;
using NHSOnline.IntegrationTests.UI.Drivers;

namespace NHSOnline.IntegrationTests.Pages.WebPageContent.NhsAppWeb
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
            "Order a repeat prescription");

        private WebMenuItem ViewYourOrdersMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "View your orders",
            "view-orders");

        private WebText ViewYourOrdersText => WebText.WithTagAndText(_interactor,
            "p",
            "See repeat prescriptions you have ordered" );

        private WebMenuItem PkbHospitalAndOtherPrescriptionsMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Hospital and other prescriptions",
            "btn_pkb_medicines");

        private WebText PkbHospitalAndOtherPrescriptionsText => WebText.WithTagAndText(
            _interactor,
            "p",
            "See your current and past prescriptions");

        private WebMenuItem PkbCieHospitalAndOtherMedicinesMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Hospital and other medicines",
            "btn_pkb_cie_medicines");

        private WebText PkbCieHospitalAndOtherMedicinesText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View your current and past medicines or add a record of your own");

        private WebMenuItem PkbSecondaryCareHospitalAndOtherMedicinesMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Hospital and other medicines",
            "btn_pkb_secondary_care_medicines");

        private WebText PkbSecondaryCareHospitalAndOtherMedicinesText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View your current and past medicines or add a record of your own");

        private WebMenuItem PkbMyCareViewHospitalAndOtherMedicinesMenuItem => WebMenuItem.WithTitle(
            _interactor,
            "Hospital and other medicines",
            "btn_pkb_my_care_view_medicines");

        private WebText PkbMyCareViewHospitalAndOtherMedicinesText => WebText.WithTagAndText(
            _interactor,
            "p",
            "View your current and past medicines or add a record of your own");

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

        public PrescriptionsPageContent AssertPkbCieElements()
        {
            PkbCieHospitalAndOtherMedicinesMenuItem.AssertVisible();
            PkbCieHospitalAndOtherMedicinesText.AssertVisible();
            return this;
        }

        public PrescriptionsPageContent AssertPkbSecondaryCareElements()
        {
            PkbSecondaryCareHospitalAndOtherMedicinesMenuItem.AssertVisible();
            PkbSecondaryCareHospitalAndOtherMedicinesText.AssertVisible();
            return this;
        }

        public PrescriptionsPageContent AssertPkbMyCareViewElements()
        {
            PkbMyCareViewHospitalAndOtherMedicinesMenuItem.AssertVisible();
            PkbMyCareViewHospitalAndOtherMedicinesText.AssertVisible();
            return this;
        }

        public IEnumerable<IFocusable> FocusableElements => new IFocusable[]
        {
            OrderARepeatPrescriptionButton,
            ViewYourOrdersMenuItem,
            PkbHospitalAndOtherPrescriptionsMenuItem,
            PkbCieHospitalAndOtherMedicinesMenuItem,
            PkbSecondaryCareHospitalAndOtherMedicinesMenuItem,
            PkbMyCareViewHospitalAndOtherMedicinesMenuItem
        };

        public void NavigateToOrderARepeatPrescription() => OrderARepeatPrescriptionButton.Click();

        public void NavigateToHospitalAndOtherPrescriptions() => PkbHospitalAndOtherPrescriptionsMenuItem.Click();

        public void KeyboardNavigateToOrderARepeatPrescription(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(OrderARepeatPrescriptionButton, navigation);

        public void KeyboardNavigateToViewYourOrders(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(ViewYourOrdersMenuItem, navigation);

        public void KeyboardNavigateToPkbHospitalAndOtherPrescriptions(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(PkbHospitalAndOtherPrescriptionsMenuItem, navigation);

        public void KeyboardNavigateToPkbCieHospitalAndOtherMedicines(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(PkbCieHospitalAndOtherMedicinesMenuItem, navigation);

        public void KeyboardNavigateToPkbSecondaryCareHospitalAndOtherMedicines(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(PkbSecondaryCareHospitalAndOtherMedicinesMenuItem, navigation);

        public void KeyboardNavigateToPkbMyCareViewHospitalAndOtherMedicines(AndroidKeyboardNavigation navigation)
            => KeyboardNavigateToAndActivateMenuItem(PkbMyCareViewHospitalAndOtherMedicinesMenuItem, navigation);

        private void KeyboardNavigateToAndActivateMenuItem(IFocusable menuItem, AndroidKeyboardNavigation keyboardPageContentNavigation)
        {
            keyboardPageContentNavigation.TabBetween(OrderARepeatPrescriptionButton, menuItem);
            keyboardPageContentNavigation.PressEnterKey();
        }
    }
}
