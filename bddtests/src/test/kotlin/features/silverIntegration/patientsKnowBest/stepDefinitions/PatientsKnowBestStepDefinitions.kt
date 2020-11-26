package features.silverIntegration.patientsKnowBest.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import pages.HybridPageObject
import pages.PrescriptionsHubPage
import pages.RedirectorPage
import pages.appointments.HospitalAppointmentsPage
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class PatientsKnowBestStepDefinitions : HybridPageObject() {
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private lateinit var redirector: RedirectorPage
    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage
    private lateinit var prescriptionsHubPage: PrescriptionsHubPage

    @Given("^I am a user who can view Appointments from Patients Know Best$")
    fun iAmAUserWhoCanViewAppointmentsFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB)
    }

    @Given("^I am a user who cannot view Appointments from Patients Know Best$")
    fun iAmAUserWhoCannotViewAppointmentsFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
    }

    @Given("^I am a user who can view Medicines from Patients Know Best$")
    fun iAmAUserWhoCanViewMedicinesFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_PKB)
    }

    @Given("^I am a user who cannot view Medicines from Patients Know Best$")
    fun iAmAUserWhoCannotViewMedicinesFromPatientsKnowBest(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE)
    }

    @Given("^I am a user who can view test results from Patients Know Best$")
    fun iAmAUserWhoCanViewTestResultsFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_PKB)
    }

    @Given("^I am a user who can view care plans from Patients Know Best$")
    fun iAmAUserWhoCanViewCarePlansFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB)
    }

    @Given("^I am a user who cannot view test results from Patients Know Best$")
    fun iAmAUserWhoCannotViewTestResultsFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_NONE)
    }

    @Given("^I am a user who cannot view care plans from Patients Know Best$")
    fun iAmAUserWhoCannotViewCarePlansFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE)
    }

    @Given("^I am a user who cannot view care plans from Patients Know Best and has an IM1 Medical Record journey$")
    fun iAmAUserWhoCannotViewCarePlansFromPatientsKnowBestAndHasAnIM1MedicalRecordJourney() {
        setupPatient(arrayListOf(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE, SJRJourneyType.MEDICAL_RECORD_IM1));
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

    @Given("^I am a user with proof level 5 who can view Messages and Online Consultations from Patients Know Best$")
    fun iAmAUserWithProofLevel5WhoCanViewMessagesAndOnlineConsultationsFromPatientsKnowBest(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB, IdentityProofingLevel.P5)
    }

    @Given("^I am a user who cannot view Messages and Online Consultations from Patients Know Best$")
    fun iAmAUserWhoCannotViewMessagesAndOnlineConsultationsFromPatientsKnowBest() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @Given("^I am a user who can view Shared Links from Patients Know Best$")
    fun iAmAUserWhoCanViewSharedLinksFromPatientsKnowBest(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB)
    }

    @Given("^I am a user who cannot view Shared Links from Patients Know Best$")
    fun iAmAUserWhoCannotViewSharedLinksFromPatientsKnowBest(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_LIBRARY_NONE)
    }

    @Then("^the link to PKB Track your health is not available on the health record hub page$")
    fun theLinkToPkbHealthTrackerIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Track your health").assertElementNotPresent()
    }

    @Then("^the link to PKB shared links is not available on the health record hub page$")
    fun theLinkToPkbSharedLinksIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Shared links").assertElementNotPresent()
    }

    @Then("^the link to PKB test results is not available on the health record hub page$")
    fun theLinkToPkbTestResultsIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Test results").assertElementNotPresent()
    }

    @Then("^the link to PKB Care plans is not available on the health record hub page$")
    fun theLinkToPkbCarePlansIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Care plans").assertElementNotPresent()
    }

    @When("^I click the PKB View Appointments link on the Appointments page")
    fun iClickThePkbViewAppointmentsLinkOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.click()
    }

    @Then("the messages and consultations warning on the page explains the service is from Patients Know Best$")
    fun assertMessagesAndconsultationsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Messages and online consultations\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the hospital and prescriptions warning on the page explains the service is from Patients Know Best$")
    fun assertHospitalAndPrescriptionsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Hospital and other prescriptions\nThis service is provided by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the shared links warning on the page explains the service is from Patients Know Best$")
    fun assertSharedLinksWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Shared links\nThis service is provided by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the test results warning on the page explains the service is from Patients Know Best$")
    fun assertTestResultsMessageContent() {
        redirector.interruptionCard.assertContent(
            "Test results\nThis service is provided by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the view appointments warning on the page explains the service is from Patients Know Best$")
    fun assertViewAppointmentsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "View appointments\nThis service is provided by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the care plans warning message on the Redirector page explains the service is from Patients Know Best$")
    fun assertCarePlansWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Care plans\nThis service is provided by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the track your health warning message on the page explains the service is from Patients Know Best$")
    fun assertTrackYourHealthWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Track your health\nThis service is provided by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("^the link to PKB View Appointments is not available on the Appointments page$")
    fun theLinkToPkbViewAppointmentsIsNotAvailableOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.assertElementNotPresent()
    }

    @Then("^I can see the PKB View Appointments link on the Appointments page$")
    fun iCanSeeThePkbViewAppointmentsLinkOnTheAppointmentsPage(){
        hospitalAppointmentsPage.assertPkbViewAppointmentsIsDisplayed()
    }

    @Then("^the PKB View Medicines link is available on the Prescriptions Hub$")
    fun thePKBViewMedicinesLinkIsAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbMedicinesJumpOffButton.assertIsVisible()
    }

    @Then("^the PKB View Medicines link is not available on the Prescriptions Hub$")
    fun thePKBViewMedicinesLinkIsNotAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbMedicinesJumpOffButton.assertElementNotPresent()
    }

    @Then("^I click the PKB View Medicines link on the Prescriptions hub$")
    fun iClickThePKBViewMedicinesLink(){
        prescriptionsHubPage.pkbMedicinesJumpOffButton.click()
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        setupJourney(patient)
    }

    private fun setupPatient(configuration: ArrayList<SJRJourneyType>, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        setupJourney(patient)
    }

    private fun setupJourney(patient: Patient) {
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
