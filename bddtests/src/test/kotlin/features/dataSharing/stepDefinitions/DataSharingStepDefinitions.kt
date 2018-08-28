package features.dataSharing.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.dataSharing.steps.DataSharingSteps
import features.myrecord.stepDefinitions.AbstractDemographicsStepDefinitions
import net.thucydides.core.annotations.Steps

class DataSharingStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var dataSharing: DataSharingSteps

    @When("^I am on the data sharing final page$")
    fun iAmOnTheDataSharingCompletionPage() {
        assert(dataSharing.dataSharingCompleteButtonVisible())
    }

    @Then("^a new web page opens for Ndop$")
    fun aNewWebPageOpensForNdop() {
        assert(dataSharing.isNdopTestTextIsVisible())
    }

    @Given("^Ndop wiremock is set up$")
    fun ndopWiremockIsSetUp() {
        mockingClient.forNdop {
            linkToNdopRequest()
                    .respondWithNdopMockPage()
        }

    }

    @Given("^I am on the Data Sharing page$")
    fun iAmOnTheDataSharingPage() {
        assert(dataSharing.isDisplayed())
    }

    @Given("^I am on the Data Sharing Overview page$")
    fun iAmOnTheDataSharingOverviewPage(): Boolean {
        return dataSharing.isOverviewTitleVisible()
    }

    @Given("^I am on the Data Sharing Manage Your Choice page$")
    fun iAmOnTheDataSharingManageYourChoicePage(): Boolean {
        return dataSharing.isManageYourChoiceTitleVisible()
    }

    @When("^I click the Overview contents link$")
    fun iClickTheOverviewContentsLink() {
        dataSharing.clickOverviewLink()
    }

    @When("^I click the Manage Your Choice contents link$")
    fun iClickTheManageYourChoiceContentsLink() {
        dataSharing.clickManageYourChoiceLink()
    }

    @When("^I click the Next button$")
    fun iClickTheNextButton() {
        dataSharing.clickNextButton()
    }

    @When("^I click the Previous button$")
    fun iClickThePreviousButton() {
        dataSharing.clickPreviousButton()
    }

    @When("^I click the Start Now button$")
    fun iClickTheStartNowButton() {
        dataSharing.clickStartNowButton()
    }

    @Then("^I am taken to Data Sharing Overview page$")
    fun iAmTakenToTheDataSharingOverviewPage() {
        assert(dataSharing.isOverviewTitleVisible())
    }

    @Then("^I am taken to Data Sharing Manage Your Choice page$")
    fun iAmTakenToTheDataSharingManageYourChoicePage() {
        assert(dataSharing.isManageYourChoiceTitleVisible());
    }
}