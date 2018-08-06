package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentGuidanceSteps
import features.appointments.steps.AvailableAppointmentsSteps
import features.appointments.steps.MyAppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps

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


    @Given("^I am on my appointments page$")
    fun iAmOnMyAppointmentsPage() {
        navigation.select("Appointments")
        myAppointments.waitForSpinnerToDisappear()
    }

    @Given("^I am on the guidance page$")
    fun iAmOnTheGuidancePage() {
        iAmOnMyAppointmentsPage()
        myAppointments.clickOnBookAppointmentButton()
        myAppointments.waitForSpinnerToDisappear()
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
    }

    @Given("^I am on the available appointments page$")
    fun iAmOnTheAvailableAppointmentsPage() {
        iAmOnTheGuidancePage()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
        myAppointments.waitForSpinnerToDisappear()
        availableAppointments.checkIfPageHeaderIsCorrect()
    }

    @When("^I try to progress to the available appointments page$")
    fun iTryToProgressToTheAvailableAppointmentsPage() {
        iAmOnTheGuidancePage()
        appointmentGuidanceSteps.clickBookAnAppointmentButton()
    }


}