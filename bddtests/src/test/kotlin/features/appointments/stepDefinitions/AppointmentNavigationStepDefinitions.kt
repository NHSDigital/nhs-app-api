package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentGuidanceSteps
import features.appointments.steps.AppointmentsBookingSteps
import features.appointments.steps.AppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps

class AppointmentNavigationStepDefinitions {
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var appointments: AppointmentsSteps
    @Steps
    lateinit var appointmentGuidanceSteps: AppointmentGuidanceSteps
    @Steps
    lateinit var appointmentsBooking: AppointmentsBookingSteps

    val bookAnAppiontmentButtonText = "Book an appointment"

    @Given("^I am on the appointments page$")
    fun iAmOnTheAppointmentsPage() {
        browser.goToApp()
        login.asDefault()
        navigation.select("appointments")
    }

    @Given("^I am on the guidance page$")
    @Throws(Exception::class)
    fun i_am_on_the_guidance_page() {
        iAmOnTheAppointmentsPage()
        appointments.clickOnBookAppointmentButton()
    }

    @Given("^I am on the appointments booking page$")
    fun iAmOnTheAppointmentsBookingPage() {
        iTryToProgressToTheAppointmentsBookingPage()
        appointmentsBooking.checkIfPageHeaderIsCorrect()
    }

    @When("^I try to progress to the appointments booking page$")
    fun iTryToProgressToTheAppointmentsBookingPage() {
        i_am_on_the_guidance_page()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
    }
}