package features.throttling.stepDefinitions

import cucumber.api.java.en.Then
import pages.assertIsVisible
import pages.throttling.WaitingListJoinedPage

open class WaitingListPageStepDefinitions {

    lateinit var waitingListJoinedPage: WaitingListJoinedPage

    @Then("^I see the GP Search Waiting List Joined page$")
    fun iSeeTheGPSearchWaitingListJoinedPage() {
        waitingListJoinedPage.waitingListResultsHeader.waitForElement()
        waitingListJoinedPage.assertJoined()
        waitingListJoinedPage.assertWhatToDoUntilThen()
        waitingListJoinedPage.homeButton.assertIsVisible()
    }

    @Then("^I see the GP Search Waiting List Not Joined page$")
    fun iSeeTheGPSearchWaitingListNotJoinedPage() {
        waitingListJoinedPage.waitingListResultsHeader.waitForElement()
        waitingListJoinedPage.assertNotJoined()
        waitingListJoinedPage.assertWhatToDoUntilThen()
        waitingListJoinedPage.homeButton.assertIsVisible()
    }
}

