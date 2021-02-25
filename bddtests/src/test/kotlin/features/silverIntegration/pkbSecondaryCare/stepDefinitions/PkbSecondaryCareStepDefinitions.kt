package features.silverIntegration.pkbSecondaryCare.stepDefinitions

import features.serviceJourneyRules.factories.SJRJourneyType
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.IdentityProofingLevel
import pages.HybridPageObject
import pages.PrescriptionsHubPage
import pages.RedirectorPage
import pages.assertElementNotPresent
import pages.assertIsVisible
import utils.SerenityHelpers

class PkbSecondaryCareStepDefinitions : HybridPageObject() {
    private lateinit var redirector: RedirectorPage
    private lateinit var prescriptionsHubPage: PrescriptionsHubPage

    @Given("^I am a user who can view Medicines from PKB Secondary Care$")
    fun iAmAUserWhoCanViewMedicinesFromPkbSecondaryCare() {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MEDICINES_PKB_SECONDARY_CARE)
    }

    @Given("^I am a user who cannot view Medicines from PKB Secondary Care$")
    fun iAmAUserWhoCannotViewMedicinesFromPkbSecondaryCare(){
        setupPatient( SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE)
    }

    @Then("^the PKB Secondary Care View Medicines link is available on the Prescriptions Hub$")
    fun thePKBSecondaryCareViewMedicinesLinkIsAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.assertIsVisible()
    }

    @Then("^the PKB Secondary Care View Medicines link is not available on the Prescriptions Hub$")
    fun theCieViewMedicinesLinkIsNotAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.assertElementNotPresent()
    }

    @Then("^I click the PKB Secondary Care View Medicines link on the Prescriptions hub$")
    fun iClickThePkbSecondaryCareViewMedicinesLink(){
        prescriptionsHubPage.pkbSecondaryCareMedicinesJumpOffButton.click()
    }

    @Then("the hospital and medicines warning on the page explains the service is from PKB Secondary Care$")
    fun assertHospitalAndMedicinesWarningMessageContent() {
        redirector.interruptionCard.assertContent(
                "Hospital and other medicines\nThis service is provided by Patients Know Best",
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
