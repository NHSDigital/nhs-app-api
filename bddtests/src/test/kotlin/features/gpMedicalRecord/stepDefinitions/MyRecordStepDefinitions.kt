package features.gpMedicalRecord.stepDefinitions

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
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertEquals
import pages.assertIsVisible
import pages.gpMedicalRecord.MyRecordInfoPage
import pages.myrecord.MyRecordWarningPage
import pages.navigation.NavBarNative
import pages.text
import utils.SerenityHelpers

open class MyRecordStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps

    private lateinit var myRecordWarningPage: MyRecordWarningPage

    lateinit var myRecordInfoPage: MyRecordInfoPage



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

    @Then("^I see a message telling me to contact my GP for information on My Record - GP Medical Record$")
    fun thenISeeAMessageTellingMeToContactMyGP() {
        assertTextOnPage(
                "Sorry, this information isn't available through the NHS App. To access it, contact your GP surgery.")
    }

    @When("I click the Allergies and adverse reactions link on my record - GP Medical Record")
    fun iClickTheAllergiesLinkOnTheAccountPage(){
        myRecordInfoPage.allergies.allergies.click()
    }

    @Then("^I see an error occurred message on My Record - GP Medical Record$")
    fun thenISeeAnErrorOccuredMessageForProblems() {
        assertTextOnPage("An error has occurred trying to retrieve this data.")
    }

    @Then("^I see a message that I have no information recorded for a specific record - GP Medical Record$")
    fun thenISeeAMessageIndicatingThatIHaveNoInformationRecorded() {
        assertTextOnPage( "No information recorded")
    }

    @Then("^I see a message indicating that I have no access to view this section on My Record - GP Medical Record$")
    fun thenISeeAMessageIndicatingThatIHaveNoAccessToViewSection() {
        assertTextOnPage("You do not currently have access to this section")
    }

    private fun assertTextOnPage(message: String) {
        val section = myRecordInfoPage.getBody(message)
        assertEquals(message, section.text)
    }
}
