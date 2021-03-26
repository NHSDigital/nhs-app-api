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
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class PkbMyCareViewStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var prescriptionsHubPage: PrescriptionsHubPage
    private lateinit var hospitalAppointmentsPage: HospitalAppointmentsPage
    private lateinit var myCareViewPage: MyCareViewPage
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage

    @Given("^I am a user who can view Medicines from PKB My Care View$")
    fun iAmAUserWhoCanViewMedicinesFromPkbMyCareView() {
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_PKB_MY_CARE_VIEW)
    }

    @Given("^I am a user who cannot view Medicines from PKB My Care View$")
    fun iAmAUserWhoCannotViewMedicinesFromMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE)
    }

    @Given("^I am a user who can view Appointments from PKB My Care View$")
    fun iAmAUserWhoCanViewAppointmentsFromPkbMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_APPOINTMENTS_PKB_MY_CARE_VIEW)
    }

    @Given("^I am a user who cannot view Appointments from PKB My Care View$")
    fun iAmAUserWhoCannotViewAppointmentsFromPkbMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS)
    }

    @Given("^I am a user who can view Record Sharing from PKB My Care View$")
    fun iAmAUserWhoCanViewRecordSharingFromPkbMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_PKB_MY_CARE_VIEW)
    }

    @Given("^I am a user who cannot view Record Sharing from PKB My Care View$")
    fun iAmAUserWhoCannotViewRecordSharingFromPkbMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_NONE)
    }

    @Given("^I am a user who can view Shared Links from PKB My Care View$")
    fun iAmAUserWhoCanViewSharedLinksFromPkbMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB_MY_CARE_VIEW)
    }

    @Given("^I am a user who cannot view Shared Links from PKB My Care View$")
    fun iAmAUserWhoCannotViewSharedLinksFromPkbMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_LIBRARY_NONE)
    }

    @Given("^I am a user who can view Messages and Online Consultations from PKB My Care View$")
    fun iAmAUserWhoCanViewMessagesAndOnlineConsultationsFromPkbMyCareView(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB_MY_CARE_VIEW)
    }

    @Given("^I am a user who cannot view Messages and Online Consultations from PKB My Care View$")
    fun iAmAUserWhoCannotViewMessagesAndOnlineConsultationsFromPkbMyCareView() {
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @Given("^My Care View responds to requests for appointments$")
    fun myCareViewRespondsToRequestsForAppointments() {
        MockingClient.instance.forMyCareView.mock { MyCareViewRequestBuilder().appointmentRequest().respondWithPage() }
    }

    @Given("^My Care View responds to requests for messages$")
    fun myCareViewRespondsToRequestsForMessages() {
        MockingClient.instance.forMyCareView.mock { MyCareViewRequestBuilder().messagesRequest().respondWithPage() }
    }

    @Given("^My Care View responds to requests for medicines$")
    fun myCareViewRespondsToRequestsForMedicines() {
        MockingClient.instance.forMyCareView.mock { MyCareViewRequestBuilder().medicinesRequest().respondWithPage() }
    }

    @Given("^My Care View responds to requests for record sharing$")
    fun myCareViewRespondsToRequestsForRecordSharing() {
        MockingClient.instance.forMyCareView.mock {
            MyCareViewRequestBuilder().recordSharingRequest().respondWithPage()
        }
    }

    @Given("^My Care View responds to requests for shared links$")
    fun myCareViewRespondsToRequestsForSharedLinks() {
        MockingClient.instance.forMyCareView.mock { MyCareViewRequestBuilder().sharedLinksRequest().respondWithPage() }
    }

    @Then("^the link to PKB My Care View record sharing is not available on the Health Records Hub$")
    fun thePKBMyCareViewRecordSharingLinkIsNotAvailableOnTheHealthRecordsHub() {
        medicalRecordHubPage.getHeaderElement("Record Sharing").assertElementNotPresent()
    }

    @Then("^the link to PKB My Care View shared links is not available on the Health Records Hub$")
    fun thePKBMyCareViewSharedLinksLinkIsNotAvailableOnTheHealthRecordsHub() {
        medicalRecordHubPage.getHeaderElement("Shared health links").assertElementNotPresent()
    }

    @Given("^I am a user with proof level 5 who can view" +
            " Messages and Online Consultations from PKB My Care View$")
    fun iAmAUserWithProofLevel5WhoCanViewMessagesAndOnlineConsultationsFromPkbMyCareView(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB_MY_CARE_VIEW, IdentityProofingLevel.P5)
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

    @Then("the view appointments warning on the page explains the service is from PKB My Care View")
    fun assertViewAppointmentsWarningMessageContentForPkbMyCareView() {
        redirector.interruptionCard.assertContent(
                "View appointments\nThis service is provided by MyCareView powered by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the consultations warning on the page explains the service is from PKB My Care View$")
    fun assertConsultationsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Consultations, events and messages\n" +
                        "This service is provided by MyCareView powered by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the hospital and medicines warning on the page explains the service is from PKB My Care View$")
    fun assertHospitalAndMedicinesWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Hospital and other medicines\n" +
                        "This service is provided by MyCareView powered by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the record sharing warning on the page explains the service is from PKB My Care View$")
    fun assertRecordSharingWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Record Sharing\nThis service is provided by MyCareView powered by Patients Know Best",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the shared health links warning on the page explains the service is from PKB My Care View$")
    fun assertSharedLinksWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Shared health links\nThis service is provided by MyCareView powered by Patients Know Best",
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
