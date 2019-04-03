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
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertArrayEquals
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

    lateinit var myRecordWarningPage: MyRecordWarningPage

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Given("^the my record wiremocks are initialised for (.*)$")
    fun givenMyRecordWiremocksAreInitialisedFor(getService: String) {
        SerenityHelpers.setGpSupplier(getService)
        setPatientToDefaultFor(getService)
        CitizenIdSessionCreateJourney(mockingClient).createFor(this.patient)
        SessionCreateJourneyFactory.getForSupplier(getService, mockingClient).createFor(this.patient)
        MyRecordFactory.getForSupplier(getService).enabledWithBlankRecord(patient)
    }

    @Given("^the GP Practice has disabled summary care record functionality$")
    fun givenTheGPPracticeHasDisabledSummaryCareRecordFunctionality() {
        val getService = SerenityHelpers.getGpSupplier()
        setPatientToDefaultFor(getService)
        MyRecordFactory.getForSupplier(getService).disabled(patient)
    }

    @Given("^I am on the record warning page$")
    fun givenIAmOnTheRecordWarningPage() {
        browser.goToApp()
        login.using(this.patient)
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }

    @Given("^I am on my record information page$")
    fun givenIAmOnMyRecordInformationPage() {
        browser.goToApp()
        login.using(this.patient)
        nav.select(NavBarNative.NavBarType.MY_RECORD)
        myRecordWarningPage.clickAgreeAndContinue()
        myRecordInfoPage.locatorMethods.waitForNativeStepToComplete()
        myRecordInfoPage.myDetails.header.assertSingleElementPresent().assertIsVisible()
        myRecordInfoPage.clinicalAbbreviationsLink.assertIsVisible()
        myRecordInfoPage.waitForSpinnerToDisappear()
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

    @When("^I click agree and continue$")
    fun whenIClickAgreeAndContinue() {
        myRecordWarningPage.clickAgreeAndContinue()
    }

    @When("^I click the back to home$")
    fun whenIClickTheBackToHome() {
        myRecordWarningPage.clickBacktoHome()
    }

    @When("^I click My details heading$")
    fun whenIClickMyDetailsHeading() {
        myRecordInfoPage.myDetails.toggleShrub()
    }

    @When("^I click the (.*) section on My Record$")
    fun iClickOnTheSection(heading: String) {
        myRecordInfoPage.waitForSpinnerToDisappear()
        myRecordInfoPage.getSection(heading).toggleShrub()
    }

    @When("^I navigate away from the medical record page$")
    fun iNavigateAwayFromTheMedicalRecordPage() {
        nav.select(NavBarNative.NavBarType.SYMPTOMS)
    }

    @Then("^I see record warning page opened$")
    fun thenISeeRecordWarningPageOpened() {
        thenISeeHeaderTextIsMyMedicalRecord()
        thenISeeAgreeAndContinueButton()
        thenISeeBackToHome()
    }

    @Then("^I see the my record warning page")
    fun iSeeTheMyRecordWarningPage() {
        thenISeeRecordWarningPageOpened()
        thenISeeHeaderTextIsMyMedicalRecord()
        theISeeYourRecordMayContainSensitiveInformationMessage()
        thenISeeListOfSensitiveDataInformation()
        thenISeeAgreeAndContinueButton()
        thenISeeBackToHome()
        thenISeeMyRecordButtonOnTheNavBarIsHighlighted()
    }

    @Then("^I see the my medical record page$")
    fun iSeeTheMyMedicalRecordPage() {
        thenISeeHeaderTextIsMyMedicalRecord()
        iSeeTheHeadingOnMyRecord("My details")
        iSeePatientInformationDetails()
        thenISeeMyRecordButtonOnTheNavBarIsHighlighted()
    }

    @Then("^I see header text is My medical record$")
    fun thenISeeHeaderTextIsMyMedicalRecord() {
        headerNative.waitForPageHeaderText("My medical record")
    }

    @Then("^I see your record may contain sensitive information message$")
    fun theISeeYourRecordMayContainSensitiveInformationMessage() {
        assertEquals("Your record may contain sensitive information. If someone is pressuring you for this" +
                " information, contact your GP surgery immediately.", myRecordWarningPage.warningText())
    }

    @Then("^I see list of sensitive data information$")
    fun thenISeeListOfSensitiveDataInformation() {
        val expected = ArrayList<String>()
        expected.add("personal data, such as your details, allergies and medications")
        expected.add("clinical terms that you may not be familiar with")
        expected.add("your medical history, including problems and consultation notes")
        expected.add("test results that you may not have discussed with your doctor")
        assertArrayEquals(expected.toArray(), myRecordWarningPage.getSensitiveList().toArray())
    }

    @Then("^I see agree and continue button$")
    fun thenISeeAgreeAndContinueButton() {
        Assert.assertTrue("isAgreePresent", myRecordWarningPage.isAgreePresent())
    }

    @Then("^I see back to home$")
    fun thenISeeBackToHome() {
        Assert.assertTrue("isBackToHomePresent", myRecordWarningPage.isBackToHomePresent())
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
        val sex = this.patient.sex.name;
        val address = patient.address.full()

        myRecordInfoPage.assertLabelAndValue("Name", patient.formattedFullName())
        myRecordInfoPage.assertLabelAndValue("Date of birth", patient.formattedDateOfBirth())
        myRecordInfoPage.assertLabelAndValue("Sex", sex)
        myRecordInfoPage.assertLabelAndValue("Address", address)
        myRecordInfoPage.assertLabelAndValue("NHS number", patient.formattedNHSNumber())
    }

    @Then("^I click the clinical abbreviations link$")
    fun thenIClickTheClinicalAbbreviationsLink() {
        myRecordInfoPage.clickClinicalAbbreviationsLink()
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

    @Then("^I see the (.*) section collapsed on My Record$")
    fun iSeeTheSectionCollapsed(heading: String) {
        val section = myRecordInfoPage.getSection(heading)
        assertFalse("Expected section paragraph to not be visible", section.firstParagraph.isCurrentlyVisible)
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

    @Then("^I am redirected to the my record page$")
    fun thenIAmRedirectedToTheMyRecordPage() {
        val redirectUrl = Config.instance.url + "/my-record"
        browser.shouldHaveUrl(redirectUrl)
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

    private fun getScrollPositionX(): Any {
        val jsExecutor = myRecordInfoPage.driver as JavascriptExecutor
        return jsExecutor.executeScript("return window.scrollX") as Any
    }

    private fun getScrollPositionY(): Any {
        val jsExecutor = myRecordInfoPage.driver as JavascriptExecutor
        return jsExecutor.executeScript("return window.scrollY") as Any
    }
}
