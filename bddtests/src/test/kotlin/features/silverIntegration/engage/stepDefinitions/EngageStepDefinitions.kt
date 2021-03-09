package features.silverIntegration.engage.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.pages.EngagePage
import mocking.thirdPartyProviders.engage.EngageRequestBuilder
import models.IdentityProofingLevel
import pages.HealthAdvicePage
import pages.HybridPageObject
import pages.RedirectorPage
import pages.assertIsVisible
import utils.SerenityHelpers

class EngageStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var healthAdvicePage: HealthAdvicePage
    private lateinit var engagePage: EngagePage

    @Given("^I am a user who can view Medical Advice from Engage$")
    fun iAmAUserWhoCanViewMedicalAdviceFromEngage() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ENGAGE)
    }

    @Given("^I am a user who cannot view Medical Advice from Engage$")
    fun iAmAUserWhoCannotViewMedicalAdviceFromEngage(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE)
    }

    @Given("^I am a user who can view Admin from Engage$")
    fun iAmAUserWhoCanViewAdminFromEngage() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ADMIN_ENGAGE)
    }

    @Given("^I am a user who cannot view Admin from Engage$")
    fun iAmAUserWhoCannotViewAdminFromEngage(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ADMIN_NONE)
    }

    @Given("^Engage responds to requests for admin$")
    fun engageRespondsToRequestsForAdmin() {
        MockingClient.instance.forEngage.mock { EngageRequestBuilder().adminRequest().respondWithPage() }
    }

    @Given("^Engage responds to requests for medical advice")
    fun engageRespondsToRequestsForMedicalAdvice() {
        MockingClient.instance.forEngage.mock { EngageRequestBuilder().medicalAdviceRequest().respondWithPage() }
    }

    @Given("^Engage responds to requests for messages")
    fun engageRespondsToRequestsForMessages() {
        MockingClient.instance.forEngage.mock { EngageRequestBuilder().messagesRequest().respondWithPage() }
    }

    @Then("^the link to Engage Medical Advice is available on the Advice page$")
    fun theLinkToMedicalAdviceIsAvailableOnTheAdvicePage() {
        healthAdvicePage.engageMedicalAdvice.assertIsVisible()
    }

    @When("^I click the Engage Medical Advice link on the Advice page")
    fun iClickTheMedicalAdviceLinkOnTheAdvicePage() {
        healthAdvicePage.engageMedicalAdvice.click()
    }

    @Then("the advice warning message on the Redirector page explains the service is from Engage$")
    fun assertAdviceWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Ask your GP for advice\nThis service is provided by Engage Health Systems Limited",
            "Your GP surgery has chosen this online consultation service provider.",
            "Find out more about online consultation services")
    }

    @Then("the messages warning message on the Redirector page explains the service is from Engage$")
    fun assertMessagesWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Messages\nThis service is provided by Engage Health Systems Limited",
            "Your GP surgery has chosen this online consultation service provider.",
            "Find out more about online consultation services")
    }

    @Then("the additional services warning message on the Redirector page explains the service is from Engage$")
    fun assertAdditionalServicesWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Additional GP services\nThis service is provided by Engage Health Systems Limited",
            "Your GP surgery has chosen this online consultation service provider.",
            "Find out more about online consultation services")
    }

    @Given("^I am a user who can view Messages and Online Consultations from Engage$")
    fun iAmAUserWhoCanViewMessagesAndOnlineConsultationsFromEngage(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ENGAGE)
    }

    @Given("^I am a user with proof level 5 who can view Messages and Online Consultations from Engage$")
    fun iAmAUserWithProofLevel5WhoCanViewMessagesAndOnlineConsultationsFromEngage(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ENGAGE, IdentityProofingLevel.P5)
    }

    @Given("^I am a user who cannot view Messages and Online Consultations from Engage$")
    fun iAmAUserWhoCannotViewMessagesAndOnlineConsultationsFromEngage() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }
    @Then("^I am navigated to a third party site for Engage$")
    fun iNavigateToThirdPartySiteForEngage() {
        engagePage.assertTitleVisible()
    }


    private fun setupPatient(configuration: SJRJourneyType, proofLevel: IdentityProofingLevel? = null) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, configuration, proofLevel)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
