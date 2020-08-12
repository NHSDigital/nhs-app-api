package features.im1Appointments.stepDefinitions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
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

    @Then("^I am on the Appointments Guidance page$")
    fun thenIAmOnTheAppointmentsGuidancePage() {
        appointmentGuidanceSteps.checkThePageHeaderIsCorrect()
    }

    @When("^I select the Book an Appointment button on the guidance page$")
    fun whenISelectBookAppointmentButton() {
        appointmentGuidanceSteps.appointmentGuidancePage.bookButton.click()
    }

    @When("^I select the GP Advice menu item on the guidance page$")
    fun whenISelectTheGPAdviceMenuItem() {
        appointmentGuidanceSteps.appointmentGuidancePage.gpAdviceMenuItem.click()
    }

    @When("^I select the GP Admin menu item on the guidance page$")
    fun whenISelectTheGPAdminMenuItem() {
        appointmentGuidanceSteps.appointmentGuidancePage.gpAdminMenuItem.click()
    }
}
