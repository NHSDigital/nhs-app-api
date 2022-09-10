package features.silverIntegration.healthcarecomms.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.pages.HealthcareCommsPage
import utils.SerenityHelpers

class HealthcareCommsStepDefinitions {

    private lateinit var healthcareCommsPage: HealthcareCommsPage
    private val mockingClient = SerenityHelpers.getMockingClient()

    @Given("^HealthcareComms responds to requests for viewing an appointment$")
    fun healthcareCommsResponsesToRequestsForViewingAnAppointment() {
        mockingClient.forHealthcareComms.mock { respondWithPage() }
    }

    @Then("^I am navigated to a third party site for HealthcareComms$")
    fun iNavigateToThirdPartySiteForHealthcareComms() {
        healthcareCommsPage.assertTitleVisible()
    }
}
