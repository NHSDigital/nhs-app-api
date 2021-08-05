package features.myrecord.stepDefinitions

import constants.Supplier
import models.Patient
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import pages.gpMedicalRecord.MedicalRecordHubPage
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import pages.assertSingleElementPresent
import pages.assertElementNotPresent
import pages.assertIsVisible
import utils.SerenityHelpers

open class MedicalRecordHubPageStepDefinitions {
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private val organDonationTitle = "Manage your organ donation decision"
    private val dataSharingTitle = "Choose if data from your health records is shared for research and planning"

    @Given("^I am an (.*) patient and I have NDOP enabled$")
    fun setupNDOPUser(supplier: String) {
        setupPatient(supplier)
    }

    @Given("^I am an (.*) patient and I have NDOP disabled$")
    fun setupNonNDOPUser(supplier: String) {
        setupPatient(supplier, SJRJourneyType.NDOP_DISABLED)
    }

    @Given("^I am an (.*) patient with no access to any Third Party Health Record Hub Features$")
    fun setupUser(supplier: String) {
        setupPatient(supplier, SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE)
    }

    @Given("^I am an (.*) patient and I have access to Patients Know Best Test Results$")
    fun setupPKBTestResultsPatient(supplier: String) {
        setupPatient(supplier, SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_PKB)
    }

    @Given("^I am an (.*) patient and I have access to Patients Know Best Care Plans$")
    fun setupPKBCarePlansPatient(supplier: String) {
        setupPatient(supplier, SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB)
    }

    @Given("^I am an (.*) patient and I have access to Patients Know Best Health Tracker$")
    fun setupPKBHealthTrackerPatient(supplier: String) {
        setupPatient(supplier, SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB)
    }

    @Given("^I am an (.*) patient and I have access to Care Information Exchange Care Plans$")
    fun setupCIECarePlansPatient(supplier: String) {
        setupPatient(supplier, SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_CIE)
    }

    @Given("^I am an (.*) patient and I have access to Care Information Exchange Health Tracker$")
    fun setupCIEHealthTrackerPatient(supplier: String) {
        setupPatient(supplier, SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_CIE)
    }

    @When("I click the menu item '(.*)'$")
    fun clickMenuItem(title: String) {
        medicalRecordHubPage.getHeaderElement(title).click()
    }

    @When("^I click the Data Sharing link on the health hub page")
    fun iClickTheDataSharingLinkOnTheHealthHubPage() {
        clickMenuItem(dataSharingTitle)
    }

    @When("^I choose to set my organ donation preferences")
    fun iChooseToSetMyOrganDonationPreferences() {
        clickMenuItem(organDonationTitle)
    }

    @Then("I see the 'GP health record' page")
    fun assertMedicalHubPage() {
        medicalRecordHubPage.pageTitleGpMedicalRecord().assertSingleElementPresent()
    }

    @Then("I see the health records hub page")
    fun assertHealthRecordsHubPage() {
        medicalRecordHubPage.pageTitleYourHealth().assertSingleElementPresent()
    }

    @Then("^I see the Third Party menu item '(.*)'$")
    fun assertISeeTheseMenuItems(item: String) {
        medicalRecordHubPage.getHeaderElement(item).assertSingleElementPresent()
    }

    @Then("^I do not see the Third Party menu item '(.*)'$")
    fun assertIDoNoSeeTheseMenuItems(item: String) {
        medicalRecordHubPage.getHeaderElement(item).assertElementNotPresent()
    }

    @When("^I click on the Gp medical record link$")
    fun clickGpMedicalRecordLink() {
        medicalRecordHubPage.gpMedicalRecordPanel.click()
    }

    @Then("^I see the NDOP data sharing link$")
    fun assertISeeNDOPMenuItem() {
        medicalRecordHubPage.ndopLink.assertIsVisible()
    }

    @Then("^I do not see the NDOP data sharing link$")
    fun assertIDoNotSeeNDOPMenuItem() {
        medicalRecordHubPage.ndopLink.assertElementNotPresent()
    }

    private fun setupPatient(gpSystem: String, configuration: SJRJourneyType? = null) {
        val supplier = Supplier.valueOf(gpSystem)
        var patient = Patient.getDefault(supplier)
        if(configuration != null){
            patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                supplier, configuration)
        }
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)

        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
