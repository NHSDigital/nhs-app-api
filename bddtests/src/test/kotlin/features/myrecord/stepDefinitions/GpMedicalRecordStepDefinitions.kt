package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.myrecord.factories.DemographicsFactory
import features.myrecord.factories.MyRecordFactory
import features.serviceJourneyRules.factories.ServiceJourneyRulesConfiguration
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.TestResultOptions
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertEquals
import pages.assertIsVisible
import pages.gpMedicalRecord.MyRecordConsultationsAndEventsModule
import pages.gpMedicalRecord.MyRecordHealthConditionsModule
import pages.gpMedicalRecord.MyRecordImmunisationsModule
import pages.gpMedicalRecord.MyRecordMedicinesModule
import pages.gpMedicalRecord.MyRecordRecallsModule
import pages.gpMedicalRecord.MyRecordInfoPage
import pages.myrecord.MyRecordWarningPage
import pages.navigation.NavBarNative
import pages.text
import utils.SerenityHelpers

open class GpMedicalRecordStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var medicinesModule: MyRecordMedicinesModule
    @Steps
    lateinit var immunisationModule: MyRecordImmunisationsModule
    @Steps
    lateinit var consultationsAndEventsModule: MyRecordConsultationsAndEventsModule
    @Steps
    lateinit var healthConditionsModule: MyRecordHealthConditionsModule

    lateinit var recallsModule: MyRecordRecallsModule

    private lateinit var myRecordWarningPage: MyRecordWarningPage

    lateinit var myRecordInfoPage: MyRecordInfoPage

    var myRecordModuleCounts = MyRecordModuleCounts()

    var testResultOptions = TestResultOptions()

    @Given("^I am a (\\w+) user setup to use medical record version 2$")
    fun iAmAUserWishingToRegisterTheirDeviceForPushNotifications(gpSystem: String) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(gpSystem,
                listOf(ServiceJourneyRulesConfiguration("medical record version", "2")))

        SerenityHelpers.setGpSupplier(gpSystem)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        MyRecordFactory.getForSupplier(gpSystem).enabledWithBlankRecord(patient)
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    @Given("^I am on my record information page - GP Medical Record$")
    fun givenIAmOnTheGpMedicalRecordInformationPage() {
        navigateToInformationPage()
    }

    @Given("^I have my medical record available to view for (\\w+) - GP Medical Record$")
    fun givenIHaveMyMedicalRecordAvailableToView(gpSystem: String) {
        MyRecordFactory.getForSupplier(gpSystem).
                enabledWithData(SerenityHelpers.getPatient(), myRecordModuleCounts, testResultOptions)
    }

    @Given("^I am on my record information page and glossary is visible - GP Medical Record$")
    fun givenIAmOnTheGpMedicalRecordInformationPageAndGlossaryIsVisible() {
        navigateToInformationPage()
        myRecordInfoPage.clinicalAbbreviationsLink.assertIsVisible()
    }

    fun navigateToInformationPage() {
        nav.select(NavBarNative.NavBarType.MY_RECORD)
        myRecordWarningPage.clickWarningContinue()
        myRecordInfoPage.locatorMethods.waitForNativeStepToComplete()
    }

    @Then("^I am on the medical record v2 page$")
    fun givenIAmOnTheGpMedicalRecordPage(){
        myRecordInfoPage.pageTitle.assertIsVisible()
        myRecordInfoPage.clinicalAbbreviationsLink.assertIsVisible()
    }

    @Then("^I see a message telling me to contact my GP for information on My Record - GP Medical Record$")
    fun thenISeeAMessageTellingMeToContactMyGP() {
        assertTextOnPage(
                "Sorry, this information isn't available through the NHS App. To access it, contact your GP surgery.")
    }

    @Given("^I have (.*) Allergies - GP Medical Record$")
    fun givenIHaveCountOfAllergiesGpMedicalRecord(count: Int) {
        myRecordModuleCounts.allergyCount = count
    }

    @Given("^MICROTEST have enabled medical record and records exist - GP Medical Record$")
    fun givenMicrotestHaveEnabledMedicalRecordAndRecordsExist() {
        val myRecordModuleCounts = MyRecordModuleCounts()
        createRecordStubsMicrotest(myRecordModuleCounts)
    }

    @Given("^the my record wiremocks are initialised when the patient is already set for (.*)$")
    fun givenMyRecordWiremocksAreInitialisedForNoPatient(gpSystem: String) {
        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^the my record wiremocks have data when the patient is already set for (.*)$")
    fun givenMyRecordWiremocksHaveDataForNoPatient(gpSystem: String) {
        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(gpSystem).enabledWithAllRecords(SerenityHelpers.getPatient())
    }


    @When("I click the Allergies and adverse reactions link on my record - GP Medical Record")
    fun iClickTheAllergiesLinkOnTheAccountPage(){
        myRecordInfoPage.allergies.allergies.click()
    }

    @When("I click the Immunisations link on my record - GP Medical Record")
    fun iClickTheImmunisationsLinkOnTheAccountPage(){
        immunisationModule.link.click()
    }

    @When("I click the Consultations and events link on my record - GP Medical Record")
    fun iClickTheConsultationsLinkOnTheAccountPage() {
        consultationsAndEventsModule.link.click()
    }

    @When("I click the Health conditions link on my record - GP Medical Record")
    fun iClickTheHealthConditionsLinkOnTheAccountPage() {
        healthConditionsModule.link.click()
    }

    @When("I click the Medicines link on my record - GP Medical Record")
    fun iClickTheMedicinesLinkOnTheAccountPage(){
        medicinesModule.link.click()
    }

    @When("^I click the Recalls link on my record - GP Medical Record$")
    fun iClickTheRecallsLinkOnTheAccountPage(){
        recallsModule.link.click()
    }

    @When("I click the test result link on my record - GP Medical Record")
    fun iClickTheTestResultsLinkOnTheAccountPage(){
        myRecordInfoPage.testResults.testResults.click()
    }

    @Then("^I see an error occurred message on My Record - GP Medical Record$")
    fun thenISeeAnErrorOccuredMessageForProblems() {
        assertTextOnPage("An error has occurred trying to retrieve this data.")
    }

    @Then("^I see a message that I have no information recorded for a specific record - GP Medical Record$")
    fun thenISeeAMessageIndicatingThatIHaveNoInformationRecorded() {
        assertTextOnPage( "No information recorded")
    }

    @Then("^I see a message that this information isn't available through the NHS App - GP Medical Record$")
    fun thenISeeAMessageIndicatingThatInformationIsntAvailable() {
        assertTextOnPage("Sorry, this information isn't available through the NHS App. To access it, "+
        "contact your GP surgery.")
    }

    @Then("^I see a message indicating that I have no access to view this section on My Record" +
            " - GP Medical Record$")
    fun thenISeeAMessageIndicatingThatIHaveNoAccessToViewSection() {
        assertTextOnPage("You do not currently have access to this section")
    }

    private fun assertTextOnPage(message: String) {
        val section = myRecordInfoPage.getBody(message)
        assertEquals(message, section.text)
    }

    private fun createRecordStubsMicrotest(moduleCounts: MyRecordModuleCounts) {
        val supplier = "MICROTEST"

        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(supplier).enabledWithData(
                SerenityHelpers.getPatient(),
                moduleCounts,
                TestResultOptions())
    }
}
