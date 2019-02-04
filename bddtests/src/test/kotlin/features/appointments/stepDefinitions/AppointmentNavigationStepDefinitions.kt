package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentGuidanceSteps
import features.appointments.steps.AvailableAppointmentsSteps
import features.appointments.steps.MyAppointmentsUISteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.navigation.NavBarNative

class AppointmentNavigationStepDefinitions {

    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var myAppointmentsUI: MyAppointmentsUISteps
    @Steps
    lateinit var appointmentGuidanceSteps: AppointmentGuidanceSteps
    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps


    @Given("^I am on the My Appointments page$")
    fun iAmOnMyAppointmentsPage() {
        navigation.waitForSpinnerToDisappear()
        navigation.select(NavBarNative.NavBarType.APPOINTMENTS)
        waitForSpinnerToDisappear()
    }

    @Given("^I am on the Appointment Guidance page$")
    fun iAmOnTheGuidancePage() {
        iAmOnMyAppointmentsPage()
        myAppointmentsUI.clickOnBookAppointmentButton()
        waitForSpinnerToDisappear()
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
        appointmentGuidanceSteps.checkGuidanceItemsHeadersAreCorrect()
    }

    @Given("^I am on the Available Appointments page$")
    fun iAmOnTheAvailableAppointmentsPage() {
        iAmOnTheGuidancePage()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
        waitForSpinnerToDisappear()
        availableAppointments.checkIfPageHeaderIsCorrect()
    }

    @When("^I try to progress to the available appointments page$")
    fun iTryToProgressToTheAvailableAppointmentsPage() {
        iAmOnTheGuidancePage()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
    }

    private fun waitForSpinnerToDisappear() {
        myAppointmentsUI.myAppointmentsPage.waitForSpinnerToDisappear()
    }
}