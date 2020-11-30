package features.authentication.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.authentication.steps.NavigationLinkText
import features.authentication.steps.PatientDetail
import features.serviceJourneyRules.stepDefinitions.ServiceJourneyRulesSerenityHelpers
import features.sharedSteps.BrowserSteps
import mockingFacade.linkedProfiles.LinkedProfileFacade
import net.thucydides.core.annotations.Steps
import org.junit.Assert
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
    private lateinit var browser: BrowserSteps
    @Steps
    private lateinit var homeSteps: HomeSteps
    @Steps
    private lateinit var loginSteps: LoginSteps
    @Steps
    private lateinit var navBar: NavBarNative

    private lateinit var prescriptionsHubPage: PrescriptionsHubPage

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

    @When("I follow the unread messages link from the home page$")
    fun iFollowTheUnreadMessagesLinkFromTheHomePage() {
        homeSteps.assertLinkIsVisible(NavigationLinkText.UNREAD_MESSAGES).click()
    }

    @When("I follow the Messages link from the home page$")
    fun iFollowTheMessagesLinkFromTheHomePage() {
        homeSteps.assertLinkIsVisible(NavigationLinkText.MESSAGES).click()
    }

    @When("^I can see I have unread messages on the home page$")
    fun iCanSeeUnreadMessageIndicatorOnTheHomePage() {
        homeSteps.assertUnreadMessageIndicatorIsDisplayed()
    }

    @Then("^I see the home page$")
    fun iSeeTheHomePage() {
        homeSteps.assertHeaderVisible()
    }

    @Then("^I see the proxy home page$")
    fun iSeeTheProxyHomePage() {
        homeSteps.assertProxyHeaderVisible()
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

    @Then("^I can't see the (.*) link on the homepage$")
    fun iCantSeeTheSpecifiedLink(linkText: String) {
        homeSteps.homePage.assertLinkNotPresent(linkText)
    }

    @Then("^I can see and follow the (.*) link$")
    fun iCanSeeAndFollowTheSpecifiedLink(linkText: String) {
        val linkElement = homeSteps.homePage.assertLinkIsVisible(linkText)

        when (linkText) {
            NavigationLinkText.PRESCRIPTIONS.linkText -> followPrescriptionLink(linkElement)
            NavigationLinkText.GP_HEALTH_RECORD.linkText -> followGPHealthRecordLink(linkElement)
            NavigationLinkText.MESSAGES.linkText -> followMessagesLink(linkElement)
            NavigationLinkText.LINKED_PROFILES.linkText -> followLinkedProfilesLink(linkElement)
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

    private fun followPrescriptionLink(linkElement: HybridPageElement) {
        linkElement.click()
        prescriptionsHubPage.assertPrescriptionsHubIsDisplayed()
    }

    private fun followGPHealthRecordLink(linkElement: HybridPageElement) {
        linkElement.click()
        navBar.isHighlighted(NavBarNative.NavBarType.YOUR_HEALTH)
    }

    private fun followMessagesLink(linkElement: HybridPageElement) {
        linkElement.click()
        navBar.isHighlighted(NavBarNative.NavBarType.MESSAGES)
    }

    private fun followLinkedProfilesLink(linkElement: HybridPageElement) {
        linkElement.click()
    }
}

