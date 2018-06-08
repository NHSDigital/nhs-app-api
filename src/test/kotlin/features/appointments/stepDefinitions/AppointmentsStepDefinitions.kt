package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.AppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import junit.framework.Assert.*
import mocking.defaults.MockDefaults
import mocking.MockingClient
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.models.*
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus.*
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import worker.models.appointments.SlotResponseObject
import java.text.ParsePosition
import java.text.SimpleDateFormat
import java.time.Duration
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter
import java.util.*
import javax.servlet.http.Cookie


class AppointmentsStepDefinitions {


    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var appointments: AppointmentsSteps
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    private lateinit var slotsModel: GetAppointmentSlotsResponseModel
    private lateinit var metaModel: GetAppointmentSlotsMetaResponseModel

    private val pastFromDate = "2017-12-24T14:00:00"
    private val pastToDate = "2017-12-30T14:00:00"
    private val explicitFromDate = "2018-12-24T14:00:00"
    private val explicitToDate = "2018-12-30T14:00:00"
    private val defaultFromDateIfExplicitToDate = "2018-12-16T00:00:00"
    private val defaultToDateIfExplicitFromDate = "2019-01-08T00:00:00"
    private val dateTimeFormat = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss")
    private val defaultFromDateAsDate = LocalDateTime.now()
    private val defaultFromDate = defaultFromDateAsDate.format(dateTimeFormat)
    private val defaultToDate = LocalDateTime.parse(defaultFromDate, dateTimeFormat)
            .plusWeeks(2)
            .plusDays(1)
            .withHour(0)
            .withMinute(0)
            .withSecond(0)
            .format(dateTimeFormat)

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

    private val expiredCookie = Cookie(
            "Set-Cookie",
            "NHSO-Session-Id=CfDJ8E-ofjSQjqFFrq_TwyjSrr7YjXlzOKAjF2FCuRKQQd8XJLpr5jIZqua3RLYU0ItlMH7Df-uLnLiWc-mUSPveE-ElNNa-tsTVCxD_SomXW3aSvuGh3Dc9Dqe9jFyGLVu5SPrcqg9hafdTKTS7EqEaz2fwsQK8Br_flD7PpImRUjNNFEF0iFNsJTXJm5FZBVBeXvbPe8obyufPFt2Lpti8naW2xlbMb9wGq5g--UjOyDnQbxY1RxCR4tU-rHpdyz0JcbStgePRwhiM14wfoUsUFz4tnNeoYbaPLXaCiXVNm6NzG9SaQMheda0A6zxTv1y0nwu8AAXcUg7EFlSxIKLJV7B7aC0GCiUDAwkxMnzHP6sm; path=/; secure; samesite=lax; httponly"
    )


    @Given("^I am on the appointments page")
    fun iAmOnTheAppointmentsPage() {
        browser.goToApp()
        login.asDefault()
        navigation.select("appointments")
    }

    @Given("^there are available appointment slots for an explicit date-time range$")
    fun thereAreAvailableAppointmentSlotsForAnExplicitDateTimeRange() {
        generateStubsForAppointmentSlotsForSpecificDates()
    }

    @Given("^there are available appointment slots$")
    fun thereAreAvailableAppointmentSlots() {
        generateStubsForAppointmentSlotsForNextTwoWeeks()
    }

    @Given("^GP system doesn't respond a timely fashion for available appointment slots$")
    @Throws(Exception::class)
    fun gp_system_doesn_t_respond_a_timely_fashion_for_available_appointment_slots() {
        generateStubsForAppointmentSlotsForNextTwoWeeks(delayedInSeconds = 30)
    }

    @When("^GP system responds a timely fashion for available appointment slots$")
    @Throws(Exception::class)
    fun gp_system_responds_a_timely_fashion_for_available_appointment_slots() {
        thereAreAvailableAppointmentSlots()
    }

    @Given("^GP system is unavailable for available appointment slots$")
    @Throws(Exception::class)
    fun gp_system_is_unavailable_for_available_appointment_slots() {
        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWith(SC_INTERNAL_SERVER_ERROR, 0, {
                                andHtmlBody("Internal server Error")
                            })
                }

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWith(SC_INTERNAL_SERVER_ERROR, 0, {
                                andHtmlBody("Internal server Error")
                            })
                }
    }

    @Given("^GP system returns corrupt data for appointment slots$")
    @Throws(Exception::class)
    fun gp_system_returns_corrupt_data_for_appointment_slots() {
        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWith(SC_OK, 0, {
                                andHtmlBody("appointment slots metadata")
                            })
                }

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWith(SC_OK, 0, {
                                andHtmlBody("appointment slots")
                            })
                }
    }


    @Given("^there are available appointment slots, but session has expired$")
    fun thereAreAvailableAppointmentSlotsButExpiredSession() {
        thereAreAvailableAppointmentSlots()
        Serenity.setSessionVariable(Cookie::class).to(expiredCookie)
    }

    @Given("^the practice does not offer online booking to my patient$")
    fun appointmentBookingUnavailableToPatientWhenWantingToViewAppointmentSlots() {
        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient, defaultSessionStartDate, defaultSessionEndDate)
                            .respondWithExceptionWhenNotEnabled()
                }

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient, defaultSessionStartDate, defaultSessionEndDate)
                            .respondWithExceptionWhenNotEnabled()
                }
    }

    @Given("^an unknown exception will occur when wanting to view appointment slots$")
    fun unknownExceptionWhenWantingToViewAppointmentSlots() {
        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient, defaultSessionStartDate, defaultSessionEndDate)
                            .respondWithUnknownException()
                }

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient, defaultSessionStartDate, defaultSessionEndDate)
                            .respondWithUnknownException()
                }
    }

    @When("^the available appointment slots are retrieved for explicit date-time range$")
    fun theAvailableAppointmentSlotsAreRetrievedForExplicitDateTimeRange() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .getAppointmentSlots(toLocalTime(defaultSessionStartDate),
                            toLocalTime(defaultSessionEndDate),
                            Serenity.sessionVariableCalled<Cookie>(Cookie::class))
            Serenity.setSessionVariable(AppointmentSlotsResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }
    }

    @Then("^available slots, locations, clinicians and appointment sessions are returned for the given date-time range$")
    fun availableSlotsLocationsCliniciansAndAppointmentSessionsAreReturned() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull(result)
        assertNotNull(result.slots)
        assertNotNull(result.locations)
        assertNotNull(result.clinicians)
        assertNotNull(result.appointmentSessions)
        assertEquals(2, result.slots.size)
        assertEquals(2, result.locations.size)
        assertEquals(2, result.clinicians.size)
        assertEquals(2, result.appointmentSessions.size)
    }

    @Then("^available slots are returned containing id, start date and time, end date and time, location identifier, appointment session identifier, clinician identifiers$")
    fun availableSlotsAreReturnedWithAppropriateFields() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedSlots = HashMap<String, SlotResponseObject>()
        for (i in 0..1)
            unmatchedExpectedSlots[defaultEmisAppointmentSlots[i].slotId.toString()] = SlotResponseObject(
                    defaultEmisAppointmentSlots[i].slotId.toString(),
                    defaultEmisAppointmentSlots[i].startTime!!,
                    defaultEmisAppointmentSlots[i].endTime!!,
                    defaultEmisMetaSlotSessions[i].locationId.toString(),
                    defaultEmisMetaSlotSessions[i].sessionId.toString(),
                    defaultEmisMetaSlotSessions[i].clinicianIds.map { it.toString() }.toTypedArray()
            )
        for (actualSlot in result.slots) {
            println(actualSlot.toString())
            assertNotNull(actualSlot.id)
            assertNotNull(actualSlot.startTime)
            assertNotNull(actualSlot.endTime)
            assertNotNull(actualSlot.locationId)
            assertNotNull(actualSlot.appointmentSessionId)
            assertNotNull(actualSlot.clinicianIds)
            val expectedSlot = unmatchedExpectedSlots[actualSlot.id]!!
            assertNotNull("Expected slot not found. ", expectedSlot)
            assertEquals(expectedSlot.startTime + "+00:00", actualSlot.startTime)
            assertEquals(expectedSlot.endTime + "+00:00", actualSlot.endTime)
            assertEquals(expectedSlot.locationId, actualSlot.locationId)
            assertEquals(expectedSlot.appointmentSessionId, actualSlot.appointmentSessionId)
            assertEquals(expectedSlot.clinicianIds.toSet(), actualSlot.clinicianIds.toSet())
            unmatchedExpectedSlots.remove(actualSlot.id)
        }
        assertTrue("Expected Slots missing. ", unmatchedExpectedSlots.isEmpty())
    }


    @Then("^available locations are returned containing an id and display name$")
    fun availableLocationsAreReturnedWithAppropriateFields() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedLocations = HashMap<String, Location>()
        for (location in defaultEmisMetaSlotLocations)
            unmatchedExpectedLocations[location.locationId.toString()] = location
        for (actualLocation in result.locations) {
            println(actualLocation.toString())
            assertNotNull(actualLocation.id)
            assertNotNull(actualLocation.displayName)
            val expectedLocation = unmatchedExpectedLocations[actualLocation.id]
            assertNotNull(expectedLocation)
            assertEquals(expectedLocation!!.locationName, actualLocation.displayName)
            unmatchedExpectedLocations.remove(actualLocation.id)
        }
        assertTrue("Expected Locations missing. ", unmatchedExpectedLocations.isEmpty())
    }

    @Then("^available clinicians are returned containing an id and display name$")
    fun availableCliniciansAreReturnedWithAppropriateFields() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedClinicians = HashMap<String, SessionHolder>()
        for (clinician in defaultEmisMetaSlotSessionHolders)
            unmatchedExpectedClinicians[clinician.clinicianId.toString()] = clinician
        for (actualClinician in result.clinicians) {
            println(actualClinician.toString())
            assertNotNull(actualClinician.id)
            assertNotNull(actualClinician.displayName)
            val expectedClinician = unmatchedExpectedClinicians[actualClinician.id]
            assertNotNull(expectedClinician)
            assertEquals(expectedClinician!!.displayName, actualClinician.displayName)
            unmatchedExpectedClinicians.remove(actualClinician.id)
        }
        assertTrue("Expected Clinicians missing. ", unmatchedExpectedClinicians.isEmpty())
    }

    @Then("^available appointment session are returned containing an id and display name$")
    fun availableAppointmentSessionsAreReturnedWithAppropriateFields() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedSessions = HashMap<String, Session>()
        for (session in defaultEmisMetaSlotSessions)
            unmatchedExpectedSessions[session.sessionId.toString()] = session
        for (actualAppointmentSession in result.appointmentSessions) {
            println(actualAppointmentSession.toString())
            assertNotNull(actualAppointmentSession.id)
            assertNotNull(actualAppointmentSession.displayName)
            val expectedSession = unmatchedExpectedSessions[actualAppointmentSession.id]
            assertNotNull(expectedSession)
            assertEquals(expectedSession!!.sessionType.toString(), actualAppointmentSession.displayName)
            unmatchedExpectedSessions.remove(actualAppointmentSession.id)
        }
        assertTrue("Expected Appointment Session missing. ", unmatchedExpectedSessions.isEmpty())
    }

    @Then("^I get a response with an empty set of slots$")
    fun emptySetsAreReturned() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull(result)
        assertNotNull(result.slots)
        assertNotNull(result.locations)
        assertNotNull(result.clinicians)
        assertNotNull(result.appointmentSessions)
        assertEquals(0, result.slots.size)
        assertEquals(0, result.locations.size)
        assertEquals(0, result.clinicians.size)
        assertEquals(0, result.appointmentSessions.size)
    }

    @Then("^I see appropriate information message after 10 seconds when it times-out$")
    @Throws(Exception::class)
    fun i_see_appropriate_information_message_after_seconds_when_it_times_out() {
        appointments.checkTimeoutErrorMessage()
    }

    @Then("^there should be a button to try again$")
    @Throws(Exception::class)
    fun there_should_be_a_button_to_try_again() {
        appointments.checkIfTyAgainButtonDisplayed()
    }

    @Then("^I see appropriate information message when there is a error retrieving data$")
    @Throws(Exception::class)
    fun i_see_appropriate_information_message_when_there_is_a_error_retrieving_data() {
        appointments.checkUnavailableErrorMessage()
    }

    @Then("^there should not be an option to try again$")
    @Throws(Exception::class)
    fun there_should_not_be_an_option_to_try_again() {
        appointments.checkIfTyAgainButtonIsNotDisplayed()
    }

    @When("^I click try again button on appointment page$")
    @Throws(Exception::class)
    fun i_click_try_again_button_on_appointment_page() {
        appointments.clickOnTryAgainButton()
    }

    @Then("^I see available appointment slots$")
    @Throws(Exception::class)
    fun i_see_available_appointment_slots() {
        appointments.checkIfSlotsAreDisplayed()
    }


    private fun generateStubsForAppointmentSlotsForSpecificDates(
            emisSlotLocations: ArrayList<Location> = defaultEmisMetaSlotLocations,
            emisSlotSessionHolders: ArrayList<SessionHolder> = defaultEmisMetaSlotSessionHolders,
            emisSlotSessions: ArrayList<Session> = defaultEmisMetaSlotSessions,
            emisAppointmentSessions: ArrayList<AppointmentSession> = defaultEmisAppointmentSessions,
            delayedInSeconds: Long = 0
    ) {
        generateStubForMetaAppointmentSlotRequest(
                emisSlotLocations,
                emisSlotSessionHolders,
                emisSlotSessions,
                delayedInSeconds,
                defaultSessionStartDate,
                defaultSessionEndDate
        )

        generateStubForAppointmentSlotRequest(
                emisAppointmentSessions,
                delayedInSeconds,
                defaultSessionStartDate,
                defaultSessionEndDate
        )
    }

    private fun generateStubsForAppointmentSlotsForNextTwoWeeks(
            emisSlotLocations: ArrayList<Location> = defaultEmisMetaSlotLocations,
            emisSlotSessionHolders: ArrayList<SessionHolder> = defaultEmisMetaSlotSessionHolders,
            emisSlotSessions: ArrayList<Session> = defaultEmisMetaSlotSessions,
            emisAppointmentSessions: ArrayList<AppointmentSession> = defaultEmisAppointmentSessions,
            delayedInSeconds: Long = 0
    ) {
        generateStubForMetaAppointmentSlotRequest(
                emisSlotLocations,
                emisSlotSessionHolders,
                emisSlotSessions,
                delayedInSeconds
        )

        generateStubForAppointmentSlotRequest(emisAppointmentSessions, delayedInSeconds)

        appointmentsConfirmationSteps.mockEmisSuccessResponse()
    }

    private fun generateStubForMetaAppointmentSlotRequest(
            emisSlotLocations: ArrayList<Location>,
            emisSlotSessionHolders: ArrayList<SessionHolder>,
            emisSlotSessions: ArrayList<Session>,
            delayedInSeconds: Long,
            fromDate: String? = null,
            toDate: String? = null
    ) {
        val getAppointmentSlotsMetaResponseModel = GetAppointmentSlotsMetaResponseModel(
                emisSlotLocations,
                emisSlotSessionHolders,
                emisSlotSessions
        )
        mockingClient.forEmis {
            appointmentSlotsMetaRequest(patient, fromDate, toDate)
                    .respondWithSuccess(getAppointmentSlotsMetaResponseModel)
                    .delayedBy(Duration.ofSeconds(delayedInSeconds))
        }
    }

    private fun generateStubForAppointmentSlotRequest(
            emisAppointmentSessions: ArrayList<AppointmentSession>,
            delayedInSeconds: Long,
            fromDate: String? = null,
            toDate: String? = null
    ) {
        val getAppointmentSlotsResponseModel = GetAppointmentSlotsResponseModel(emisAppointmentSessions)
        mockingClient.forEmis {
            appointmentSlotsRequest(patient, fromDate, toDate)
                    .respondWithSuccess(getAppointmentSlotsResponseModel)
                    .delayedBy(Duration.ofSeconds(delayedInSeconds))
        }
    }

    private fun toLocalTime(date: String?): String {
        val currentDateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")
        currentDateFormat.timeZone = TimeZone.getDefault()
        val dateToPass = currentDateFormat.parse(date, ParsePosition(0))
        val queryDateFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ssZ")
        return queryDateFormat.format(dateToPass)
    }


    /*************************************************/
    // STEPS COPIED FROM OLD REPO -- needs cleaning
    /*************************************************/


//    val availableAppointmentsPage = appointments
//
//
//    @And("^there are available appointment slots$")
//    fun there_are_available_appointment_slots() {
//        val expectedSlots = createAppointmentSlotsResponse()
//        val expectedMeta = createAppointmentSlotMetaModel()
//        setExpectedSlots(expectedSlots)
//        setExpectedMeta(expectedMeta)
//
//        mockingClient
//                .forEmis {
//                    appointmentSlotsRequest(patient)
//                            .respondWithSuccess(expectedSlots)
//                }
//
//        mockingClient
//                .forEmis {
//                    appointmentSlotsMetaRequest(patient)
//                            .respondWithSuccess(expectedMeta)
//                }
//    }
//
//    @Given("^there are available appointment slots with long location name$")
//    fun there_are_available_appointment_slots_with_long_location_name() {
//
//        val model = createAppointmentSlotMetaModel()
//        model.locations[0].locationName = "Extra long location name that will be truncated"
//
//        mockingClient
//                .forEmis {
//                    appointmentSlotsRequest(patient)
//                            .respondWithSuccess(createAppointmentSlotsResponse())
//                }
//
//        mockingClient
//                .forEmis {
//                    appointmentSlotsMetaRequest(patient)
//                            .respondWithSuccess(model)
//                }
//    }
//
//    @Given("^there are available appointment slots with location name length less or equal 24 characters$")
//    fun there_are_available_appointment_slots_with_location_name_length_less_or_equal_24_characters() {
//        val model = createAppointmentSlotMetaModel()
//        model.locations[0].locationName = "Location with 24 chars."
//
//        mockingClient
//                .forEmis {
//                    appointmentSlotsRequest(patient)
//                            .respondWithSuccess(createAppointmentSlotsResponse())
//                }
//
//        mockingClient
//                .forEmis {
//                    appointmentSlotsMetaRequest(patient)
//                            .respondWithSuccess(model)
//                }
//    }
//
//    @Given("^there are available appointment slots with long clinician name$")
//    fun there_are_available_appointment_slots_with_long_clinician_name() {
//        AppointmentsWithCustomClinicianNameLengthFactory().mockEmisWithLongClinicianName()
//    }
//
//    @Given("^there are available appointment slots with the Clinician Name length less or equal than 24 characters$")
//    fun there_are_available_appointment_slots_with_the_Clinician_Name_length_less_or_equal_than_24_characters() {
//        AppointmentsWithCustomClinicianNameLengthFactory().mockEmisWithShortClinicianName()
//    }
//
//    @Given("^there are available appointment slots with long session name$")
//    fun there_are_available_appointment_slots_with_long_session_name() {
//        AppointmentsWithCustomSessionNameLengthFactory().mockEmisWithLongSessionName()
//    }
//
//    @Given("^there are available appointment slots with the AppointmentSlotSession Name length less or equal than 24 characters$")
//    fun there_are_available_appointment_slots_with_the_Session_Name_length_less_or_equal_than_24_characters() {
//        AppointmentsWithCustomSessionNameLengthFactory().mockEmisWithShortSessionName()
//    }
//
//    @Given("^there are available appointment slots with some in BST and some in GMT$")
//    fun there_are_available_appointment_slots_with_some_in_BST_and_some_in_GMT() {
//        AppointmentsInBSTAndGMTtimeZoneFactory().mockEmis()
//    }
//
//    @Given("^there are no available slots$")
//    fun there_are_no_available_slots() {
//
//    }
//
//    @Then("^I see available appointment slots for the next 2 weeks$")
//    fun i_see_available_appointment_slots_for_the_next_2_weeks() {
//
//    }
//
//    @Then("^the appointment slots are ordered ascending start date and time then first clinician name$")
//    fun the_appointment_slots_are_ordered_ascending_start_date_and_time_then_first_clinician_name() {
//
//    }
//
//    @Then("^I see available slots display date in correct format$")
//    fun i_see_available_slots_display_date_in_correct_format() {
//        var dayOfTheWeek = "(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)"
//        var months = "(January|February|March|April|May|June|July|August|September|October|November|December)"
//        var correctFormat = "^$dayOfTheWeek\\s([0-9]{2})\\s$months\\s([0-9]{4})\$"
//        var slots = availableAppointmentsPage.appointments.getAllSlots()
//
//        Assert.assertThat(slots.size, GreaterThan(0))
//
//        slots.forEach { slot ->
//            var hasCorrectFormat = Regex(correctFormat).matches(slot.date)
//            Assert.assertTrue(hasCorrectFormat)
//        }
//    }
//
//    @Then("^I see available slots display start time in correct format includes AM or PM$")
//    fun i_see_available_slots_display_start_time_in_correct_format_includes_AM_or_PM() {
//        var correctFormat = "^([01][0-9]|2[0-3]):[0-5][0-9]\\s(AM|PM)\$"
//        var slots = availableAppointmentsPage.appointments.getAllSlots()
//
//        Assert.assertThat(slots.size, GreaterThan(0))
//
//        slots.forEach { slot ->
//            var hasCorrectFormat = Regex(correctFormat).matches(slot.time)
//            Assert.assertTrue(hasCorrectFormat)
//        }
//    }
//
//    @Then("^each slot displays the start time in the timezone effective on that date$")
//    fun each_slot_displays_the_start_time_in_the_timezone_effective_on_that_date() {
//        val expectedSlots = AppointmentsInBSTAndGMTtimeZoneFactory().createExpectedSlots()
//        val actualSlots = availableAppointmentsPage.appointments.getAllSlots()
//
//        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
//    }
//
//    @Then("^I see appropriate information message when no slots are available$")
//    fun i_see_appropriate_information_message_when_no_slots_are_available() {
//        var message = availableAppointmentsPage.appointments.getInformationMessage()
//        Assert.assertEquals("There are no appointments available at the moment", message.findElement(By.cssSelector("h3")).text)
//    }
//
//    @Given("^GP system is unavailable$")
//    fun gp_system_is_unavailable() {
//
//    }
//
//    @Then("^I see appropriate information message when GP system is unavailable$")
//    fun i_see_appropriate_information_message_when_GP_system_is_unavailable() {
//        var message = availableAppointmentsPage.appointments.getInformationMessage()
//        Assert.assertEquals("GP system is unavailable", message.findElement(By.cssSelector("h3")).text)
//    }
//
//    @Then("^I see available slots with the location length greater than 24 characters is truncated$")
//    fun i_see_available_slots_with_the_location_length_greater_than_24_characters_is_truncated() {
//        var expectedSlots = AppointmentsWithCustomLocationNameLengthFactory(mockingClient).createExpectedSlotsWithLongLocationName()
//        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()
//
//        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
//    }
//
//    @Then("^I see available slots with the location length less or equal than 24 characters is shown in full$")
//    fun i_see_available_slots_with_the_location_length_less_or_equal_than_24_characters_is_shown_in_full() {
//
//    }
//
//    @Then("^I see available slots with the Clinician Name length greater than 24 characters is truncated$")
//    fun i_see_available_slots_with_the_Clinician_Name_length_greater_than_24_characters_is_truncated() {
//        var expectedSlots = AppointmentsWithCustomClinicianNameLengthFactory().createExpectedSlotsWithLongClinicianName()
//        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()
//
//        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
//    }
//
//    @Then("^I see available slots with the Clinician Name length less or equal than 24 characters is shown in full$")
//    fun i_see_available_slots_with_the_Clinician_Name_length_less_or_equal_than_24_characters_is_shown_in_full() {
//        var expectedSlots = AppointmentsWithCustomClinicianNameLengthFactory().createExpectedSlotsWithShortClinicianName()
//        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()
//
//        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
//    }
//
//    @Then("^I see available slots with the AppointmentSlotSession Name length greater than 24 characters is truncated$")
//    fun i_see_available_slots_with_the_Session_Name_length_greater_than_24_characters_is_truncated() {
//        var expectedSlots = AppointmentsWithCustomSessionNameLengthFactory().createExpectedSlotsWithLongSessionName()
//        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()
//
//        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
//    }
//
//    @Then("^I see available slots with the AppointmentSlotSession Name length less or equal 24 characters is shown in full$")
//    fun i_see_available_slots_with_the_Session_Name_length_less_or_equal_24_characters_is_shown_in_full() {
//        var expectedSlots = AppointmentsWithCustomSessionNameLengthFactory().createExpectedSlotsWithShortSessionName()
//        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()
//
//        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
//    }
//
//
//    private fun getExpectedSlots(): ArrayList<AppointmentSlot> {
//        return this.slotsModel.sessions[0].slots
//    }
//
//    private fun getExpectedMeta(): GetAppointmentSlotsMetaResponseModel {
//        return this.metaModel
//    }
//
//    private fun setExpectedMeta(meta: GetAppointmentSlotsMetaResponseModel) {
//        this.metaModel = meta
//    }
//
//    private fun setExpectedSlots(slots: GetAppointmentSlotsResponseModel) {
//        this.slotsModel = slots
//    }
//
//    private fun createAppointmentSlotMetaModel(): GetAppointmentSlotsMetaResponseModel {
//        val locations = arrayListOf(
//                Location(
//                        locationId = 1,
//                        locationName = "Some surgery",
//                        numberAndStreet = "12 Some Street",
//                        village = "A Village",
//                        town = "N E Town",
//                        postcode = "ANY12 2NE"
//                )
//        )
//
//        val sessionHolders = arrayListOf(
//                SessionHolder(
//                        clinicianId = 1,
//                        displayName = "Mr. Frank Pickles",
//                        forenames = "Frank Timothy",
//                        surname = "Pickles",
//                        title = "Mr",
//                        sex = Sex.Male,
//                        jobRole = "Nurse"
//                )
//        )
//
//        val sessions = arrayListOf(
//                Session(
//                        sessionId = 1,
//                        sessionName = "Ear syringing",
//                        locationId = 1,
//                        defaultDuration = 20,
//                        sessionType = SessionType.Untimed,
//                        numberOfSlots = 4,
//                        clinicianIds = listOf(1)
//                ),
//                Session(
//                        sessionName = "GP consultation",
//                        sessionId = 2,
//                        locationId = 1,
//                        defaultDuration = 15,
//                        sessionType = SessionType.Timed,
//                        numberOfSlots = 2,
//                        clinicianIds = listOf(2)
//                )
//        )
//
//        return GetAppointmentSlotsMetaResponseModel(
//                locations = locations,
//                sessionHolders = sessionHolders,
//                sessions = sessions
//        )
//    }
//
//    private fun createAppointmentSlotsResponse(): GetAppointmentSlotsResponseModel {
//        return GetAppointmentSlotsResponseModel(
//                arrayListOf(
//                        AppointmentSession(
//                                slots = arrayListOf(
//                                        AppointmentSlot(
//                                                slotId = 1,
//                                                startTime = "2018-05-08T13:00:00.000Z",
//                                                endTime = "2018-05-08T13:15:00.000Z",
//                                                slotTypeName = "GP appointment",
//                                                slotTypeStatus = SlotTypeStatus.Practice
//                                        ),
//                                        AppointmentSlot(
//                                                slotId = 2,
//                                                startTime = "2018-05-08T13:00:00.000Z",
//                                                endTime = "2018-05-08T13:15:00.000Z",
//                                                slotTypeName = "Hearing test",
//                                                slotTypeStatus = SlotTypeStatus.Practice
//                                        )
//                                )
//                        )
//                )
//        )
//    }
}