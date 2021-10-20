package features.silverIntegration.careInformationExchange.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.pages.CareInformationExchangePage
import mocking.thirdPartyProviders.pkb.CIERequestBuilder
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
    private lateinit var careInformationExchangePage: CareInformationExchangePage

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

    @Given("^I am a user who can view test results and imaging from Care Information Exchange$")
    fun iAmAUserWhoCanViewTestResultsAndImagingFromCareInformationExchange() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_CIE)
    }

    @Given("^I am a user who cannot view test results and imaging from Care Information Exchange$")
    fun iAmAUserWhoCannotViewTestResultsAndImagingFromCareInformationExchange() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_NONE)
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

    @Given("^I am a user who can view Record Sharing from Care Information Exchange$")
    fun iAmAUserWhoCanViewRecordSharingFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_CIE)
    }

    @Given("^I am a user who cannot view Record Sharing from Care Information Exchange$")
    fun iAmAUserWhoCannotViewRecordSharingFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_NONE)
    }

    @Given("^I am a user who can view Shared Health Links from Care Information Exchange$")
    fun iAmAUserWhoCanViewSharedHealthLinksFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_CIE)
    }

    @Given("^I am a user who cannot view Shared Health Links from Care Information Exchange$")
    fun iAmAUserWhoCannotViewSharedHealthLinksFromCareInformationExchange(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_NONE)
    }

    @Given("^CIE responds to requests for appointments$")
    fun cieRespondsToRequestsForAppointments() {
        MockingClient.instance.forCIE.mock { CIERequestBuilder().appointmentsRequest().respondWithPage() }
    }

    @Given("^CIE responds to requests for care plans$")
    fun cieRespondsToRequestsForCarePlans() {
        MockingClient.instance.forCIE.mock { CIERequestBuilder().carePlanRequest().respondWithPage() }
    }

    @Given("^CIE responds to requests for health tracker$")
    fun cieRespondsToRequestsForHealthTracker() {
        MockingClient.instance.forCIE.mock { CIERequestBuilder().healthTrackerRequest().respondWithPage() }
    }

    @Given("^CIE responds to requests for medicines$")
    fun cieRespondsToRequestsForMedicines() {
        MockingClient.instance.forCIE.mock { CIERequestBuilder().medicinesRequest().respondWithPage() }
    }

    @Given("^CIE responds to requests for messages$")
    fun cieRespondsToRequestsForMessages() {
        MockingClient.instance.forCIE.mock { CIERequestBuilder().messagesRequest().respondWithPage() }
    }

    @Given("^CIE responds to requests for record sharing$")
    fun cieRespondsToRequestsForRecordSharing() {
        MockingClient.instance.forCIE.mock { CIERequestBuilder().recordSharingRequest().respondWithPage() }
    }

    @Given("^CIE responds to requests for shared links$")
    fun cieRespondsToRequestsForSharedLinks() {
        MockingClient.instance.forCIE.mock { CIERequestBuilder().sharedLinksRequest().respondWithPage() }
    }

    @Given("^CIE responds to requests for test results and imaging$")
    fun cieRespondsToRequestsForTestResultsAndImaging() {
        MockingClient.instance.forCIE.mock { CIERequestBuilder().testResultsAndImagingRequest().respondWithPage() }
    }

    @Then("^the link to CIE Track your health is not available on the health record hub page$")
    fun theLinkToCieHealthTrackerIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Track your health").assertElementNotPresent()
    }

    @Then("^the link to CIE record sharing is not available on the health record hub page$")
    fun theLinkToCieRecordSharingIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Record Sharing").assertElementNotPresent()
    }

    @Then("^the link to CIE shared health links is not available on the health record hub page$")
    fun theLinkToCieSharedLinksIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Shared health links").assertElementNotPresent()
    }

    @Then("^the link to CIE test results and imaging is not available on the health record hub page$")
    fun theLinkToCieTestResultsAndImagingIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Test results and imaging").assertElementNotPresent()
    }

    @Then("^the link to CIE Care plans is not available on the health record hub page$")
    fun theLinkToCieCarePlansIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Care plans").assertElementNotPresent()
    }

    @When("^I click the CIE View Appointments link on the Appointments page")
    fun iClickTheCieViewAppointmentsLinkOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnCieAppointments.click()
    }

    @Then("the hospital and other warning explains the service is from Care Information Exchange$")
    fun assertHospitalAndOtherMedicinesWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Hospital and other medicines\n" +
                        "This service is provided by Care Information Exchange powered by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the track your health warning explains the service is from Care Information Exchange$")
    fun assertTrackYourHealthWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Track your health\nThis service is provided by Care Information Exchange powered by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the view appointments warning on the page explains the service is from Care Information Exchange$")
    fun assertViewAppointmentsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "View appointments\nThis service is provided by Care Information Exchange powered by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the test results and imaging warning on the page explains the service is from Care Information Exchange$")
    fun assertTestResultsAndImagingWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Test results and imaging\nThis service is provided by Care Information Exchange powered by " +
                    "Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the record sharing warning on the page explains the service is from Care Information Exchange$")
    fun assertRecordSharingWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Record Sharing\nThis service is provided by Care Information Exchange powered by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the shared health warning on the page explains the service is from Care Information Exchange$")
    fun assertVSharedHealthWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Shared health links\nThis service is provided by Care Information Exchange powered by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the consultations warning on the page explains the service is from Care Information Exchange$")
    fun assertConsultationsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Consultations, events and messages\n" +
                    "This service is provided by Care Information Exchange powered by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the care plan warning on the page explains the service is from Care Information Exchange$")
    fun assertCarePlanWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Care plans\nThis service is provided by Care Information Exchange powered by Patients Know Best",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
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

    @Then("^I am navigated to a third party site for CIE$")
    fun iNavigateToThirdPartySiteForCIE() {
        careInformationExchangePage.assertTitleVisible()
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
