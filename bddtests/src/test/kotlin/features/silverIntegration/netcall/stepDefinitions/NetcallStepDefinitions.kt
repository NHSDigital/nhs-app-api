package features.silverIntegration.netcall.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import mocking.pages.NetcallPage
import pages.HybridPageObject
import utils.SerenityHelpers

class NetcallStepDefinitions : HybridPageObject() {

    private lateinit var netcallPage: NetcallPage
    private val mockingClient = SerenityHelpers.getMockingClient()

    @Given("^Netcall responds to requests for viewing an appointment$")
    fun netcallResponsesToRequestsForViewingAnAppointment() {
        mockingClient.forNetcall.mock { respondWithPage() }
    }

    @Then("^I am navigated to a third party site for Netcall$")
    fun iNavigateToThirdPartySiteForNetcall() {
        netcallPage.assertTitleVisible()
    }
}
