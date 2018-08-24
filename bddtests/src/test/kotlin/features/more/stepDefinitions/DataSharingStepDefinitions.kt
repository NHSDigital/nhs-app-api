package features.more.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.more.steps.DataSharingSteps
import features.myrecord.stepDefinitions.AbstractDemographicsStepDefinitions
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps

class DataSharingStepDefinitions: AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var dataSharing: DataSharingSteps

    @When("^I am on the data sharing final page$")
    fun iAmOnTheDataSharingCompletionPage() {
        assert(dataSharing.dataSharingCompleteButtonVisible())
    }

    @Then("^I click to complete data sharing$")
    fun iClickToCompleteDataSharing() {
        dataSharing.clickCompleteDataShardTerms()
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
}