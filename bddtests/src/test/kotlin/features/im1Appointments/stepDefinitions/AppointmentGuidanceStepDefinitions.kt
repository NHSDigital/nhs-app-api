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
        appointmentGuidanceSteps.appointmentGuidancePage.checkSymptomsButton.click()
    }

    //online-consultation-journey
    @When("^I select the Appointment Guidance Check symptoms menu item$")
    @Throws(Exception::class)
    fun whenISelectAppointmentGuidancePageCheckSymptomsMenuItem() {
        appointmentGuidanceSteps.appointmentGuidancePage.menuCheckSymptomsButton.click()
    }

    @Then("^I am given guidance as to my options before booking an appointment$")
    @Throws(Exception::class)
    fun thenIAmGivenGuidanceAsToMyOptionsBeforeBookingAnAppointment() {
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
        appointmentGuidanceSteps.checkTheContentHeaderIsCorrect()
        appointmentGuidanceSteps.checkGuidanceItemsHeadersAreCorrect()
    }
}
