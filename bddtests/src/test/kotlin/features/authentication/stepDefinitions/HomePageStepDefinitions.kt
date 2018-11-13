package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.appointments.steps.MyAppointmentsSteps
import features.authentication.steps.HomeSteps
import features.myrecord.stepDefinitions.MyRecordStepDefinitions
import features.oneOneOneOnline.steps.CheckMySymptoms
import features.organDonation.stepDefinitions.OrganDonationStepDefinitions
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.navigation.NavBarNative
import java.net.URL

private const val surveyUrl = "https://in.hotjar.com/s?siteId=859152&surveyId=95785"
private const val organDonationUrl = "https://www.organdonation.nhs.uk/"

class HomePageDefinitions : AbstractSteps() {

    @Steps
    private lateinit var browser: BrowserSteps
    @Steps
    private lateinit var navBar: NavBarNative
    @Steps
    private lateinit var navHeader: NavigationSteps
    @Steps
    private lateinit var homeSteps: HomeSteps
    @Steps
    private lateinit var checkMySymptoms: CheckMySymptoms
    @Steps
    private lateinit var myAppointmentsSteps: MyAppointmentsSteps
    @Steps
    private lateinit var prescriptions: PrescriptionsSteps
    @Steps
    private lateinit var recordSteps: MyRecordStepDefinitions
    @Steps
    private lateinit var organDonationSteps: OrganDonationStepDefinitions

    @Then("^I see the beta banner$")
    fun iSeeTheBetaBanner() {
        homeSteps.homePage.assertBetaBannerVisible()
    }

    @Given("^I am at the login page")
    fun givenIAmAtTheLoginPage() {
        browser.goToApp()
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
        browser.changeTab(URL(surveyUrl))
        browser.shouldHaveUrl(surveyUrl)
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
        myAppointmentsSteps.checkHeaderTextIsCorrect()
        myAppointmentsSteps.checkNoUpcomingAppointmentsTextIsDisplaying()
        navBar.isHighlighted(NavBarNative.NavBarType.APPOINTMENTS)
    }

    private fun followPrescriptionLink() {
        homeSteps.homePage.orderRepeatPrescriptionLink.click()
        prescriptions.isLoaded()
        navBar.isHighlighted(NavBarNative.NavBarType.PRESCRIPTIONS)
    }

    private fun followMedicalRecordLink() {
        homeSteps.homePage.viewMedicalRecordLink.click()
        recordSteps.i_see_record_warning_page_opened()
        navBar.isHighlighted(NavBarNative.NavBarType.MY_RECORD)
    }
    private fun followOrganDonationLink() {
        homeSteps.homePage.organDonationLink.click()
        organDonationSteps.iAmOnTheOrganDonationPage()
    }
}

