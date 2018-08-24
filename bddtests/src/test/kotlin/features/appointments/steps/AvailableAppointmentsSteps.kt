package features.appointments.steps

import constants.AppointmentDateTimeFormat
import constants.AppointmentDateTimeFormat.Companion.backendDateTimeFormatWithoutTimezone
import features.appointments.data.AppointmentsBookingData
import features.appointments.data.AppointmentsSlotsExampleBuilder
import features.appointments.data.AppointmentsSlotsExampleBuilder.Companion.EXPECTED_APPOINTMENT_LOCATIONS_KEY
import features.appointments.factories.AppointmentsFactory
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert
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
                Pair(TimePeriodOptionKeys.TODAY_KEY, TODAY_OPTION),
                Pair(TimePeriodOptionKeys.TOMORROW_KEY, TOMORROW_OPTION),
                Pair(TimePeriodOptionKeys.THIS_WEEK_KEY, THIS_WEEK_OPTION),
                Pair(TimePeriodOptionKeys.NEXT_WEEK_KEY, NEXT_WEEK_OPTION),
                Pair(TimePeriodOptionKeys.ALL_KEY, ALL_OPTION)
        )
    }

    private lateinit var availableAppointments: AvailableAppointmentsPage
    private lateinit var errorPage: ErrorPage


    @Step
    fun checkIfPageHeaderIsCorrect() {
        val actualHeader = availableAppointments.getPageHeaderText()
        assertEquals("Expected Header text $pageHeader of the page is not found",
                pageHeader, actualHeader)
    }

    @Step
    fun assertTimeSlotPresent(expectedDateHeading:String, expectedTimeSlot:String) {
        Assert.assertTrue("Appointment Slot Date heading not found: $expectedDateHeading",
                availableAppointments.isDateHeadingPresent(expectedDateHeading))
        Assert.assertTrue("Appointment Time Slot not found: $expectedTimeSlot for date: $expectedDateHeading",
                availableAppointments.isTimeSlotPresent(expectedDateHeading, expectedTimeSlot)
        )
    }

    @Step
    fun waitForSpinnerToDisappearBecauseOfTimeout() {
        availableAppointments.waitForSpinnerToDisappear(70)
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
    fun checkTimeoutErrorMessage() {
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedSubHeader = "Please try again"
        val expectedMessageText = "If the problem persists and you need to book an appointment now, contact your GP surgery directly."
        errorPage.waitForSpinnerToDisappear(70)
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected  sub-header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedSubHeader, errorPage.subHeading.element.text)
        assertEquals("expected error text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedMessageText, errorPage.errorText1.element.text)
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
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected error text $expectedBody but found ${errorPage.errorText1.element.text}",
                expectedBody, errorPage.errorText1.element.text)
    }

    @Step
    fun clickOnTryAgainButton() {
        errorPage.waitForSpinnerToDisappear(11) // 1 second more than timeout
        errorPage.clickOnButtonContainingText("Try again")
    }

    @Step
    fun theAvailableAppointmentSlotsAreRetrieved() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .getAppointmentSlots(null,
                            null,
                            Serenity.sessionVariableCalled<Cookie>(Cookie::class))
            Serenity.setSessionVariable(AppointmentSlotsResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }
    }

    @Step
    fun theAvailableAppointmentSlotsAreRetrievedForExplicitDateTimeRange() {
        try {

            val startDate = Serenity.sessionVariableCalled<String>(AppointmentsFactory.AppointmentStartTimeKey)
            val endDate = Serenity.sessionVariableCalled<String>(AppointmentsFactory.AppointmentEndTimeKey)
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .getAppointmentSlots(startDate,
                            endDate,
                            Serenity.sessionVariableCalled<Cookie>(Cookie::class))
            setSessionVariable(AppointmentSlotsResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            setSessionVariable("HttpException").to(httpException)
        }
    }

    @Step
    fun verifyThatAvailableSlotsAreReturnedWithAppropriateFields() {
        val result = sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedSlots =getExpectedResponseSlots()

        Assert.assertEquals("Number of response slots", unmatchedExpectedSlots.count(), result.slots.count())

        for (resultSlot in result.slots) {
            assertSlotIsNotNull(resultSlot)
            val key = resultSlot.id
            val keys = unmatchedExpectedSlots.keys
            Assert.assertTrue(errorMessageForNotFindingResultSlot(key, keys),
                    unmatchedExpectedSlots.containsKey(key))
            val matchingExpectedSlot = unmatchedExpectedSlots[key]!!
            assertSlotsAreEqual(matchingExpectedSlot, actualSlot = resultSlot)
            unmatchedExpectedSlots.remove(key)
        }
    }

    private fun errorMessageForNotFindingResultSlot(targetKey:String, actualKeys: MutableSet<String>):String{
        val keys = actualKeys.joinToString(", ")
        return "Result Slot Id not Expected. Actual '$targetKey', Expected '$keys'"
    }

    private fun getExpectedResponseSlots(): HashMap<String, SlotResponseObject> {
        val expectedResponseSlots = sessionVariableCalled<ArrayList<SlotResponseObject>>(AppointmentsSlotsExampleBuilder.EXPECTED_RESPONSE_SLOTS_KEY)
        val unmatchedExpectedSlots = HashMap<String, SlotResponseObject>()
        expectedResponseSlots.forEach { slot -> unmatchedExpectedSlots[slot.id] = slot }
        return unmatchedExpectedSlots
    }

    private fun assertSlotsAreEqual(expectedSlot : SlotResponseObject, actualSlot: SlotResponseObject) {
        assertEquals("Slot type", expectedSlot.type, actualSlot.type)
        assertEquals("Slot start time", toUTC(expectedSlot.startTime), toUTC(actualSlot.startTime))
        assertEquals("Slot end time", toUTC(expectedSlot.endTime), toUTC(actualSlot.endTime))
        assertEquals("Slot location", expectedSlot.location, actualSlot.location)
        assertEquals("Slot clinicians", expectedSlot.clinicians.toSet(), actualSlot.clinicians.toSet())
    }

    private fun assertSlotIsNotNull(actualSlot: SlotResponseObject) {
        assertNotNull("Null id", actualSlot.id)
        assertNotNull("Null type", actualSlot.type)
        assertNotNull("Null startTime", actualSlot.startTime)
        assertNotNull("Null endTime", actualSlot.endTime)
        assertNotNull("Null location", actualSlot.location)
        assertNotNull("Null clinicians", actualSlot.clinicians)}

    @Step
    fun verifyThatAppointmentTypesFilterExistsAndIsCorrectlyPopulated() {
        val actualAppointmentTypeOptions = availableAppointments.getAppointmentTypeFilterContents()
        assertOptionExists(appointmentTypeDefaultOption, actualAppointmentTypeOptions, "default")

        val expected = Serenity.sessionVariableCalled<ArrayList<String>>(AppointmentsSlotsExampleBuilder.EXPECTED_APPOINTMENT_TYPE_KEY)

        expected.forEach { expectedAppointmentType -> assertOptionExists(expectedAppointmentType, actualAppointmentTypeOptions) }

        verifyThatNoAppointmentTypesIsSelected()
    }

    @Step
    fun verifyThatLocationsFilterExistsAndIsCorrectlyPopulated() {
        val actualLocationOptions = availableAppointments.getLocationFilterContents()
        val expectedLocations =
                Serenity.sessionVariableCalled<ArrayList<String>>(AppointmentsSlotsExampleBuilder.EXPECTED_APPOINTMENT_LOCATIONS_KEY)

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

        val expectedClinicians =
                sessionVariableCalled<ArrayList<String>>(AppointmentsSlotsExampleBuilder.EXPECTED_APPOINTMENT_CLINICIANS_KEY)

        Assert.assertNotNull("Expected session variable 'EXPECTED_APPOINTMENT_CLINICIANS_KEY' to have value", expectedClinicians)

        for (expectedClinician in expectedClinicians) {
            assertOptionExists(expectedClinician, actualClinicianOptions)

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
        val locations = Serenity.sessionVariableCalled<ArrayList<String>>(EXPECTED_APPOINTMENT_LOCATIONS_KEY)
        assertEquals("Test setup incorrect, expected only one location",1, locations.count())
        assertEquals(
                "Incorrect location option currently selected. ",
                locations.first(),
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
                timePeriodOptions[TimePeriodOptionKeys.THIS_WEEK_KEY],
                availableAppointments.getSelectedTimePeriod()
        )
    }

    @Step
    fun verifyThatNoAppointmentsErrorIsDisplayed() {
        assertEquals("No appointments available\n" +
                "There are currently no appointments available to book online right now. If you need to book one now, call your GP surgery.\n" +
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
        availableAppointments.selectTimePeriodByText(timePeriodOptions[TimePeriodOptionKeys.ALL_KEY]!!)
    }

    @Step
    fun selectOptionsToRevealNoResults() {
        selectFilterOptionsToRevealSlots()
        availableAppointments.selectTimePeriodByText(timePeriodOptions[TimePeriodOptionKeys.TODAY_KEY]!!)
    }

    @Step
    fun verifyThatValidationErrorsForMissingTypeAndLocationAreDisplayed() {
        assertEquals("There's a problem", availableAppointments.getErrorSummarySubHeading())
        assertEquals("Choose a type of appointment", availableAppointments.getErrorSummaryBodyAtRow(1))
        assertEquals("Choose a location", availableAppointments.getErrorSummaryBodyAtRow(2))
        assertEquals("Select an appointment slot", availableAppointments.getErrorSummaryBodyAtRow(3))
        assertEquals("Choose a type of appointment", availableAppointments.getInlineTypeValidationError())
        assertEquals("Choose a location", availableAppointments.getInlineLocationValidationError())
    }

    @Step
    fun verifyThatValidationErrorsForMissingTypeIsDisplayed() {
        assertEquals("There's a problem", availableAppointments.getErrorSummarySubHeading())
        assertEquals("Choose a type of appointment", availableAppointments.getErrorSummaryBodyAtRow(1))
        assertEquals("Select an appointment slot", availableAppointments.getErrorSummaryBodyAtRow(2))
        assertEquals("Choose a type of appointment", availableAppointments.getInlineTypeValidationError())
    }

    @Step
    fun verifyThatValidationErrorsForMissingLocationIsDisplayed() {
        assertEquals("There's a problem", availableAppointments.getErrorSummarySubHeading())
        assertEquals("Choose a location", availableAppointments.getErrorSummaryBodyAtRow(1))
        assertEquals("Select an appointment slot", availableAppointments.getErrorSummaryBodyAtRow(2))
        assertEquals("Choose a location", availableAppointments.getInlineLocationValidationError())
    }

    @Step
    fun verifyThatValidationErrorsForNoSelectedAppointmentIsDisplayed() {
        assertEquals("There's a problem", availableAppointments.getErrorSummarySubHeading())
        assertEquals("Select an appointment slot", availableAppointments.getErrorSummaryBodyAtRow(1))
        assertEquals("Select an appointment slot", availableAppointments.getInlineSlotValidationError())
    }

    @Step
    fun verifyThatSlotNoLongerAvailableMessageIsDisplayed() {
        val expectedHeader = "This slot is no longer available"
        val expectedMsg = "Please select a different time."

        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected error text $expectedMsg but found ${errorPage.errorText1.element.text}",
                expectedMsg, errorPage.errorText1.element.text)
    }

    @Step
    fun clickOnASlot(slotNumber: Int = 1) {
        availableAppointments.selectSlotByPositionNumber(slotNumber)
    }

    @Step
    fun selectSlot(date: String, time: String) {
        return availableAppointments.selectSlot(date, time)
    }

    @Step
    fun expandAppointmentSlotGuidance() {
        availableAppointments.expandGuidance()
    }

    @Step
    fun collapseAppointmentSlotGuidance() {
        availableAppointments.collapseGuidance()
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

    @Step
    fun verifyGuidanceIsDisplayed() {
        assertTrue("Appointment guidance isn't present. ", availableAppointments.isGuidancePresent())
    }

    @Step
    fun verifyTheLabelIsCorrect() {
        assertEquals("Appointment guidance help text is incorrect. ",
                "Which type of appointment do I need?",
                availableAppointments.getGuidanceLabelText()
        )
    }

    fun verifyThatAppointmentGuidanceContentIsDisplayed() {
        val expectedGuidanceContent = sessionVariableCalled<String>(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)
        assertEquals("Guidance content not displayed correctly. ", expectedGuidanceContent, availableAppointments.getGuidanceContent())
    }

    fun verifyGuidanceContentIsNotDisplayed() {
        assertFalse("Appointment slot guidance is displayed when it shouldn't be. ", availableAppointments.isGuidanceContentVisible())
    }

    @Step
    fun verifyThatAppointmentGuidanceIsNotDisplayedAtAll() {
        assertFalse("Appointment guidance is present when it shouldn't be. ", availableAppointments.isGuidancePresent())
    }

    private fun assertOptionExists(defaultOption: String, actualOptions: ArrayList<String>, optionType: String = "an") {
        assertTrue(
                String.format("%s not present as %s option", defaultOption, optionType),
                actualOptions.contains(defaultOption)
        )
    }

    private fun selectFilterOptionsToRevealSlots() {
        val filterValues = sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilder.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY)
        availableAppointments.selectAppointmentTypeByText(filterValues.type)
        availableAppointments.selectLocationByText(filterValues.location)
        availableAppointments.selectClinicianByText(filterValues.doctor ?: "")
    }

    private fun toUTC(date: String?): String {
        val currentDateFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        val dateToPass = currentDateFormat.parse(date, ParsePosition(0))
        val queryDateFormat = SimpleDateFormat(AppointmentDateTimeFormat.backendDateTimeFormat)
        queryDateFormat.timeZone = TimeZone.getTimeZone("UTC")
        return queryDateFormat.format(dateToPass).removeSuffix("00").plus(":00")
    }

    companion object {
        private const val TODAY_OPTION = "Today"
        private const val TOMORROW_OPTION = "Tomorrow"
        private const val THIS_WEEK_OPTION = "This week"
        private const val NEXT_WEEK_OPTION = "Next week"
        private const val ALL_OPTION = "All available"
    }

    enum class AppointmentSessionVariableKeys {
        EXPECTED_SESSIONS_KEY,
        EXPECTED_APPOINTMENT_SESSIONS_KEY,
        EXPECTED_APPOINTMENT_SLOTS_KEY,
        EXPECTED_GUIDANCE_CONTENT_KEY
    }

    enum class TimePeriodOptionKeys {
        TODAY_KEY,
        TOMORROW_KEY,
        THIS_WEEK_KEY,
        NEXT_WEEK_KEY,
        ALL_KEY
    }
}
