package features.silverIntegration.accurxWayfinder.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.pages.AccurxWayfinderPage
import utils.SerenityHelpers

class AccurxWayfinderStepDefinitions {

    private lateinit var accurxWayfinderPage: AccurxWayfinderPage
    private val mockingClient = SerenityHelpers.getMockingClient()

    @Given("^AccurxWayfinder responds to requests for viewing an appointment$")
    fun accurxWayfinderResponsesToRequestsForViewingAnAppointment() {
        mockingClient.forAccurxWayfinder.mock { respondWithPage() }
    }

    @Then("^I am navigated to a third party site for AccurxWayfinder$")
    fun iNavigateToThirdPartySiteForAccurxWayfinder() {
        accurxWayfinderPage.assertTitleVisible()
    }
}
