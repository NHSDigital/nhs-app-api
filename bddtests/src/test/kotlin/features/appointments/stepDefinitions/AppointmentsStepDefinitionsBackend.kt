package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.defaults.MockDefaults
import mocking.MockingClient
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.models.*
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import java.text.ParsePosition
import java.text.SimpleDateFormat
import java.time.Duration
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*
import javax.servlet.http.Cookie
import org.junit.Assert


class AppointmentsStepDefinitionsBackend {

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    //Should be moved to a shared area with other appointments mocking data
    private val pastFromDate = "2017-12-24T14:00:00"
    private val pastToDate = "2017-12-30T14:00:00"
    private val explicitFromDate = "2018-12-24T14:00:00"
    private val explicitToDate = "2018-12-30T14:00:00"
    private val defaultFromDateIfExplicitToDate = "2018-12-16T00:00:00"
    private val defaultToDateIfExplicitFromDate = "2019-01-08T00:00:00"
    private val dateTimeFormat = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss")
    private val defaultSessionStartDate = explicitFromDate
    private val defaultSessionEndDate = explicitToDate


    private val defaultEmisAppointmentSlots = arrayListOf(
            AppointmentSlot(
                    slotId = 301,
                    startTime = "2018-12-27T14:30:00",
                    endTime = "2018-12-27T15:00:00",
                    slotTypeName = "Physio",
                    slotTypeStatus = SlotTypeStatus.Visit
            ),
            AppointmentSlot(
                    slotId = 302,
                    startTime = "2018-12-28T09:00:00",
                    endTime = "2018-12-28T09:30:00",
                    slotTypeName = "Physio",
                    slotTypeStatus = SlotTypeStatus.Practice
            )
    )

    private val defaultEmisMetaSlotLocations = arrayListOf(
            Location(
                    1,
                    "Sheffield"
            ),
            Location(
                    2,
                    "Leeds"
            )
    )

    private val defaultEmisMetaSlotSessionHolders = arrayListOf(
            SessionHolder(
                    101,
                    "Bob"
            ),
            SessionHolder(
                    102,
                    "Steve"
            )
    )

    private val defaultEmisMetaSlotSessions = arrayListOf(
            Session(
                    "Bob 1",
                    201,
                    1,
                    30,
                    SessionType.Timed,
                    1,
                    arrayListOf(101),
                    defaultEmisAppointmentSlots[0].startTime,
                    defaultEmisAppointmentSlots[0].endTime
            ),
            Session(
                    "Steve 2",
                    202,
                    2,
                    30,
                    SessionType.Timed,
                    1,
                    arrayListOf(102),
                    defaultEmisAppointmentSlots[1].startTime,
                    defaultEmisAppointmentSlots[1].endTime
            )
    )

    private val defaultEmisAppointmentSessions = arrayListOf(
            AppointmentSession(
                    sessionDate = defaultEmisAppointmentSlots[0].startTime,
                    sessionId = 201,
                    slots = arrayListOf(defaultEmisAppointmentSlots[0])
            ),
            AppointmentSession(
                    sessionDate = defaultEmisAppointmentSlots[1].startTime,
                    sessionId = 202,
                    slots = arrayListOf(defaultEmisAppointmentSlots[1])
            )
    )

    @Given("^there are available appointment slots within the next two weeks$")
    fun thereAreAvailableAppointmentSlotsWithinTheNextTwoWeeks() {
        val getAppointmentSlotsMetaQueryParamsForNextTwoWeeks = getAppointmentSlotsMetaQueryParams.copy(sessionStartDate = defaultSessionStartDate, sessionEndDate = defaultSessionEndDate)
        generateAppropriateStubsForAppointmentSlots(appointmentSlotsMetaQueryParams = getAppointmentSlotsMetaQueryParamsForNextTwoWeeks)
    }

    @Given("^there are available appointment slots two weeks from a specific from date$")
    fun thereAreAvailableAppointmentSlotsTwoWeeksAfterFromDate() {
        val getAppointmentSlotsMetaQueryParamsForNextTwoWeeks = getAppointmentSlotsMetaQueryParams.copy(sessionStartDate = explicitFromDate, sessionEndDate = defaultToDateIfExplicitFromDate)
        generateAppropriateStubsForAppointmentSlots(appointmentSlotsMetaQueryParams = getAppointmentSlotsMetaQueryParamsForNextTwoWeeks)
    }

    @Given("^there are available appointment slots two weeks preceding a specific to date$")
    fun thereAreAvailableAppointmentSlotsTwoWeeksBeforeToDate() {
        val getAppointmentSlotsMetaQueryParamsForNextTwoWeeks = getAppointmentSlotsMetaQueryParams.copy(sessionStartDate = defaultFromDateIfExplicitToDate, sessionEndDate = explicitToDate)
        generateAppropriateStubsForAppointmentSlots(appointmentSlotsMetaQueryParams = getAppointmentSlotsMetaQueryParamsForNextTwoWeeks)
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
        val emisSlotLocations = defaultEmisMetaSlotLocations
        val emisSlotSessionHolders = defaultEmisMetaSlotSessionHolders
        val emisSlotSessions = defaultEmisMetaSlotSessions
        val emisAppointmentSessions = defaultEmisAppointmentSessions

        mockingClient.forEmis {
            appointmentSlotsMetaRequest(getAppointmentSlotsMetaQueryParams.patient,
                    getAppointmentSlotsMetaQueryParams.sessionStartDate,
                    getAppointmentSlotsMetaQueryParams.sessionEndDate)
                    .withDelay(Duration.ofSeconds(31))
                    .respondWithSuccess(GetAppointmentSlotsMetaResponseModel(
                            emisSlotLocations,
                            emisSlotSessionHolders,
                            emisSlotSessions
                    ))
        }

        mockingClient.forEmis {
            appointmentSlotsRequest(getAppointmentSlotsMetaQueryParams.patient,
                    getAppointmentSlotsMetaQueryParams.sessionStartDate,
                    getAppointmentSlotsMetaQueryParams.sessionEndDate)
                    .withDelay(Duration.ofSeconds(31))
                    .respondWithSuccess(GetAppointmentSlotsResponseModel(emisAppointmentSessions))
        }
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

    @When("^the available appointment slots are retrieved with just a to date$")
    fun theAvailableAppointmentSlotsAreRetrievedWithJustToDate() {

        retrieveAppointmentSlots(fromDate=null, toDate = toLocalTime(explicitToDate))
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
        Assert.assertEquals("result.locations.size", 0, result.locations.size)
        Assert.assertEquals("result.clinicians.size", 0, result.clinicians.size)
        Assert.assertEquals("result.appointmentSessions.size", 0, result.appointmentSessions.size)
    }

    //This step needs to assert the date, and also have a reason for asserting the number '2'
    @Then("^available slots, locations, clinicians and appointment sessions are returned for the two weeks following the from date$")
    fun availableSlotsLocationsCliniciansAndAppointmentSessionsForTwoWeeksFollowingFromDate() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        AssertAppointmentSlotsResponseNotNull(result)
        Assert.assertEquals("result.slots", 2, result.slots.size)
        Assert.assertEquals("result.locations.size", 2, result.locations.size)
        Assert.assertEquals("result.clinicians.size", 2, result.clinicians.size)
        Assert.assertEquals("result.appointmentSessions.size", 2, result.appointmentSessions.size)
    }

    //This step needs to assert the date, and also have a reason for asserting the number '2'
    @Then("^available slots, locations, clinicians and appointment sessions are returned for the two weeks preceding the to date$")
    fun availableSlotsLocationsCliniciansAndAppointmentSessionsForTwoWeeksPrecedingToDate() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        AssertAppointmentSlotsResponseNotNull(result)
        Assert.assertEquals("result.slots", 2, result.slots.size)
        Assert.assertEquals("result.locations.size", 2, result.locations.size)
        Assert.assertEquals("result.clinicians.size", 2, result.clinicians.size)
        Assert.assertEquals("result.appointmentSessions.size", 2, result.appointmentSessions.size)
    }

    private fun AssertAppointmentSlotsResponseNotNull(result: AppointmentSlotsResponse) {
        Assert.assertNotNull("result", result)
        Assert.assertNotNull("result.slots", result.slots)
        Assert.assertNotNull("result.locations", result.locations)
        Assert.assertNotNull("result.clinicians", result.clinicians)
        Assert.assertNotNull("result.appointmentSessions", result.appointmentSessions)
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
        val currentDateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")
        currentDateFormat.timeZone = TimeZone.getDefault()
        val dateToPass = currentDateFormat.parse(date, ParsePosition(0))
        val queryDateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ssZ")
        return queryDateFormat.format(dateToPass)
    }
}
