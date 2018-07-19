package features.appointments.steps

import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithTimezone
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import constants.AppointmentDateTimeFormat.Companion.frontendDateFormat
import constants.AppointmentDateTimeFormat.Companion.frontendTimeFormat
import features.appointments.data.AppointmentsBookingData
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.models.*
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert.*
import pages.appointments.AvailableAppointmentsPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import worker.models.appointments.SlotResponseObject
import java.text.ParsePosition
import java.text.SimpleDateFormat
import java.time.Duration
import java.util.*
import javax.servlet.http.Cookie
import kotlin.collections.ArrayList

open class AvailableAppointmentsSteps : AppointmentsBookingData() {

    private val pageHeader by lazy { "Book new appointment" }
    private val bookThisButtonText by lazy { "Continue" }
    private val backButtonText by lazy { "Back to my appointments" }
    private val appointmentTypeDefaultOption by lazy { "Select type" }
    private val locationDefaultOption by lazy { "Select location" }
    private val clinicianDefaultOption by lazy { "No preference" }

    private val timePeriodOptions by lazy {
        mapOf(
                Pair(TODAY_KEY, TODAY_KEY),
                Pair(TOMORROW_KEY, TOMORROW_KEY),
                Pair(THIS_WEEK_KEY, THIS_WEEK_KEY),
                Pair(NEXT_WEEK_KEY, NEXT_WEEK_KEY),
                Pair(ALL_KEY, ALL_KEY)
        )
    }

    @Steps
    private lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    private lateinit var availableAppointments: AvailableAppointmentsPage

    @Step
    fun selectSlot() {
        availableAppointments.selectFirstSlot()
    }

    @Step
    fun checkIfPageHeaderIsCorrect() {
        val actualHeader = availableAppointments.getPageHeaderText()
        assertEquals("Expected Header text $pageHeader of the page is not found",
                pageHeader, actualHeader)
    }

    @Step
    fun clickOnBookAppointmentButton() {
        availableAppointments.clickOnButtonContainingText(bookThisButtonText)
    }

    @Step
    fun clickOnBackButton() {
        availableAppointments.clickOnButtonContainingText(backButtonText)
    }

    @Step
    fun checkTimeoutErrorMessage(presence: Boolean = true) {
        val errorSummary = availableAppointments.errorSummaryBody

        if (presence) {
            val expectedHeader = "Sorry, there's been a problem loading this page"
            val expectedBody = "Please try again\n" +
                    "If the problem persists and you need to book an appointment now, contact your GP surgery directly."

            assertEquals("$expectedHeader\n$expectedBody", errorSummary.element.text)
        } else {
            assertEquals("Expected no timeout error message elements, but there were ${errorSummary.elements.count()} elements matching $errorSummary.", 0, errorSummary.elements.count())
        }
    }

    @Step
    fun checkIfTryAgainButtonDisplayed() {
        val buttonExists = doesTryAgainButtonExist()
        assertTrue("Try again button is not present. ", buttonExists)
    }

    @Step
    fun checkIfTryAgainButtonIsNotDisplayed() {
        val buttonExists = doesTryAgainButtonExist()
        assertFalse("Try again button is present, but shouldn't be. ", buttonExists)
    }

    private fun doesTryAgainButtonExist(): Boolean {
        return availableAppointments.tryAgainButton.elements.count() == 1
    }

    @Step
    fun checkUnavailableErrorMessage() {
        val message = availableAppointments.errorSummaryBody.element.text
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedBody = "Please try again later. If the problem persists and you need to book an appointment now, contact your GP surgery directly."
        assertNotNull("No error message displayed, expecting $expectedHeader\n$expectedBody", message)
        assertTrue("Actual text: $message. Expected text: $expectedHeader\n$expectedBody", message!!.contains("$expectedHeader\n$expectedBody"))
    }

    @Step
    fun clickOnTryAgainButton() {
        availableAppointments.clickOnButtonContainingText("Try again")
    }

    @Step
    fun generateDefaultUserData() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patient)
        EmisSessionCreateJourneyFactory(mockingClient).createFor(MockDefaults.patient)
    }

    @Step
    fun generateAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem(gpSystem: String) {
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                Serenity.setSessionVariable(EXPECTED_SESSIONS_KEY).to(generateEmisSessions(13))

                val numberOfSessionsPerDay = defaultEmisMetaSlotLocations.size * (defaultEmisMetaSlotSessionHolders.size + 1)
                val allAppointmentSlots = arrayListOf<AppointmentSlot>()
                val arrayOfArrayOfAppointmentSlots = arrayListOf<ArrayList<AppointmentSlot>>()
                for (i in 2..13) {
                    repeat(numberOfSessionsPerDay) {
                        arrayOfArrayOfAppointmentSlots.add(generateEmisAppointmentSlots(i))
                        allAppointmentSlots.addAll(arrayOfArrayOfAppointmentSlots.last())
                    }
                }

                setSessionVariable(EXPECTED_APPOINTMENT_SLOTS_KEY).to(allAppointmentSlots)

                setSessionVariable(EXPECTED_APPOINTMENT_SESSIONS_KEY).to(
                        generateEmisAppointmentSessions(
                                Serenity.sessionVariableCalled<ArrayList<Session>>(EXPECTED_SESSIONS_KEY),
                                arrayOfArrayOfAppointmentSlots
                        )
                )

                generateEmisStubsForAppointmentSlotsForNextTwoWeeks(
                        defaultEmisMetaSlotLocations,
                        defaultEmisMetaSlotSessionHolders,
                        sessionVariableCalled<ArrayList<Session>>(EXPECTED_SESSIONS_KEY),
                        sessionVariableCalled<ArrayList<AppointmentSession>>(EXPECTED_APPOINTMENT_SESSIONS_KEY),
                        sessionVariableCalled<ArrayList<AppointmentSlot>>(EXPECTED_APPOINTMENT_SLOTS_KEY)
                )
            }
        }
    }

    @Step
    fun generateNoAvailableAppointmentSlotsForGPSystem(gpSystem: String) {
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                generateEmisStubsForAppointmentSlotsForNextTwoWeeks(
                        arrayListOf(),
                        arrayListOf(),
                        arrayListOf(),
                        arrayListOf()
                )
            }
        }
    }

    @Step
    fun generateAvailableOneAppointmentSlotForGPSystem(gpSystem: String) {
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                generateEmisStubsForAppointmentSlotsForNextTwoWeeks(
                        arrayListOf(defaultEmisMetaSlotLocations[0]),
                        arrayListOf(defaultEmisMetaSlotSessionHolders[0]),
                        arrayListOf(defaultEmisMetaSlotSessions[0]),
                        arrayListOf(defaultEmisAppointmentSessions[0])
                )
            }
        }
    }

    @Step
    fun generateAvailableAppointmentSlotsForGPSystemForOneLocation(gpSystem: String) {
        val modifiedSlotSessions = ArrayList<Session>(defaultEmisMetaSlotSessions)
        modifiedSlotSessions[1].locationId = modifiedSlotSessions[0].locationId
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                generateEmisStubsForAppointmentSlotsForNextTwoWeeks(
                        arrayListOf(defaultEmisMetaSlotLocations[0]),
                        defaultEmisMetaSlotSessionHolders,
                        modifiedSlotSessions,
                        defaultEmisAppointmentSessions
                )
            }
        }
    }

    @Step
    fun generateStubsForAppointmentSlotsForSpecificDates() {
        Serenity.setSessionVariable(EXPECTED_SESSIONS_KEY).to(defaultEmisMetaSlotSessions)
        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_SESSIONS_KEY).to(defaultEmisAppointmentSessions)
        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_SLOTS_KEY).to(defaultEmisAppointmentSlots)

        generateStubForMetaAppointmentSlotRequest(
                defaultEmisMetaSlotLocations,
                defaultEmisMetaSlotSessionHolders,
                defaultEmisMetaSlotSessions,
                0,
                defaultSessionStartDate,
                defaultSessionEndDate
        )

        generateStubForAppointmentSlotRequest(
                defaultEmisAppointmentSessions,
                0,
                defaultSessionStartDate,
                defaultSessionEndDate
        )
    }

    @Step
    fun generateEmisStubsForAppointmentSlotsForNextTwoWeeks(
            emisSlotLocations: ArrayList<Location> = defaultEmisMetaSlotLocations,
            emisSlotSessionHolders: ArrayList<SessionHolder> = defaultEmisMetaSlotSessionHolders,
            emisSlotSessions: ArrayList<Session> = defaultEmisMetaSlotSessions,
            emisAppointmentSessions: ArrayList<AppointmentSession> = defaultEmisAppointmentSessions,
            emisAppointmentSlots: ArrayList<AppointmentSlot> = defaultEmisAppointmentSlots,
            delayedInSeconds: Long = 0
    ) {
        Serenity.setSessionVariable(EXPECTED_SESSIONS_KEY).to(emisSlotSessions)
        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_SESSIONS_KEY).to(emisAppointmentSessions)
        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_SLOTS_KEY).to(emisAppointmentSlots)

        generateStubForMetaAppointmentSlotRequest(
                emisSlotLocations,
                emisSlotSessionHolders,
                emisSlotSessions,
                delayedInSeconds
        )

        generateStubForAppointmentSlotRequest(emisAppointmentSessions, delayedInSeconds)

        appointmentsConfirmationSteps.mockEmisSuccessResponse()
    }

    @Step
    fun generateStubForMetaAppointmentSlotRequest(
            emisSlotLocations: java.util.ArrayList<Location>,
            emisSlotSessionHolders: java.util.ArrayList<SessionHolder>,
            emisSlotSessions: java.util.ArrayList<Session>,
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

    @Step
    fun generateStubForAppointmentSlotRequest(
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

    @Step
    fun generateEmisStubsForAvailableSlotsGivingUnknownException() {
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

    @Step
    fun generateEmisStubsForAvailableSlotsWhenUnavailableToPatient() {
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

    @Step
    fun appointmentSlotsTimesOut() {
        val emisSlotLocations = defaultEmisMetaSlotLocations
        val emisSlotSessionHolders = defaultEmisMetaSlotSessionHolders
        val emisSlotSessions = defaultEmisMetaSlotSessions
        val emisAppointmentSessions = defaultEmisAppointmentSessions

        mockingClient.forEmis {
            appointmentSlotsMetaRequest(patient,
                    defaultSessionStartDate,
                    defaultSessionEndDate
            )
                    .withDelay(Duration.ofSeconds(31))
                    .respondWithSuccess(GetAppointmentSlotsMetaResponseModel(
                            emisSlotLocations,
                            emisSlotSessionHolders,
                            emisSlotSessions
                    ))
        }

        mockingClient.forEmis {
            appointmentSlotsRequest(patient,
                    defaultSessionStartDate,
                    defaultSessionEndDate
            )
                    .withDelay(Duration.ofSeconds(31))
                    .respondWithSuccess(GetAppointmentSlotsResponseModel(emisAppointmentSessions))
        }
    }

    @Step
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

    @Step
    fun verifyThatAvailableSlotsAreReturnedWithAppropriateFields() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedSlots = HashMap<String, SlotResponseObject>()
        for (i in 0 until result.slots.size)
            unmatchedExpectedSlots[defaultEmisAppointmentSlots[i].slotId.toString()] = SlotResponseObject(
                    defaultEmisAppointmentSlots[i].slotId.toString(),
                    getExpectedAppointmentTypeByIndexes(i, i),
                    defaultEmisAppointmentSlots[i].startTime!!,
                    defaultEmisAppointmentSlots[i].endTime!!,
                    defaultEmisMetaSlotLocations[i].locationName.toString(),
                    arrayOf(defaultEmisMetaSlotSessionHolders[i].displayName)
            )
        for (actualSlot in result.slots) {
            println(actualSlot.toString())
            assertNotNull("Null id", actualSlot.id)
            assertNotNull("Null type", actualSlot.type)
            assertNotNull("Null startTime", actualSlot.startTime)
            assertNotNull("Null endTime", actualSlot.endTime)
            assertNotNull("Null location", actualSlot.location)
            assertNotNull("Null clinicians", actualSlot.clinicians)
            val expectedSlot = unmatchedExpectedSlots[actualSlot.id]!!
            assertNotNull("Expected slot not found. ", expectedSlot)
            assertEquals("Slot type does not match. ", expectedSlot.type, actualSlot.type)
            assertEquals("Slot start time does not match. ", toUTC(expectedSlot.startTime), toUTC(actualSlot.startTime))
            assertEquals("Slot end time does not match. ", toUTC(expectedSlot.endTime), toUTC(actualSlot.endTime))
            assertEquals("Slot location does not match. ", expectedSlot.location, actualSlot.location)
            assertEquals("Slot clinicians do not match. ", expectedSlot.clinicians.toSet(), actualSlot.clinicians.toSet())
            unmatchedExpectedSlots.remove(actualSlot.id)
        }
        assertTrue("Expected Slots missing. ", unmatchedExpectedSlots.isEmpty())
    }

    @Step
    fun verifyThatAppointmentTypesFilterExistsAndIsCorrectlyPopulated() {
        val actualAppointmentTypeOptions = availableAppointments.getAppointmentTypeFilterContents()
        assertOptionExists(appointmentTypeDefaultOption, actualAppointmentTypeOptions, "default")

        val expectedSessions = sessionVariableCalled<ArrayList<Session>>(EXPECTED_SESSIONS_KEY)
        val expectedAppointmentSessions = sessionVariableCalled<ArrayList<AppointmentSession>>(EXPECTED_APPOINTMENT_SESSIONS_KEY)

        val sessionsById = generateMapOfSessionsAgainstId(expectedSessions)
        var uniqueAppointmentTypes = setOf<String>()

        for (i in 0 until expectedAppointmentSessions.size) {
            for (j in 0 until expectedAppointmentSessions[i].slots.size) {
                val session = sessionsById[expectedAppointmentSessions[i].sessionId]
                val appointmentSlotType = expectedAppointmentSessions[i].slots[j].slotTypeName
                val expectedOption = "${session!!.sessionName} - $appointmentSlotType"
                uniqueAppointmentTypes = uniqueAppointmentTypes.plus(expectedOption)
            }
        }
        for (expectedAppointmentType in uniqueAppointmentTypes) {
            assertOptionExists(
                    expectedAppointmentType,
                    actualAppointmentTypeOptions
            )
        }

        verifyThatNoAppointmentTypesIsSelected()
    }

    @Step
    fun verifyThatLocationsFilterExistsAndIsCorrectlyPopulated() {
        val actualLocationOptions = availableAppointments.getLocationFilterContents()
        assertOptionExists(locationDefaultOption, actualLocationOptions, "default")
        for (expectedLocationData in defaultEmisMetaSlotLocations) {
            val expectedLocation = expectedLocationData.locationName
            assertOptionExists(
                    expectedLocation,
                    actualLocationOptions
            )
        }

        assertEquals(
                "Incorrect location option currently selected. ",
                locationDefaultOption,
                availableAppointments.getSelectedLocation()
        )
    }

    @Step
    fun verifyThatCliniciansFilterExistsAndIsCorrectlyPopulated() {
        val actualClinicianOptions = availableAppointments.getClinicianFilterContents()
        assertOptionExists(clinicianDefaultOption, actualClinicianOptions, "default")
        for (expectedClinicianData in defaultEmisMetaSlotSessionHolders) {
            val expectedClinician = expectedClinicianData.displayName
            assertOptionExists(
                    expectedClinician!!,
                    actualClinicianOptions
            )
        }

        verifyThatNoSpecificClinicianIsSelected()
    }

    @Step
    fun verifyThatTimePeriodFilterExistsAndIsCorrectlyPopulated() {
        val actualTimePeriodOptions = availableAppointments.getTimePeriodFilterContents()
        for (expectedTimePeriod in timePeriodOptions) {
            assertOptionExists(
                    expectedTimePeriod.value,
                    actualTimePeriodOptions
            )
        }

        verifyThatTimePeriodIsSetAsTheDefault()
    }

    fun verifyThatTheFiltersAreNotDisplayed() {
        assertFalse("Appointment Type filter is displayed. ", availableAppointments.isTypeFilterPresent())
        assertFalse("Appointment Location filter is displayed. ", availableAppointments.isLocationsFilterPresent())
        assertFalse("Appointment Clinicians filter is displayed. ", availableAppointments.isCliniciansFilterPresent())
        assertFalse("Appointment Time Period filter is displayed. ", availableAppointments.isTimePeriodFilterPresent())
    }

    @Step
    fun verifyThatNoSlotsAreDisplayed() {
        assertFalse("Slots are displayed. ", availableAppointments.areAnySlotsPresent())
    }

    @Step
    fun verifyThatNoAppointmentTypesIsSelected() {
        assertEquals(
                "Incorrect appointment type option currently selected. ",
                appointmentTypeDefaultOption,
                availableAppointments.getSelectedAppointmentType()
        )
    }

    @Step
    fun verifyThatLocationIsSelected() {
        assertEquals(
                "Incorrect location option currently selected. ",
                defaultEmisMetaSlotLocations[0].locationName,
                availableAppointments.getSelectedLocation()
        )
    }

    @Step
    fun verifyThatNoSpecificClinicianIsSelected() {
        assertEquals(
                "Incorrect clinician option currently selected. ",
                clinicianDefaultOption,
                availableAppointments.getSelectedClinician()
        )
    }

    @Step
    fun verifyThatTimePeriodIsSetAsTheDefault() {
        assertEquals(
                "Incorrect time period option currently selected. ",
                timePeriodOptions[THIS_WEEK_KEY],
                availableAppointments.getSelectedTimePeriod()
        )
    }

    @Step
    fun verifyThatNoAppointmentsErrorIsDisplayed() {
        assertEquals("There are currently no appointments available to book online in the next two weeks. If you need to book one now, call your GP surgery.\n" +
                "If it's urgent and you don't know what to do, call 111 to get help near you.", availableAppointments.getWarningText())
    }

    @Step
    fun verifyThatNoAppointmentsForSelectedCriteriaErrorIsDisplayed() {
        assertEquals("Try selecting a different date and time, or without a preferred practice member selected. If you can't find the appointment you need, call your GP surgery.\n" +
                "If it's urgent and you don't know what to do, call 111 to get help near you.", availableAppointments.getWarningText())
    }

    @Step
    fun selectAnAppointmentType() {
        availableAppointments.selectAnAppointmentType()
    }

    @Step
    fun selectALocation() {
        availableAppointments.selectALocation()
    }

    @Step
    fun selectOptionsToRevealSlots() {
        selectFilterOptionsToRevealSlots()
        availableAppointments.selectTimePeriodByText(timePeriodOptions[ALL_KEY]!!)
    }

    @Step
    fun selectOptionsToRevealNoResults() {
        selectFilterOptionsToRevealSlots()
        availableAppointments.selectTimePeriodByText(timePeriodOptions[TODAY_KEY]!!)
    }

    @Step
    fun verifyThatValidationErrorsForMissingTypeAndLocationAreDisplayed() {
        assertEquals("There's a problem", availableAppointments.getErrorSummaryAtRow(1))
        assertEquals("- Choose a type of appointment", availableAppointments.getErrorSummaryAtRow(2))
        assertEquals("- Choose a location", availableAppointments.getErrorSummaryAtRow(3))
        assertEquals("- Select an appointment slot", availableAppointments.getErrorSummaryAtRow(4))
        assertEquals("Choose a type of appointment", availableAppointments.getInlineTypeValidationError())
        assertEquals("Choose a location", availableAppointments.getInlineLocationValidationError())
    }

    @Step
    fun verifyThatValidationErrorsForMissingTypeIsDisplayed() {
        assertEquals("There's a problem", availableAppointments.getErrorSummaryAtRow(1))
        assertEquals("- Choose a type of appointment", availableAppointments.getErrorSummaryAtRow(2))
        assertEquals("- Select an appointment slot", availableAppointments.getErrorSummaryAtRow(3))
        assertEquals("Choose a type of appointment", availableAppointments.getInlineTypeValidationError())
    }

    @Step
    fun verifyThatValidationErrorsForMissingLocationIsDisplayed() {
        assertEquals("There's a problem", availableAppointments.getErrorSummaryAtRow(1))
        assertEquals("- Choose a location", availableAppointments.getErrorSummaryAtRow(2))
        assertEquals("- Select an appointment slot", availableAppointments.getErrorSummaryAtRow(3))
        assertEquals("Choose a location", availableAppointments.getInlineLocationValidationError())
    }

    @Step
    fun verifyThatValidationErrorsForNoSelectedAppointmentIsDisplayed() {
        assertEquals("There's a problem", availableAppointments.getErrorSummaryAtRow(1))
        assertEquals("- Select an appointment slot", availableAppointments.getErrorSummaryAtRow(2))

        // TODO reinstate during NHSO-1772 (NHSO-1821)
//        assertEquals("Select an appointment slot", availableAppointments.getInlineSlotValidationError())
    }

    @Step
    fun verifyThatSlotNoLongerAvailableMessageIsDisplayed() {
        assertEquals("This slot is no longer available. Please select a different time.", availableAppointments.getWarningText())
    }

    @Step
    fun verifyThatAppropriateDateHeadingIsDisplayed() {
        val expectedAppointmentSlots = sessionVariableCalled<ArrayList<AppointmentSlot>>(EXPECTED_APPOINTMENT_SLOTS_KEY)
        var expectedDateHeadings = setOf<String>()
        for (expectedAppointmentSlot in expectedAppointmentSlots) {
            val expectedDateTime = backendDateTimeFormat.parse(expectedAppointmentSlot.startTime)
            expectedDateHeadings = expectedDateHeadings.plus(generateExpectedDateHeading(expectedDateTime))
        }
        for (expectedDateHeading in expectedDateHeadings) {
            assertTrue(
                    "Appointment Slot Date heading not found: $expectedDateHeading",
                    availableAppointments.isDateHeadingPresent(expectedDateHeading)
            )
        }
    }

    @Step
    fun verifyThatAppropriateTimeSlotIsDisplayed() {
        val expectedAppointmentSlots = sessionVariableCalled<ArrayList<AppointmentSlot>>(EXPECTED_APPOINTMENT_SLOTS_KEY)
        // expectedTimes as set of pairs of dates and times
        var expectedTimes = setOf<Pair<String, String>>()
        for (expectedAppointmentSlot in expectedAppointmentSlots) {
            val expectedDateTime = backendDateTimeFormat.parse(expectedAppointmentSlot.startTime)
            val expectedDateHeading = generateExpectedDateHeading(expectedDateTime)
            val expectedTimeOnSlot = generateExpectedTimeOnSlot(expectedDateTime)
            expectedTimes = expectedTimes.plus(Pair(expectedDateHeading, expectedTimeOnSlot))
        }
        for (expectedTime in expectedTimes) {
            assertTrue(
                    "Appointment Slot Time not found: ${expectedTime.second}",
                    availableAppointments.isTimeSlotPresent(expectedTime.first, expectedTime.second)
            )
        }
    }

    @Step
    fun clickOnASlot(slotNumber: Int = 1) {
        availableAppointments.selectSlotByPositionNumber(slotNumber)
    }

    @Step
    fun verifyThatDifferentSlotIsHighlighted() {
        assertTrue("New time slot is not highlighted. ", availableAppointments.isTimeSlotAtPositionSelected(2))
    }

    @Step
    fun verifyThatFirstSlotIsNotHighlighted() {
        assertFalse("Old time slot is still highlighted. ", availableAppointments.isTimeSlotAtPositionSelected(1))
    }

    @Step
    fun verifyThatSlotIsStillHighlighted() {
        assertTrue("Time slot is no longer highlighted. ", availableAppointments.isTimeSlotAtPositionSelected(1))
    }

    private fun getExpectedTimeOnSlot(slotNumber: Int): Pair<String, String> {
        val expectedAppointmentSlots = sessionVariableCalled<ArrayList<AppointmentSlot>>(EXPECTED_APPOINTMENT_SLOTS_KEY)
        val timeToSelect = backendDateTimeFormat.parse(expectedAppointmentSlots[slotNumber].startTime)
        val expectedDateHeading = generateExpectedDateHeading(timeToSelect)
        val expectedTimeOnSlot = generateExpectedTimeOnSlot(timeToSelect)
        return Pair(expectedDateHeading, expectedTimeOnSlot)
    }

    private fun getExpectedAppointmentTypeByIndexes(sessionIndex: Int, appointmentSlotIndex: Int): String {
        val sessionName = sessionVariableCalled<ArrayList<Session>>(EXPECTED_SESSIONS_KEY)[sessionIndex].sessionName
        val appointmentSlotType = sessionVariableCalled<ArrayList<AppointmentSlot>>(EXPECTED_APPOINTMENT_SLOTS_KEY)[appointmentSlotIndex].slotTypeName
        return "$sessionName - $appointmentSlotType"
    }

    private fun assertOptionExists(defaultOption: String, actualOptions: ArrayList<String>, optionType: String = "an") {
        assertTrue(
                String.format("%s not present as %s option", defaultOption, optionType),
                actualOptions.contains(defaultOption)
        )
    }

    private fun selectFilterOptionsToRevealSlots() {
        val expectedAppointmentSession = sessionVariableCalled<ArrayList<AppointmentSession>>(EXPECTED_APPOINTMENT_SESSIONS_KEY)[0]
        val expectedSessions = sessionVariableCalled<ArrayList<Session>>(EXPECTED_SESSIONS_KEY)
        val expectedSessionId = expectedAppointmentSession.sessionId
        val expectedSession = generateMapOfSessionsAgainstId(expectedSessions)[expectedSessionId]

        var locationName = ""
        for (location in defaultEmisMetaSlotLocations) {
            if (location.locationId == expectedSession!!.locationId) {
                locationName = location.locationName
                break
            }
        }

        val sessionName = expectedSession!!.sessionName
        val appointmentSlotType = expectedAppointmentSession.slots[0].slotTypeName
        val expectedOption = "$sessionName - $appointmentSlotType"

        availableAppointments.selectAppointmentTypeByText(expectedOption)
        availableAppointments.selectLocationByText(locationName)
        availableAppointments.selectClinicianByText(defaultEmisMetaSlotSessionHolders[0].displayName!!)
    }

    private fun generateMapOfSessionsAgainstId(expectedSessions: ArrayList<Session>): Map<Int, Session> {
        var sessionsById = mapOf<Int, Session>()
        for (session in expectedSessions) {
            sessionsById = sessionsById.plus(Pair(session.sessionId, session))
        }
        return sessionsById
    }

    private fun generateExpectedDateHeading(expectedDate: Date?): String {
        val expectedDateFormat = SimpleDateFormat(frontendDateFormat)
        return expectedDateFormat.format(expectedDate)
    }

    private fun generateExpectedTimeOnSlot(expectedTime: Date?): String {
        val expectedTimeFormat = SimpleDateFormat(frontendTimeFormat)
        return expectedTimeFormat.format(expectedTime)
    }

    private fun toLocalTime(date: String?): String {
        val currentDateFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        currentDateFormat.timeZone = TimeZone.getDefault()
        val dateToPass = currentDateFormat.parse(date, ParsePosition(0))
        val queryDateFormat = SimpleDateFormat(backendDateTimeFormatWithTimezone)
        return queryDateFormat.format(dateToPass)
    }

    private fun toUTC(date: String?): String {
        val currentDateFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        val dateToPass = currentDateFormat.parse(date, ParsePosition(0))
        val queryDateFormat = SimpleDateFormat(backendDateTimeFormatWithTimezone)
        queryDateFormat.timeZone = TimeZone.getTimeZone("UTC")
        return queryDateFormat.format(dateToPass).removeSuffix("00").plus(":00")
    }

    companion object {
        private const val EXPECTED_SESSIONS_KEY = "Expected Sessions"
        private const val EXPECTED_APPOINTMENT_SESSIONS_KEY = "Expected Appointment Sessions"
        private const val EXPECTED_APPOINTMENT_SLOTS_KEY = "Expected Appointment Slots"

        private const val TODAY_KEY = "Today"
        private const val TOMORROW_KEY = "Tomorrow"
        private const val THIS_WEEK_KEY = "This week"
        private const val NEXT_WEEK_KEY = "Next week"
        private const val ALL_KEY = "All"
    }
}
