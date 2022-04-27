package features.silverIntegration.accuRx.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.pages.AccuRxPage
import mocking.thirdPartyProviders.accuRx.AccuRxRequestBuilder
import models.IdentityProofingLevel
import pages.HealthAdvicePage
import pages.HybridPageObject
import pages.RedirectorPage
import pages.assertIsVisible
import pages.assertElementNotPresent
import utils.SerenityHelpers

class AccuRxStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var healthAdvicePage: HealthAdvicePage
    private lateinit var accuRxPage: AccuRxPage

    @Given("^I am a user who can view Medical Advice from accuRx$")
    fun iAmAUserWhoCanViewMedicalAdviceFromAccuRx() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ACCURX)
    }

    @Given("^I am a user who cannot view Medical Advice from accuRx$")
    fun iAmAUserWhoCannotMedicalAdviceFromAccuRx(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE)
    }

    @Given("^accuRx responds to requests for Medical Advice")
    fun accuRxRespondsToRequestsForMedicalAdvice() {
        MockingClient.instance.forAccuRx.mock { AccuRxRequestBuilder().medicalAdviceRequest().respondWithPage() }
    }

    @Then("^the link to accuRx Medical Advice is available on the Advice page$")
    fun theLinkToAccuRxMedicalAdviceIsAvailableOnTheAdvicePage() {
        healthAdvicePage.accuRxMedicalAdvice.assertIsVisible()
    }

    @Then("^the link to accuRx Medical Advice is not available on the Advice page$")
    fun theLinkToAccuRxMedicalAdviceIsNotAvailableOnTheAdvicePage() {
        healthAdvicePage.accuRxMedicalAdvice.assertElementNotPresent()
    }

    @When("^I click the accuRx Medical Advice link on the Advice page")
    fun iClickTheAccuRxMedicalAdviceLinkOnTheAdvicePage() {
        healthAdvicePage.accuRxMedicalAdvice.click()
    }

    @Then("the Medical Advice warning message on the Redirector page explains the service is from accuRx$")
    fun assertAdviceWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Ask your GP for advice\nThis service is provided by accuRx Limited",
            "Your GP surgery has chosen this online consultation service provider.",
            "Find out more about online consultation services")
    }

    @Then("^I am navigated to a third party site for accuRx$")
    fun iNavigateToThirdPartySiteForAccuRx() {
        accuRxPage.assertTitleVisible()
    }

    @Given("^AccuRx responds to requests for messages$")
    fun accurxRespondsToRequestsForMessages() {
        MockingClient.instance.forAccuRx.mock { AccuRxRequestBuilder().messagesRequest().respondWithPage() }
    }

    @Given("^I am a user with proof level 5 who can view Ask Your Gp Surgery a Question from AccuRx$")
    fun iAmAUserWithProofLevel5WhoCanViewAskYourGpSurgeryAQuestionFromAccuRxt(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ACCURX, IdentityProofingLevel.P5)
    }

    @Given("^I am a user who can view Ask Your Gp Surgery a Question from AccuRx$")
    fun iAmAUserWhoCanViewAskYourGpSurgeryAQuestionFromAccuRx(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ACCURX)
    }

    @Given("^I am a user who cannot view Ask Your Gp Surgery a Question from AccuRx$")
    fun iAmAUserWhoCannotViewAskYourGpSurgeryAQuestionFromAccuRx() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }


    @Then("the question warning message on the Redirector page explains the service is from AccuRx$")
    fun assertQuestionWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Ask your GP surgery a question\nThis service is provided by accuRx Limited",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
