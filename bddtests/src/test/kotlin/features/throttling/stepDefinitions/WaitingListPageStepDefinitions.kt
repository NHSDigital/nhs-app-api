package features.throttling.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.assertIsVisible
import pages.throttling.WaitingListJoinedPage

const val WAIT_FOR_WAITING_LIST = 1000L

open class WaitingListPageStepDefinitions {

    lateinit var waitingListJoinedPage: WaitingListJoinedPage

    @When("^I click the go to home screen button$")
    fun iClickTheGoToHomeScreenButton() {
        waitingListJoinedPage.homeButton.click()
    }

    @Then("^I see the Waiting List (Joined|Not Joined) page$")
    fun iSeeTheSendingEmailResultsPage(joinedOrNot: String) {
        Thread.sleep(WAIT_FOR_WAITING_LIST)
        waitingListJoinedPage.waitingListResultsHeader.assertIsVisible()
        when (joinedOrNot) {
            "Joined" -> {
                waitingListJoinedPage.assertJoined()
            }
            "Not Joined" -> {
                waitingListJoinedPage.assertNotJoined()
            }
        }
        waitingListJoinedPage.assertWhatToDoUntilThen()
        waitingListJoinedPage.homeButton.assertIsVisible()
    }
}

