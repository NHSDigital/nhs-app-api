package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.myrecord.factories.MyRecordFactory
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.microtest.myRecord.MyRecordModuleCounts
import mocking.microtest.myRecord.TestResultOptions
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.openqa.selenium.JavascriptExecutor
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.myrecord.MyRecordInfoPage
import pages.myrecord.MyRecordWarningPage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import pages.navigation.WebHeader
import pages.text
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.myrecord.MyRecordResponse

open class MyRecordStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var webHeader: WebHeader

    lateinit var headerNative: HeaderNative

    private lateinit var myRecordWarningPage: MyRecordWarningPage

    lateinit var myRecordInfoPage: MyRecordInfoPage

    var myRecordModuleCounts = MyRecordModuleCounts()

    var testResultOptions = TestResultOptions()

    @Given("^the my record wiremocks are initialised for (.*)$")
    fun givenMyRecordWiremocksAreInitialisedFor(gpSystem: String) {
        SerenityHelpers.setGpSupplier(gpSystem)
        setPatientToDefaultFor(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(gpSystem).enabledWithBlankRecord(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has disabled summary care record functionality$")
    fun givenTheGPPracticeHasDisabledSummaryCareRecordFunctionality() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        MyRecordFactory.getForSupplier(gpSystem).disabled(SerenityHelpers.getPatient())
    }

    @Given("^I am on my record information page$")
    fun givenIAmOnTheGpMedicalRecordInformationPage() {
        browser.goToApp()
        login.using(SerenityHelpers.getPatient())
        nav.select(NavBarNative.NavBarType.MY_RECORD)
        myRecordWarningPage.clickWarningContinue()
        myRecordInfoPage.locatorMethods.waitForNativeStepToComplete()
        myRecordInfoPage.clinicalAbbreviationsLink.assertIsVisible()
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

    @Then("^I see a message telling me to contact my GP for (.*) information on My Record$")
    fun thenISeeAMessageTellingMeToContactMyGP(heading: String) {
        assertTextInSection(heading,
                "Sorry, this information isn't available through the NHS App. To access it, contact your GP surgery.")
    }

    @Given("^the my record wiremocks return a 403 for (.*)$")
    fun givenMyRecordWiremocksReturnA403For(gpSystem: String) {
        SerenityHelpers.setGpSupplier(gpSystem)
        setPatientToDefaultFor(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(gpSystem).respondWithForbidden(SerenityHelpers.getPatient())
    }

    @Given("^the my record wiremocks are populated for (.*)$")
    fun givenMyRecordWiremocksArePopulatedFor(gpSystem: String) {
        setPatientToDefaultFor(gpSystem)
        givenMyRecordWiremocksArePopulatedForNoPatient(gpSystem)
    }

    @Given("^the my record wiremocks are populated when the patient is already set for (.*)$")
    fun givenMyRecordWiremocksArePopulatedForNoPatient(gpSystem: String) {
        SerenityHelpers.setGpSupplier(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(SerenityHelpers.getPatient())
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(SerenityHelpers.getPatient())
        MyRecordFactory.getForSupplier(gpSystem).
                enabledWithData(SerenityHelpers.getPatient(), myRecordModuleCounts, testResultOptions)
    }


    @When("^I enter url address for my record directly into the url$")
    fun whenIEnterUrlAddressForMyRecordDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record"
        browser.browseTo(fullUrl)
    }

    @When("^I get the users my record data$")
    fun whenIGetTheUsersMyRecordData() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).myRecord.getMyRecord()
            Serenity.setSessionVariable(MyRecordResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I click My details heading$")
    fun whenIClickMyDetailsHeading() {
        myRecordInfoPage.myDetails.toggleShrub()
    }

    @When("^I click the (.*) section on My Record$")
    fun iClickOnTheSection(heading: String) {
        myRecordInfoPage.getSection(heading).toggleShrub()
    }

    @When("^I navigate away from the medical record page$")
    fun iNavigateAwayFromTheMedicalRecordPage() {
        nav.select(NavBarNative.NavBarType.SYMPTOMS)
    }

    @Then("^I see the my medical record page$")
    fun iSeeTheMyMedicalRecordPage() {
        thenISeeHeaderTextIsMyGPMedicalRecord()
        iSeeTheHeadingOnMyRecord("My details")
        iSeePatientInformationDetails()
        thenISeeMyRecordButtonOnTheNavBarIsHighlighted()
    }

    @Then("^I see header text is My GP medical record$")
    fun thenISeeHeaderTextIsMyGPMedicalRecord() {
        headerNative.waitForPageHeaderText("My GP medical record")
    }

    @Then("^I see my record button on the nav bar is highlighted$")
    fun thenISeeMyRecordButtonOnTheNavBarIsHighlighted() {
        assertTrue(nav.hasSelectedTab(NavBarNative.NavBarType.MY_RECORD))
    }

    @Then("^the my record information screen is loaded$")
    fun thenTheMyRecordInformationScreenIsLoaded() {
        myRecordInfoPage.myDetails.header.assertSingleElementPresent().assertIsVisible()
    }

    @Then("^I will return to the home page$")
    fun thenIWillReturnToTheHomePage() {
        webHeader.isPageTitleCorrect("Home")
    }

    @Then("^No navigation menu bar item will be selected$")
    fun thenNoNavigationMenuBarItemWillBeSelected() {
        if(headerNative.onMobile()) {
            assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.SYMPTOMS))
            assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.APPOINTMENTS))
            assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.PRESCRIPTIONS))
            assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.MY_RECORD))
            assertTrue(!nav.hasSelectedTab(NavBarNative.NavBarType.MORE))
        }
    }

    @Then("^I see the patient information details$")
    fun iSeePatientInformationDetails() {
        val patient = SerenityHelpers.getPatient()
        val sex = patient.sex.name
        val address = patient.address.full()

        myRecordInfoPage.assertLabelAndValue("Name", patient.formattedFullName())
        myRecordInfoPage.assertLabelAndValue("Date of birth", patient.formattedDateOfBirth())
        myRecordInfoPage.assertLabelAndValue("Sex", sex)
        myRecordInfoPage.assertLabelAndValue("Address", address)
        myRecordInfoPage.assertLabelAndValue("NHS number", patient.formattedNHSNumber())
    }

    @Then("^I do not see patient information details$")
    fun thenIDoNotSeePatientInformationDetails() {
        assertFalse("Name field was visible.", myRecordInfoPage.isNameVisible())
    }

    @Then("^I see Service not offered by GP or to specific user or access revoked warning message$")
    fun thenISeeServiceNotOfferedByGPOrToSpecificUserOrAccessRevokedWarningMessage() {
        assertEquals("You do not currently have online access to your medical record\n" +
                "Contact your GP surgery for more information.",
                myRecordInfoPage.getSummaryCareNoAccessMessage())
    }

    @Then("^I see the (.*) heading on My Record$")
    fun iSeeTheHeadingOnMyRecord(heading: String) {
        myRecordInfoPage.assertSectionHeaderIsVisible(heading)
    }

    @Then("^I do not see the (.*) heading on My Record$")
    fun iDoNotSeeTheHeadingOnMyRecord(heading: String) {
        myRecordInfoPage.assertSectionHeaderNotPresent(heading)
    }

    @Then("^I see the (.*) section collapsed on My Record$")
    fun iSeeTheSectionCollapsed(heading: String) {
        myRecordInfoPage.assertSectionCollapsed(heading)
    }

    @Then("^I see a message indicating that I have no access to view my summary care record$")
    fun thenISeeAMessageIndicatingThatIHaveNoAccessToViewMyRecord() {
        assertEquals("You do not currently have online access to your medical record\n" +
                "Contact your GP surgery for more information.",
                myRecordInfoPage.getSummaryCareNoAccessMessage())
    }

    @Then("^I see a message indicating that I have no access to view (.*) on My Record$")
    fun thenISeeAMessageIndicatingThatIHaveNoAccessToViewSection(heading: String) {
        assertTextInSection(heading, "You do not currently have access to this section")
    }

    @Then("^I see an error occurred message with (.*) on My Record$")
    fun thenISeeAnErrorOccuredMessageForProblems(heading: String) {
        assertTextInSection(heading, "An error has occurred trying to retrieve this data.")
    }

    @Then("^I see a message indicating that I have no information recorded for (.*) on My Record$")
    fun thenISeeAMessageIndicatingThatIHaveNoInformationRecorded(heading: String) {
        assertTextInSection(heading, "No information recorded")
    }

    private fun assertTextInSection(heading: String, message: String) {
        val section = myRecordInfoPage.getSection(heading)
        section.header.assertSingleElementPresent()
        assertEquals(message, section.firstParagraph.text)
    }

    @Then("^the field indicating supplier is set$")
    fun thenTheFlagIndicatingSupplierIsSetTo() {
        val result = Serenity.sessionVariableCalled<MyRecordResponse>(MyRecordResponse::class)
        assertEquals(SerenityHelpers.getGpSupplier(), result.response.supplier)
    }

    @Then("^I return to my medical record page$")
    fun thenIReturnToMyMedicalRecordPage() {
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }

    @Then("^I see the top of my medical record page$")
    fun andISeeTheTopOfMyMedicalRecordPage(){
        assertEquals(0L, getScrollPositionX())
        assertEquals(0L, getScrollPositionY())
    }

    @Then("^I click on the Back link on my records page$")
    fun iClickOnBackButtonOnMyRecordsPage(){
        myRecordInfoPage.clickOnBackLink()
    }

    private fun getScrollPositionX(): Any {
        val jsExecutor = myRecordInfoPage.driver as JavascriptExecutor
        return jsExecutor.executeScript("return window.scrollX") as Any
    }

    private fun getScrollPositionY(): Any {
        val jsExecutor = myRecordInfoPage.driver as JavascriptExecutor
        return jsExecutor.executeScript("return window.scrollY") as Any
    }
}
