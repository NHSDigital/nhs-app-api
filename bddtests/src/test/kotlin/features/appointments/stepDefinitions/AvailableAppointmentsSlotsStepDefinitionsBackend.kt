package features.appointments.stepDefinitions

import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithTimezone
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.data.AppointmentsBookingData
import features.appointments.steps.AvailableAppointmentsSteps
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.models.*
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import java.text.ParsePosition
import java.text.SimpleDateFormat
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*
import javax.servlet.http.Cookie
import org.junit.Assert


class AvailableAppointmentsSlotsStepDefinitionsBackend: AppointmentsBookingData() {

    @Steps
    lateinit var availableAppointments: AvailableAppointmentsSteps


    @Given("^there are available appointment slots within the next four weeks$")
    fun thereAreAvailableAppointmentSlotsWithinTheNextFourWeeks() {
        val getAppointmentSlotsMetaQueryParamsForNextFourWeeks = getAppointmentSlotsMetaQueryParams.copy(sessionStartDate = defaultSessionStartDate, sessionEndDate = defaultSessionEndDate)
        generateAppropriateStubsForAppointmentSlots(appointmentSlotsMetaQueryParams = getAppointmentSlotsMetaQueryParamsForNextFourWeeks)
    }

    @Given("^there are available appointment slots four weeks from a specific from date$")
    fun thereAreAvailableAppointmentSlotsFourWeeksAfterFromDate() {
        val getAppointmentSlotsMetaQueryParamsForNextFourWeeks = getAppointmentSlotsMetaQueryParams.copy(sessionStartDate = explicitFromDate, sessionEndDate = defaultToDateIfExplicitFromDate)
        generateAppropriateStubsForAppointmentSlots(appointmentSlotsMetaQueryParams = getAppointmentSlotsMetaQueryParamsForNextFourWeeks)
    }

    private val getAppointmentSlotsMetaQueryParams = AppointmentSlotsParams(
            patient = patient,
            sessionStartDate = explicitFromDate,
            sessionEndDate = explicitToDate
    )

    //should be combined with the analogous function in appointmentsStepDefintions
    private fun generateAppropriateStubsForAppointmentSlots(emisSlotLocations: ArrayList<Location> = defaultEmisMetaSlotLocations,
                                                            emisSlotSessionHolders: ArrayList<SessionHolder> = defaultEmisMetaSlotSessionHolders,
                                                            emisSlotSessions: ArrayList<Session> = defaultEmisMetaSlotSessions,
                                                            emisAppointmentSessions: ArrayList<AppointmentSession> = defaultEmisAppointmentSessions,
                                                            appointmentSlotsMetaQueryParams: AppointmentSlotsParams = getAppointmentSlotsMetaQueryParams) {

        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(appointmentSlotsMetaQueryParams.patient,
                            appointmentSlotsMetaQueryParams.sessionStartDate,
                            appointmentSlotsMetaQueryParams.sessionEndDate)
                            .respondWithSuccess(GetAppointmentSlotsMetaResponseModel(
                                    emisSlotLocations,
                                    emisSlotSessionHolders,
                                    emisSlotSessions
                            ))
                }

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(appointmentSlotsMetaQueryParams.patient,
                            appointmentSlotsMetaQueryParams.sessionStartDate,
                            appointmentSlotsMetaQueryParams.sessionEndDate)
                            .respondWithSuccess(GetAppointmentSlotsResponseModel(
                                    emisAppointmentSessions
                            ))
                }
    }

    @Given("^the system will time out when trying to retrieve appointment slots$")
    fun appointmentSlotsTimesOut() {
        availableAppointments.appointmentSlotsTimesOut()
    }

    @Given("^online appointment booking is not available to the patient, when wanting to view appointment slots$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots() {
        mockingClient.forEmis {
            appointmentSlotsMetaRequest(getAppointmentSlotsMetaQueryParams.patient,
                    getAppointmentSlotsMetaQueryParams.sessionStartDate,
                    getAppointmentSlotsMetaQueryParams.sessionEndDate)
                    .respondWithExceptionWhenNotEnabled()
        }

        mockingClient.forEmis {
            appointmentSlotsRequest(getAppointmentSlotsMetaQueryParams.patient,
                    getAppointmentSlotsMetaQueryParams.sessionStartDate,
                    getAppointmentSlotsMetaQueryParams.sessionEndDate)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    @Given("^unknown exception will occur when wanting to view appointment slots$")
    fun unknownExceptionWhenWantingToViewAppointmentSlots() {

        mockingClient.forEmis {
            appointmentSlotsMetaRequest(getAppointmentSlotsMetaQueryParams.patient,
                    getAppointmentSlotsMetaQueryParams.sessionStartDate,
                    getAppointmentSlotsMetaQueryParams.sessionEndDate)
                    .respondWithUnknownException()
        }

        mockingClient.forEmis {
            appointmentSlotsRequest(getAppointmentSlotsMetaQueryParams.patient,
                    getAppointmentSlotsMetaQueryParams.sessionStartDate,
                    getAppointmentSlotsMetaQueryParams.sessionEndDate)
                    .respondWithUnknownException()
        }
    }

    @When("^the available appointment slots are retrieved for explicit date-time range without a cookie$")
    fun theAvailableAppointmentSlotsAreRetrievedWithoutACookie() {
        retrieveAppointmentSlots(toLocalTime(defaultSessionStartDate),
                toLocalTime(defaultSessionEndDate),
                includeCookie = false)
    }


    @When("^the available appointment slots are retrieved without a given date-time range$")
    fun theAvailableAppointmentSlotsAreRetrievedWithoutExplicitDateRange() {

        retrieveAppointmentSlots()
    }

    @When("^the available appointment slots are retrieved with just a from date$")
    fun theAvailableAppointmentSlotsAreRetrievedWithJustFromDate() {

        retrieveAppointmentSlots(fromDate = toLocalTime(explicitFromDate), toDate = null)
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

        var toDate = LocalDateTime.parse(defaultSessionEndDate, dateTimeFormat).format(DateTimeFormatter.BASIC_ISO_DATE)
        retrieveAppointmentSlots(defaultSessionStartDate, toDate)
    }

    @When("^I try to retrieve appointment slots with a malformed from Date$")
    fun tryToRetrieveAppointmentSlotsWithMalformedFromDate() {

        var fromDate = LocalDateTime.parse(defaultSessionStartDate, dateTimeFormat).format(DateTimeFormatter.BASIC_ISO_DATE)
        retrieveAppointmentSlots(fromDate, defaultSessionEndDate)
    }

    private fun retrieveAppointmentSlots(fromDate: String? = null, toDate: String? = null, includeCookie: Boolean = true) {
        var cookie: Cookie? = null;

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
        AssertAppointmentSlotsResponseNotNull(result)
        Assert.assertEquals("result.slots", 0, result.slots.size)
    }

    //This step needs to assert the date, and also have a reason for asserting the number '2'
    @Then("^available slots are returned for the two weeks following the from date$")
    fun availableSlotsLocationsCliniciansAndAppointmentSessionsForTwoWeeksFollowingFromDate() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        AssertAppointmentSlotsResponseNotNull(result)
        Assert.assertEquals("result.slots", 2, result.slots.size)
    }

    private fun AssertAppointmentSlotsResponseNotNull(result: AppointmentSlotsResponse) {
        Assert.assertNotNull("result", result)
        Assert.assertNotNull("result.slots", result.slots)
    }

    //This last line is based on breaking down the request and asserting details from that. This seems incorrect
    @Then("^available slots, locations, clinicians and appointment sessions are returned for the next two weeks$")
    fun availableSlotsLocationsCliniciansAndAppointmentSessionsForNextTwoWeeksAreReturned() {
        val wiremockRequests = mockingClient.getRequests().split("\n")
        var fromDateLineIndex = 0
        for (mappingline in wiremockRequests) {
            if (mappingline.contains("\"key\" : \"fromDateTime\"")) {
                fromDateLineIndex = wiremockRequests.indexOf(mappingline) + 1
                break
            }
        }
        val actualFromDate = LocalDateTime.parse(wiremockRequests[fromDateLineIndex].split("\"values\" : [ \"")[1].dropLast(3))
        val expectedFromDate = LocalDateTime.parse(defaultSessionStartDate)
        // Verify that the actual fromDate is within 10 minutes either side of the expected date to account for slight time difference on the server
        Assert.assertTrue(String.format("Expected a time around %s, but actual was %s", expectedFromDate.toString(), actualFromDate.toString()),
                actualFromDate.isAfter(expectedFromDate.minusMinutes(10))
                        && actualFromDate.isBefore(expectedFromDate.plusMinutes(10)))
    }


    private fun toLocalTime(date: String?): String {
        val currentDateFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        currentDateFormat.timeZone = TimeZone.getDefault()
        val dateToPass = currentDateFormat.parse(date, ParsePosition(0))
        val queryDateFormat = SimpleDateFormat(backendDateTimeFormatWithTimezone)
        return queryDateFormat.format(dateToPass)
    }
}
