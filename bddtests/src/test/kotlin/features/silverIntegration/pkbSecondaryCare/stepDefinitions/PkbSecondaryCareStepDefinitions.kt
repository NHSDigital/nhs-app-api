package features.silverIntegration.pkbSecondaryCare.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.pages.SecondaryCarePage
import mocking.thirdPartyProviders.pkb.SecondaryCareRequestBuilder
import models.IdentityProofingLevel
import pages.HybridPageObject
import pages.PrescriptionsHubPage
import pages.RedirectorPage
import pages.appointments.HospitalAppointmentsPage
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class PkbSecondaryCareStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var prescriptionsHubPage: PrescriptionsHubPage
    private lateinit var secondaryCarePage: SecondaryCarePage
    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage

    @Given("^I am a user who can view Medicines from PKB Secondary Care$")
    fun iAmAUserWhoCanViewMedicinesFromPkbSecondaryCare() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MEDICINES_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Medicines from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewMedicinesFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE)
    }

    @Given("^I am a user who can view Appointments from PKB Secondary Care$")
    fun iAmAUserWhoCanViewAppointmentsFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_APPOINTMENTS_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Appointments from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewAppointmentsFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
    }

    @Given("^I am a user who can view Record Sharing from PKB Secondary Care$")
    fun iAmAUserWhoCanViewRecordSharingFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Record Sharing from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewRecordSharingFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_NONE)
    }

    @Given("^I am a user who can view Shared Links from PKB Secondary Care$")
    fun iAmAUserWhoCanViewSharedLinksFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Shared Links from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewSharedLinksFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_NONE)
    }

    @Given("^I am a user who can view Health Tracker from PKB Secondary Care$")
    fun iAmAUserWhoCanViewHealthTrackerFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Health Tracker from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewHealthTrackerFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_NONE)
    }

    @Given("^I am a user who can view Messages and Online Consultations from PKB Secondary Care$")
    fun iAmAUserWhoCanViewMessagesAndOnlineConsultationsFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Messages and Online Consultations from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewMessagesAndOnlineConsultationsFromPkbSecondaryCare() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @Given("^I am a user who can view test results and imaging from PKB Secondary Care$")
    fun iAmAUserWhoCanViewTestResultsAndImagingFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view test results and imaging from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewTestResultsAndImagingFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_NONE)
    }

    @Given("^I am a user who can view care plans from PKB Secondary Care$")
    fun iAmAUserWhoCanViewCarePlansFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view care plans from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewCarePlansFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE)
    }

    @Given("^Secondary Care responds to requests for appointments$")
    fun secondaryCareRespondsToRequestsForAppointments() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().appointmentRequest().respondWithPage()
        }
    }

    @Given("^Secondary Care responds to requests for messages$")
    fun secondaryCareRespondsToRequestsForMessages() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().messagesRequest().respondWithPage()
        }
    }

    @Given("^Secondary Care responds to requests for medicines$")
    fun secondaryCareRespondsToRequestsForMedicines() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().medicineRequest().respondWithPage()
        }
    }

    @Given("^Secondary Care responds to requests for test results and imaging$")
    fun secondaryCareRespondsToRequestsForTestResultsAndImaging() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().testResultsAndImagingRequest().respondWithPage()
        }
    }

    @Given("^Secondary Care responds to requests for care plans$")
    fun secondaryCareRespondsToRequestsForCarePlans() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().carePlanRequest().respondWithPage()
        }
    }

    @Given("^I am a user with proof level 5 who can view" +
            " Messages and Online Consultations from PKB Secondary Care$")
    fun iAmAUserWithProofLevel5WhoCanViewMessagesAndOnlineConsultationsFromPkbSecondaryCare(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB_SECONDARY_CARE, IdentityProofingLevel.P5)
    }

    @Given("^Secondary Care responds to requests for record sharing$")
    fun secondaryCareRespondsToRequestsForRecordSharing() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().recordSharingRequest().respondWithPage() }
    }

    @Given("^Secondary Care responds to requests for shared links$")
    fun secondaryCareRespondsToRequestsForSharedLinks() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().sharedLinksRequest().respondWithPage() }
    }

    @Given("^Secondary Care responds to requests for health tracker$")
    fun secondaryCareRespondsToRequestsForHealthTracker() {
        MockingClient.instance.forSecondaryCare.mock {
            SecondaryCareRequestBuilder().healthTrackerRequest().respondWithPage() }
    }

    @Then("^the link to PKB Secondary Care record sharing is not available on the Health Records Hub$")
    fun thePKBSecondaryCareRecordSharingLinkIsNotAvailableOnTheHealthRecordsHub() {
        medicalRecordHubPage.getHeaderElement("Record Sharing").assertElementNotPresent()
    }

    @Then("^the link to PKB Secondary Care shared links is not available on the Health Records Hub$")
    fun thePKBSecondaryCareSharedLinksLinkIsNotAvailableOnTheHealthRecordsHub() {
        medicalRecordHubPage.getHeaderElement("Shared health links").assertElementNotPresent()
    }

    @Then("^the link to PKB Secondary Care test results and imaging is not available on the Health Records Hub$")
    fun thePKBSecondaryCareTestResultsAndImagingLinkIsNotAvailableOnTheHealthRecordsHub() {
        medicalRecordHubPage.getHeaderElement("Test results and imaging").assertElementNotPresent()
    }

    @Then("^the link to PKB Secondary Care Care plans is not available on the Health Records Hub$")
    fun thePKBSecondaryCareCarePlansLinkIsNotAvailableOnTheHealthRecordsHub() {
        medicalRecordHubPage.getHeaderElement("Care plans").assertElementNotPresent()
    }

     @Then("^the link to PKB Secondary Care Track Your Health is not available on the Health Record Hub$")
    fun thePKBSecondaryCareTrackYourHealthLinkIsNotAvailableOnTheHealthRecordsHub() {
        medicalRecordHubPage.getHeaderElement("Track your health").assertElementNotPresent()
    }

    @Then("^the PKB Secondary Care View Medicines link is available on the Prescriptions Hub$")
    fun thePKBSecondaryCareViewMedicinesLinkIsAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.assertIsVisible()
    }

    @Then("^the PKB Secondary Care View Medicines link is not available on the Prescriptions Hub$")
    fun theCieViewMedicinesLinkIsNotAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.assertElementNotPresent()
    }

    @Then("^I click the PKB Secondary Care View Medicines link on the Prescriptions hub$")
    fun iClickThePkbSecondaryCareViewMedicinesLink(){
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.click()
    }

    @Then("^I can see the PKB Secondary Care View Appointments link on the Appointments page$")
    fun iCanSeeThePkbViewAppointmentsLinkOnTheAppointmentsPage(){
        hospitalAppointmentsPage.assertPkbViewAppointmentsIsDisplayed()
    }

    @When("^I click the PKB Secondary Care View Appointments link on the Appointments page")
    fun iClickThePkbViewAppointmentsLinkOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.click()
    }

    @Then("the view appointments warning on the page explains the service is from PKB Secondary Care$")
    fun assertViewAppointmentsWarningMessageContentForPkbSecondaryCare() {
        redirector.interruptionCard.assertContent(
                "View appointments\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the consultations warning on the page explains the service is from PKB Secondary Care$")
    fun assertConsultationsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Consultations, events and messages\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the hospital and medicines warning on the page explains the service is from PKB Secondary Care$")
    fun assertHospitalAndMedicinesWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Hospital and other medicines\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the record sharing warning on the page explains the service is from PKB Secondary Care$")
    fun assertRecordSharingWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Record Sharing\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the Track your Health warning on the page explains the service is from PKB Secondary Care$")
    fun assertHealthTrackerWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Track your health\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the shared health links warning on the page explains the service is from PKB Secondary Care$")
    fun assertSharedLinksWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Shared health links\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the test results and imaging warning on the page explains the service is from PKB Secondary Care$")
    fun assertTestResultsAndImagingWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Test results and imaging\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the care plan warning on the page explains the service is from PKB Secondary Care$")
    fun assertCarePlansWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Care plans\nThis service is provided by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("^the link to PKB Secondary Care View Appointments is not available on the Appointments page$")
    fun theLinkToPkbViewAppointmentsIsNotAvailableOnTheAppointmentsPage() {
        hospitalAppointmentsPage.btnPkbAppointments.assertElementNotPresent()
    }

    @Then("^I am navigated to a third party site for Secondary Care")
    fun iNavigateToThirdPartySiteForSecondaryCare() {
        secondaryCarePage.assertTitleVisible()
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
