package features.appointments.steps

import constants.AppointmentDateTimeFormat
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import constants.AppointmentDateTimeFormat.Companion.frontendDateFormat
import constants.AppointmentDateTimeFormat.Companion.frontendTimeFormat
import features.appointments.data.AppointmentsBookingData
import features.appointments.stepDefinitions.factories.AppointmentsBookingFactory
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert.*
import pages.ErrorPage
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

    private val pageHeader = "Book new appointment"
    private val bookThisButtonText = "Continue"
    private val backButtonText = "Back to my appointments"
    private val appointmentTypeDefaultOption = "Select type"
    private val locationDefaultOption = "Select location"
    private val clinicianDefaultOption = "No preference"

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
    lateinit var myAppointments: MyAppointmentsSteps
    @Steps
    private lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    private lateinit var availableAppointments: AvailableAppointmentsPage
    private lateinit var errorPage: ErrorPage

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
        val expectation = if (presence) "should be displayed but got" else "shouldn't be displayed but still got"
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedFirstBodyLine = "Please try again"
        val expectedSecondBodyLine = "If the problem persists and you need to book an appointment now, contact your GP surgery directly."
        errorPage.waitForSpinnerToDisappear(11) // 1 second more than timeout
        assertEquals("\"$expectedHeader\" $expectation \"${errorPage.paragraph(1).element.text}\"", presence, errorPage.hasSubHeading(expectedHeader))
        assertEquals("\"$expectedHeader\" $expectation \"${errorPage.paragraph(2).element.text}\"", presence, errorPage.hasDetailParagraphOne(expectedFirstBodyLine))
        assertEquals("\"$expectedHeader\" $expectation \"${errorPage.paragraph(3).element.text}\"", presence, errorPage.hasDetailParagraphTwo(expectedSecondBodyLine))
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
        return errorPage.hasButton("Try again")
    }

    @Step
    fun checkUnavailableErrorMessage() {
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedBody = "Please try again later. If the problem persists and you need to book an appointment now, contact your GP surgery directly."
        errorPage.waitForSpinnerToDisappear()
        assertTrue("$expectedHeader\n$expectedBody", errorPage.hasSubHeading(expectedHeader))
        assertTrue("$expectedHeader\n$expectedBody", errorPage.hasDetailParagraphTwo(expectedBody))
    }

    @Step
    fun clickOnTryAgainButton() {
        errorPage.waitForSpinnerToDisappear(11) // 1 second more than timeout
        errorPage.clickOnButtonContainingText("Try again")
    }

    @Step
    fun generateDefaultUserData(gpSystem: String = "EMIS") {
        var patient = Patient.getDefault(gpSystem)
        Serenity.setSessionVariable(Patient::class).to(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                myAppointments.mockGPServiceMyAppointmentResponse("EMIS", true)
            }
        }
    }

    @Step
    fun generateAvailableAppointmentSlotsWithDifferentCriteriaForGPSystem(gpSystem: String) {
        when (gpSystem.toUpperCase()) {
            "TPP" -> {
                var tppAppointmentSessions = generateTppSessions(13)

                val numberOfSessionTypes = 2 // "Walk-in" and "Clinic"
                val numberOfsessionsPerDay = numberOfSessionTypes * defaultTppLocations.size * defaultTppClinicians.size
                for(i in 1..13) {
                  val slots = generateTppAppointmentSlots(i)
                    val sessionIndex = (i-1) * numberOfsessionsPerDay
                    for(dailySessionIndex in 0 until numberOfsessionsPerDay) {
                        tppAppointmentSessions[sessionIndex + dailySessionIndex].slots = slots
                    }
                }

                generateTppStubsForAppointmentSlotsForNextFourWeeks(tppAppointmentSessions)
            }
            "EMIS" -> {
                Serenity.setSessionVariable(EXPECTED_SESSIONS_KEY).to(generateEmisSessions(13))

                val numberOfSessionTypes = 2 // "Walk-in" and "Clinic"
                val numberOfSessionsPerDay = numberOfSessionTypes * defaultEmisMetaSlotLocations.size * (defaultEmisMetaSlotSessionHolders.size + 1)
                val allAppointmentSlots = arrayListOf<AppointmentSlotFacade>()
                val arrayOfArrayOfAppointmentSlots = arrayListOf<ArrayList<AppointmentSlotFacade>>()
                for (i in 1..13) {
                    repeat(numberOfSessionsPerDay) {
                        arrayOfArrayOfAppointmentSlots.add(generateEmisAppointmentSlots(i))
                        allAppointmentSlots.addAll(arrayOfArrayOfAppointmentSlots.last())
                    }
                }

                setSessionVariable(EXPECTED_APPOINTMENT_SLOTS_KEY).to(allAppointmentSlots)

                setSessionVariable(EXPECTED_APPOINTMENT_SESSIONS_KEY).to(
                        generateEmisAppointmentSessions(
                                Serenity.sessionVariableCalled<ArrayList<mocking.emis.models.Session>>(EXPECTED_SESSIONS_KEY),
                                arrayOfArrayOfAppointmentSlots
                        )
                )

                generateEmisStubsForAppointmentSlotsForNextFourWeeks(
                        defaultEmisMetaSlotLocations,
                        defaultEmisMetaSlotSessionHolders,
                        sessionVariableCalled<ArrayList<mocking.emis.models.Session>>(EXPECTED_SESSIONS_KEY),
                        sessionVariableCalled<ArrayList<AppointmentSessionFacade>>(EXPECTED_APPOINTMENT_SESSIONS_KEY),
                        sessionVariableCalled<ArrayList<AppointmentSlotFacade>>(EXPECTED_APPOINTMENT_SLOTS_KEY)
                )
            }
        }
    }

    @Step
    fun generateNoAvailableAppointmentSlotsForGPSystem(gpSystem: String) {
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                generateEmisStubsForAppointmentSlotsForNextFourWeeks(
                        arrayListOf(),
                        arrayListOf(),
                        arrayListOf(),
                        arrayListOf()
                )
            }
            "TPP" -> {
                generateTppStubsForAppointmentSlotsForNextFourWeeks(
                        arrayListOf()
                )
            }
        }
    }

    @Step
    fun generateAvailableOneAppointmentSlotForGPSystem(gpSystem: String) {
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                generateEmisStubsForAppointmentSlotsForNextFourWeeks(
                        arrayListOf(defaultEmisMetaSlotLocations[0]),
                        arrayListOf(defaultEmisMetaSlotSessionHolders[0]),
                        arrayListOf(defaultEmisMetaSlotSessions[0]),
                        arrayListOf(defaultEmisAppointmentSessions[0])
                )
                Serenity.setSessionVariable("Location").to(defaultEmisMetaSlotLocations[0].locationName)
            }
            "TPP" -> {
                generateTppStubsForAppointmentSlotsForNextFourWeeks(getDefaultTppAppointmentSessions())
                Serenity.setSessionVariable("Location").to(getDefaultTppAppointmentSessions()[0].location)
            }
        }
    }

    @Step
    fun generateAvailableAppointmentSlotsForGPSystemForOneLocation(gpSystem: String) {
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                val modifiedSlotSessions = ArrayList<mocking.emis.models.Session>(defaultEmisMetaSlotSessions)
                modifiedSlotSessions[1].locationId = modifiedSlotSessions[0].locationId
                generateEmisStubsForAppointmentSlotsForNextFourWeeks(
                        arrayListOf(defaultEmisMetaSlotLocations[0]),
                        defaultEmisMetaSlotSessionHolders,
                        modifiedSlotSessions,
                        defaultEmisAppointmentSessions
                )
                Serenity.setSessionVariable("Location").to(defaultEmisMetaSlotLocations[0].locationName)
            }
            "TPP" -> {
                val tppAppointmentSessions = getDefaultTppAppointmentSessions()
                generateTppStubsForAppointmentSlotsForNextFourWeeks(tppAppointmentSessions)
                Serenity.setSessionVariable("Location").to(tppAppointmentSessions[0].location)
            }
        }
    }

    @Step
    fun generateEmisStubsForAppointmentSlotsForSpecificDates() {
        setSessionVariable(EXPECTED_SESSIONS_KEY).to(defaultEmisMetaSlotSessions)
        setSessionVariable(EXPECTED_APPOINTMENT_SESSIONS_KEY).to(defaultEmisAppointmentSessions)
        setSessionVariable(EXPECTED_APPOINTMENT_SLOTS_KEY).to(getDefaultEmisAppointmentSlots())

        generateStubForMetaAppointmentSlotRequest(
                defaultEmisMetaSlotLocations,
                defaultEmisMetaSlotSessionHolders,
                defaultEmisMetaSlotSessions,
                0,
                defaultSessionStartDate,
                defaultSessionEndDate
        )

        generateEmisStubForAppointmentSlotRequest(
                defaultEmisAppointmentSessions,
                0,
                defaultSessionStartDate,
                defaultSessionEndDate
        )
    }

    @Step
    fun generateTppStubsForAppointmentSlotsForSpecificDates() {
        setSessionVariable(EXPECTED_APPOINTMENT_SESSIONS_KEY).to(getDefaultTppAppointmentSessions())

        generateTppStubForAppointmentSlotRequest(
                getDefaultTppAppointmentSessions(),
                0,
                defaultSessionStartDate,
                defaultSessionEndDate
        )
    }

    @Step
    fun generateEmisStubsForAppointmentSlotsForNextFourWeeks(
            emisSlotLocations: ArrayList<Location> = defaultEmisMetaSlotLocations,
            emisSlotSessionHolders: ArrayList<SessionHolder> = defaultEmisMetaSlotSessionHolders,
            emisSlotSessions: ArrayList<Session> = defaultEmisMetaSlotSessions,
            emisAppointmentSessions: ArrayList<AppointmentSessionFacade> = defaultEmisAppointmentSessions,
            emisAppointmentSlots: ArrayList<AppointmentSlotFacade> = getDefaultEmisAppointmentSlots(),
            delayedInSeconds: Long = 0
    ) {
        setSessionVariable(EXPECTED_SESSIONS_KEY).to(emisSlotSessions)
        setSessionVariable(EXPECTED_APPOINTMENT_SESSIONS_KEY).to(emisAppointmentSessions)
        setSessionVariable(EXPECTED_APPOINTMENT_SLOTS_KEY).to(emisAppointmentSlots)

        generateStubForMetaAppointmentSlotRequest(
                emisSlotLocations,
                emisSlotSessionHolders,
                emisSlotSessions,
                delayedInSeconds
        )

        generateEmisStubForAppointmentSlotRequest(emisAppointmentSessions, delayedInSeconds)

        appointmentsConfirmationSteps.mockEmisSuccessResponse()
    }

    @Step
    private fun generateTppStubsForAppointmentSlotsForNextFourWeeks(
            tppSessions: ArrayList<AppointmentSessionFacade>
    ) {
        Serenity.setSessionVariable(EXPECTED_APPOINTMENT_SESSIONS_KEY).to(tppSessions)

        generateTppStubForAppointmentSlotRequest(tppSessions)
    }


    @Step
    fun generateStubForMetaAppointmentSlotRequest(
            emisSlotLocations: java.util.ArrayList<Location>,
            emisSlotSessionHolders: java.util.ArrayList<SessionHolder>,
            emisSlotSessions: java.util.ArrayList<Session>,
            delayedInSeconds: Long = 0,
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
    fun generateEmisStubForAppointmentSlotRequest(
            emisAppointmentSessions: ArrayList<AppointmentSessionFacade>,
            delayedInSeconds: Long = 0,
            fromDate: String? = null,
            toDate: String? = null
    ) {
        val getAppointmentSlotsResponseModel = AppointmentSlotsResponseFacade(emisAppointmentSessions)
        mockingClient.forEmis {
            appointmentSlotsRequest(patient, fromDate, toDate)
                    .respondWithSuccess(getAppointmentSlotsResponseModel)
                    .delayedBy(Duration.ofSeconds(delayedInSeconds))
        }
    }

    @Step
    fun generateTppStubForAppointmentSlotRequest(
            tppAppointmentSessions: ArrayList<AppointmentSessionFacade>,
            delayedInSeconds: Long = 0,
            fromDate: String? = null,
            toDate: String? = null
    ) {
        val getAppointmentSlotsResponseModel = AppointmentSlotsResponseFacade(tppAppointmentSessions, "1")
        mockingClient.forTpp {
            appointmentSlotsRequest(tppPatient, fromDate, toDate)
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
                    .respondWithSuccess(AppointmentSlotsResponseFacade(emisAppointmentSessions))
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
        val result = sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val expectedSessions = sessionVariableCalled<ArrayList<AppointmentSessionFacade>>(EXPECTED_APPOINTMENT_SESSIONS_KEY)
        val unmatchedExpectedSlots = HashMap<AppointmentSlotFacade, SlotResponseObject>()
        for (session in expectedSessions) {
            for (slot in session.slots) {
                unmatchedExpectedSlots[slot] = SlotResponseObject(
                        slot.slotId.toString(),
                        getExpectedAppointmentTypeByIndexes(slot.sessionTypeName!!, slot.slotTypeName!!),
                        slot.startTime!!,
                        slot.endTime!!,
                        session.location!!,
                        arrayOf(session.staffDetails)
                )
            }
        }
        for (actualSlot in result.slots) {
            assertNotNull("Null id", actualSlot.id)
            assertNotNull("Null type", actualSlot.type)
            assertNotNull("Null startTime", actualSlot.startTime)
            assertNotNull("Null endTime", actualSlot.endTime)
            assertNotNull("Null location", actualSlot.location)
            assertNotNull("Null clinicians", actualSlot.clinicians)
            val actualSlotTypeParts = actualSlot.type.split(Regex("( - )"))
            val startTimeWithoutTimezone = timeWithoutTimezone(actualSlot.startTime)
            val endTimeWithoutTimezone = timeWithoutTimezone(actualSlot.endTime)
            val expectedSlotKey = AppointmentSlotFacade(
                    actualSlot.id.toInt(),
                    startTimeWithoutTimezone,
                    endTimeWithoutTimezone,
                    actualSlotTypeParts[0],
                    actualSlotTypeParts[1]
            )
            assertTrue("Expected slot not found:\n$expectedSlotKey should be in\n$unmatchedExpectedSlots", unmatchedExpectedSlots.containsKey(expectedSlotKey))
            val expectedSlot = unmatchedExpectedSlots[expectedSlotKey]!!
            assertEquals("Slot type does not match. ", expectedSlot.type, actualSlot.type)
            assertEquals("Slot start time does not match. ", toUTC(expectedSlot.startTime), toUTC(actualSlot.startTime))
            assertEquals("Slot end time does not match. ", toUTC(expectedSlot.endTime), toUTC(actualSlot.endTime))
            assertEquals("Slot location does not match. ", expectedSlot.location, actualSlot.location)
            assertEquals("Slot clinicians do not match. ", expectedSlot.clinicians.toSet(), actualSlot.clinicians.toSet())
            unmatchedExpectedSlots.remove(expectedSlotKey)
        }
        assertTrue("Expected Slots missing. ", unmatchedExpectedSlots.isEmpty())
    }

    @Step
    fun verifyThatAppointmentTypesFilterExistsAndIsCorrectlyPopulated() {
        val actualAppointmentTypeOptions = availableAppointments.getAppointmentTypeFilterContents()
        assertOptionExists(appointmentTypeDefaultOption, actualAppointmentTypeOptions, "default")

        var expected =
                Serenity.sessionVariableCalled<ArrayList<String>>(AppointmentsBookingFactory.ExpectedAppointmentTypesKey)
                        ?: getExpectedAppointmentTypesFromSession()
        for (expectedAppointmentType in expected) {
            assertOptionExists(expectedAppointmentType, actualAppointmentTypeOptions)
        }

        verifyThatNoAppointmentTypesIsSelected()
    }

    private fun getExpectedAppointmentTypesFromSession(): ArrayList<String> {
        val expectedAppointmentSessions = sessionVariableCalled<ArrayList<AppointmentSessionFacade>>(EXPECTED_APPOINTMENT_SESSIONS_KEY)

        val sessionsById = generateMapOfSessionsAgainstId(expectedAppointmentSessions)
        var uniqueAppointmentTypes = setOf<String>()

        for (i in 0 until expectedAppointmentSessions.size) {
            for (j in 0 until expectedAppointmentSessions[i].slots.size) {
                val session = sessionsById[expectedAppointmentSessions[i].sessionId]
                val appointmentSlotType = expectedAppointmentSessions[i].slots[j].slotTypeName
                val expectedOption = "${session!!.sessionType} - $appointmentSlotType"
                uniqueAppointmentTypes = uniqueAppointmentTypes.plus(expectedOption)
            }
        }
        var list = arrayListOf<String>()
        uniqueAppointmentTypes.forEach{appointment->list.add(appointment)}
        return list
    }

    @Step
    fun verifyThatLocationsFilterExistsAndIsCorrectlyPopulated() {
        val actualLocationOptions = availableAppointments.getLocationFilterContents()
        var expectedLocations =
                Serenity.sessionVariableCalled<ArrayList<String>>(AppointmentsBookingFactory.ExpectedAppointmentLocationsKey)
                        ?: defaultEmisMetaSlotLocations.map { l -> l.locationName }

        for (expectedLocation in expectedLocations) {
            assertOptionExists(expectedLocation, actualLocationOptions)
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

        var expectedClinicians =
                Serenity.sessionVariableCalled<ArrayList<String>>(AppointmentsBookingFactory.ExpectedAppointmentCliniciansKey)
                        ?: defaultEmisMetaSlotSessionHolders.map { l -> l.displayName }

        for (expectedClinician in expectedClinicians) {
            assertOptionExists(expectedClinician!!, actualClinicianOptions)

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
        val location = Serenity.sessionVariableCalled<String>("Location")
        assertEquals(
                "Incorrect location option currently selected. ",
                location,
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
        val expectedAppointmentSlots = sessionVariableCalled<ArrayList<AppointmentSlotFacade>>(EXPECTED_APPOINTMENT_SLOTS_KEY)
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
        val expectedAppointmentSlots = sessionVariableCalled<ArrayList<AppointmentSlotFacade>>(EXPECTED_APPOINTMENT_SLOTS_KEY)
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
        val expectedAppointmentSlots = sessionVariableCalled<ArrayList<AppointmentSlotFacade>>(EXPECTED_APPOINTMENT_SLOTS_KEY)
        val timeToSelect = backendDateTimeFormat.parse(expectedAppointmentSlots[slotNumber].startTime)
        val expectedDateHeading = generateExpectedDateHeading(timeToSelect)
        val expectedTimeOnSlot = generateExpectedTimeOnSlot(timeToSelect)
        return Pair(expectedDateHeading, expectedTimeOnSlot)
    }

    private fun getExpectedAppointmentTypeByIndexes(sessionName: String, appointmentSlotType: String): String {
        return "$sessionName - $appointmentSlotType"
    }

    private fun assertOptionExists(defaultOption: String, actualOptions: ArrayList<String>, optionType: String = "an") {
        assertTrue(
                String.format("%s not present as %s option", defaultOption, optionType),
                actualOptions.contains(defaultOption)
        )
    }

    private fun selectFilterOptionsToRevealSlots() {
        val filterValues = sessionVariableCalled<AppointmentFilterFacade>(AppointmentsBookingFactory.ExpectedAppointmentFilterFacadeKey)
                ?: getFilterValuesFromSession()
        availableAppointments.selectAppointmentTypeByText(filterValues.type)
        availableAppointments.selectLocationByText(filterValues.location)
        availableAppointments.selectClinicianByText(filterValues.doctor ?: "")
    }

    private fun getFilterValuesFromSession(): AppointmentFilterFacade {
        val expectedAppointmentSessions = sessionVariableCalled<ArrayList<AppointmentSessionFacade>>(EXPECTED_APPOINTMENT_SESSIONS_KEY)
        val expectedAppointmentSession = expectedAppointmentSessions[0]
        val expectedSessionId = expectedAppointmentSession.sessionId
        val expectedSession = generateMapOfSessionsAgainstId(expectedAppointmentSessions)[expectedSessionId]

        var locationName = ""
        for (location in defaultEmisMetaSlotLocations) {
            if (location.locationName == expectedSession!!.location) {
                locationName = location.locationName
                break
            }
        }

        val sessionType = expectedSession!!.sessionType
        val appointmentSlotType = expectedAppointmentSession.slots[0].slotTypeName
        val expectedOption = "$sessionType - $appointmentSlotType"
        return AppointmentFilterFacade(expectedOption, locationName,
                defaultEmisMetaSlotSessionHolders[0].displayName!!)

    }

    private fun generateMapOfSessionsAgainstId(expectedSessions: ArrayList<AppointmentSessionFacade>): Map<Int?, AppointmentSessionFacade> {
        var sessionsById = mapOf<Int?, AppointmentSessionFacade>()
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
        val queryDateFormat = SimpleDateFormat(AppointmentDateTimeFormat.backendDateTimeFormat)
        return queryDateFormat.format(dateToPass)
    }

    private fun toUTC(date: String?): String {
        val currentDateFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        val dateToPass = currentDateFormat.parse(date, ParsePosition(0))
        val queryDateFormat = SimpleDateFormat(AppointmentDateTimeFormat.backendDateTimeFormat)
        queryDateFormat.timeZone = TimeZone.getTimeZone("UTC")
        return queryDateFormat.format(dateToPass).removeSuffix("00").plus(":00")
    }

    private fun timeWithoutTimezone(date: String): String? {
        return date.split(Regex("\\+"))[0]
    }

    companion object {
        const val EXPECTED_SESSIONS_KEY = "Expected Sessions"
        const val EXPECTED_APPOINTMENT_SESSIONS_KEY = "Expected Appointment Sessions"
        const val EXPECTED_APPOINTMENT_SLOTS_KEY = "Expected Appointment Slots"

        private const val TODAY_KEY = "Today"
        private const val TOMORROW_KEY = "Tomorrow"
        private const val THIS_WEEK_KEY = "This week"
        private const val NEXT_WEEK_KEY = "Next week"
        private const val ALL_KEY = "All"
    }
}
