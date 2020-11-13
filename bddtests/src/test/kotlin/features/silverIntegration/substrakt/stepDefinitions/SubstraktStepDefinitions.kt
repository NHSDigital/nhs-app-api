package features.silverIntegration.substrakt.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import pages.HybridPageObject
import pages.RedirectorPage
import pages.assertElementNotPresent
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class SubstraktStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage

    @Given("^I am a user who can view Ask Your Gp Surgery a Question from Substrakt$")
    fun iAmAUserWhoCanViewAskYourGpSurgeryAQuestionFromSubstrakt(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MESSAGES_SUBSTRAKT)
    }

    @Given("^I am a user with proof level 5 who can view Ask Your Gp Surgery a Question from Substrakt$")
    fun iAmAUserWithProofLevel5WhoCanViewAskYourGpSurgeryAQuestionFromSubstrakt(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_SUBSTRAKT, IdentityProofingLevel.P5)
    }

    @Given("^I am a user who cannot view Ask Your Gp Surgery a Question from Substrakt$")
    fun iAmAUserWhoCannotViewAskYourGpSurgeryAQuestionFromSubstrakt() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE)
    }

    @Given("^I am a user who can view Update your personal details from Substrakt$")
    fun iAmAUserWhoCanUpdateYourPersonalDetailsFromSubstrakt(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_ACCOUNT_ADMIN_SUBSTRAKT)
    }

    @Given("^I am a user who cannot view Update your personal details from Substrakt$")
    fun iAmAUserWhoCannotViewUpdateYourPersonalDetailsFromSubstrakt(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_ACCOUNT_ADMIN_NONE)
    }

    @Then("^the link to Substrakt 'Update your personal details' is not available on the Health Records Hub page$")
    fun theLinkToSubstraktUpateYourPersonalDetailsIsNotAvailableMedicalRecordHubPage() {
        medicalRecordHubPage.getHeaderElement("Update your personal details").assertElementNotPresent()
    }

    @Then("the warning message on the Redirector page explains the service is from Substrakt$")
    fun assertWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "This service is provided by Substrakt Health",
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
