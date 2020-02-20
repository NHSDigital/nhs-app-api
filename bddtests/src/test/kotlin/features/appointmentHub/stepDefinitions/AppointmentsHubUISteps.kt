package features.appointmentHub.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import mocking.MockingClient
import pages.AppointmentHubPage

open class AppointmentsHubUISteps {

    val mockingClient = MockingClient.instance

    lateinit var appointmentHubPage: AppointmentHubPage


    @Then("^the Appointments Hub page is displayed$")
    fun assertIsDisplayed() {
        appointmentHubPage.assertAppointmentsHubIsDisplayed()
        appointmentHubPage.assertLinksPresent()
    }

    @And("^I click the GP Appointments link$")
    fun clickOnGPAppointmentsButton() {
        appointmentHubPage.assertLinksPresent()
        appointmentHubPage.btnGPAppointmentsLinksWithDescriptionsContent
                .click()
    }

}
