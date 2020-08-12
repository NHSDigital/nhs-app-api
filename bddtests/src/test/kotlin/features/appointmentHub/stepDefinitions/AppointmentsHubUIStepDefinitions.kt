package features.appointmentHub.stepDefinitions

import io.cucumber.java.en.When
import io.cucumber.java.en.Then
import pages.AppointmentHubPage

class AppointmentsHubUIStepDefinitions {

    private lateinit var appointmentHubPage: AppointmentHubPage

    @When("^I click the GP Appointments link$")
    fun clickOnGPAppointmentsButton() {
        appointmentHubPage.btnGPAppointmentsLinksWithDescriptionsContent.click()
    }

    @When("^I click the 'Hospital and other services' link on the Appointments Hub$")
    fun iClickTheHospitalAndOtherServicesLinkOnTheAppointmentsHub() {
        appointmentHubPage.hospitalAppointmentsLink.click()
    }

    @Then("^the Appointments Hub page is displayed$")
    fun assertIsDisplayed() {
        appointmentHubPage.assertAppointmentsHubIsDisplayed()
    }

    @Then("^the 'Hospital and other services' link is not available on the Appointments Hub$")
    fun theHospitalAndOtherServicesLinkIsNotAvailableOnTheAppointmentsHub() {
        appointmentHubPage.hospitalAppointmentsLink.assertElementNotPresent()
    }
}
