package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentsSteps
import net.thucydides.core.annotations.Steps

class AppointmentsStepDefinitions {

    @Steps
    lateinit var appointmentsSteps: AppointmentsSteps

    @Then("^booking request is successfully made with valid details$")
    @Throws(Exception::class)
    fun bookingRequestIsMade() {
        appointmentsSteps.checkBookingWasRequested()
    }

    @Then("^Appointment Booking confirmation screen is displayed$")
    @Throws(Exception::class)
    fun appointmentBookingConfirmationScreenIsDisplayed() {
        appointmentsSteps.checkBookingSuccessMessage()
    }

    @Then("^I can book an appointment$")
    @Throws(Exception::class)
    fun iCanBookAnAppointment() {
        appointmentsSteps.checkIfBookAnAppointmentButtonExistAndEnabled()
    }

    @Then("^the page title is \"My appointments\"$")
    fun thePageTitleIsMyAppointments() {
        appointmentsSteps.checkHeaderTextIsCorrect()
    }

    @Then("^I will be on the My appointments screen$")
    fun iWillBeOnTheMyAppointmentsScreen() {
        iCanBookAnAppointment()
        thePageTitleIsMyAppointments()
    }

    @Then("^I am informed I have no booked appointments$")
    @Throws(Exception::class)
    fun i_am_informed_I_have_no_booked_appointments() {
        appointmentsSteps.checkNoUpcomingAppointmentsTextIsDisplaying()
    }

    @Given("^I have no upcoming appointments$")
    @Throws(Exception::class)
    fun i_have_no_upcoming_appointments() {
        appointmentsSteps.mockEMISMyAppointmentResponse(true)
    }

    @Given("^I have upcoming appointments$")
    @Throws(Exception::class)
    fun i_have_upcoming_appointments() {
        appointmentsSteps.mockEMISMyAppointmentResponse()
    }

    @When("^I select \"([^\"]*)\" button$")
    @Throws(Exception::class)
    fun i_select_button(buttonText: String) {
        appointmentsSteps.clickOnButtonByText(buttonText)
    }

    @Then("^I am given the list of upcoming appointments$")
    @Throws(Exception::class)
    fun i_am_given_the_list_of_upcoming_appointments() {
        appointmentsSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated()
    }

    @Then("^appointments are in chronological order$")
    @Throws(Exception::class)
    fun appointments_are_in_chronological_order() {
        appointmentsSteps.checkIfSlotsAreInCorrectOrder()
    }

    @Then("^each appointment can be cancelled$")
    fun eachAppointmentCanBeCancelled() {
        appointmentsSteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment()
    }

    @When("^the API retrieves upcoming appointments from \"([^\"]*)\"$")
    @Throws(Exception::class)
    fun the_API_retrieves_upcoming_appointments_from(provider: String) {
        appointmentsSteps.createSerenityEmisMyAppointmentSessionVariable()
    }


    @Then("^I will only receive upcoming appointments$")
    @Throws(Exception::class)
    fun i_will_only_receive_upcoming_appointments() {
        appointmentsSteps.checkEmisMyAppointmentsAreAllUpcomingOnes()
    }

    @Then("^a list of cancellation reasons$")
    @Throws(Exception::class)
    fun a_list_of_cancellation_reasons() {
        appointmentsSteps.checkEmisCancellationReasonExist()
    }

    @When("^I select a \"Cancel appointment\" link$")
    fun iSelectACancelLink() {
        appointmentsSteps.storeDetailsOfFirstAppointment()
        appointmentsSteps.clickFirstCancelLink()
    }

    @Then("^a \"Cancellation confirmed\" message is displayed$")
    fun cancellationConfirmationMessage() {
        appointmentsSteps.verifyCancellationConfirmationMessage()
    }
}
