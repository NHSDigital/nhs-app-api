package features.appointments.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.UpcomingAppointmentsFactory
import features.appointments.steps.MyAppointmentsSteps
import org.junit.Assert
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.util.*

class MyAppointmentsStepDefinitions {

    @Steps
    lateinit var myAppointmentsSteps: MyAppointmentsSteps


    @Then("^the Appointment Slot page is displayed$")
    fun theAppointmentSlotPageIsDisplayed() {
        myAppointmentsSteps.myAppointmentsPage.waitForPageHeaderText("Confirm appointment")
    }


    @Then("^the Appointment Booking confirmation screen is displayed$")
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

    @Given("^I have no upcoming appointments for (.*)$")
    fun i_have_no_upcoming_appointments(gpService: String) {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulEmptyUpcomingAppointmentResponse()
    }

    @Given("^I have upcoming appointments for (.*)$")
    fun i_have_upcoming_appointments(gpService: String) {
        val viewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulUpcomingAppointmentsResponse()
    }

    @Given("^the (.*) does not offer online booking to my patient$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots(provider: String) {
        myAppointmentsSteps.generateStubsForMyAppointmentsWhenUnavailableToPatient(provider)
    }

    @Given("^(.*) returns corrupted response for my appointments")
    fun corruptedResponseFromMyAppointments(provider: String) {
        myAppointmentsSteps.generateCorruptedStubForMyAppointment(provider)
    }

    @When("^I select \"([^\"]*)\" button$")
    fun i_select_button(buttonText: String) {
        myAppointmentsSteps.myAppointmentsPage.clickOnButtonContainingText(buttonText)
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

    @When("^the API retrieves upcoming appointments$")
    fun the_API_retrieves_upcoming_appointments() {
        myAppointmentsSteps.createSerenityMyAppointmentSessionVariable()
    }

    @When("^the \"([^\"]*)\" API call fails with csrf token of \"([^\"]*)\"$")
    fun the_API_call_failes_with_csrf_token_of(provider: String, csrfToken: String) {
        Assert.assertEquals("Test setup incorrect: Step only implemented for EMIS", "EMIS", provider.toUpperCase())

        try {
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.setCsrfToken(csrfToken).getMyAppointments(LocalDateTime.now().toString())
            Serenity.setSessionVariable(MyAppointmentsResponse::class.java).to(result)
            Assert.fail("The API did not fail with invalid token.")
        } catch (exception: NhsoHttpException) {
        }
    }

    @Then("^I will only receive upcoming appointments$")
    fun i_will_only_receive_upcoming_appointments() {
        myAppointmentsSteps.checkMyAppointmentsAreAllUpcomingOnes()
    }

    @Then("^a list of cancellation reasons if the GP Service provides the list$")
    fun a_list_of_cancellation_reasons() {
        myAppointmentsSteps.checkCancellationReasonExistForApplicableGPService()
    }

    @When("^I select a \"Cancel appointment\" link$")
    fun iSelectACancelLink() {
        myAppointmentsSteps.clickFirstCancelLink()
    }

    @Then("^a \"Cancellation confirmed\" message is displayed$")
    fun cancellationConfirmationMessage() {
        myAppointmentsSteps.verifyCancellationConfirmationMessage()
    }
}
