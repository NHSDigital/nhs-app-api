package features.prescriptionsSubmission.stepDefinitions

import constants.Supplier
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import features.prescriptions.stepDefinitions.PrescriptionsSerenityHelpers
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem

import pages.prescription.PrescriptionSuccessPage
import utils.ProxySerenityHelpers
import utils.getOrFail
import utils.getOrNull

open class PrescriptionsSuccessStepDefinitions {
    lateinit var prescriptionSuccessPage: PrescriptionSuccessPage

    @When("^I click the Go to your prescription orders link$")
    fun iClickTheGoToYourPrescriptionOrdersLink() {
        prescriptionSuccessPage.clickGoToViewOrdersLink()
    }

    @Then("^I see the Order Success page$")
    fun iSeeTheOrderSuccessPage() {
        prescriptionSuccessPage.checkSuccessHeading()
    }

    @Then("^I see the Order Success page with a playback of my order and what happens next with no nominated " +
        "pharmacy$")
    fun iSeeTheOrderSuccessPageWithAPlaybackOfMyOrderAndWhatHappensNextWithNoNominatedPharmacy() {
        prescriptionSuccessPage.checkHasOrderSummary(PrescriptionsSerenityHelpers.SELECTED_COURSES.getOrFail())
        prescriptionSuccessPage.checkWhatHappensNextNoNominatedPharmacy()
    }

    @Then("^I see the Order Success page with a playback of my order and what happens next for (.*)$")
    fun iSeeTheOrderSuccessPageWithAPlaybackOfMyOrderAndWhatHappensNextForPharmacy(pharmacyType: String) {
        prescriptionSuccessPage.checkHasOrderSummary(PrescriptionsSerenityHelpers.SELECTED_COURSES.getOrFail())
        val nominatedPharmacy: NhsAzureSearchOrganisationItem =
            NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.getOrFail()
        when (pharmacyType) {
            "nominated pharmacy" -> {
                prescriptionSuccessPage
                    .checkWhatHappensNextHighStreetPharmacy(nominatedPharmacy.OrganisationName)
            }
            "Internet pharmacy" -> {
                prescriptionSuccessPage.checkWhatHappensNextInternetPharmacy(nominatedPharmacy)
            }
            else -> {
                throw IllegalArgumentException("$pharmacyType not implemented")
            }
        }
    }

    @Then("^I see the Order Success page with a playback of my order and what happens next$")
    fun iSeeTheOrderSuccessPageWithAPlaybackOfMyOrderAndWhatHappensNext() {
        prescriptionSuccessPage.checkHasOrderSummary(PrescriptionsSerenityHelpers.SELECTED_COURSES.getOrFail())
        prescriptionSuccessPage.checkWhatHappensNext()
    }

    @Then("^I see the Order Success page with what happens next for proxy$")
    fun iSeeTheOrderSuccessPageWithWhatHappensNextForProxy() {
        val patientName = if (PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>() === Supplier.TPP) {
            ProxySerenityHelpers.getPatientOrProxy().formattedFullName()
        } else {
            ProxySerenityHelpers.getPatientOrProxy().name.firstName
        }

        prescriptionSuccessPage.checkSuccessHeadingForProxy(patientName)
        prescriptionSuccessPage.checkWhatHappensNextForProxy(patientName)
        prescriptionSuccessPage.checkHasNoOrderSummary()
    }
}

