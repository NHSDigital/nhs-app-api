package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentGuidanceSteps
import features.appointments.steps.AvailableAppointmentsSteps
import features.appointments.steps.MyAppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.appointments.AvailableAppointmentsPage
import java.net.URI

class AppointmentNavigationStepDefinitions {
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
        navigation.select("Appointments")
    }

    @Given("^I am on the guidance page$")
    fun iAmOnTheGuidancePage() {
        navigation.select("Appointments")
        myAppointments.clickOnBookAppointmentButton()
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
    }

    @Given("^I am on the available appointments page$")
    fun iAmOnTheAvailableAppointmentsPage() {
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
        navigation.select("Appointments")
        myAppointments.clickOnBookAppointmentButton()
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
    }
}