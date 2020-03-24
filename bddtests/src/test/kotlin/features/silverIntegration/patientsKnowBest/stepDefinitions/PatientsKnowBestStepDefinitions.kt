package features.silverIntegration.patientsKnowBest.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import pages.HybridPageObject
import pages.RedirectorPage
import utils.SerenityHelpers

class PatientsKnowBestStepDefinitions : HybridPageObject() {

    private val mockingClient = MockingClient.instance
    private lateinit var redirector: RedirectorPage

    @Given("^I am a user who can view their Messages and Online Consultations from Patients Know Best$")
    fun iAmAUserWhoCanViewTheirMessagesAndOnlineConsultationsFromPatientsKnowBest(){
        setupPatient( ServiceJourneyRulesMapper.Companion.JourneyType.SILVER_INTEGRATION_MESSAGES_PKB)
    }

    @Given("^I am a user who cannot view Messages and Online Consultations from Patients Know Best$")
    fun iAmAUserWhoCannotViewMessagesAndOnlineConsultationsFromPatientsKnowBest(){
        setupPatient( ServiceJourneyRulesMapper.Companion.JourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @When("^I click the 'Continue' button on the redirector page with a url starting with '(.*)'$")
    fun iClickTheContinueButtonOnTheRedirectorPageWithAUrlOf(continueUrl: String) {
        redirector.interruptionCard.assertContinueAndClick(continueUrl)
    }

    @Then("I am redirected to the redirector page with the header '(.*)'$")
    fun assertRedirectorPageIsVisible(header: String) {
        redirector.title(header).waitForElement()
    }

    @Then("the warning message on the Redirector page explains the service is from Patients Know Best$")
    fun assertWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "This is a connected service",
                "Your GP surgery or hospital has chosen this personal health record " +
                        "service provided by Patients Know Best.",
                "Find out more about personal health record services.")
    }

    private fun setupPatient(configuration: ServiceJourneyRulesMapper.Companion.JourneyType) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                null, configuration)
        val supplier = SerenityHelpers.getGpSupplier()
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
    }
}
