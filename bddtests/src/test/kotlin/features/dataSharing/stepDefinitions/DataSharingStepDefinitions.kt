package features.dataSharing.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.dataSharing.steps.NdopSteps
import features.myrecord.stepDefinitions.AbstractDemographicsStepDefinitions
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.DataSharingPage
import pages.navigation.Header
import java.net.URL

class DataSharingStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var navbarSteps: NavigationSteps
    @Steps
    lateinit var ndop: NdopSteps
    @Steps
    lateinit var browser: BrowserSteps

    lateinit var dataSharing: DataSharingPage

    private val overviewId = "Overview"
    private val benefitsId = "Benefits"
    private val dataUseId = "Data Use"
    private val optOutId = "Where Opt Out Doesn't Apply"
    private val manageChoiceId = "Manage Your Choice"

    @Given("^I am on the Data Sharing page$")
    fun iAmOnTheDataSharingPage() {
        dataSharing.waitForPageHeaderText("Sharing health data preferences")
        navbarSteps.assertSelectedTab("More")
    }

    @Given("^I am on the Data Sharing (.*) page$")
    fun iAmOnTheDataSharingXPage(page: String) {
        when (page) {
            overviewId -> {
                dataSharing.onOverviewPage()
            }
            benefitsId -> {
                dataSharing.onBenefitsPage()
            }
            dataUseId -> {
                dataSharing.onDataUsePage()
            }
            optOutId -> {
                dataSharing.onWhereOptOutDoesntApplyPage()
            }
            manageChoiceId -> {
                dataSharing.onManageYourChoicePage()
            }
            else -> throw IllegalArgumentException("$page is not a valid page name.")
        }
    }

    @Given("^I click the (.*) contents link$")
    fun iClickTheXContentsLink(link: String) {
        when (link) {
            overviewId -> {
                dataSharing.linkContentsOverview.element.click()
            }
            benefitsId -> {
                dataSharing.linkContentsBenefits.element.click()
            }
            dataUseId -> {
                dataSharing.linkContentsDataUse.element.click()
            }
            optOutId -> {
                dataSharing.linkContentsWhereOptOutDoesntApply.element.click()
            }
            manageChoiceId -> {
                dataSharing.linkContentsManageYourChoice.element.click()
            }
            else -> throw IllegalArgumentException("$link is not a valid link name.")
        }
    }

    @When("^I click the Manage Your Choice direct link$")
    fun iClickTheManageYourChoiceDirectLink() {
        dataSharing.linkManageYourChoice.element.click()
    }

    @When("^I click the Data Sharing More Info link$")
    fun iClickTheDataSharingMoreInfoLink() {
        dataSharing.linkDataSharingMoreInfo.element.click()
    }

    @When("^I click the Start Now button$")
    fun iClickTheStartNowButton() {
        dataSharing.btnStartNow.element.click()
    }

    @When("^I click the (.*) button (.*) times$")
    fun iClickTheNextButtonXTimes(_button: String, _clicks: String) {
        var clicks = _clicks.toInt()
        if (clicks <= 0) {
            throw IllegalArgumentException("At least one click required")
        }

        while (clicks > 0) {
            when (_button) {
                "Next" -> dataSharing.btnNext.element.click()
                "Previous" -> dataSharing.btnPrevious.element.click()
            }
            if (--clicks != 0) {
                Thread.sleep(500)
            }
        }
    }

    @Then("I am on the Ndop website")
    fun iAmOnTheNDOPWebsite() {
        Thread.sleep(1000)
        browser.changeTab(URL(Config.instance.dataPreferencesUrl))
        assert(ndop.tokenIsDisplayed())
    }
}