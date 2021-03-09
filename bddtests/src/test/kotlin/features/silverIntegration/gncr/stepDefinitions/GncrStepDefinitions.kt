package features.silverIntegration.gncr.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.pages.GNCRPage
import mocking.thirdPartyProviders.gncr.GNCRRequestBuilder
import models.IdentityProofingLevel
import pages.HybridPageObject
import pages.RedirectorPage
import pages.appointments.HospitalAppointmentsPage
import pages.assertElementNotPresent
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class GncrStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private lateinit var gncrPage: GNCRPage

    @Given("^I am a user who can view Appointments from GNCR$")
    fun iAmAUserWhoCanViewAppointmentsfromGNCR(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_GNCR)
    }

    @Given("^I am a user who cannot view Appointments from GNCR$")
    fun iAmAUserWhoCannotViewAppointmentsfromGNCR(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
    }

    @Given("^GNCR responds to requests for appointments$")
    fun gncrRespondsToRequestsForAppointments() {
        MockingClient.instance.forGNCR.mock { GNCRRequestBuilder().gncrAppointmentsRequest().respondWithPage() }
    }

    @Given("^GNCR responds to requests for correspondence$")
    fun gncrRespondsToRequestsForCorrespondence() {
        MockingClient.instance.forGNCR.mock { GNCRRequestBuilder().gncrCorrespondenceRequest().respondWithPage() }
    }

    @When("^I click the GNCR View Appointments link on the Appointments page")
    fun iClickTheGncrViewAppointmentsLinkOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnGncrAppointments.click()
    }

    @Then("the hospital and other warning message on the Redirector page explains the service is from GNCR$")
    fun assertHospitalAndOtherAppointmentsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Hospital and other appointments\nThis service is provided by Great North Care Record",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the healthcare letters warning message on the Redirector page explains the service is from GNCR$")
    fun assertHospitalAndOtherHealthCareLettersWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Hospital and other healthcare letters\nThis service is provided by Great North Care Record",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("^the link to GNCR View Appointments is not available on the Appointments page$")
    fun theLinkToGNCRViewAppointmentsIsNotAvailableOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnGncrAppointments.assertElementNotPresent()
    }

    @Then("^I can see the GNCR View Appointments link on the Appointments page$")
    fun iCanSeeTheGNCRViewAppointmentsLinkOnTheAppointmentsPage(){
        hospitalAppointmentsPage.assertGncrViewAppointmentsIsDisplayed()
    }

    @Given("^I am a user who can view Correspondence from GNCR$")
    fun iAmAUserWhoCanViewCorrespondenceFromGNCR(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_GNCR)
    }

    @Given("^I am a user who cannot view Correspondence from GNCR$")
    fun iAmAUserWhoCannotViewCorrespondenceFromGNCR(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @Then("^the link to GNCR 'Hospital and other healthcare letters' is not available on the Health Records Hub page$")
    fun theLinkToGNCRCorrespondenceIsNotAvailableOnTheAppointmentsPage() {
        medicalRecordHubPage.getHeaderElement("Hospital and other healthcare letters").assertElementNotPresent()
    }

    @Then("^I am navigated to a third party site for GNCR$")
    fun iNavigateToThirdPartySiteForGNCR() {
        gncrPage.assertTitleVisible()
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
