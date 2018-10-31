package features.appointments.steps

import features.appointments.factories.AppointmentsFactory
import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.data.appointments.AppointmentsBookingData
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotNull
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

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage
    lateinit var headerNative: HeaderNative


    @Step
    fun checkIfPageHeaderIsCorrect() {
        headerNative.waitForPageHeaderText(pageHeader)
    }

    @Step
    fun assertTimeSlotPresent(expectedDateHeading: String, expectedTimeSlot: String) {
        availableAppointmentsPage.assertDateHeadingPresent(expectedDateHeading)
        availableAppointmentsPage.timeSlotForDateAndTime(expectedDateHeading, expectedTimeSlot).assertIsVisible()
    }

    @Step
    fun assertNumberOfSlotsPresent(size: Int) {
        assertEquals("Incorrect number of slots displayed",
                size,
                availableAppointmentsPage.getNumberOfTimeSlotsPresent())
    }

    @Step
    fun assertOnlyOneTimeSlotPresent(expectedDateHeading: String, expectedTimeSlot: String) {
        assertEquals(
                "Incorrect number of time-slots present",
                1,
                availableAppointmentsPage.timeSlotForDateAndTime(expectedDateHeading, expectedTimeSlot).elements.count()
        )
    }

    @Step
    fun assertThatOtherDatesAreNotDisplayed(expectedDates: Set<String>) {
        assertEquals("Incorrect number of dates displayed. ",
                expectedDates.size, availableAppointmentsPage.getNumberOfDateHeadingsPresent())
    }

    @Step
    fun assertThatRemainingDaysAreDisplayedWithAppropriateMessage(expectedDates: Set<String>, allDates: ArrayList<String>) {
        for (date in allDates) {
            if (!expectedDates.contains(date)) {
                assertEquals(
                        "Incorrect text found when expecting there to be no appointments for $date.",
                        "No appointments available",
                        availableAppointmentsPage.getNoSlotsAvailableTextAtDate(date)
                )
            }
        }
    }

    @Step
    fun clickOnBackButton() {
        availableAppointmentsPage.clickOnButtonContainingText(backButtonText)
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
    fun verifyThatNoSlotsAreDisplayed() {
        assertFalse("Slots are displayed. ", availableAppointmentsPage.getAreAnySlotsPresent())
    }

    @Step
    fun verifyThatNoAppointmentsErrorIsDisplayed() {
        assertEquals("No appointments available\n" +
                "There are currently no appointments available to book online right now. If you need to book one now, call your GP surgery.\n" +
                "If it's urgent and you don't know what to do, call 111 to get help near you.",
                availableAppointmentsPage.warningMessage.assertSingleElementPresent().element.text)
    }

    @Step
    fun verifyThatNoAppointmentsForSelectedCriteriaErrorIsDisplayed() {
        assertEquals("Try selecting a different date and time, or without a preferred practice member selected. If you can't find the appointment you need, call your GP surgery.\n" +
                "If it's urgent and you don't know what to do, call 111 to get help near you.",
                availableAppointmentsPage.warningMessage.assertSingleElementPresent().element.text)
    }

    @Step
    fun verifyThatAppointmentGuidanceContentIsDisplayed() {
        val expectedGuidanceContent = sessionVariableCalled<String>(AppointmentSessionVariableKeys.EXPECTED_GUIDANCE_CONTENT_KEY)
        assertEquals("Guidance content not displayed correctly. ",
                expectedGuidanceContent,
                availableAppointmentsPage.guidance.content.element.text)
    }


}
