package features.silverIntegration.wellnessAndPrevention.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.pages.WellnessAndPreventionPage
import mocking.thirdPartyProviders.wellnessAndPrevention.WellnessAndPreventionRequestBuilder
import models.IdentityProofingLevel
import models.Patient
import pages.HybridPageObject
import pages.RedirectorPage
import pages.assertElementNotPresent
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class WellnessAndPreventionStepDefinitions : HybridPageObject() {

    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private lateinit var redirector: RedirectorPage
    private lateinit var wellnessAndPreventionPage: WellnessAndPreventionPage

    @Given("^I am a user who can view Wellness and Prevention from Health Record Hub$")
    fun iAmAUserWhoCanViewWellnessAndPreventionFromHealthRecordHub(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_WELLNESS_AND_PREVENTION)
    }

    @Given("^I am a user who cannot view Wellness and Prevention from Health Record Hub$")
    fun iAmAUserWhoCannotViewWellnessAndPreventionFromHealthRecordHub(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_NONE)
    }

    @Then("^the link to Wellness and Prevention is not available on the Health Record Hub page$")
    fun theLinkToWellnessAndPreventionIsNotAvailableOnTheHealthRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Wellness and Prevention").assertElementNotPresent()
    }

    @Given("^Wellness and Prevention responds to requests$")
    fun wellnessAndPreventionRequestsForSso() {
        MockingClient.instance.forWellness.mock { WellnessAndPreventionRequestBuilder().respondWithPage() }
    }

    @Given("^Wellness and Prevention responds to privacy requests$")
    fun wellnessAndPreventionRequestsForPrivacy() {
        MockingClient.instance.favicon()
        MockingClient.instance.forExternalSites.mock { wellnessAndPreventionRequest("/privacy")
                .respondWithPage("Wellness and Prevention Privacy") }
    }

    @Then("^the warning on the page explains the service is from Wellness and Prevention$")
    fun assertWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Third Party Feature Name\nThis service is provided by Wellness and Prevention",
                "Service Purchaser has chosen this Service Type provider.",
                "Find out more about Service Type Plural")
    }

    @Then("^I am navigated to a third party site for Wellness and Prevention$")
    fun iNavigateToThirdPartySiteForWellnessAndPrevention() {
        wellnessAndPreventionPage.assertTitleVisible()
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        setupJourney(patient)
    }

    private fun setupJourney(patient: Patient) {
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
