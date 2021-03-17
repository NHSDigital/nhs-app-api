package features.silverIntegration.pkbSecondaryCare.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.pages.MyCareViewPage
import mocking.thirdPartyProviders.pkb.MyCareViewRequestBuilder
import models.IdentityProofingLevel
import pages.HybridPageObject
import pages.PrescriptionsHubPage
import pages.RedirectorPage
import pages.appointments.HospitalAppointmentsPage
import pages.assertElementNotPresent
import pages.assertIsVisible
import utils.SerenityHelpers

class PkbMyCareViewStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var prescriptionsHubPage: PrescriptionsHubPage
    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage
    private lateinit var myCareViewPage: MyCareViewPage

    @Given("^I am a user who can view Medicines from PKB My Care View$")
    fun iAmAUserWhoCanViewMedicinesFromPkbMyCareView() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MEDICINES_PKB_MY_CARE_VIEW)
    }

    @Given("^I am a user who cannot view Medicines from PKB My Care View$")
    fun iAmAUserWhoCannotViewMedicinesFromMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE)
    }

    @Given("^I am a user who can view Appointments from PKB My Care View$")
    fun iAmAUserWhoCanViewAppointmentsFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_APPOINTMENTS_PKB_MY_CARE_VIEW)
    }

    @Given("^I am a user who cannot view Appointments from PKB My Care View$")
    fun iAmAUserWhoCannotViewAppointmentsFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
    }

    @Given("^My Care View responds to requests for appointments$")
    fun myCareViewRespondsToRequestsForAppointments() {
        MockingClient.instance.forMyCareView.mock { MyCareViewRequestBuilder().appointmentRequest().respondWithPage() }
    }

    @Given("^My Care View responds to requests for medicines$")
    fun myCareViewRespondsToRequestsForMedicines() {
        MockingClient.instance.forMyCareView.mock { MyCareViewRequestBuilder().medicinesRequest().respondWithPage() }
    }

    @Then("^the PKB My Care View Medicines link is available on the Prescriptions Hub$")
    fun thePKBMyCareViewMedicinesLinkIsAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbMyCareViewMedicinesJumpOffButton.assertIsVisible()
    }

    @Then("^the PKB My Care View Medicines link is not available on the Prescriptions Hub$")
    fun theCieViewMedicinesLinkIsNotAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbMyCareViewMedicinesJumpOffButton.assertElementNotPresent()
    }

    @Then("^I click the PKB My Care View Medicines link on the Prescriptions hub$")
    fun iClickThePkbMyCareViewMedicinesLink(){
        prescriptionsHubPage.pkbMyCareViewMedicinesJumpOffButton.click()
    }

    @Then("^I can see the PKB My Care View Appointments link on the Appointments page$")
    fun iCanSeeThePkbViewAppointmentsLinkOnTheAppointmentsPage(){
        hospitalAppointmentsPage.assertPkbViewAppointmentsIsDisplayed()
    }

    @When("^I click the PKB My Care View Appointments link on the Appointments page")
    fun iClickThePkbViewAppointmentsLinkOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.click()
    }

    @Then("the hospital and medicines warning on the page explains the service is from PKB My Care View$")
    fun assertHospitalAndMedicinesWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Hospital and other medicines\nThis service is provided by MyCareView powered by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("^the link to PKB My Care View Appointments is not available on the Appointments page$")
    fun theLinkToPkbViewAppointmentsIsNotAvailableOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.assertElementNotPresent()
    }

    @Then("^I am navigated to a third party site for My Care View$")
    fun iNavigateToThirdPartySiteForMyCareView() {
        myCareViewPage.assertTitleVisible()
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
