package features.appointments.stepDefinitions

import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormat
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.data.AppointmentsBookingData.Companion.dateTimeFormat
import features.appointments.data.AppointmentsBookingData.Companion.defaultSessionEndDate
import features.appointments.data.AppointmentsBookingData.Companion.defaultSessionStartDate
import features.appointments.data.AppointmentsBookingData.Companion.pastFromDate
import features.appointments.data.AppointmentsBookingData.Companion.pastToDate
import features.appointments.factories.AppointmentsSlotsFactory
import features.appointments.steps.AvailableAppointmentsSteps
import features.sharedStepDefinitions.BaseStepDefinition
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import java.text.ParsePosition
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*
import javax.servlet.http.Cookie

class AvailableAppointmentsSlotsStepDefinitionsBackend : BaseStepDefinition() {

    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps

    @Given("^the system will time out when trying to retrieve appointment slots$")
    fun appointmentSlotsTimesOut() {
        availableAppointments.appointmentSlotsTimesOut()
    }

    @Given("^online appointment booking is not available to the patient, when wanting to view appointment slots$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots() {

        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        factory.generateAppointmentSlotResponse(null, null)
        { respondWithExceptionWhenNotEnabled() }
    }

    @Given("^unknown exception will occur when wanting to view appointment slots$")
    fun unknownExceptionWhenWantingToViewAppointmentSlots() {

        val factory = AppointmentsSlotsFactory.getForSupplier("EMIS")
        factory.generateAppointmentSlotResponse(null, null)
        { respondWithUnknownException() }
    }

    @When("^the available appointment slots are retrieved without a cookie$")
    fun theAvailableAppointmentSlotsAreRetrievedWithoutACookie() {
        retrieveAppointmentSlots(null, null, includeCookie = false)
    }

    @When("^I try to retrieve appointment slots with fromDate after the toDate$")
    fun tryToRetrieveAppointmentSlotsWithFromDateAfterToDate() {
        retrieveAppointmentSlots(toLocalTime(defaultSessionEndDate), toLocalTime(defaultSessionStartDate))
    }

    @When("^I try to retrieve appointment slots only from the past$")
    fun tryToRetrieveAppointmentSlotsFromThePast() {
        retrieveAppointmentSlots(toLocalTime(pastFromDate), toLocalTime(pastToDate))
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
                    .getAppointmentSlots(fromDate,
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

    private fun toLocalTime(date: String?): String {
        val currentDateFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        currentDateFormat.timeZone = TimeZone.getDefault()
        val dateToPass = currentDateFormat.parse(date, ParsePosition(0))
        val queryDateFormat = SimpleDateFormat(backendDateTimeFormat)
        return queryDateFormat.format(dateToPass)
    }
}
