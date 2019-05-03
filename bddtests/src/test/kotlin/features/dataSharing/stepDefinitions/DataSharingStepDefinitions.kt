package features.dataSharing.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.dataSharing.steps.NdopSteps
import features.myrecord.stepDefinitions.AbstractDemographicsStepDefinitions
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.DataSharingPage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative

private const val NEXT_BUTTON_WAIT_TIME = 500L

class DataSharingStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var navbarSteps: NavigationSteps
    @Steps
    lateinit var ndop: NdopSteps

    lateinit var dataSharing: DataSharingPage

    lateinit var headerNative: HeaderNative

    private val overviewId = "Overview"
    private val dataUseId = "Where confidential patient information is used"
    private val manageChoiceId = "Where your choice does not apply"
    private val makeChoiceId = "Make your choice"

    @Given("^I am on the Data Sharing page$")
    fun iAmOnTheDataSharingPage() {
        headerNative.waitForPageHeaderText("Find out why your data matters")
        navbarSteps.assertSelectedTab(NavBarNative.NavBarType.MORE)
    }

    @Given("^I am on the Data Sharing (.*) page$")
    fun iAmOnTheDataSharingXPage(page: String) {
        when (page) {
            overviewId -> {
                dataSharing.onOverviewPage()
            }
            dataUseId -> {
                dataSharing.onDataUsePage()
            }
            manageChoiceId -> {
                dataSharing.onManageYourChoicePage()
            }
            makeChoiceId -> {
                dataSharing.onMakeYourChoicePage()
            }
            else -> throw IllegalArgumentException("$page is not a valid page name.")
        }
    }

    @Given("^I click the (.*) contents link$")
    fun iClickTheXContentsLink(link: String) {
        when (link) {
            overviewId -> {
                dataSharing.linkContentsOverview.click()
            }
            dataUseId -> {
                dataSharing.linkContentsDataUse.click()
            }
            manageChoiceId -> {
                dataSharing.linkContentsManageYourChoice.click()
            }
            makeChoiceId -> {
                dataSharing.linkContentsMakeYourChoice.click()
            }
            else -> throw IllegalArgumentException("$link is not a valid link name.")
        }
    }

    @When("^I click the Start Now button$")
    fun iClickTheStartNowButton() {
        dataSharing.btnStartNow.click()
    }

    @When("^I click the (.*) button (.*) times$")
    fun iClickTheNextButtonXTimes(_button: String, _clicks: String) {
        var clicks = _clicks.toInt()
        if (clicks <= 0) {
            throw IllegalArgumentException("At least one click required")
        }

        while (clicks > 0) {
            when (_button) {
                "Next" -> dataSharing.btnNext.click()
                "Previous" -> dataSharing.btnPrevious.click()
            }
            if (--clicks != 0) {
                Thread.sleep(NEXT_BUTTON_WAIT_TIME)
            }
        }
    }

    @Then("I am on the Ndop website")
    fun iAmOnTheNDOPWebsite() {
        assert(ndop.tokenIsDisplayed())
    }
}
