package features.silverIntegration.drDoctor.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.pages.DrDoctorPage
import utils.SerenityHelpers

class DrDoctorStepDefinitions {

    private lateinit var drDoctorPage: DrDoctorPage
    private val mockingClient = SerenityHelpers.getMockingClient()

    @Given("^DrDoctor responds to requests for viewing an appointment$")
    fun drDoctorResponsesToRequestsForViewingAnAppointment() {
        mockingClient.forDrDoctor.mock { respondWithPage() }
    }

    @Then("^I am navigated to a third party site for DrDoctor$")
    fun iNavigateToThirdPartySiteForDrDoctor() {
        drDoctorPage.assertTitleVisible()
    }
}
