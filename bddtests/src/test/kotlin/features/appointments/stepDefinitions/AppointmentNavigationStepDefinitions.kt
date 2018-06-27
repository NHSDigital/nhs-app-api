package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import features.appointments.steps.AppointmentGuidanceSteps
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
        i_am_on_the_guidance_page()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
    }
}