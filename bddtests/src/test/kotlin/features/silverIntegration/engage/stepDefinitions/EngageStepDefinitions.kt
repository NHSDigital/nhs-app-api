package features.silverIntegration.engage.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import pages.HealthAdvicePage
import pages.HybridPageObject
import pages.RedirectorPage
import pages.assertIsVisible
import utils.SerenityHelpers

class EngageStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var healthAdvicePage: HealthAdvicePage

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

    @Then("^the link to Engage Medical Advice is available on the Advice page$")
    fun theLinkToMedicalAdviceIsAvailableOnTheAdvicePage() {
        healthAdvicePage.engageMedicalAdvice.assertIsVisible()
    }

    @When("^I click the Engage Medical Advice link on the Advice page")
    fun iClickTheMedicalAdviceLinkOnTheAdvicePage() {
        healthAdvicePage.engageMedicalAdvice.click()
    }

    @Then("the warning message on the Redirector page explains the service is from Engage$")
    fun assertWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "This service is provided by Engage Health Systems Limited",
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
