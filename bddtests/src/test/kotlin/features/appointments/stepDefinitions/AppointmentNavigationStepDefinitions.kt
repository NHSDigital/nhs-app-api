package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentGuidanceSteps
import features.appointments.steps.AvailableAppointmentsSteps
import features.appointments.steps.MyAppointmentsUISteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.ErrorPage
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
    @Steps
    lateinit var errorPage: ErrorPage


    @Given("^I am on the My Appointments page$")
    fun iAmOnMyAppointmentsPage() {
        navigation.waitForSpinnerToDisappear()
        navigation.select(NavBarNative.NavBarType.APPOINTMENTS)
        waitForSpinnerToDisappear()
        myAppointmentsUI.myAppointmentsPage.
                locatorMethods.assertNativeElementsLoaded(myAppointmentsUI.myAppointmentsPage.bookButton)
    }

    @Given("^I am on the My Appointments error page$")
    fun iAmOnMyAppointmentsErrorPage() {
        navigation.waitForSpinnerToDisappear()
        navigation.select(NavBarNative.NavBarType.APPOINTMENTS)
        waitForSpinnerToDisappear()
        errorPage.locatorMethods.assertNativeElementsLoaded(errorPage.heading)
    }

    @Given("^I am on the Appointment Guidance page$")
    fun iAmOnTheGuidancePage() {
        iAmOnMyAppointmentsPage()
        myAppointmentsUI.clickOnBookAppointmentButton()
        waitForSpinnerToDisappear()
        appointmentGuidanceSteps.appointmentGuidancePage.
                locatorMethods.assertNativeElementsLoaded(appointmentGuidanceSteps.appointmentGuidancePage.bookButton)
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
        appointmentGuidanceSteps.checkGuidanceItemsHeadersAreCorrect()
    }

    @Given("^I am on the Available Appointments page$")
    fun iAmOnTheAvailableAppointmentsPage() {
        iAmOnTheGuidancePage()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
        waitForSpinnerToDisappear()
        availableAppointments.availableAppointmentsPage.
                locatorMethods.assertNativeElementsLoaded(
                    availableAppointments.availableAppointmentsPage.backToMyAppointmentsButton)
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