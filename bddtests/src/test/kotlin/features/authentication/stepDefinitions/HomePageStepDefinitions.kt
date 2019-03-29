package features.authentication.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.MyAppointmentsUISteps
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.stepDefinitions.MyRecordStepDefinitions
import features.oneOneOneOnline.steps.CheckMySymptoms
import features.organDonation.stepDefinitions.OrganDonationStepDefinitions
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.assertSingleElementPresent
import pages.navigation.NavBarNative
import pages.navigation.WebFooter
import java.net.URL

private const val SURVEY_URL = "https://in.hotjar.com/s?siteId=859152&surveyId=95785"
private const val NEW_TAB_WAIT_TIME = 1000L

class HomePageStepDefinitions : AbstractSteps() {

    @Steps
    private lateinit var browser: BrowserSteps
    @Steps
    private lateinit var navBar: NavBarNative
    @Steps
    private lateinit var webFooter: WebFooter
    @Steps
    private lateinit var navHeader: NavigationSteps
    @Steps
    private lateinit var homeSteps: HomeSteps
    @Steps
    private lateinit var checkMySymptoms: CheckMySymptoms
    @Steps
    private lateinit var myAppointmentsUISteps: MyAppointmentsUISteps
    @Steps
    private lateinit var prescriptions: PrescriptionsSteps
    @Steps
    private lateinit var recordSteps: MyRecordStepDefinitions
    @Steps
    private lateinit var organDonationSteps: OrganDonationStepDefinitions
    @Steps
    private lateinit var loginSteps: LoginSteps

    @Then("^I see the beta banner$")
    fun iSeeTheBetaBanner() {
        homeSteps.homePage.assertBetaBannerVisible()
    }

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
        loginSteps.loginPage.clickHelpIcon()
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

    @Then("^I click the NHS sites link in the footer")
    fun iClickFooterNHSSitesLink() {
        followNHSSiteLink()
        browser.changeTabToHome()
    }

    @Then("^I click the about us link in the footer")
    fun iClickFooterAboutUsLink() {
        followAboutUsLink()
        browser.changeTabToHome()
    }

    @Then("^I click the contact us link in the footer")
    fun iClickFooterContactUsLink() {
        followContactUSLink()
        browser.changeTabToHome()
    }

    @Then("^I click the site map link in the footer")
    fun iClickFooterSiteMapLink() {
        followsiteMapLink()
        browser.changeTabToHome()
    }

    @Then("^I click the accessibility link in the footer")
    fun iClickFooterAccessibilityLink() {
        followAccessibilityLink()
        browser.changeTabToHome()
    }

    @Then("^I click the policies link in the footer")
    fun iClickFooterPoliciesLink() {
        followPoliciesLink()
        browser.changeTabToHome()
    }

    @Then("^I click the home link in the footer")
    fun iClickFooterHomeLink() {
        followFooterHomeLink()
    }

    private fun navigateBackToHomePage(){
        navHeader.headerNative.clickHome()
        homeSteps.assertHeaderVisible()
    }

    private fun followSymptomLink() {
        homeSteps.homePage.checkSymptomsLink.click()
        checkMySymptoms.assertConditionsHeaderVisible()
        checkMySymptoms.assertNhs111HeaderVisible()
        navBar.isHighlighted(NavBarNative.NavBarType.SYMPTOMS)
    }

    private fun followAppointmentsLink() {
        homeSteps.homePage.bookAndManageAppointmentsLink.click()
        myAppointmentsUISteps.checkHeaderTextIsCorrect()
        myAppointmentsUISteps.checkNoUpcomingAppointmentsTextIsDisplaying()
        navBar.isHighlighted(NavBarNative.NavBarType.APPOINTMENTS)
    }

    private fun followPrescriptionLink() {
        homeSteps.homePage.orderRepeatPrescriptionLink.click()
        prescriptions.isLoaded()
        navBar.isHighlighted(NavBarNative.NavBarType.PRESCRIPTIONS)
    }

    private fun followMedicalRecordLink() {
        homeSteps.homePage.viewMedicalRecordLink.click()
        recordSteps.thenISeeRecordWarningPageOpened()
        navBar.isHighlighted(NavBarNative.NavBarType.MY_RECORD)
    }

    private fun followOrganDonationLink() {
        homeSteps.homePage.organDonationLink.click()
        organDonationSteps.iAmOnTheOrganDonationPage()
    }

    private fun followNHSSiteLink() {
        webFooter.nhsSitesLink()
        aNewTabOpens(Config.instance.nhsSites)
    }

    private fun followAboutUsLink() {
        webFooter.aboutUsLink()
        aNewTabOpens(Config.instance.aboutUs)
    }

    private fun followContactUSLink() {
        webFooter.contactUsLink()
        aNewTabOpens(Config.instance.contactUs)
    }

    private fun followsiteMapLink() {
        webFooter.siteMapLink()
        aNewTabOpens(Config.instance.siteMap)
    }

    private fun followAccessibilityLink() {
        webFooter.accessibilityInformationLink()
        aNewTabOpens(Config.instance.accessibilityInformation)
    }

    private fun followPoliciesLink() {
        webFooter.policiesLink()
        aNewTabOpens(Config.instance.policies)
    }

    private fun followFooterHomeLink() {
        webFooter.homeLink()
    }

    private fun aNewTabOpens(url: String) {
        //wait required to load page as in testing the should have url will never continue without it
        Thread.sleep(NEW_TAB_WAIT_TIME)
        browser.changeTab(URL(url))
        browser.shouldHaveUrl(url)
    }
}

