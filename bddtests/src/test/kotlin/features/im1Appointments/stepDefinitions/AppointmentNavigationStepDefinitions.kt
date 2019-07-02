package features.im1Appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.im1Appointments.steps.AppointmentGuidanceSteps
import features.im1Appointments.steps.AvailableAppointmentsSteps
import features.im1Appointments.steps.MyAppointmentsUISteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.ErrorPage
import pages.navigation.NavBarNative

class AppointmentNavigationStepDefinitions {

    @Steps
    lateinit var appointmentGuidanceSteps: AppointmentGuidanceSteps
    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps
    @Steps
    lateinit var errorPage: ErrorPage
    @Steps
    lateinit var myAppointmentsUI: MyAppointmentsUISteps
    @Steps
    lateinit var navigation: NavigationSteps

    @Given("^I am on the My Appointments page$")
    fun iAmOnMyAppointmentsPage() {
        navigation.select(NavBarNative.NavBarType.APPOINTMENTS)
        myAppointmentsUI.myAppointmentsPage.
                locatorMethods.assertNativeElementsLoaded(myAppointmentsUI.myAppointmentsPage.bookButton)
    }

    @Given("^I am on the My Appointments error page$")
    fun iAmOnMyAppointmentsErrorPage() {
        navigation.select(NavBarNative.NavBarType.APPOINTMENTS)
        errorPage.locatorMethods.assertNativeElementsLoaded(errorPage.heading)
    }

    @Given("^I am on the Appointment Guidance page$")
    fun iAmOnTheGuidancePage() {
        iAmOnMyAppointmentsPage()
        myAppointmentsUI.clickOnBookAppointmentButton()
        appointmentGuidanceSteps.appointmentGuidancePage.
                locatorMethods.assertNativeElementsLoaded(appointmentGuidanceSteps.appointmentGuidancePage.bookButton)
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
        appointmentGuidanceSteps.checkGuidanceItemsHeadersAreCorrect()
    }

    @Given("^I click through to the online consultations Appointment Guidance page$")
    fun iAmOnTheOnlineConsultationsGuidancePage() {
        myAppointmentsUI.clickOnBookAppointmentButton()
        appointmentGuidanceSteps.appointmentGuidancePage.
                locatorMethods.assertNativeElementsLoaded(appointmentGuidanceSteps.appointmentGuidancePage.bookButton)
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
    }

    @Given("^I am on the Available Appointments page$")
    fun iAmOnTheAvailableAppointmentsPage() {
        iTryToProgressToTheAvailableAppointmentsPage()
        availableAppointments.availableAppointmentsPage.
                locatorMethods.assertNativeElementsLoaded(
                    availableAppointments.availableAppointmentsPage.backToMyAppointmentsButton)
        availableAppointments.checkIfPageHeaderIsCorrect()
    }

    @When("^I try to progress to the available appointments page$")
    fun iTryToProgressToTheAvailableAppointmentsPage() {
        iAmOnTheGuidancePage()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
    }
}
