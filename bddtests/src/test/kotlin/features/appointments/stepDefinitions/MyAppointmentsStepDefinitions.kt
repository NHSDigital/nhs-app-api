package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.MyAppointmentsSteps
import net.thucydides.core.annotations.Steps

class MyAppointmentsStepDefinitions {

    @Steps
    lateinit var myAppointmentsSteps: MyAppointmentsSteps

    @Then("^booking request is successfully made with valid details$")
    fun bookingRequestIsMade() {
        myAppointmentsSteps.checkBookingWasRequested()
    }

    @Then("^Appointment Booking confirmation screen is displayed$")
    fun appointmentBookingConfirmationScreenIsDisplayed() {
        myAppointmentsSteps.checkBookingSuccessMessage()
    }

    @Then("^I can book an appointment$")
    fun iCanBookAnAppointment() {
        myAppointmentsSteps.checkIfBookAnAppointmentButtonExistAndEnabled()
    }

    @Then("^the page title is \"My appointments\"$")
    fun thePageTitleIsMyAppointments() {
        myAppointmentsSteps.checkHeaderTextIsCorrect()
    }

    @Then("^I will be on the My appointments screen$")
    fun iWillBeOnTheMyAppointmentsScreen() {
        iCanBookAnAppointment()
        thePageTitleIsMyAppointments()
    }

    @Then("^I am informed I have no booked appointments$")
    fun i_am_informed_I_have_no_booked_appointments() {
        myAppointmentsSteps.checkNoUpcomingAppointmentsTextIsDisplaying()
    }

    @Given("^I have no upcoming appointments$")
    fun i_have_no_upcoming_appointments() {
        myAppointmentsSteps.mockEMISMyAppointmentResponse(true)
    }

    @Given("^I have upcoming appointments$")
    fun i_have_upcoming_appointments() {
        myAppointmentsSteps.mockEMISMyAppointmentResponse()
    }

    @When("^I select \"([^\"]*)\" button$")
    fun i_select_button(buttonText: String) {
        myAppointmentsSteps.clickOnButtonByText(buttonText)
    }

    @Then("^I am given the list of upcoming appointments$")
    fun i_am_given_the_list_of_upcoming_appointments() {
        myAppointmentsSteps.checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated()
    }

    @Then("^appointments are in chronological order$")
    fun appointments_are_in_chronological_order() {
        myAppointmentsSteps.checkIfSlotsAreInCorrectOrder()
    }

    @Then("^each appointment can be cancelled$")
    fun eachAppointmentCanBeCancelled() {
        myAppointmentsSteps.verifyThatThereIsACancelLinkForEachUpcomingAppointment()
    }

    @When("^the API retrieves upcoming appointments from \"([^\"]*)\"$")
    fun the_API_retrieves_upcoming_appointments_from(provider: String) {
        myAppointmentsSteps.createSerenityEmisMyAppointmentSessionVariable()
    }


    @Then("^I will only receive upcoming appointments$")
    fun i_will_only_receive_upcoming_appointments() {
        myAppointmentsSteps.checkEmisMyAppointmentsAreAllUpcomingOnes()
    }

    @Then("^a list of cancellation reasons$")
    fun a_list_of_cancellation_reasons() {
        myAppointmentsSteps.checkEmisCancellationReasonExist()
    }

    @When("^I select a \"Cancel appointment\" link$")
    fun iSelectACancelLink() {
        myAppointmentsSteps.storeDetailsOfFirstAppointment()
        myAppointmentsSteps.clickFirstCancelLink()
    }

    @Then("^a \"Cancellation confirmed\" message is displayed$")
    fun cancellationConfirmationMessage() {
        myAppointmentsSteps.verifyCancellationConfirmationMessage()
    }
}
