package features.authentication.stepDefinitions

import cucumber.api.java.en.Then
import features.appointments.steps.MyAppointmentsSteps
import features.authentication.steps.HomeSteps
import features.myrecord.stepDefinitions.MyRecordStepDefinitions
import features.oneOneOneOnline.Steps.CheckMySymptoms
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.navigation.NavBar
import java.net.URL

private const val surveyUrl = "https://in.hotjar.com/s?siteId=859152&surveyId=95785"
private const val organDonationUrl = "https://www.organdonation.nhs.uk/"

class HomePageDefinitions : AbstractSteps() {

    @Steps
    private lateinit var browser: BrowserSteps
    @Steps
    private lateinit var navBar: NavBar
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

    @Then("^I see the beta banner$")
    fun iSeeTheBetaBanner() {
        homeSteps.homePage.assertBetaBannerVisible()
    }

    @Then("^I see a collapsible link to a survey, which I can follow$")
    fun iSeeTheSurveyLink() {
        homeSteps.homePage.assertSurveyLinkCollapsibleAndExpandable()

        homeSteps.homePage.assertSurveyLinkContent()
        homeSteps.homePage.assertSurveyLinkCollapsibleAndExpandable()
        homeSteps.homePage.surveyContentLink.assertSingleElementPresent().element.click()
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

        Assert.assertEquals("Test Setup Incorrect. Expected Number of links does not match those to follow. This test must be updated if a link is added or removed.",
                homeSteps.homePage.expectedLinks.count(),
                linksToFollow.count())

        linksToFollow.forEachIndexed { index, link ->
            link.invoke()
            if (index != linksToFollow.size - 1)
                navigateBackToHomePage()
        }
    }

    private fun navigateBackToHomePage(){
        navHeader.header.clickHome()
        homeSteps.assertHeaderVisible()
    }

    private fun followSymptomLink() {
        homeSteps.homePage.checkSymptomsLink.element.click()
        checkMySymptoms.assertConditionsHeaderVisible()
        checkMySymptoms.assertNhs111HeaderVisible()
        navBar.isHighlighted(NavBar.NavBarType.SYMPTOMS)
    }

    private fun followAppointmentsLink() {
        homeSteps.homePage.bookAndManageAppointmentsLink.element.click()
        myAppointmentsSteps.checkHeaderTextIsCorrect()
        myAppointmentsSteps.checkNoUpcomingAppointmentsTextIsDisplaying()
        navBar.isHighlighted(NavBar.NavBarType.APPOINTMENTS)
    }

    private fun followPrescriptionLink() {
        homeSteps.homePage.orderRepeatPrescriptionLink.element.click()
        prescriptions.isLoaded()
        navBar.isHighlighted(NavBar.NavBarType.PRESCRIPTIONS)
    }

    private fun followMedicalRecordLink() {
        homeSteps.homePage.viewMedicalRecordLink.element.click()
        recordSteps.i_see_record_warning_page_opened()
        navBar.isHighlighted(NavBar.NavBarType.MY_RECORD)
    }
    private fun followOrganDonationLink() {
        homeSteps.homePage.organDonationLink.element.click()
        browser.changeTab(URL(organDonationUrl))
        browser.shouldHaveUrl(organDonationUrl)
    }
}

