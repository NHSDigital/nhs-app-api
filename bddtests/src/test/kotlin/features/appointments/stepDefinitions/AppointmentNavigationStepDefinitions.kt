package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentGuidanceSteps
import features.appointments.steps.AvailableAppointmentsSteps
import features.appointments.steps.MyAppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.MockDefaults
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.HybridPageObject
import pages.appointments.AvailableAppointmentsPage
import java.net.URI

class AppointmentNavigationStepDefinitions {
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var myAppointments: MyAppointmentsSteps
    @Steps
    lateinit var appointmentGuidanceSteps: AppointmentGuidanceSteps
    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage

    @Given("^I am on my appointments page$")
    fun iAmOnMyAppointmentsPage() {
        val patient = Serenity.sessionVariableCalled<Patient>(Patient::class)
        browser.goToApp()
        login.asDefault(patient ?: MockDefaults.patient)
        navigation.select("Appointments")
    }

    @Given("^I am on the guidance page$")
    fun iAmOnTheGuidancePage() {
        iAmOnMyAppointmentsPage()
        myAppointments.clickOnBookAppointmentButton()
    }

    @Given("^I am on the available appointments page$")
    fun iAmOnTheAvailableAppointmentsPage() {
        // TODO remove if-else statement when TPP upcoming appointment stubs are created - NHSO-1672
        val patient = Serenity.sessionVariableCalled<Patient>(Patient::class)
        Assert.assertNotNull("Patient not initialised. ", patient)
        navigation.select("Appointments")
        if (patient.firstName == "Kevin" && patient.surname == "Barry") {
            availableAppointmentsPage.driver.get(URI(availableAppointmentsPage.driver.currentUrl).resolve("appointments/booking").toString())
        } else {
            myAppointments.clickOnBookAppointmentButton()
            appointmentGuidanceSteps.clickBookAnAppointmentButton()
        }
        availableAppointments.checkIfPageHeaderIsCorrect()
    }

    @When("^I try to progress to the available appointments page$")
    fun iTryToProgressToTheAvailableAppointmentsPage() {
        iAmOnTheGuidancePage()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
    }
}