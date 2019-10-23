package features.im1Appointments.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.im1Appointments.steps.AppointmentGuidanceSteps
import net.thucydides.core.annotations.Steps

class AppointmentGuidanceStepDefinitions {

    @Steps
    lateinit var appointmentGuidanceSteps: AppointmentGuidanceSteps

    @When("^I select Appointment Guidance Page Check symptoms button$")
    @Throws(Exception::class)
    fun whenISelectAppointmentGuidancePageCheckSymptomsButton() {
        appointmentGuidanceSteps.appointmentGuidancePage.menuCheckSymptomsButton.click()
    }

    @When("^I select the Appointment Guidance Check symptoms menu item$")
    @Throws(Exception::class)
    fun whenISelectAppointmentGuidancePageCheckSymptomsMenuItem() {
        appointmentGuidanceSteps.appointmentGuidancePage.menuCheckSymptomsButton.click()
    }

    @When("^I am given guidance as to my options with OLC enabled before booking$")
    @Throws(Exception::class)
    fun thenIAmGivenGuidanceAsToMyOptionsBeforeBookingAnAppointmentOLCEnabled() {
        appointmentGuidanceSteps.checkGuidanceItemsAreCorrectOLCEnabled()
    }

    @Then("^I am given guidance as to my options before booking an appointment$")
    @Throws(Exception::class)
    fun thenIAmGivenGuidanceAsToMyOptionsBeforeBookingAnAppointment() {
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
        appointmentGuidanceSteps.checkGuidanceItemsHeadersAreCorrect()
    }

    @Then("^I am on the Appointments Guidance page$")
    fun thenIAmOnTheAppointmentsGuidancePage() {
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
    }
}
