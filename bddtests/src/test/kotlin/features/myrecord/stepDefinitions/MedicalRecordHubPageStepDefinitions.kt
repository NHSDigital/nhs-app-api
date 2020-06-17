package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import pages.gpMedicalRecord.MedicalRecordHubPage
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import pages.assertElementNotPresent
import pages.assertSingleElementPresent

open class MedicalRecordHubPageStepDefinitions {
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage

    @Given("I am an (.*) patient with no access to any Third Party Health Record Hub Features")
    fun setupUser(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE, supplier)
    }

    @Given("I am an (.*) patient and I have access to Patients Know Best Test Results")
    fun setupPKBTestResultsPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_PKB, supplier)
    }

    @Given("I am an (.*) patient and I have access to Patients Know Best Care Plans")
    fun setupPKBCarePlansPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB, supplier)
    }

    @Given("I am an (.*) patient and I have access to Patients Know Best Health Tracker")
    fun setupPKBHealthTrackerPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB, supplier)
    }

    @Given("I am an (.*) patient and I have access to Care Information Exchange Care Plans")
    fun setupCIECarePlansPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_CIE, supplier)
    }

    @Given("I am an (.*) patient and I have access to Care Information Exchange Health Tracker")
    fun setupCIEHealthTrackerPatient(supplier: String) {
        setupPatient(SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_CIE, supplier)
    }

    @When("I click the menu item '(.*)'$")
    fun clickMenuItem(title: String) {
        medicalRecordHubPage.getHeaderElement(title).click()
    }

    @Then("I see the 'GP medical record' page")
    fun assertMedicalHubPage() {
        medicalRecordHubPage.pageTitleGpMedicalRecord().assertSingleElementPresent()
    }

    @Then("I see the health records hub page")
    fun assertHealthRecordsHubPage() {
        medicalRecordHubPage.pageTitleHealthRecords().assertSingleElementPresent()
    }

    @Then("^I see the Third Party menu item '(.*)'$")
    fun assertISeeTheseMenuItems(item: String) {
        medicalRecordHubPage.getHeaderElement(item).assertSingleElementPresent()
    }

    @Then("^I do not see the Third Party menu item '(.*)'$")
    fun assertIDontSeeTheseMenuItems(item: String) {
        medicalRecordHubPage.getHeaderElement(item).assertElementNotPresent()
    }

    @Then("I see 'Your GP medical record' page")
    fun iSeeGpMedicalRecordPage() {
        medicalRecordHubPage.getGpRecordHeader("Your GP medical record").assertSingleElementPresent()
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
