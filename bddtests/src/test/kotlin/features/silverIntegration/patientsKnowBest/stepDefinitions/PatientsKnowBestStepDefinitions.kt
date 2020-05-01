package features.silverIntegration.patientsKnowBest.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import pages.HybridPageObject
import pages.RedirectorPage
import pages.appointments.HospitalAppointmentsPage
import pages.assertElementNotPresent
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class PatientsKnowBestStepDefinitions : HybridPageObject() {
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private lateinit var redirector: RedirectorPage
    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage

    @Given("^I am a user who can view Appointments from Patients Know Best$")
    fun iAmAUserWhoCanViewAppointmentsFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB)
    }

    @Given("^I am a user who cannot view Appointments from Patients Know Best$")
    fun iAmAUserWhoCannotViewAppointmentsFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
    }

    @Given("^I am a user who can view care plans from Patients Know Best$")
    fun iAmAUserWhoCanViewCarePlansFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB)
    }

    @Given("^I am a user who cannot view care plans from Patients Know Best$")
    fun iAmAUserWhoCannotViewCarePlansFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE)
    }

    @Given("^I am a user who can view health tracker from Patients Know Best$")
    fun iAmAUserWhoCanViewHealthTrackerFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB)
    }

    @Given("^I am a user who cannot view health tracker from Patients Know Best$")
    fun iAmAUserWhoCannotViewHealthTrackerFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_NONE)
    }

    @Given("^I am a user who can view Messages and Online Consultations from Patients Know Best$")
    fun iAmAUserWhoCanViewMessagesAndOnlineConsultationsFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB)
    }

    @Given("^I am a user who cannot view Messages and Online Consultations from Patients Know Best$")
    fun iAmAUserWhoCannotViewMessagesAndOnlineConsultationsFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @Given("^I am a user who can view Shared Links from Patients Know Best$")
    fun iAmAUserWhoCanViewSharedLinksFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB)
    }

    @Given("^I am a user who cannot view Shared Links from Patients Know Best$")
    fun iAmAUserWhoCannotViewSharedLinksFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_NONE)
    }

    @Then("the link to Track your health is not available on the health record hub page")
    fun theLinkToHealthTrackerIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Track your health").assertElementNotPresent()
    }

    @Then("the link to Care plans is not available on the health record hub page")
    fun theLinkToCarePlansIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Care plans").assertElementNotPresent()
    }

    @When("^I click the 'Continue' button on the redirector page with a url starting with '(.*)'$")
    fun iClickTheContinueButtonOnTheRedirectorPageWithAUrlOf(continueUrl: String) {
        redirector.interruptionCard.assertContinueAndClick(continueUrl)
    }

    @When("^I click the PKB View Appointments link on the Appointments page")
    fun iClickThePkbViewAppointmentsLinkOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.click()
    }

    @Then("I am redirected to the redirector page with the header '(.*)'$")
    fun assertRedirectorPageIsVisible(header: String) {
        redirector.title(header).waitForElement()
    }

    @Then("the warning message on the Redirector page explains the service is from Patients Know Best$")
    fun assertWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "This is a connected service",
                "Your GP surgery or hospital has chosen this personal health record " +
                        "service provided by Patients Know Best.",
                "Find out more about personal health record services.")
    }

    @Then("the link to PKB View Appointments is not available on the Appointments page")
    fun theLinkToPkbViewAppointmentsIsNotAvailableOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.assertElementNotPresent()
    }

    @Then("^I can see the PKB View Appointments link on the Appointments page$")
    fun iCanSeeThePkbViewAppointmentsLinkOnTheAppointmentsPage(){
        hospitalAppointmentsPage.assertPkbViewAppointmentsIsDisplayed()
    }

    private fun setupPatient(configuration: SJRJourneyType) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null, configuration)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
