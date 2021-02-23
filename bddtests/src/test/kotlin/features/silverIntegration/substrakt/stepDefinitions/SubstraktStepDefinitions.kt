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
import pages.more.MorePage
import pages.assertElementNotPresent
import pages.gpMedicalRecord.MedicalRecordHubPage
import utils.SerenityHelpers

class SubstraktStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private lateinit var morePage: MorePage

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

    @Given("^I am a user with proof level 5 who can view Participation groups from Substrakt$")
    fun iAmAUserWithProofLevel5WhoCanViewPatientParticipationGroupsFromSubstrakt(){
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_PARTICIPATION_SUBSTRAKT, IdentityProofingLevel.P5)
    }

    @Given("^I am a user who can view Patient participation groups from Substrakt$")
    fun iAmAUserWhoCanViewPatientParticipationGroupsFromSubstrakt(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_PARTICIPATION_SUBSTRAKT)
    }

    @Given("^I am a user who cannot view Patient participation groups from Substrakt$")
    fun iAmAUserWhoCannotViewPatientParticipationGroupsFromSubstrakt(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_PARTICIPATION_NONE)
    }

    @Then("^the link to Substrakt 'Patient participation groups' is not available on the Account page$")
    fun theLinkToSubstraktPatientParticipationGroupsIsNotAvailableMedicalRecordHubPage() {
        morePage.getHeaderElement("Patient participation groups").assertElementNotPresent()
    }

    @Then("the question warning message on the Redirector page explains the service is from Substrakt$")
    fun assertQuestionWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Ask your GP surgery a question\nThis service is provided by Substrakt Health",
                "Your GP surgery or hospital has chosen this personal health record service provider.",
                "Find out more about personal health record services")
    }

    @Then("the participation warning message on the Redirector page explains the service is from Substrakt$")
    fun assertParticipationWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Patient participation groups\nThis service is provided by Substrakt Health",
            "Your GP surgery or hospital has chosen this personal health record service provider.",
            "Find out more about personal health record services")
    }

    @Then("the personal details warning message on the Redirector page explains the service is from Substrakt$")
    fun assertPersonalDetailsWarningMessageContent() {
        redirector.interruptionCard.assertContent(
            "Update your personal details\nThis service is provided by Substrakt Health",
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
