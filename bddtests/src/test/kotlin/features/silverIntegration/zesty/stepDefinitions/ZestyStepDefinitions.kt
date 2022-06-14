package features.silverIntegration.zesty.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.pages.ZestyPage
import utils.SerenityHelpers

class ZestyStepDefinitions {

    private lateinit var zestyPage: ZestyPage
    private val mockingClient = SerenityHelpers.getMockingClient()

    @Given("^Zesty responds to requests for viewing an appointment$")
    fun zestyResponsesToRequestsForViewingAnAppointment() {
        mockingClient.forZesty.mock { respondWithPage() }
    }

    @Then("^I am navigated to a third party site for Zesty$")
    fun iNavigateToThirdPartySiteForZesty() {
        zestyPage.assertTitleVisible()
    }
}
