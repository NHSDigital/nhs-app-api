package features.im1Appointments.stepDefinitions

import features.im1Appointments.steps.AvailableAppointmentsSteps
import features.im1Appointments.steps.YourAppointmentsUISteps
import io.cucumber.java.en.Given
import net.thucydides.core.annotations.Steps
import pages.AppointmentHubPage
import pages.assertIsVisible
import pages.navigation.WebHeader

class AppointmentNavigationStepDefinitions {

    @Steps
    lateinit var appointmentsHubPage: AppointmentHubPage
    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps
    @Steps
    lateinit var myAppointmentsUI: YourAppointmentsUISteps
    lateinit var webHeader: WebHeader

    @Given("^I am on the Your Appointments page$")
    fun  iAmOnMyAppointmentsPage() {
        webHeader.clickAppointmentsPageLink()
        appointmentsHubPage.assertAppointmentsHubIsDisplayed()
        appointmentsHubPage.btnGPAppointmentsLinksWithDescriptionsContent.click()
        myAppointmentsUI.yourAppointmentsPage.bookButton.assertIsVisible()
    }

    @Given("^I am on the Available Appointments page$")
    fun iAmOnTheAvailableAppointmentsPage() {
        iAmOnMyAppointmentsPage()
        myAppointmentsUI.clickOnBookAppointmentButton()
        availableAppointments.availableAppointmentsPage.assertPageFullyLoaded()
    }
}
