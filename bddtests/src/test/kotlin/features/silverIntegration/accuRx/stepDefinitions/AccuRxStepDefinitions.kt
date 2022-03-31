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

    @Given("^I am a user who can view Triage Advice from accuRx$")
    fun iAmAUserWhoCanViewTriageAdviceFromAccuRx() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ACCURX)
    }

    @Given("^I am a user who cannot view Triage Advice from accuRx$")
    fun iAmAUserWhoCannotTriageAdviceFromAccuRx(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE)
    }

    @Given("^accuRx responds to requests for Triage Advice")
    fun accuRxRespondsToRequestsForTriageAdvice() {
        MockingClient.instance.forAccuRx.mock { AccuRxRequestBuilder().accuRxTriageAdviceRequest().respondWithPage() }
    }

    @Then("^the link to accuRx Triage Advice is available on the Advice page$")
    fun theLinkToAccuRxTriageAdviceIsAvailableOnTheAdvicePage() {
        healthAdvicePage.accuRxTriageAdvice.assertIsVisible()
    }

    @Then("^the link to accuRx Triage Advice is not available on the Advice page$")
    fun theLinkToAccuRxTriageAdviceIsNotAvailableOnTheAdvicePage() {
        healthAdvicePage.accuRxTriageAdvice.assertElementNotPresent()
    }

    @When("^I click the accuRx Triage Advice link on the Advice page")
    fun iClickTheAccuRxTriageAdviceLinkOnTheAdvicePage() {
        healthAdvicePage.accuRxTriageAdvice.click()
    }

    @Then("the Triage Advice warning message on the Redirector page explains the service is from accuRx$")
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

    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
