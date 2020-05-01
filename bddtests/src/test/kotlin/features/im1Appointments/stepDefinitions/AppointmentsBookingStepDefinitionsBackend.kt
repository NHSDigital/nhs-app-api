package features.im1Appointments.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import mocking.stubs.appointments.factories.AppointmentsBookingFactory.Companion.defaultTelephoneNumber
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import net.serenitybdd.core.Serenity
import org.apache.http.HttpResponse
import org.apache.http.HttpStatus
import org.junit.Assert.assertEquals
import utils.LinkedProfilesSerenityHelpers
import utils.ProxySerenityHelpers
import utils.SerenityHelpers
import utils.getOrNull
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentBookRequest
import java.time.Duration
import javax.servlet.http.Cookie

private const val TIMEOUT_IN_SECONDS = 31L

open class AppointmentsBookingStepDefinitionsBackend {

    @Given("^an appointment booking for (.*) can be successful$")
    fun anAppointmentBookingForCanBeSuccessful(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder.respondWithSuccess() }
    }

    @Given("^(.*) returns corrupted response for booking request")
    fun corruptedResponseFromMyAppointments(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder.respondWithCorrupted() }
    }

    @Given("^an appointment booking for (.*) can be successful with slot identifier of (\\d+) characters?$")
    fun anAppointmentBookingForCanBeSuccessfulWithANumberOfCharactersForSlotId(gpSystem: String,
                                                                               numberOfCharacters: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsBookingFactory.getForSupplier(supplier)
        val patient = ProxySerenityHelpers.getPatientOrProxy()
        val slotId = "1".repeat(numberOfCharacters).toInt()
        val request = factory.defaultAppointmentRequest(patient, slotId = slotId)
        factory.setupRequestAndResponse(request) { bookAppointmentSlotRequest(patient, request).respondWithSuccess() }
    }

    @Given("^an appointment booking for (.*) requires a booking reason$")
    fun anAppointmentBookingForRequiresABookingReason(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsBookingFactory.getForSupplier(supplier)
        factory.generateDefaultUserData(defaultPracticeSettings = false)
        factory.requiresBookingReason(true)
    }

    @Given("^an appointment booking for (.*) can be successful with booking reason of (\\d+) characters?$")
    fun anAppointmentBookingForCanBeSuccessfulWithANumberOfCharactersForBookingReason(gpSystem: String,
                                                                                      numberOfCharacters: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsBookingFactory.getForSupplier(supplier)
        val patient = ProxySerenityHelpers.getPatientOrProxy()
        val bookingReason = "a".repeat(numberOfCharacters)
        val request = factory.defaultAppointmentRequest(patient, bookingReason = bookingReason)
        factory.setupRequestAndResponse(request) { bookAppointmentSlotRequest(patient, request).respondWithSuccess() }
    }

    @Given("^an appointment booking for (.*) generates an unknown exception$")
    fun unknownExceptionOccursWhenTryingToBookAnAppointment(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder.respondWithUnknownException() }
    }

    @Given("^an appointment booking for (.*) can be successful, but session has expired$")
    fun anAppointmentForCanBeSuccessfullyGeneratedButSessionExpired(gpSystem: String) {
        anAppointmentBookingForCanBeSuccessful(gpSystem)
        AppointmentsSharedStepDefinitions.setAppointmentsSessionCookieToExpired()
    }

    @Given("^online appointment booking is not available to the (.*) patient, when wanting to book an appointment$")
    fun appointmentBookingUnavailableToPatientWhenWantingToBookAnAppointment(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder.respondWithGPErrorWhenNotEnabled() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot is not available$")
    fun appointmentBookingForSlotNotAvailable(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder.respondWithExceptionWhenNotAvailable() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot is in the past$")
    fun appointmentBookingForSlotIsInThePast(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder.respondWithExceptionWhenInThePast() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot is before practice defined days$")
    fun appointmentBookingForSlotIsBeforePracticeDefinedDays(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder
                .respondWithExceptionWhenBeforePracticeDefinedDays() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot is after practice defined days$")
    fun appointmentBookingForSlotIsAfterPracticeDefinedDays(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder
                .respondWithExceptionWhenAfterPracticeDefinedDays() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the slot has been booked by someone else$")
    fun appointmentBookingForSlotIsBookedBySomeoneElse(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier) { builder -> builder.respondWithConflictException() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the GP system is unavailable$")
    fun appointmentBookingUnavailable(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier)
        { builder -> builder.respondWithServiceUnavailable() }
    }

    @Given("^an appointment booking for (.*) cannot be successful because the GP system will time out$")
    fun appointmentBookingTimesOut(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        defaultAppointmentBookingSetupWithResult(supplier)
        { builder -> builder.withDelay(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
                .respondWithSuccess() }
    }

    @Given("^a telephone appointment booking for (.*) can be successful$")
    fun aTelephoneAppointmentBookingForCanBeSuccessful(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        telephoneAppointmentBookingSetupWithResult(supplier, defaultTelephoneNumber) {
            builder -> builder.respondWithSuccess() }
    }

    @Given("^a telephone appointment booking for (.*) cannot be successful without phone number$")
    fun aTelephoneAppointmentBookingForCannotBeSuccessfulWithoutPhoneNumber(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        telephoneAppointmentBookingSetupWithResult(supplier) {
            builder -> builder.respondWithExceptionWhenRequiredFieldMissing() }
    }

    private fun telephoneAppointmentBookingSetupWithResult(gpSystem: Supplier,
            telephoneNumber: String? = null,
            bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {
        AppointmentsBookingFactory.getForSupplier(gpSystem)
                .telephoneAppointmentBookingSetupWithResult(
                        telephoneNumber,
                        slotId = AppointmentsBookingFactory.defaultApptBookingSlotId,
                        bookAppointmentsBuilder = bookAppointmentsBuilder
                )
    }

    @When("^an appointment booking is submitted with phone number$")
    fun anAppointmentBookingIsSubmittedWithPhoneNumber() {
        val appointmentBookRequest = AppointmentBookRequest(
                slotId = AppointmentsBookingFactory.defaultApptBookingSlotId.toString(),
                bookingReason = AppointmentsBookingFactory.defaultApptBookingReason,
                telephoneNumber = AppointmentsBookingFactory.defaultTelephoneNumber,
                telephoneContactType = AppointmentsBookingFactory.defaultTelephoneContactType)
        submitAppointmentRequest(appointmentBookRequest)
    }

    @When("^an appointment booking is submitted without phone number$")
    fun anAppointmentBookingIsSubmittedWithoutPhoneNumber() {
        val appointmentBookRequest = AppointmentBookRequest(
                slotId = AppointmentsBookingFactory.defaultApptBookingSlotId.toString(),
                bookingReason = AppointmentsBookingFactory.defaultApptBookingReason)
        submitAppointmentRequest(appointmentBookRequest)
    }

    @When("^an appointment booking is submitted$")
    fun anAppointmentBookingIsSubmitted() {
        val appointmentToBook =
                Serenity.sessionVariableCalled<AppointmentBookRequest>(AppointmentsBookingFactory
                        .appointmentToBookKey)
        submitAppointmentRequest(appointmentToBook)
    }

    @When("^an appointment booking is submitted with no slot identifier$")
    fun anAppointmentIsSubmittedWithNoSlotId() {
        val workerAppointmentRequest =
                AppointmentBookRequest(
                        slotId =  null,
                        bookingReason = AppointmentsBookingFactory. defaultApptBookingReason)
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
                       slotId = AppointmentsBookingFactory. defaultApptBookingSlotId.toString(),
                       bookingReason = null)
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

    @Then("^a successful response for appointment booking is returned$")
    fun aSuccessfulResponseIsReturned() {

        val expectedResponse = HttpStatus.SC_CREATED
        val receivedResponse = Serenity.sessionVariableCalled<HttpResponse>("Http Status Code").statusLine.statusCode

        assertEquals("Expected response $expectedResponse, but was $receivedResponse",
                HttpStatus.SC_CREATED,
                receivedResponse)
    }

    private fun defaultAppointmentBookingSetupWithResult(
            gpSystem: Supplier,
            bookAppointmentsBuilder: (IBookAppointmentsBuilder) -> Mapping) {
        AppointmentsBookingFactory.getForSupplier(gpSystem)
                .defaultAppointmentBookingSetupWithResult(bookAppointmentsBuilder)
    }

    private fun submitAppointmentRequest(workerAppointmentRequest: AppointmentBookRequest) {

        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrNull<String>()

        try {
            val workerClient = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
            val sessionCookie = Serenity.sessionVariableCalled<Cookie>(Cookie::class)
            val result = workerClient.appointments.postAppointment(patientId, workerAppointmentRequest, sessionCookie)
            Serenity.setSessionVariable("Http Status Code").to(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }
}
