package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.factories.DemographicsFactory
import features.myrecord.factories.MyRecordFactory
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.TestResultOptions
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.myrecord.MyRecordWarningPage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import utils.SerenityHelpers
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MedicalRecordStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var nav: NavigationSteps

    private lateinit var headerNative: HeaderNative
    private lateinit var myRecordWarningPage: MyRecordWarningPage

    var myRecordModuleCounts = MyRecordModuleCounts()
    var testResultOptions = TestResultOptions()

    @Given("^the GP Practice has disabled summary care record functionality$")
    fun givenTheGPPracticeHasDisabledSummaryCareRecordFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        MyRecordFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }

    @Given("^I am on the medical record page$")
    fun givenINavigateToTheMedicalRecordPage() {
        val patient = SerenityHelpers.getPatient()
        browser.goToApp()

        DemographicsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enableForPatientProxyAccounts(patient)

        login.using(patient)
        home.waitForLoginToCompleteSuccessfully()

        nav.select(NavBarNative.NavBarType.MY_RECORD)
        myRecordWarningPage.clickWarningContinue()
        myRecordWarningPage.locatorMethods.waitForNativeStepToComplete()
    }

    @Given("^I have (.*) Allergies$")
    fun givenIHaveCountOfAllergies(count: Int) {
        myRecordModuleCounts.allergyCount = count
    }

    @Given("^I have (.*) Medications$")
    fun givenIHaveCountOfMedications(count: Int) {
        myRecordModuleCounts.medicationCount = count
    }

    @Given("^I have (.*) Problems$")
    fun givenIHaveCountOfProblems(count: Int) {
        myRecordModuleCounts.problemCount = count
    }

    @Given("^I have (.*) Immunisations$")
    fun givenIHaveCountOfImmunisations(count: Int) {
        myRecordModuleCounts.vaccinationsCount = count
    }

    @Given("^I have (.*) Recalls$")
    fun givenIHaveCountOfRecalls(count: Int) {
        myRecordModuleCounts.recallCount = count
    }

    @Given("^I have (.*) Encounters$")
    fun givenIHaveCountOfEncounters(count: Int) {
        myRecordModuleCounts.encounterCount = count
    }

    @Given("^I have (.*) Referrals$")
    fun givenIHaveCountOfReferrals(count: Int) {
        myRecordModuleCounts.referralCount = count
    }

    @Given("^I have (.*) INR TestResults and (.*) Path TestResults$")
    fun givenIHaveCountOfTestResults(inrCount: Int, pathCount: Int) {
        myRecordModuleCounts.inrResultCount = inrCount
        myRecordModuleCounts.pathResultCount = pathCount
    }

    @Given("^I have Path TestResults Filtered out$")
    fun givenIHavePathTestResultsFilteredOut() {
        testResultOptions.includeFilteredOutPathStatuses = true
    }

    @Given("^the TestResults have interleaved dates$")
    fun givenTestResultsHaveInterleavedDates () {
        testResultOptions.interleavedPathAndInrDates = true
    }

    @Given("^I have (.*) MedicalHistories$")
    fun givenIHaveCountOfMedicalHistories(count: Int) {
        myRecordModuleCounts.medicalHistoryCount = count
    }

    @Given("^the my record wiremocks return a 403$")
    fun givenMyRecordWiremocksReturnA403() {
        val patient = SerenityHelpers.getPatient()
        val supplier = SerenityHelpers.getGpSupplier()

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        MyRecordFactory.getForSupplier(supplier).respondWithForbidden(patient)
    }

    @Given("^the my record wiremocks are populated$")
    fun givenMyRecordWiremocksArePopulated() {
        val supplier = SerenityHelpers.getGpSupplier()

        SerenityHelpers.setGpSupplier(supplier)
        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(supplier).
                enabledWithData(SerenityHelpers.getPatient(), myRecordModuleCounts, testResultOptions)
    }

    @Given("^the GP Practice has enabled all medical records for the patient$")
    fun givenTheGPPracticeHasEnabledAllMedicalRecordsForThePatient() {
        val supplier = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatient()

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        MyRecordFactory.getForSupplier(supplier).enabledWithAllRecords(patient)
    }

    @When("^I enter url address for my record directly into the url$")
    fun whenIEnterUrlAddressForMyRecordDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record"
        browser.browseTo(fullUrl)
    }

    @When("^I get the users my record data$")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .myRecord.getMyRecord(patientId)
            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I navigate away from the medical record page$")
    fun iNavigateAwayFromTheMedicalRecordPage() {
        nav.select(NavBarNative.NavBarType.SYMPTOMS)
    }

    @Then("^I see header text is Your GP medical record$")
    fun thenISeeHeaderTextIsYourGPMedicalRecord() {
        headerNative.waitForPageHeaderText("Your GP medical record")
    }

    @Then("^I see my record button on the nav bar is highlighted$")
    fun thenISeeMyRecordButtonOnTheNavBarIsHighlighted() {
        Assert.assertTrue(nav.hasSelectedTab(NavBarNative.NavBarType.MY_RECORD))
    }

    @Then("^No navigation menu bar item will be selected$")
    fun thenNoNavigationMenuBarItemWillBeSelected() {
        if(headerNative.onMobile()) {
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.SYMPTOMS))
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.APPOINTMENTS))
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.PRESCRIPTIONS))
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.MY_RECORD))
            Assert.assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.MORE))
        }
    }
}