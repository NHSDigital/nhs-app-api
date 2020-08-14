package features.im1Appointments.steps

import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.stubs.appointments.factories.AppointmentsSlotsFactory
import mockingFacade.appointments.AppointmentSessionFacade
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertNotNull
import pages.appointments.AvailableAppointmentsPage
import pages.navigation.BreadcrumbHeader
import pages.navigation.WebHeader
import utils.LinkedProfilesSerenityHelpers
import utils.getOrNull
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import worker.models.appointments.SlotResponseObject
import javax.servlet.http.Cookie

open class AvailableAppointmentsSteps {

    private val pageHeader = "Book an appointment"

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage
    lateinit var webHeader: WebHeader
    private lateinit var breadcrumbs: BreadcrumbHeader

    @Step
    fun checkIfPageHeaderIsCorrect() {
        webHeader.getPageTitle().withText(pageHeader)
    }

    @Step
    fun assertNumberOfSlotsPresent(size: Int) {
        assertEquals("Incorrect number of slots displayed",
                size,
                availableAppointmentsPage.getNumberOfTimeSlotsPresent())
    }

    @Step
    fun assertOnlyOneTimeSlotPresent(
            expectedDateHeading: String,
            expectedTimeSlot: String,
            expectedSessionName: String?
    ) {
        println(Serenity.sessionVariableCalled<java.util.ArrayList<AppointmentSessionFacade>>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .APPOINTMENT_SLOTS_EXAMPLE_SESSIONS
        ))
        assertEquals(
                "Incorrect number of time-slots present for " +
                        "$expectedDateHeading, $expectedTimeSlot, $expectedSessionName",
                1,
                availableAppointmentsPage.timeSlotForDateTimeSession(
                        expectedDateHeading,
                        expectedTimeSlot,
                        expectedSessionName
                ).elements.count()
        )
    }

    @Step
    fun assertThatOtherDatesAreNotDisplayed(expectedDates: Set<String>) {
        assertEquals("Incorrect number of dates displayed. ",
                expectedDates.size, availableAppointmentsPage.getNumberOfDateHeadingsPresent())
    }

    @Step
    fun clickOnBackLink() {
        if (availableAppointmentsPage.onMobile()) {
            breadcrumbs.selectBreadcrumbLink("Your appointments")
        }
        else
            availableAppointmentsPage.clickDesktopBackButton()
    }

    @Step
    fun theAvailableAppointmentSlotsAreRetrieved() {
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrNull<String>()
        val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .appointments.getAppointmentSlots(
                        patientId, null, null, Serenity.sessionVariableCalled<Cookie>(Cookie::class))
        Serenity.setSessionVariable(AppointmentSlotsResponse::class).to(result)
    }

    @Step
    fun verifyThatAvailableSlotsAreReturnedWithAppropriateFields() {
        val result = Serenity.sessionVariableCalled<AppointmentSlotsResponse>(AppointmentSlotsResponse::class)
        val unmatchedExpectedSlots = getExpectedResponseSlots()

        assertEquals("Number of response slots", unmatchedExpectedSlots.count(), result.slots.count())

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
        val expectedResponseSlots = Serenity.sessionVariableCalled<List<SlotResponseObject>>(
                AppointmentsSlotsFactory.Expectations.EXPECTED_API_RESPONSE_OF_AVAILABLE_APPOINTMENTS
        )
        val unmatchedExpectedSlots = HashMap<String, SlotResponseObject>()
        expectedResponseSlots.forEach { slot -> unmatchedExpectedSlots[slot.id] = slot }
        return unmatchedExpectedSlots
    }

    private fun assertSlotsAreEqual(expectedSlot: SlotResponseObject, actualSlot: SlotResponseObject) {
        assertEquals("Incorrect Slot type", expectedSlot.type, actualSlot.type)
        assertEquals("Incorrect Session type", expectedSlot.sessionName, actualSlot.sessionName)
        assertEquals("Incorrect Slot start time", expectedSlot.startTime, actualSlot.startTime)
        assertEquals("Incorrect Slot end time", expectedSlot.endTime, actualSlot.endTime)
        assertEquals("Incorrect Slot location", expectedSlot.location, actualSlot.location)
        assertEquals("Incorrect Slot clinicians", expectedSlot.clinicians.toSet(), actualSlot.clinicians.toSet())
        assertEquals("Incorrect Channel", expectedSlot.channel, actualSlot.channel)
    }

    private fun assertSlotIsNotNull(actualSlot: SlotResponseObject) {
        assertNotNull("Null id", actualSlot.id)
        assertNotNull("Null slot type", actualSlot.type)
        assertNotNull("Null session type", actualSlot.sessionName)
        assertNotNull("Null startTime", actualSlot.startTime)
        assertNotNull("Null endTime", actualSlot.endTime)
        assertNotNull("Null location", actualSlot.location)
        assertNotNull("Null clinicians", actualSlot.clinicians)
        assertNotNull("Null channel", actualSlot.channel)
    }

    @Step
    fun verifyThatNoSlotsAreDisplayed() {
        assertFalse("Slots are displayed. ", availableAppointmentsPage.getAreAnySlotsPresent())
    }
}
