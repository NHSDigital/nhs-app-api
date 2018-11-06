package features.appointments.steps

import mocking.data.appointments.AppointmentsBookingData
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import features.appointments.factories.AppointmentsFactory
import mocking.data.appointments.AppointmentSessionVariableKeys
import mockingFacade.appointments.AppointmentFilterFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import pages.ErrorPage
import pages.appointments.AvailableAppointmentsPage
import pages.navigation.HeaderNative
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import worker.models.appointments.SlotResponseObject
import javax.servlet.http.Cookie
import kotlin.collections.set

open class AvailableAppointmentsSteps : AppointmentsBookingData() {

    private val pageHeader = "Book new appointment"
    private val backButtonText = "Back to my appointments"
    private val appointmentTypeDefaultOption = "Select type"
    private val locationDefaultOption = "Select location"
    private val clinicianDefaultOption = "No preference"

    private lateinit var availableAppointments: AvailableAppointmentsPage
    private lateinit var errorPage: ErrorPage
    lateinit var headerNative: HeaderNative


    @Step
    fun checkIfPageHeaderIsCorrect() {
        headerNative.waitForPageHeaderText(pageHeader)
    }

    @Step
    fun assertTimeSlotPresent(expectedDateHeading: String, expectedTimeSlot: String) {
        availableAppointments.assertDateHeadingPresent(expectedDateHeading)
        availableAppointments.timeSlotForDateAndTime(expectedDateHeading, expectedTimeSlot).assertIsVisible()
    }

    @Step
    fun assertNumberOfSlotsPresent(size: Int) {
        assertEquals("Incorrect number of slots displayed", size, availableAppointments.getNumberOfTimeSlotsPresent())
    }

    @Step
    fun assertOnlyOneTimeSlotPresent(expectedDateHeading: String, expectedTimeSlot: String) {
        assertEquals(
                "Incorrect number of time-slots present",
                1,
                availableAppointments.timeSlotForDateAndTime(expectedDateHeading, expectedTimeSlot).elements.count()
        )
    }

    @Step
    fun assertThatOtherDatesAreNotDisplayed(expectedDates: Set<String>) {
        assertEquals("Incorrect number of dates displayed. ", expectedDates.size, availableAppointments.getNumberOfDateHeadingsPresent())
    }

    @Step
    fun assertThatRemainingDaysAreDisplayedWithAppropriateMessage(expectedDates: Set<String>, allDates: ArrayList<String>) {
        for (date in allDates) {
            if (!expectedDates.contains(date)) {
                assertEquals(
                        "Incorrect text found when expecting there to be no appointments for $date.",
                        "No appointments available",
                        availableAppointments.getNoSlotsAvailableTextAtDate(date)
                )
            }
        }
    }

    @Step
    fun waitForSpinnerToDisappearBecauseOfTimeout() {
        availableAppointments.waitForSpinnerToDisappear(70)
    }

    @Step
    fun clickOnBackButton() {
        availableAppointments.clickOnButtonContainingText(backButtonText)
    }

    @Step
    fun checkTimeoutErrorMessage() {
        val expectedHeader = "There's been a problem loading this page"
        val expectedMessageText = "Try again now. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111."
        errorPage.waitForSpinnerToDisappear(70)
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        errorPage.subHeading.assertElementNotPresent()
        assertEquals("expected error text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedMessageText, errorPage.errorText1.element.text)
    }

    @Step
    fun checkIfTryAgainButtonDisplayed() {
        errorPage.assertHasButton("Try again")
    }

    @Step
    fun checkIfTryAgainButtonIsNotDisplayed() {
        errorPage.assertNoButton("Try again")
    }

    @Step
    fun checkUnavailableErrorMessage() {
        val expectedHeader = "There's been a problem loading this page"
        val expectedBody = "Try again later. If the problem continues and you need to book an appointment now, contact your GP surgery directly. For urgent medical advice, call 111."
        errorPage.waitForSpinnerToDisappear()
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected error text $expectedBody but found ${errorPage.errorText1.element.text}",
                expectedBody, errorPage.errorText1.element.text)
    }

    @Step
    fun checkAppointmentsDisabledMessage() {
        val expectedHeader = "You are not currently able to book appointments online"
        val expectedBody = "Contact your GP surgery for more information. For urgent medical help, call 111."
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
                    .appointments.getAppointmentSlots(null,
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
                    .appointments.getAppointmentSlots(
                    startDate,
                    endDate,
                    Serenity.sessionVariableCalled<Cookie>(Cookie::class)
            )
            setSessionVariable(AppointmentSlotsResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            setSessionVariable("HttpException").to(httpException)
        }
    }

    @Step
    fun verifyThatAvailableSlotsAreReturnedWithAppropriateFields() {
        val result = sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedSlots = getExpectedResponseSlots()

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

    private fun errorMessageForNotFindingResultSlot(targetKey: String, actualKeys: MutableSet<String>): String {
        val keys = actualKeys.joinToString()
        return "Result Slot Id not Expected. Actual '$targetKey', Expected '$keys'"
    }

    private fun getExpectedResponseSlots(): HashMap<String, SlotResponseObject> {
        val expectedResponseSlots = sessionVariableCalled<ArrayList<SlotResponseObject>>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_RESPONSE_SLOTS_KEY)
        val unmatchedExpectedSlots = HashMap<String, SlotResponseObject>()
        expectedResponseSlots.forEach { slot -> unmatchedExpectedSlots[slot.id] = slot }
        return unmatchedExpectedSlots
    }

    private fun assertSlotsAreEqual(expectedSlot: SlotResponseObject, actualSlot: SlotResponseObject) {
        assertEquals("Slot type", expectedSlot.type, actualSlot.type)
        assertEquals("Slot start time", expectedSlot.startTime, actualSlot.startTime)
        assertEquals("Slot end time", expectedSlot.endTime, actualSlot.endTime)
        assertEquals("Slot location", expectedSlot.location, actualSlot.location)
        assertEquals("Slot clinicians", expectedSlot.clinicians.toSet(), actualSlot.clinicians.toSet())
    }

    private fun assertSlotIsNotNull(actualSlot: SlotResponseObject) {
        assertNotNull("Null id", actualSlot.id)
        assertNotNull("Null type", actualSlot.type)
        assertNotNull("Null startTime", actualSlot.startTime)
        assertNotNull("Null endTime", actualSlot.endTime)
        assertNotNull("Null location", actualSlot.location)
        assertNotNull("Null clinicians", actualSlot.clinicians)
    }

    @Step
    fun verifyThatAppointmentTypesFilterExistsAndIsCorrectlyPopulated() {
        val actualAppointmentTypeOptions = availableAppointments.appointmentTypeFilter.getFilterContents()
        assertOptionExists(appointmentTypeDefaultOption, actualAppointmentTypeOptions, "default")

        val expected = Serenity.sessionVariableCalled<ArrayList<String>>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_TYPE_KEY)

        expected.forEach { expectedAppointmentType -> assertOptionExists(expectedAppointmentType, actualAppointmentTypeOptions) }

        verifyThatNoAppointmentTypesIsSelected()
    }

    @Step
    fun verifyThatLocationsFilterExistsAndIsCorrectlyPopulated() {
        val actualLocationOptions = availableAppointments.locationFilter.getFilterContents()
        val expectedLocations =
                Serenity.sessionVariableCalled<ArrayList<String>>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_LOCATIONS_KEY)

        for (expectedLocation in expectedLocations) {
            assertOptionExists(expectedLocation, actualLocationOptions)
        }

        assertEquals(
                "Incorrect location option currently selected. ",
                locationDefaultOption,
                availableAppointments.locationFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatCliniciansFilterExistsAndIsCorrectlyPopulated() {
        val actualClinicianOptions = availableAppointments.clinicianFilter.getFilterContents()
        assertOptionExists(clinicianDefaultOption, actualClinicianOptions, "default")

        val expectedClinicians =
                sessionVariableCalled<ArrayList<String>>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_CLINICIANS_KEY)

        Assert.assertNotNull("Expected session variable 'EXPECTED_APPOINTMENT_CLINICIANS_KEY' to have value", expectedClinicians)

        for (expectedClinician in expectedClinicians) {
            assertOptionExists(expectedClinician, actualClinicianOptions)

        }
        verifyThatNoSpecificClinicianIsSelected()
    }

    @Step
    fun verifyThatTimePeriodFilterExistsAndIsCorrectlyPopulated() {
        val actualTimePeriodOptions = availableAppointments.timePeriodFilter.getFilterContents()
        assertOptionExists(TODAY_OPTION, actualTimePeriodOptions)
        assertOptionExists(TOMORROW_OPTION, actualTimePeriodOptions)
        assertOptionExists(THIS_WEEK_OPTION, actualTimePeriodOptions)
        assertOptionExists(NEXT_WEEK_OPTION, actualTimePeriodOptions)
        assertOptionExists(ALL_OPTION, actualTimePeriodOptions)
        verifyThatTimePeriodIsSetAsTheDefault()
    }

    fun verifyThatTheFiltersAreNotDisplayed() {
        availableAppointments.appointmentTypeFilter.assertNotPresent()
        availableAppointments.locationFilter.assertNotPresent()
        availableAppointments.clinicianFilter.assertNotPresent()
        availableAppointments.timePeriodFilter.assertNotPresent()
    }

    @Step
    fun verifyThatNoSlotsAreDisplayed() {
        assertFalse("Slots are displayed. ", availableAppointments.getAreAnySlotsPresent())
    }

    @Step
    fun verifyThatNoAppointmentTypesIsSelected() {
        assertEquals(
                "Incorrect appointment type option currently selected. ",
                appointmentTypeDefaultOption,
                availableAppointments.appointmentTypeFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatLocationIsSelected() {
        val locations = Serenity.sessionVariableCalled<ArrayList<String>>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_LOCATIONS_KEY)
        assertEquals("Test setup incorrect, expected only one location", 1, locations.count())
        assertEquals(
                "Incorrect location option currently selected. ",
                locations.first(),
                availableAppointments.locationFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatNoSpecificClinicianIsSelected() {
        assertEquals(
                "Incorrect clinicians option currently selected. ",
                clinicianDefaultOption,
                availableAppointments.clinicianFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatTimePeriodIsSetAsTheDefault() {
        assertEquals(
                "Incorrect time period option currently selected. ",
                THIS_WEEK_OPTION,
                availableAppointments.timePeriodFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatNoAppointmentsErrorIsDisplayed() {
        assertEquals("No appointments available\n" +
                "There are currently no appointments available to book online right now. If you need to book one now, call your GP surgery.\n" +
                "If it's urgent and you don't know what to do, call 111 to get help near you.",
                availableAppointments.warningMessage.assertSingleElementPresent().element.text)
    }

    @Step
    fun verifyThatNoAppointmentsForSelectedCriteriaErrorIsDisplayed() {
        assertEquals("Try selecting a different date and time, or without a preferred practice member selected. If you can't find the appointment you need, call your GP surgery.\n" +
                "If it's urgent and you don't know what to do, call 111 to get help near you.",
                availableAppointments.warningMessage.assertSingleElementPresent().element.text)
    }

    @Step
    fun selectOptionsToRevealSlots() {
        selectFilterOptionsToRevealSlots()
        availableAppointments.timePeriodFilter.selectByText(ALL_OPTION)
    }

    @Step
    fun selectOptionsToRevealNoResults() {
        selectFilterOptionsToRevealSlots()
        availableAppointments.timePeriodFilter.selectByText(TODAY_OPTION)
    }

    @Step
    fun selectTimePeriodOption(timePeriod: String) {
        availableAppointments.timePeriodFilter.selectByText(timePeriod)
    }

    @Step
    fun selectFilterOptionsToRevealSlots() {
        val filterValues = sessionVariableCalled<AppointmentFilterFacade>(AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotExpectations.EXPECTED_APPOINTMENT_FILTER_FACADE_KEY)
        if (!filterValues.type.isNullOrEmpty()) availableAppointments.appointmentTypeFilter.selectByText(filterValues.type!!)
        if (!filterValues.location.isNullOrEmpty()) availableAppointments.locationFilter.selectByText(filterValues.location!!)
        if (!filterValues.doctor.isNullOrEmpty()) availableAppointments.clinicianFilter.selectByText(filterValues.doctor!!)
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
    fun selectSlot(date: String, time: String) {
        availableAppointments.selectSlot(date, time)
    }

    @Step
    fun expandAppointmentSlotGuidance() {
        availableAppointments.guidance.expand.element.click()
    }

    @Step
    fun collapseAppointmentSlotGuidance() {
        availableAppointments.guidance.collapse.element.click()
    }

    @Step
    fun verifyGuidanceIsDisplayed() {
        availableAppointments.guidance.appointmentSlotGuidance.assertIsVisible()
    }

    @Step
    fun verifyTheLabelIsCorrect() {
        assertEquals("Appointment guidance help text is incorrect. ",
                "Which type of appointment do I need?",
                availableAppointments.guidance.label.element.text
        )
    }

    @Step
    fun verifyThatAppointmentGuidanceContentIsDisplayed() {
        val expectedGuidanceContent = sessionVariableCalled<String>(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)
        assertEquals("Guidance content not displayed correctly. ",
                expectedGuidanceContent,
                availableAppointments.guidance.content.element.text)
    }

    @Step
    fun verifyGuidanceContentIsNotDisplayed() {
        availableAppointments.guidance.content.assertElementNotPresent()
    }

    @Step
    fun verifyThatAppointmentGuidanceIsNotDisplayedAtAll() {
        availableAppointments.guidance.content.assertElementNotPresent()
    }

    private fun assertOptionExists(defaultOption: String, actualOptions: ArrayList<String>, optionType: String = "an") {
        assertTrue(
                String.format("%s not present as %s option", defaultOption, optionType),
                actualOptions.contains(defaultOption)
        )
    }

    companion object {
        private const val TODAY_OPTION = "Today"
        private const val TOMORROW_OPTION = "Tomorrow"
        private const val THIS_WEEK_OPTION = "This week"
        private const val NEXT_WEEK_OPTION = "Next week"
        private const val ALL_OPTION = "All available"
    }


}
