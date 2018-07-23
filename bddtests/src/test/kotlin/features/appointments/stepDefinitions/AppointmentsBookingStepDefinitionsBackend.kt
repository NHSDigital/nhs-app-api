package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.IBookAppointmentsBuilder
import mocking.models.Mapping
import net.serenitybdd.core.Serenity
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.junit.Assert.assertEquals
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentBookRequest
import java.time.Duration
import javax.servlet.http.Cookie


class AppointmentsBookingStepDefinitionsBackend {

    @Given("^an appointment booking for (.*) can be successful$")
    fun anAppointmentBookingForCanBeSuccessful(gpSystem: String) {

        defaultAppointmentBookingSetupWithResult(gpSystem) { builder -> builder.respondWithSuccess() }
    }

    @Given("^an appointment booking for (.*) can be successful with slot identifier of (\\d+) characters?$")
    fun anAppointmentBookingForCanBeSuccessfulWithANumberOfCharactersForSlotId(gpSystem: String, numberOfCharacters: Int) {
        var dataController = AppointmentsBookingFactory.getForSupplier(gpSystem)
        var patient = dataController.getDefaultPatient()
        var slotId = "1".repeat(numberOfCharacters).toInt()
        var request = dataController.defaultAppointmentRequest(patient, slotId = slotId)
        dataController.setupRequestAndResponse(request) { bookAppointmentSlotRequest(patient, request).respondWithSuccess() }
    }


    @Given("^an appointment booking for (.*) can be successful with booking reason of (\\d+) characters?$")
    fun anAppointmentBookingForCanBeSuccessfulWithANumberOfCharactersForBookingReason(gpSystem: String, numberOfCharacters: Int) {
        var dataController = AppointmentsBookingFactory.getForSupplier(gpSystem)
        var patient = dataController.getDefaultPatient()
        var bookingReason = "a".repeat(numberOfCharacters)
        var request = dataController.defaultAppointmentRequest(patient, bookingReason = bookingReason)
        dataController.setupRequestAndResponse(request) { bookAppointmentSlotRequest(patient, request).respondWithSuccess() }
    }

    @Given("^an appointment booking for (.*) generates an unknown exception$")
    fun unknownExceptionOccursWhenTryingToBookAnAppointment(gpSystem: String) {

        defaultAppointmentBookingSetupWithResult(gpSystem) { builder -> builder.respondWithUnknownException() }
    }

    @Given("^an appointment booking for (.*) can be successful, but session has expired$")
    fun anAppointmentForCanBeSuccessfullyGeneratedButSessionExpired(gpSystem: String) {
        anAppointmentBookingForCanBeSuccessful(gpSystem)
        AppointmentsSharedStepDefinitions.SetAppointmentsSessionCookieToExpired()
    }

    @Given("^online appointment booking is not available to the (.*) patient, when wanting to book an appointment$")
    fun appointmentBookingUnavailableToPatientWhenWantingToBookAnAppointment(gpSystem: String) {

        defaultAppointmentBookingSetupWithResult(gpSystem) { builder -> builder.respondWithExceptionWhenNotEnabled() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot is not available$")
    fun appointmentBookingForSlotNotAvailable(gpSystem: String) {

        defaultAppointmentBookingSetupWithResult(gpSystem) { builder -> builder.respondWithExceptionWhenNotAvailable() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot is in the past$")
    fun appointmentBookingForSlotIsInThePast(gpSystem: String) {

        defaultAppointmentBookingSetupWithResult(gpSystem) { builder -> builder.respondWithExceptionWhenInThePast() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot has been booked by someone else$")
    fun appointmentBookingForSlotIsBookedBySomeoneElse(gpSystem: String) {

        defaultAppointmentBookingSetupWithResult(gpSystem) { builder -> builder.respondWithConflictException() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the GP system is unavailable$")
    fun appointmentBookingUnavailable(gpSystem: String) {
        var dataController = AppointmentsBookingFactory.getForSupplier(gpSystem)
        var patient = dataController.getDefaultPatient()
        var request = dataController.defaultAppointmentRequest(patient)
        dataController.setupRequestAndResponse(request)
    }

    @Given("^an appointment booking for (.*) cannot be successful because the GP system will time out$")
    fun appointmentBookingTimesOut(gpSystem: String) {

        defaultAppointmentBookingSetupWithResult(gpSystem) { builder -> builder.withDelay(Duration.ofSeconds(31)).respondWithSuccess() }
    }

    private fun defaultAppointmentBookingSetupWithResult(
            gpSystem: String,
            bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {
        AppointmentsBookingFactory.getForSupplier(gpSystem).defaultAppointmentBookingSetupWithResult(bookAppointmentsBuilder)
    }

    @When("^an appointment booking is submitted$")
    fun anAppointmentBookingIsSubmitted() {
        val appointmentToBook = Serenity.sessionVariableCalled<AppointmentBookRequest>("AppointmentToBook")
        submitAppointmentRequest(appointmentToBook)
    }

    @When("^an appointment booking is submitted with no slot identifier$")
    fun anAppointmentIsSubmittedWithNoSlotId() {
        val workerAppointmentRequest =
                AppointmentBookRequest(
                        null,
                       AppointmentsBookingFactory. defaultApptBookingReason)
        submitAppointmentRequest(workerAppointmentRequest)
    }

    @When("^an appointment booking is submitted with slot identifier of (\\d+) characters?$")
    fun anAppointmentIsSubmittedWithANumberOfCharactersForSlotId(numberOfCharacters: Int) {
        val slotIdOfSpecifiedLength = "1".repeat(numberOfCharacters)
        val workerAppointmentRequest = AppointmentBookRequest(
                slotId = slotIdOfSpecifiedLength,
                bookingReason = AppointmentsBookingFactory. defaultApptBookingReason)
        submitAppointmentRequest(workerAppointmentRequest)
    }

    @When("^an appointment booking is submitted with no booking reason$")
    fun anAppointmentIsSubmittedWithNoBookingReason() {
        val workerAppointmentRequest =
                AppointmentBookRequest(
                        AppointmentsBookingFactory. defaultApptBookingSlotId.toString(),
                        null)
        submitAppointmentRequest(workerAppointmentRequest)
    }

    @When("^an appointment booking is submitted with booking reason of (\\d+) characters?$")
    fun anAppointmentIsSubmittedWithANumberOfCharactersForBookingReason(numberOfCharacters: Int) {
        val bookingReasonOfSpecifiedLength = "x".repeat(numberOfCharacters)
        val workerAppointmentRequest = AppointmentBookRequest(
                slotId = AppointmentsBookingFactory. defaultApptBookingSlotId.toString(),
                bookingReason = bookingReasonOfSpecifiedLength)
        submitAppointmentRequest(workerAppointmentRequest)
    }


    private fun submitAppointmentRequest(workerAppointmentRequest: AppointmentBookRequest) {
        try {
            val workerClient = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
            val sessionCookie = Serenity.sessionVariableCalled<Cookie>(Cookie::class)
            val result = workerClient.postAppointment(workerAppointmentRequest, sessionCookie)
            Serenity.setSessionVariable("Http Status Code").to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }
    }

    @Then("^a successful response for appointment booking is returned$")
    fun aSuccessfulResponseIsReturned() {

        val expectedResponse = HttpStatus.SC_CREATED;
        val receivedResponse = Serenity.sessionVariableCalled<HttpResponse>("Http Status Code").statusLine.statusCode

        assertEquals("Expected response $expectedResponse, but was $receivedResponse",
                HttpStatus.SC_CREATED,
                receivedResponse)
    }
}