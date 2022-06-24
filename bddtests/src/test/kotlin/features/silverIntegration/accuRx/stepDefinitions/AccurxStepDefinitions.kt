package features.silverIntegration.accurx.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.pages.AccurxPage
import mocking.thirdPartyProviders.accurx.AccurxRequestBuilder
import models.IdentityProofingLevel
import pages.HealthAdvicePage
import pages.HybridPageObject
import pages.RedirectorPage
import pages.assertIsVisible
import pages.assertElementNotPresent
import utils.SerenityHelpers

class AccurxStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var healthAdvicePage: HealthAdvicePage
    private lateinit var accurxPage: AccurxPage

    @Given("^I am a user who can view Medical Advice from Accurx$")
    fun iAmAUserWhoCanViewMedicalAdviceFromAccurx() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ACCURX)
    }

    @Given("^I am a user who cannot view Medical Advice from Accurx$")
    fun iAmAUserWhoCannotMedicalAdviceFromAccurx(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE)
    }

    @Given("^Accurx responds to requests for Medical Advice")
    fun accurxRespondsToRequestsForMedicalAdvice() {
        MockingClient.instance.forAccurx.mock { AccurxRequestBuilder().medicalAdviceRequest().respondWithPage() }
    }

    @Then("^the link to Accurx Medical Advice is available on the Advice page$")
    fun theLinkToAccurxMedicalAdviceIsAvailableOnTheAdvicePage() {
        healthAdvicePage.accurxMedicalAdvice.assertIsVisible()
    }

    @Then("^the link to Accurx Medical Advice is not available on the Advice page$")
    fun theLinkToAccurxMedicalAdviceIsNotAvailableOnTheAdvicePage() {
        healthAdvicePage.accurxMedicalAdvice.assertElementNotPresent()
    }

    @When("^I click the Accurx Medical Advice link on the Advice page")
    fun iClickTheAccurxMedicalAdviceLinkOnTheAdvicePage() {
        healthAdvicePage.accurxMedicalAdvice.click()
    }

    @Then("the Medical Advice warning message on the Redirector page explains the service is from Accurx$")
    fun assertAdviceWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Ask your GP for medical advice\nThis service is provided by Accurx Limited",
            "Your GP surgery has chosen this online consultation service provider.",
            "Find out more about online consultation services")
    }

    @Then("^I am navigated to a third party site for Accurx$")
    fun iNavigateToThirdPartySiteForAccurx() {
        accurxPage.assertTitleVisible()
    }

    @Given("^Accurx responds to requests for messages$")
    fun accurxRespondsToRequestsForMessages() {
        MockingClient.instance.forAccurx.mock { AccurxRequestBuilder().messagesRequest().respondWithPage() }
    }

    @Given("^I am a user with proof level 5 who can view Ask Your Gp Surgery a Question from Accurx$")
    fun iAmAUserWithProofLevel5WhoCanViewAskYourGpSurgeryAQuestionFromAccurx(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ACCURX, IdentityProofingLevel.P5)
    }

    @Given("^I am a user who can view Ask Your Gp Surgery a Question from Accurx$")
    fun iAmAUserWhoCanViewAskYourGpSurgeryAQuestionFromAccurx(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ACCURX)
    }

    @Given("^I am a user who cannot view Ask Your Gp Surgery a Question from Accurx$")
    fun iAmAUserWhoCannotViewAskYourGpSurgeryAQuestionFromAccurx() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @Given("^I am a patient with access to all Accurx services$")
    fun initialiseAccurxPatientWithUnableGpSystem() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null,
            arrayListOf(
                SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ACCURX,
                SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ACCURX))
        SerenityHelpers.setPatient(patient)
    }

    @Then("the question warning message on the Redirector page explains the service is from Accurx$")
    fun assertQuestionWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Ask your GP surgery a question\nThis service is provided by Accurx Limited",
            "Your GP surgery has chosen this online consultation service provider.",
            "Find out more about online consultation services")
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
