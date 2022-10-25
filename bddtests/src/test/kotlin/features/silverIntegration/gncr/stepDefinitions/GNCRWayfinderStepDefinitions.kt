package features.silverIntegration.gncr.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.pages.GNCRWayfinderPage
import utils.SerenityHelpers

class GNCRWayfinderStepDefinitions {

    private lateinit var gncrWayfinderPage: GNCRWayfinderPage
    private val mockingClient = SerenityHelpers.getMockingClient()

    @Given("^GNCRWayfinder responds to requests for viewing an appointment$")
    fun gncrWayfinderResponsesToRequestsForViewingAnAppointment() {
        mockingClient.forGNCRWayfinder.mock { respondWithPage() }
    }

    @Then("^I am navigated to a third party site for GNCRWayfinder$")
    fun iNavigateToThirdPartySiteForGNCRWayfinder() {
        gncrWayfinderPage.assertTitleVisible()
    }
}
