package features.authentication.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.authentication.steps.NavigationLinkText
import features.authentication.steps.PatientDetail
import features.organDonation.stepDefinitions.OrganDonationStepDefinitions
import features.serviceJourneyRules.stepDefinitions.ServiceJourneyRulesSerenityHelpers
import features.sharedSteps.BrowserSteps
import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.Patient
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.AppointmentHubPage
import pages.CheckMySymptomsPage
import pages.HybridPageElement
import pages.PrescriptionsHubPage
import pages.assertSingleElementPresent
import pages.navigation.NavBarNative
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.serviceJourneyRules.PublicHealthNotification
import java.net.URL

private const val SURVEY_URL = "https://in.hotjar.com/s?siteId=859152&surveyId=95785"

class HomePageStepDefinitions {

    @Steps
    private lateinit var appointmentHubPage: AppointmentHubPage
    @Steps
    private lateinit var browser: BrowserSteps
    @Steps
    private lateinit var homeSteps: HomeSteps
    @Steps
    private lateinit var loginSteps: LoginSteps
    @Steps
    private lateinit var navBar: NavBarNative
    @Steps
    private lateinit var organDonationSteps: OrganDonationStepDefinitions

    private lateinit var prescriptionsHubPage: PrescriptionsHubPage
    private lateinit var checkMySymptomsPage: CheckMySymptomsPage

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
    fun iFollowTheAppointmentsLinkFromTheHomePage() {
        val linkElement = homeSteps.assertLinkIsVisible(NavigationLinkText.APPOINTMENTS)
        followAppointmentsLink(linkElement)
    }

    @When("I follow the Messages link from the home page$")
    fun iFollowTheMessagesLinkFromTheHomePage() {
        homeSteps.assertLinkIsVisible(NavigationLinkText.MESSAGES).click()
    }

    @When("I follow the Health information and updates link from the home page$")
    fun iFollowTheHealthInformationAndUpdatesLinkFromTheHomePage() {
        homeSteps.assertLinkIsVisible(NavigationLinkText.HEALTH_INFORMATION_UPDATES).click()
    }

    @When("^I can see I have unread messages on the home page$")
    fun iCanSeeUnreadMessageIndicatorOnTheHomePage() {
        homeSteps.assertUnreadMessageIndicatorIsDisplayed()
    }

    @Then("^I see the home page$")
    fun iSeeTheHomePage() {
        homeSteps.assertHeaderVisible()
    }

    @Then("^I see the home page header$")
    fun iSeeTheHomePageHeader() {
        homeSteps.assertHeaderVisible()
    }

    @Then("^I see the linked profiles link$")
    fun iSeeLinkedProfilesLink() {
        homeSteps.assertLinkIsVisible(NavigationLinkText.LINKED_PROFILES)
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

    @Then("^I don't see my (.*) on the home page$")
    fun iDontSeeMyDetailOnTheHomePage(detail: String) {
        homeSteps.assertPatientDetailIsNotPresent(PatientDetail.fromLabel(detail))
    }

    @Then("^I see my (.*) on the home page$")
    fun iSeeMyDetailOnTheHomePage(detail: String) {
        val patient = SerenityHelpers.getPatient()
        homeSteps.assertPatientDetailIsVisible(patient, PatientDetail.fromLabel(detail))
    }

    @Then("^I see the proxy patient details of age and gp surgery$")
    fun iSeeProxyPatientDetails() {
        val selectedProfile = LinkedProfilesSerenityHelpers.SELECTED_PROFILE.getOrFail<LinkedProfileFacade>()
        homeSteps.assertProxyPatientDetailsShownFor(selectedProfile)
    }

    @Then("^I see a welcome message$")
    fun iSeeAWelcomeMessageFor() {
        val patient = SerenityHelpers.getPatient()
        homeSteps.assertWelcomeMessageShownFor(patient)
    }

    @Then("^I see a welcome message for the (.*) patient with no title$")
    fun iSeeAWelcomeMessageWithNoTitle(gpSystem: String) {
        val patient = Patient.getDefault(Supplier.valueOf(gpSystem))
        homeSteps.assertWelcomeMessageShownFor(patient, false)
    }

    @Then("^I can see the (.*) link on the homepage")
    fun iCanSeeTheSpecifiedLink(linkText: String){
        homeSteps.homePage.assertLinkIsVisible(linkText)
    }

    @Then("^I can't see the (.*) link on the homepage$")
    fun iCantSeeTheSpecifiedLink(linkText: String) {
        homeSteps.homePage.assertLinkNotPresent(linkText)
    }

    @Then("^I can see and follow the (.*) link$")
    fun iCanSeeAndFollowTheSpecifiedLink(linkText: String) {
        val linkElement = homeSteps.homePage.assertLinkIsVisible(linkText)

        when (linkText) {
            NavigationLinkText.SYMPTOMS.linkText -> followSymptomLink(linkElement)
            NavigationLinkText.APPOINTMENTS.linkText -> followAppointmentsLink(linkElement)
            NavigationLinkText.PRESCRIPTIONS.linkText -> followPrescriptionLink(linkElement)
            NavigationLinkText.MEDICAL_RECORD.linkText -> followMedicalRecordLink(linkElement)
            NavigationLinkText.ORGAN_DONATION.linkText -> followOrganDonationLink(linkElement)
            else -> Assert.fail("Test set up incorrect, there is no matching follow on function for `$linkText`")
        }
    }

    @Then("^I do not see the home page links$")
    fun iDoNotSeeTheHomePageLinks() {
        homeSteps.homePage.assertHomePageLinksNotPresent()
    }

    @Then("^I see the home screen public health notifications$")
    fun iSeeTheHomeScreenPublicHealthNotifications() {
        val publicHealthNotifications = ServiceJourneyRulesSerenityHelpers
                .HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATIONS.getOrFail<List<PublicHealthNotification>>()
        homeSteps.homePage.assertHasPublicHealthNotifications(publicHealthNotifications)
    }

    private fun followSymptomLink(linkElement: HybridPageElement) {
        linkElement.click()
        checkMySymptomsPage.assertPageDisplayed()
        navBar.isHighlighted(NavBarNative.NavBarType.SYMPTOMS)
    }

    private fun followAppointmentsLink(linkElement: HybridPageElement) {
        linkElement.click()
        appointmentHubPage.assertAppointmentsHubIsDisplayed()
        navBar.isHighlighted(NavBarNative.NavBarType.APPOINTMENTS)
    }

    private fun followPrescriptionLink(linkElement: HybridPageElement) {
        linkElement.click()
        prescriptionsHubPage.assertPrescriptionsHubIsDisplayed()
    }

    private fun followMedicalRecordLink(linkElement: HybridPageElement) {
        linkElement.click()
        navBar.isHighlighted(NavBarNative.NavBarType.MY_RECORD)
    }

    private fun followOrganDonationLink(linkElement: HybridPageElement) {
        linkElement.click()
        organDonationSteps.iAmOnTheOrganDonationPage()
    }
}

