package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsSlotsFactory
import mocking.data.appointments.AppointmentsBookingData.Companion.dateTimeFormat
import mocking.data.appointments.AppointmentsBookingData.Companion.defaultSessionEndDate
import mocking.data.appointments.AppointmentsBookingData.Companion.defaultSessionStartDate
import mocking.data.appointments.AppointmentsBookingData.Companion.pastFromDate
import mocking.data.appointments.AppointmentsBookingData.Companion.pastToDate
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.emis.practices.NecessityOption
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import java.time.Duration
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import javax.servlet.http.Cookie

class AvailableAppointmentsSlotsStepDefinitionsBackend  {

    @Given("^the system will time out when trying to retrieve (.*) appointment slots$")
    fun appointmentSlotsTimesOut(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample {
            respondWithSuccess(AppointmentsSlotsExample.getGenericExample())
                    .delayedBy(Duration.ofSeconds(90))
        }
    }

    @Given("^online appointment booking is not available to the patient, when wanting to view appointment slots$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots() {

        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        factory.generateAppointmentSlotResponse(null, null, true, NecessityOption.OPTIONAL, { respondWithGPErrorWhenNotEnabled() })
    }

    @Given("^unknown exception will occur when wanting to view appointment slots$")
    fun unknownExceptionWhenWantingToViewAppointmentSlots() {

        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        factory.generateAppointmentSlotResponse(null, null, true, NecessityOption.OPTIONAL, { respondWithUnknownException() })
    }

    @When("^the available appointment slots are retrieved without a cookie$")
    fun theAvailableAppointmentSlotsAreRetrievedWithoutACookie() {
        retrieveAppointmentSlots(null, null, includeCookie = false)
    }

    @When("^I try to retrieve appointment slots with fromDate after the toDate$")
    fun tryToRetrieveAppointmentSlotsWithFromDateAfterToDate() {
        retrieveAppointmentSlots(defaultSessionEndDate, defaultSessionStartDate)
    }

    @When("^I try to retrieve appointment slots only from the past$")
    fun tryToRetrieveAppointmentSlotsFromThePast() {
        retrieveAppointmentSlots(pastFromDate, pastToDate)
    }

    @When("^I try to retrieve appointment slots with a malformed to Date$")
    fun tryToRetrieveAppointmentSlotsWithMalformedToDate() {
        val toDate = LocalDateTime.parse(defaultSessionEndDate, dateTimeFormat).format(DateTimeFormatter.BASIC_ISO_DATE)
        retrieveAppointmentSlots(defaultSessionStartDate, toDate)
    }

    @When("^I try to retrieve appointment slots with a malformed from Date$")
    fun tryToRetrieveAppointmentSlotsWithMalformedFromDate() {
        val fromDate = LocalDateTime.parse(defaultSessionStartDate, dateTimeFormat).format(DateTimeFormatter.BASIC_ISO_DATE)
        retrieveAppointmentSlots(fromDate, defaultSessionEndDate)
    }

    private fun retrieveAppointmentSlots(fromDate: String? = null, toDate: String? = null, includeCookie: Boolean = true) {
        var cookie: Cookie? = null
        if (includeCookie) {
            cookie = Serenity.sessionVariableCalled<Cookie>(Cookie::class)
        }

        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.getAppointmentSlots(fromDate,
                            toDate,
                            cookie)
            Serenity.setSessionVariable(AppointmentSlotsResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }
    }

    @Then("^empty sets of data are returned$")
    fun emptySetsAreReturned() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertAppointmentSlotsResponseNotNull(result)
        Assert.assertEquals("result.slots", 0, result.slots.size)
    }

    private fun assertAppointmentSlotsResponseNotNull(result: AppointmentSlotsResponse) {
        Assert.assertNotNull("result", result)
        Assert.assertNotNull("result.slots", result.slots)
    }
}
