package features.silverIntegration.patchs.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.pages.PatchsPage
import mocking.thirdPartyProviders.patchs.PatchsRequestBuilder
import models.IdentityProofingLevel
import pages.HealthAdvicePage
import pages.HybridPageObject
import pages.MessagesHubPage
import pages.RedirectorPage
import pages.assertIsVisible
import pages.assertElementNotPresent
import utils.SerenityHelpers

class PatchsStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var healthAdvicePage: HealthAdvicePage
    private lateinit var messagesHubPage: MessagesHubPage
    private lateinit var patchsPage: PatchsPage

    @Given("^I am a user who can view Medical Advice from Patchs$")
    fun iAmAUserWhoCanViewMedicalAdviceFromPatchs() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_PATCHS)
    }

    @Given("^I am a user who cannot view Medical Advice from Patchs$")
    fun iAmAUserWhoCannotMedicalAdviceFromPatchs(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE)
    }

    @Given("^I am a user with proof level 5 who can view Fit Notes Request from Patchs$")
    fun iAmAUserWithProofLevel5WhoCanViewFitNotesRequestFromPatchs(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ADMIN_PATCHS, IdentityProofingLevel.P5)
    }

    @Given("^I am a user who can view Fit Notes Request from Patchs$")
    fun iAmAUserWhoCanViewFitNotesRequestFromPatchs(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ADMIN_PATCHS)
    }

    @Given("^I am a user who cannot view Fit Notes Request from Patchs$")
    fun iAmAUserWhoCannotViewFitNotesRequestFromPatchs(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ADMIN_NONE)
    }

    @Given("^Patchs responds to requests for Medical Advice")
    fun patchsRespondsToRequestsForMedicalAdvice() {
        MockingClient.instance.forPatchs.mock { PatchsRequestBuilder().patchsMedicalRequest().respondWithPage() }
    }

    @Given("^Patchs responds to requests for Fit Notes Request")
    fun patchsRespondsToRequestsForFitNotestRequest() {
        MockingClient.instance.forPatchs.mock { PatchsRequestBuilder().patchsAdminFitNotesRequest().respondWithPage() }
    }

    @Then("^the link to Patchs Medical Advice is available on the Advice page$")
    fun theLinkToPatchsMedicalAdviceIsAvailableOnTheAdvicePage() {
        healthAdvicePage.patchsMedicalAdvice.assertIsVisible()
    }

    @Then("^the link to Patchs Medical Advice is not available on the Advice page$")
    fun theLinkToPatchsMedicalAdviceIsNotAvailableOnTheAdvicePage() {
        healthAdvicePage.patchsMedicalAdvice.assertElementNotPresent()
    }

    @When("^I click the Patchs Medical Advice link on the Advice page")
    fun iClickThePatchsMedicalAdviceLinkOnTheAdvicePage() {
        healthAdvicePage.patchsMedicalAdvice.click()
    }

    @When("^I click the Patchs Fit Notes Request link on the Messages Hub page$")
    fun clickOnPatchsFitNoteRequestLinkOnTheMessagesHubPage() {
        messagesHubPage.clickOnMenuItem("btn_patchs_admin")
    }

    @Then("the Fit Notes Request warning message on the Redirector page explains the service is from Patchs$")
    fun assertFitNotesRequestWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Ask for a fit note (sick note) or other documents\nThis service is provided by PATCHS",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the Medical Advice warning message on the Redirector page explains the service is from Patchs$")
    fun assertAdviceWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Ask your GP for advice about a health problem\nThis service is provided by PATCHS",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("^I am navigated to a third party site for Patchs$")
    fun iNavigateToThirdPartySiteForPatchs() {
        patchsPage.assertTitleVisible()
    }

    @Given("^I am a patient with access to all Patchs services$")
    fun initialisePatchsPatientWithUnableGpSystem() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null,
            arrayListOf(
                SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_PATCHS))
        SerenityHelpers.setPatient(patient)
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
