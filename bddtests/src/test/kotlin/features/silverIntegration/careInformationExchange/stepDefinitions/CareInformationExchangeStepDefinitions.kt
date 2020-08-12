package features.silverIntegration.careInformationExchange.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import pages.HybridPageObject
import pages.PrescriptionsHubPage
import pages.RedirectorPage
import pages.appointments.HospitalAppointmentsPage
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class CareInformationExchangeStepDefinitions : HybridPageObject() {
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private lateinit var redirector: RedirectorPage
    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage
    private lateinit var prescriptionsHubPage: PrescriptionsHubPage

    @Given("^I am a user who can view Appointments from Care Information Exchange$")
    fun iAmAUserWhoCanViewAppointmentsFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_CIE)
    }

    @Given("^I am a user who cannot view Appointments from Care Information Exchange$")
    fun iAmAUserWhoCannotViewAppointmentsFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
    }

    @Given("^I am a user who can view Medicines from Care Information Exchange$")
    fun iAmAUserWhoCanViewMedicinesFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_CIE)
    }

    @Given("^I am a user who cannot view Medicines from Care Information Exchange$")
    fun iAmAUserWhoCannotViewMedicinesFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE)
    }

    @Given("^I am a user who can view care plans from Care Information Exchange$")
    fun iAmAUserWhoCanViewCarePlansFromCareInformationExchange() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_CIE)
    }

    @Given("^I am a user who cannot view care plans from Care Information Exchange$")
    fun iAmAUserWhoCannotViewCarePlansFromCareInformationExchange() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE)
    }

    @Given("^I am a user who can view health tracker from Care Information Exchange$")
    fun iAmAUserWhoCanViewHealthTrackerFromCareInformationExchange() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_CIE)
    }

    @Given("^I am a user who cannot view health tracker from Care Information Exchange$")
    fun iAmAUserWhoCannotViewHealthTrackerFromCareInformationExchange() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_NONE)
    }

    @Given("^I am a user who can view Messages and Online Consultations from Care Information Exchange$")
    fun iAmAUserWhoCanViewMessagesAndOnlineConsultationsFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_CIE)
    }

    @Given("^I am a user with proof level 5 who can view" +
            " Messages and Online Consultations from Care Information Exchange$")
    fun iAmAUserWithProofLevel5WhoCanViewMessagesAndOnlineConsultationsFromPatientsKnowBest(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_CIE, IdentityProofingLevel.P5)
    }

    @Given("^I am a user who cannot view Messages and Online Consultations from Care Information Exchange$")
    fun iAmAUserWhoCannotViewMessagesAndOnlineConsultationsFromCareInformationExchange() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @Given("^I am a user who can view Shared Links from Care Information Exchange$")
    fun iAmAUserWhoCanViewSharedLinksFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_CIE)
    }

    @Given("^I am a user who cannot view Shared Links from Care Information Exchange$")
    fun iAmAUserWhoCannotViewSharedLinksFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_NONE)
    }

    @Then("^the link to CIE Track your health is not available on the health record hub page$")
    fun theLinkToCieHealthTrackerIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Track your health").assertElementNotPresent()
    }

    @Then("^the link to CIE Care plans is not available on the health record hub page$")
    fun theLinkToCieCarePlansIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Care plans").assertElementNotPresent()
    }

    @When("^I click the CIE View Appointments link on the Appointments page")
    fun iClickTheCieViewAppointmentsLinkOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnCieAppointments.click()
    }

    @Then("the warning message on the Redirector page explains the service is from Care Information Exchange$")
    fun assertWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "This is a connected service",
                "Your GP surgery or hospital has chosen this personal health record " +
                        "service provided by Care Information Exchange (Patients Know Best).",
                "Find out more about personal health record services.")
    }

    @Then("^the link to CIE View Appointments is not available on the Appointments page$")
    fun theLinkToCieViewAppointmentsIsNotAvailableOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnCieAppointments.assertElementNotPresent()
    }

    @Then("^I can see the CIE View Appointments link on the Appointments page$")
    fun iCanSeeTheCieViewAppointmentsLinkOnTheAppointmentsPage(){
        hospitalAppointmentsPage.assertCieViewAppointmentsIsDisplayed()
    }

    @Then("^the CIE View Medicines link is available on the Prescriptions Hub$")
    fun theCieViewMedicinesLinkIsAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.cieMedicinesJumpOffButton.assertIsVisible()
    }

    @Then("^the CIE View Medicines link is not available on the Prescriptions Hub$")
    fun theCieViewMedicinesLinkIsNotAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.cieMedicinesJumpOffButton.assertElementNotPresent()
    }

    @Then("^I click the CIE View Medicines link on the Prescriptions hub$")
    fun iClickTheCieViewMedicinesLink(){
        prescriptionsHubPage.cieMedicinesJumpOffButton.click()
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
