package features.myrecord.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import pages.gpMedicalRecord.MedicalRecordHubPage
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import pages.assertSingleElementPresent

open class MedicalRecordHubPageStepDefinitions {
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private val organDonationTitle = "Manage your organ donation decision"
    private val dataSharingTitle = "Choose if data from your health records is shared for research and planning"

    @Given("^I am an (.*) patient with no access to any Third Party Health Record Hub Features$")
    fun setupUser(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE, supplier)
    }

    @Given("^I am an (.*) patient and I have access to Patients Know Best Test Results$")
    fun setupPKBTestResultsPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_PKB, supplier)
    }

    @Given("^I am an (.*) patient and I have access to Patients Know Best Care Plans$")
    fun setupPKBCarePlansPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB, supplier)
    }

    @Given("^I am an (.*) patient and I have access to Patients Know Best Health Tracker$")
    fun setupPKBHealthTrackerPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB, supplier)
    }

    @Given("^I am an (.*) patient and I have access to Care Information Exchange Care Plans$")
    fun setupCIECarePlansPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_CIE, supplier)
    }

    @Given("^I am an (.*) patient and I have access to Care Information Exchange Health Tracker$")
    fun setupCIEHealthTrackerPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_CIE, supplier)
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

    @When("^I click on the Gp medical record link$")
    fun clickGpMedicalRecordLink() {
        medicalRecordHubPage.gpMedicalRecordPanel.click()
    }

    private fun setupPatient(configuration: SJRJourneyType, gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                supplier, configuration)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
