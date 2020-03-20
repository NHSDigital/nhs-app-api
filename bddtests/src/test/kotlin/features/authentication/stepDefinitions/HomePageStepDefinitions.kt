package features.authentication.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.im1Appointments.steps.YourAppointmentsUISteps
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.stepDefinitions.MedicalRecordWarningStepDefinitions
import features.oneOneOneOnline.steps.CheckMySymptoms
import features.organDonation.stepDefinitions.OrganDonationStepDefinitions
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mockingFacade.linkedProfiles.LinkedProfileFacade
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.assertElementNotPresent
import pages.assertSingleElementPresent
import pages.navigation.NavBarNative
import pages.AppointmentHubPage
import pages.prescription.PrescriptionsPage
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import java.net.URL

private const val SURVEY_URL = "https://in.hotjar.com/s?siteId=859152&surveyId=95785"

class HomePageStepDefinitions {

    @Steps
    private lateinit var appointmentHubPage: AppointmentHubPage
    @Steps
    private lateinit var browser: BrowserSteps
    @Steps
    private lateinit var checkMySymptoms: CheckMySymptoms
    @Steps
    private lateinit var homeSteps: HomeSteps
    @Steps
    private lateinit var loginSteps: LoginSteps
    @Steps
    private lateinit var myAppointmentsUISteps: YourAppointmentsUISteps
    @Steps
    private lateinit var navBar: NavBarNative
    @Steps
    private lateinit var navHeader: NavigationSteps
    @Steps
    private lateinit var organDonationSteps: OrganDonationStepDefinitions
    @Steps
    private lateinit var recordWarning: MedicalRecordWarningStepDefinitions

    private lateinit var prescriptions: PrescriptionsPage

    @Given("^I am at the login page")
    fun givenIAmAtTheLoginPage() {
        browser.goToApp()
        loginSteps.loginPage.shouldBeDisplayed()
    }

    @Given("^I see the help icon on the login page")
    fun givenISeeTheHelpIconOnTheLoginPage() {
        loginSteps.loginPage.helpIconIsVisible()
    }

    @When("^I click the help icon on the login page")
    fun iClickTheHelpIconOnTheLoginPage() {
        browser.storeCurrentTabCount()
        loginSteps.loginPage.clickHelpIcon()
    }

    @When("I follow the Appointments link from the home page$")
    fun iFollowTheAppointmentsLinkFromHomePage() {
        followAppointmentsLink()
    }

    @When("I follow the Messages link from the home page$")
    fun iFollowTheMesaagesLinkFromHomePage() {
        followMessagesLink()
    }

    @Then("^I see the home page$")
    fun iSeeTheHomePage() {
        homeSteps.assertHeaderVisible()
    }

    @Then("^I see the home page header$")
    fun iSeeTheHomePageHeader() {
        homeSteps.assertHeaderVisible()
    }

    @Then("^I see the current app version")
    fun iSeeTheCurrentAppVersion() {
        homeSteps.homePage.assertVersionNumberVisible()
    }

    @Then("^I see a collapsible link to a survey, which I can follow$")
    fun iSeeTheSurveyLink() {
        homeSteps.homePage.assertSurveyLinkCollapsibleAndExpandable()

        homeSteps.homePage.assertSurveyLinkContent()
        homeSteps.homePage.assertSurveyLinkCollapsibleAndExpandable()
        homeSteps.homePage.surveyContentLink.assertSingleElementPresent().click()
        browser.changeTab(URL(SURVEY_URL))
        browser.shouldHaveUrl(SURVEY_URL)
    }

    @Then("I see the patient details of name, date of birth and NHS number$")
    fun iSeePatientDetails() {
        val patient = SerenityHelpers.getPatient()
        val regex = """${'^'}${'['}0-9${']'}${'{'}10${'}'}${'$'}""".toRegex()
        Assert.assertTrue("Test Setup Incorrect: Patient must have unformatted nhs number " +
                "to check front end formatting. Regex: '$regex' Number: '${patient.nhsNumbers.first()}' ",
                regex.containsMatchIn(patient.nhsNumbers.first()))
        homeSteps.assertPatientDetailsShownFor(patient)
    }

    @Then("I see the proxy patient details of age and gp surgery$")
    fun iSeeProxyPatientDetails() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        homeSteps.assertProxyPatientDetailsShownFor(selectedProfile)
    }

    @Then("the link to Messages is not available on the Home page")
    fun theLinkToMessagesIsNotAvailableOnTheHomePage() {
        homeSteps.homePage.messagesLink.assertElementNotPresent()
    }

    @Then("^I see a welcome message$")
    fun iSeeAWelcomeMessageFor() {
        val patient = SerenityHelpers.getPatient()
        homeSteps.assertWelcomeMessageShownFor(patient)
    }

    @Then("I see and can follow links within the home page body$")
    fun iSeeAndCanFollowLinksWithinTheHomePageBody() {
        homeSteps.homePage.assertLinksPresentWithinHomePageBody()

        val linksToFollow = arrayListOf(
                { followSymptomLink() },
                { followAppointmentsLink() },
                { followPrescriptionLink() },
                { followMedicalRecordLink() },
                { followOrganDonationLink() }
        )

        Assert.assertEquals("Test Setup Incorrect. Expected Number of links does not match those to follow. " +
                "This test must be updated if a link is added or removed.",
                homeSteps.homePage.expectedLinks.count(),
                linksToFollow.count())

        linksToFollow.forEachIndexed { index, link ->
            link.invoke()
            if (index != linksToFollow.size - 1)
                navigateBackToHomePage()
        }
    }

    @And("I do not see the home page links")
    fun iDoNotSeeTheHomePageLinks() {
        homeSteps.homePage.assertHomePageLinksNotPresent()
    }

    private fun navigateBackToHomePage(){
        navHeader.headerNative.clickHome()
        homeSteps.assertHeaderVisible()
    }

    private fun followSymptomLink() {
        homeSteps.homePage.checkSymptomsLink.click()
        checkMySymptoms.assertConditionsHeaderVisible()
        checkMySymptoms.assertNhs111HeaderVisible()
        checkMySymptoms.assertCoronaHeaderVisible()
        navBar.isHighlighted(NavBarNative.NavBarType.SYMPTOMS)
    }

    private fun followAppointmentsLink() {
        homeSteps.homePage.bookAndManageAppointmentsLink.click()
        appointmentHubPage.assertAppointmentsHubIsDisplayed()
        navBar.isHighlighted(NavBarNative.NavBarType.APPOINTMENTS)
    }

    private fun followMessagesLink() {
        homeSteps.homePage.messagesLink.click()
    }

    private fun followPrescriptionLink() {
        homeSteps.homePage.orderRepeatPrescriptionLink.click()
        prescriptions.isLoaded()
        navBar.isHighlighted(NavBarNative.NavBarType.PRESCRIPTIONS)
    }

    private fun followMedicalRecordLink() {
        homeSteps.homePage.viewMedicalRecordLink.click()
        recordWarning.thenISeeRecordWarningPageOpened()
        navBar.isHighlighted(NavBarNative.NavBarType.MY_RECORD)
    }

    private fun followOrganDonationLink() {
        homeSteps.homePage.organDonationLink.click()
        organDonationSteps.iAmOnTheOrganDonationPage()
    }
}

