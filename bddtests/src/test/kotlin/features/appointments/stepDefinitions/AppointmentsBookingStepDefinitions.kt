package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.AppointmentsBookingSteps
import features.appointments.steps.AppointmentsSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.MockDefaults
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.appointmentSlots.AvailableSlotsJourney
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.models.*
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus.*
import org.junit.Assert.*
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import worker.models.appointments.SlotResponseObject
import java.text.ParsePosition
import java.text.SimpleDateFormat
import java.time.Duration
import java.util.*
import javax.servlet.http.Cookie


class AppointmentsBookingStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var appointments: AppointmentsSteps
    @Steps
    lateinit var appointmentsBooking: AppointmentsBookingSteps
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    private val explicitFromDate = "2018-12-24T14:00:00"
    private val explicitToDate = "2018-12-30T14:00:00"
    private val defaultSessionStartDate = explicitFromDate
    private val defaultSessionEndDate = explicitToDate

    private val defaultEmisAppointmentSlots = arrayListOf(
            AppointmentSlot(
                    slotId = 301,
                    startTime = "2018-12-27T14:30:00",
                    endTime = "2018-12-27T15:00:00",
                    slotTypeName = "Immunisations",
                    slotTypeStatus = SlotTypeStatus.Visit
            ),
            AppointmentSlot(
                    slotId = 302,
                    startTime = "2018-12-28T09:00:00",
                    endTime = "2018-12-28T09:30:00",
                    slotTypeName = "Back",
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
                    "Nurse Clinic",
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
                    "Physio",
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

    @Given("^there are available appointment slots for an explicit date-time range$")
    fun thereAreAvailableAppointmentSlotsForAnExplicitDateTimeRange() {
        generateStubsForAppointmentSlotsForSpecificDates()
    }

    @Given("^there are available appointment slots$")
    fun thereAreAvailableAppointmentSlots() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patient)
        EmisSessionCreateJourneyFactory(mockingClient).createFor(MockDefaults.patient)
        AvailableSlotsJourney(mockingClient).create(patient = MockDefaults.patient)
        generateStubsForAppointmentSlotsForNextTwoWeeks()
    }

    @Given("^GP system doesn't respond a timely fashion for available appointment slots$")
    @Throws(Exception::class)
    fun gp_system_doesn_t_respond_a_timely_fashion_for_available_appointment_slots() {
        generateStubsForAppointmentSlotsForNextTwoWeeks(delayedInSeconds = 30)
    }

    @Given("^there is a slight delay in retrieving them$")
    @Throws(Exception::class)
    fun slightDelayForRetrievingAvailableAppointmentSlots() {
        generateStubsForAppointmentSlotsForNextTwoWeeks(delayedInSeconds = 1)
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

    @Then("^available slots are returned for the given date-time range$")
    fun availableSlotsLocationsCliniciansAndAppointmentSessionsAreReturned() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull(result)
        assertNotNull(result.slots)
        assertEquals(2, result.slots.size)
    }

    @Then("^available slots are returned containing id, start date and time, end date and time, location, clinicians, type$")
    fun availableSlotsAreReturnedWithAppropriateFields() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedSlots = HashMap<String, SlotResponseObject>()
        for (i in 0..1)
            unmatchedExpectedSlots[defaultEmisAppointmentSlots[i].slotId.toString()] = SlotResponseObject(
                    defaultEmisAppointmentSlots[i].slotId.toString(),
                    defaultEmisMetaSlotSessions[i].sessionName + " - " + defaultEmisAppointmentSlots[i].slotTypeName,
                    defaultEmisAppointmentSlots[i].startTime!!,
                    defaultEmisAppointmentSlots[i].endTime!!,
                    defaultEmisMetaSlotLocations[i].locationName.toString(),
                    arrayOf(defaultEmisMetaSlotSessionHolders[i].displayName)
            )
        for (actualSlot in result.slots) {
            println(actualSlot.toString())
            assertNotNull(actualSlot.id)
            assertNotNull(actualSlot.type)
            assertNotNull(actualSlot.startTime)
            assertNotNull(actualSlot.endTime)
            assertNotNull(actualSlot.location)
            assertNotNull(actualSlot.clinicians)
            val expectedSlot = unmatchedExpectedSlots[actualSlot.id]!!
            assertNotNull("Expected slot not found. ", expectedSlot)
            assertEquals(expectedSlot.type, actualSlot.type)
            assertEquals(expectedSlot.startTime + "+00:00", actualSlot.startTime)
            assertEquals(expectedSlot.endTime + "+00:00", actualSlot.endTime)
            assertEquals(expectedSlot.location, actualSlot.location)
            assertEquals(expectedSlot.clinicians.toSet(), actualSlot.clinicians.toSet())
            unmatchedExpectedSlots.remove(actualSlot.id)
        }
        assertTrue("Expected Slots missing. ", unmatchedExpectedSlots.isEmpty())
    }

    private fun getSlotNameForSession(expectedSession: Session): String? {
        for (appointmentSession in defaultEmisAppointmentSessions) {
            if (expectedSession.sessionId == appointmentSession.sessionId) {
                for (appointmentSlot in defaultEmisAppointmentSlots) {
                    if (appointmentSession.slots[0] == appointmentSlot) {
                        return appointmentSlot.slotTypeName
                    }
                }
            }
        }
        return null
    }

    @Then("^I get a response with an empty set of slots$")
    fun emptySetsAreReturned() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        assertNotNull(result)
        assertNotNull(result.slots)
        assertEquals(0, result.slots.size)
    }

    @Then("^I see appropriate information message for time-outs$")
    @Throws(Exception::class)
    fun iSeeAppropriateInformationMessageAfterSecondsWhenItTimesOut() {
        appointmentsBooking.checkTimeoutErrorMessage()
    }

    @Then("^I don't see a time-out error$")
    @Throws(Exception::class)
    fun iDoNotSeeATimeOutError() {
        appointmentsBooking.checkTimeoutErrorMessage(false)
    }

    @Then("^there should be a button to try again$")
    @Throws(Exception::class)
    fun there_should_be_a_button_to_try_again() {
        appointmentsBooking.checkIfTryAgainButtonDisplayed()
    }

    @Then("^I see appropriate information message when there is a error retrieving data$")
    @Throws(Exception::class)
    fun i_see_appropriate_information_message_when_there_is_a_error_retrieving_data() {
        appointmentsBooking.checkUnavailableErrorMessage()
    }

    @Then("^there should not be an option to try again$")
    @Throws(Exception::class)
    fun there_should_not_be_an_option_to_try_again() {
        appointmentsBooking.checkIfTryAgainButtonIsNotDisplayed()
    }

    @When("^I click try again button on appointment page$")
    @Throws(Exception::class)
    fun i_click_try_again_button_on_appointment_page() {
        appointmentsBooking.clickOnTryAgainButton()
    }

    @Then("^I see available appointment slots$")
    @Throws(Exception::class)
    fun i_see_available_appointment_slots() {
        appointmentsBooking.checkIfSlotsAreDisplayed()
    }

    @Then("^I don't see appointment slots$")
    @Throws(Exception::class)
    fun iDoNotSeeAppointmentSlots() {
        appointmentsBooking.checkIfSlotsAreNotDisplayed()
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
        AvailableSlotsJourney(mockingClient).create()
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

    @Then("^I am taken to the available appointment slots screen$")
    @Throws(Exception::class)
    fun i_am_taken_to_the_available_appointment_slots_screen() {
        appointmentsBooking.checkIfPageHeaderIsCorrect()
    }
}