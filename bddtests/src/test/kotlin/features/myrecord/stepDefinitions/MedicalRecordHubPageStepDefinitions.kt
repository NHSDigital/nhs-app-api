package features.myrecord.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.gpMedicalRecord.MedicalRecordHubPage
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import pages.assertElementNotPresent
import pages.assertSingleElementPresent

open class MedicalRecordHubPageStepDefinitions {
    private lateinit var medicalRecordHubPage: MedicalRecordHubPage
    private val mockingClient = MockingClient.instance

    @Given("I am an (.*) patient with no access to PKB")
    fun setupUser(supplier: String) {
        setupPatient(ServiceJourneyRulesMapper.Companion.JourneyType.SILVER_INTEGRATION_CAREPLANS_NONE, supplier)
    }

    @Given("I am an (.*) patient and I have access to Patients Know Best Care Plans")
    fun setupPKBCarePlansPatient(supplier: String) {
        setupPatient(ServiceJourneyRulesMapper.Companion.JourneyType.SILVER_INTEGRATION_CAREPLANS_PKB, supplier)
    }

    @Given("I am an (.*) patient and I have access to Patients Know Best Health Tracker")
    fun setupPKBHealthTrackerPatient(supplier: String) {
        setupPatient(ServiceJourneyRulesMapper.Companion.JourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB, supplier)
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

    @Then("^I see the PKB menu item '(.*)'$")
    fun assertISeeTheseMenuItems(item: String) {
        medicalRecordHubPage.getHeaderElement(item).assertSingleElementPresent()
    }

    @Then("^I do not see the PKB menu item '(.*)'$")
    fun assertIDontSeeTheseMenuItems(item: String) {
        medicalRecordHubPage.getHeaderElement(item).assertElementNotPresent()
    }

    @Then("I see 'Your GP medical record' page")
    fun iSeeGpMedicalRecordPage() {
        medicalRecordHubPage.getGpRecordHeader("Your GP medical record").assertSingleElementPresent()
    }

    private fun setupPatient(configuration: ServiceJourneyRulesMapper.Companion.JourneyType, gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(
                supplier, configuration)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
    }
}
