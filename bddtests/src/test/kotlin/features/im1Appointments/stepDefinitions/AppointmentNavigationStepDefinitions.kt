package features.im1Appointments.stepDefinitions

import features.im1Appointments.steps.AvailableAppointmentsSteps
import features.im1Appointments.steps.YourAppointmentsUISteps
import features.sharedSteps.NavigationSteps
import io.cucumber.java.en.Given
import net.thucydides.core.annotations.Steps
import pages.AppointmentHubPage
import pages.navigation.NavBarNative

class AppointmentNavigationStepDefinitions {

    @Steps
    lateinit var appointmentsHubPage: AppointmentHubPage
    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps
    @Steps
    lateinit var myAppointmentsUI: YourAppointmentsUISteps
    @Steps
    lateinit var navigation: NavigationSteps

    @Given("^I am on the Your Appointments page$")
    fun iAmOnMyAppointmentsPage() {
        navigation.select(NavBarNative.NavBarType.APPOINTMENTS)
        appointmentsHubPage.assertAppointmentsHubIsDisplayed()
        appointmentsHubPage.btnGPAppointmentsLinksWithDescriptionsContent.click()
        myAppointmentsUI.yourAppointmentsPage.
        locatorMethods.assertNativeElementsLoaded(myAppointmentsUI.yourAppointmentsPage.bookButton)
    }

    @Given("^I am on the Available Appointments page$")
    fun iAmOnTheAvailableAppointmentsPage() {
        iAmOnMyAppointmentsPage()
        myAppointmentsUI.clickOnBookAppointmentButton()
        availableAppointments.availableAppointmentsPage.assertPageFullyLoaded()
        availableAppointments.checkIfPageHeaderIsCorrect()
    }
}
