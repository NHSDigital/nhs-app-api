package features.prescriptionsHub.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.When
import io.cucumber.java.en.Then
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import pages.PrescriptionsHubPage
import pages.assertIsNotVisible

class PrescriptionsHubStepDefinitions {

    private lateinit var prescriptionsHubPage: PrescriptionsHubPage

    @When("^I click the View Orders link$")
    fun clickViewOrdersLink() {
        prescriptionsHubPage.viewOrdersPanel.click()
    }

    @When("^I click the Pkb Medicines link$")
    fun clickPkbMedicinesLink() {
        prescriptionsHubPage.pkbMedicinesJumpOffButton.click()
    }

    @When("^I click the nominated pharmacy link on the Prescriptions Hub$")
    fun iClickTheNominatedPharmacyLinkOnThePrescriptionsHub() {
        prescriptionsHubPage.nominatedPharmacyLink.click()
    }

    @Then("^the Prescriptions Hub page is displayed$")
    fun assertIsDisplayed() {
        prescriptionsHubPage.assertPrescriptionsHubIsDisplayed()
    }

    @Then("^the 'Nominate a pharmacy' link is not available on the Prescriptions Hub$")
    fun theNominateAPharmacyIsNotAvailableOnThePrescriptionsHub() {
        prescriptionsHubPage.nominatedPharmacyLink.assertIsNotVisible()
    }

    @Then("^I click the Order a repeat prescription button$")
    fun clickOrderARepeatPrescriptionButton() {
        prescriptionsHubPage.clickOrderARepeatPrescriptionButton()
    }

    @Given("^I am an (.*) patient and I have access to PKB Medicines$")
    fun setupPKBCarePlansPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MEDICINES_PKB, supplier)
    }

    @Given("^I am an (.*) patient without access to PKB Medicines$")
    fun setupUser(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE, supplier)
    }

    private fun setupPatient(configuration: SJRJourneyType, gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                supplier, configuration)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
