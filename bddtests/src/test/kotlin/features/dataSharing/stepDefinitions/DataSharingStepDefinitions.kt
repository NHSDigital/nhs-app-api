package features.dataSharing.stepDefinitions

import config.Config
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.dataSharing.steps.DataSharingSteps
import features.dataSharing.steps.NdopSteps
import features.myrecord.stepDefinitions.AbstractDemographicsStepDefinitions
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import java.net.URL

class DataSharingStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var dataSharing: DataSharingSteps
    @Steps
    lateinit var ndop: NdopSteps
    @Steps
    lateinit var browser: BrowserSteps

    private val overviewId = "Overview"
    private val benefitsId = "Benefits"
    private val dataUseId = "Data Use"
    private val optOutId = "Where Opt Out Doesn't Apply"
    private val manageChoiceId = "Manage Your Choice"

    @Given("^I am on the Data Sharing page$")
    fun iAmOnTheDataSharingPage() {
        dataSharing.assertIsDisplayed()
    }

    @Given("^I am on the Data Sharing (.*) page$")
    fun iAmOnTheDataSharingXPage(page: String) {
        when(page) {
            overviewId -> { assert(dataSharing.isOverviewTitleVisible()) }
            benefitsId -> { assert(dataSharing.isBenefitsTitleVisible()) }
            dataUseId -> { assert(dataSharing.isDataUseTitleVisible()) }
            optOutId -> { assert(dataSharing.isWhereOptOutDoesntApplyTitleVisible()) }
            manageChoiceId -> { assert(dataSharing.isManageYourChoiceTitleVisible()) }
            else -> throw IllegalArgumentException("$page is not a valid page name.")
        }
    }

    @Given("^I click the (.*) contents link$")
    fun iClickTheXContentsLink(link: String) {
        when(link) {
            overviewId -> { dataSharing.clickOverviewContentsLink() }
            benefitsId -> { dataSharing.clickBenefitsContentsLink() }
            dataUseId -> { dataSharing.clickDataUseContentsLink() }
            optOutId -> { dataSharing.clickWhereOptOutDoesntApplyContentsLink() }
            manageChoiceId -> { dataSharing.clickManageYourChoiceContentsLink() }
            else -> throw IllegalArgumentException("$link is not a valid link name.")
        }
    }

    @When("^I click the Manage Your Choice direct link$")
    fun iClickTheManageYourChoiceDirectLink() {
        dataSharing.clickManageYourChoiceLink()
    }

    @When("^I click the Data Sharing More Info link$")
    fun iClickTheDataSharingMoreInfoLink() {
        dataSharing.clickDataSharingMoreInfoLink()
    }

    @When("^I click the Start Now button$")
    fun iClickTheStartNowButton() {
        dataSharing.clickStartNowButton()
    }

    @When("^I click the (.*) button (.*) times$")
    fun iClickTheNextButtonXTimes(_button: String, _clicks: String) {
        var clicks = _clicks.toInt()
        if (clicks <= 0) {
            throw IllegalArgumentException("At least one click required")
        }

        while(clicks > 0) {
            when(_button) {
                "Next" -> dataSharing.clickNextButton()
                "Previous" -> dataSharing.clickPreviousButton()
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