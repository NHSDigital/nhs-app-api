package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.MockingClient
import mocking.IBookAppointmentsBuilder
import mocking.IAppointmentMappingBuilder
import mocking.models.Mapping
import models.Patient
import net.serenitybdd.core.Serenity
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentBookRequest
import worker.models.appointments.BookAppointmentSlotRequest
import java.time.Duration
import javax.servlet.http.Cookie
import org.junit.Assert.*


class AppointmentsBookingStepDefinitionsBackend {

    private val defaultApptBookingReason = "I have a bad back."
    private val defaultApptBookingSlotId = 12345

    val mockingClient = MockingClient.instance

    @Given("^an appointment booking for (.*) can be successful$")
    fun anAppointmentBookingForCanBeSuccessful(gpSystem:String) {

        defaultAppointmentBookingSetupWithResult (gpSystem) { builder->builder.respondWithSuccess()}
    }

    @Given("^an appointment booking for (.*) can be successful with slot identifier of (\\d+) characters?$")
    fun anAppointmentBookingForCanBeSuccessfulWithANumberOfCharactersForSlotId(gpSystem:String,numberOfCharacters: Int) {
        var patient = Patient.getDefault(gpSystem)
        var slotId= "1".repeat(numberOfCharacters).toInt()
        var request = defaultAppointmentRequest(patient, slotId)
        sendRequestViaMockingClient(gpSystem){bookAppointmentSlotRequest(patient, request).respondWithSuccess()}
        setAppointmentToBeBooked(getAppointmentBookRequest(request))
    }


    @Given("^an appointment booking for (.*) can be successful with booking reason of (\\d+) characters?$")
    fun anAppointmentBookingForCanBeSuccessfulWithANumberOfCharactersForBookingReason(gpSystem:String,numberOfCharacters: Int) {
        var patient = Patient.getDefault(gpSystem)
        var bookingReason= "a".repeat(numberOfCharacters)
        var request = defaultAppointmentRequest(patient, bookingReason =  bookingReason)
        sendRequestViaMockingClient(gpSystem){ bookAppointmentSlotRequest(patient, request).respondWithSuccess()}
        setAppointmentToBeBooked(getAppointmentBookRequest(request))
    }

    @Given("^an appointment booking for (.*) generates an unknown exception$")
    fun unknownExceptionOccursWhenTryingToBookAnAppointment(gpSystem:String) {

        defaultAppointmentBookingSetupWithResult  (gpSystem) { builder->builder.respondWithUnknownException()}
    }

    @Given("^an appointment booking for (.*) can be successful, but session has expired$")
    fun anAppointmentForCanBeSuccessfullyGeneratedButSessionExpired(gpSystem:String) {
        anAppointmentBookingForCanBeSuccessful(gpSystem)
        AppointmentsSharedStepDefinitions.SetAppointmentsSessionCookieToExpired()
    }

    @Given("^online appointment booking is not available to the (.*) patient, when wanting to book an appointment$")
    fun appointmentBookingUnavailableToPatientWhenWantingToBookAnAppointment(gpSystem:String) {

        defaultAppointmentBookingSetupWithResult (gpSystem) { builder->builder.respondWithExceptionWhenNotEnabled()}
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot is not available$")
    fun appointmentBookingForSlotNotAvailable(gpSystem:String) {

        defaultAppointmentBookingSetupWithResult (gpSystem) { builder->builder.respondWithExceptionWhenNotAvailable()}
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot is in the past$")
    fun appointmentBookingForSlotIsInThePast(gpSystem:String) {

        defaultAppointmentBookingSetupWithResult (gpSystem) { builder->builder.respondWithExceptionWhenInThePast()}
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot has been booked by someone else$")
    fun appointmentBookingForSlotIsBookedBySomeoneElse(gpSystem:String) {

        defaultAppointmentBookingSetupWithResult (gpSystem) { builder->builder.respondWithConflictException()}
    }

    @Given("^an appointment booking for (.*) cannot be successful because the GP system is unavailable$")
    fun appointmentBookingUnavailable(gpSystem:String) {
        var patient = Patient.getDefault(gpSystem)
        var request = defaultAppointmentRequest(patient)
        setAppointmentToBeBooked(getAppointmentBookRequest(request))
    }

    @Given("^an appointment booking for (.*) cannot be successful because the GP system will time out$")
    fun appointmentBookingTimesOut(gpSystem:String) {

        defaultAppointmentBookingSetupWithResult(gpSystem) { builder -> builder.withDelay(Duration.ofSeconds(31)).respondWithSuccess() }
    }

    private fun defaultAppointmentBookingSetupWithResult(
            gpSystem:String,
            bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {
        assertTrue("GP System must not be null in data setup", gpSystem != null)
        var patient = Patient.getDefault(gpSystem)
        var request = defaultAppointmentRequest(patient)
        sendRequestViaMockingClient(gpSystem) { bookAppointmentsBuilder(bookAppointmentSlotRequest(patient, request)) }
        setAppointmentToBeBooked(getAppointmentBookRequest(request))
    }

    private fun sendRequestViaMockingClient(gpSystem: String, resolver: IAppointmentMappingBuilder.() -> Mapping){
        if(gpSystem.toUpperCase()=="EMIS")
        {
            mockingClient.forEmis{resolver()}
        }
        else{
            fail("Mocking for $gpSystem not setup")
        }
    }

    private fun setAppointmentToBeBooked(toBeBooked: AppointmentBookRequest){
        Serenity.setSessionVariable("AppointmentToBook").to(toBeBooked)
    }

    private fun defaultAppointmentRequest(patient: Patient, slotId :Int? =null, bookingReason: String? = null): BookAppointmentSlotRequest {
        return BookAppointmentSlotRequest(
                patient.userPatientLinkToken,
                slotId ?: defaultApptBookingSlotId,
                bookingReason ?: defaultApptBookingReason
        )
    }

    private fun getAppointmentBookRequest(bookApptSlot: BookAppointmentSlotRequest): AppointmentBookRequest {
        return AppointmentBookRequest(
                bookApptSlot.slotId.toString(),
                bookApptSlot.bookingReason
        )
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
                        defaultApptBookingReason)
        submitAppointmentRequest(workerAppointmentRequest)
    }

    @When("^an appointment booking is submitted with slot identifier of (\\d+) characters?$")
    fun anAppointmentIsSubmittedWithANumberOfCharactersForSlotId(numberOfCharacters: Int) {
        val slotIdOfSpecifiedLength = "1".repeat(numberOfCharacters)
        val workerAppointmentRequest =AppointmentBookRequest(
                slotId = slotIdOfSpecifiedLength,
                bookingReason = defaultApptBookingReason)
        submitAppointmentRequest(workerAppointmentRequest)
    }

    @When("^an appointment booking is submitted with no booking reason$")
    fun anAppointmentIsSubmittedWithNoBookingReason() {
        val workerAppointmentRequest =
                AppointmentBookRequest(
                        defaultApptBookingSlotId.toString(),
                        null)
        submitAppointmentRequest(workerAppointmentRequest)
    }

    @When("^an appointment booking is submitted with booking reason of (\\d+) characters?$")
    fun anAppointmentIsSubmittedWithANumberOfCharactersForBookingReason(numberOfCharacters: Int) {
        val bookingReasonOfSpecifiedLength = "x".repeat(numberOfCharacters)
        val workerAppointmentRequest =AppointmentBookRequest(
                slotId = defaultApptBookingSlotId.toString(),
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

        var expectedResponse = HttpStatus.SC_CREATED;
        var receivedResponse = Serenity.sessionVariableCalled<HttpResponse>("Http Status Code").statusLine.statusCode

        assertEquals("Expected response $expectedResponse, but was $receivedResponse",
                HttpStatus.SC_CREATED,
                receivedResponse)
    }
}