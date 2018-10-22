package features.appointments.steps

import com.google.common.collect.Ordering
import constants.DateTimeFormats.Companion.backendDateTimeFormatWithoutTimezone
import features.appointments.factories.UpcomingAppointmentsFactory
import mocking.MockingClient
import mocking.emis.models.AppointmentCancellationReason
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert.*
import pages.appointments.MyAppointmentsPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.text.SimpleDateFormat
import java.util.*

open class MyAppointmentsSteps {

    val mockingClient = MockingClient.instance

    lateinit var myAppointmentsPage: MyAppointmentsPage

    val pageHeader = "My appointments"
    val expectedNoUpcomingText = "You don't currently have any appointments booked\n" +
            "Once you've booked an appointment here, you'll be able to view details and cancel it.\n" +
            "If you have an upcoming appointment that isn't shown here, contact your GP surgery for more information."
    val bookingSuccessMessage = "Your appointment has been booked. You can view details or cancel it here."
    val cancellationSuccessMessage = "Your appointment has been cancelled."

    @Step
    fun checkBookingSuccessMessage() {
        val message = myAppointmentsPage.getSuccessMessage()
        assertEquals(bookingSuccessMessage, message)
    }

    @Step
    fun clickOnBookAppointmentButton() {
        myAppointmentsPage.bookButton.element.click()
    }

    @Step
    fun waitForSpinnerToDisappear() {
        myAppointmentsPage.waitForSpinnerToDisappear()
    }

    @Step
    fun checkHeaderTextIsCorrect() {
        myAppointmentsPage.waitForPageHeaderText(pageHeader)
    }

    @Step
    fun checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated() {
        val expectedSlots = Serenity.sessionVariableCalled<List<Slot>>(Slot::class).map {
            slot -> slot.copy(id = null)
        }
        val areCliniciansExpected = expectedSlots.isNotEmpty() && expectedSlots[0].clinicians.isNotEmpty()
        val slots = myAppointmentsPage.getAllSlots(areCliniciansExpected)
        assertEquals("Expected upcoming Appointments size doesn't match with the actual size",
                expectedSlots.size, slots.size)
        assertEquals("Exact expected Appointments list not found. ", HashSet(expectedSlots), HashSet(slots))
    }

    @Step
    fun checkIfSlotsAreInCorrectOrder(): Boolean {
        val slotDate = myAppointmentsPage.getDateTimestampsOfSlots()
        return Ordering.natural<Long>().isOrdered(slotDate)
    }

    @Step
    fun checkNoUpcomingAppointmentsTextIsDisplaying() {
        val actualNoUpcomingText = myAppointmentsPage.getNoUpcomingText()
        assertEquals("Incorrect text when no upcoming appointments. ",
                expectedNoUpcomingText, actualNoUpcomingText)
    }

    @Step
    fun checkIfBookAnAppointmentButtonExistAndEnabled() {
        try {
            myAppointmentsPage.bookButton.element.isVisible
            assertTrue("Book an appointment is not displaying",
                    myAppointmentsPage.bookButton.element.isDisplayed)

            assertTrue("Book an appointment is not enabled",
                    myAppointmentsPage.bookButton.element.isCurrentlyEnabled)
        } catch (e: Exception) {
            fail("Book an appointment is not found")
        }
    }

    @Step
    fun generateStubsForMyAppointmentsWhenUnavailableToPatient(provider: String) {
        val currentViewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(provider)
        currentViewAppointmentFactory.createUpcomingAppointments {
            respondWithExceptionWhenNotEnabled()
        }
    }

    @Step
    fun generateCorruptedStubForMyAppointment(provider: String) {
        val currentViewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(provider)
        currentViewAppointmentFactory.createCorruptedUpcomingAppointmentsResponse()
    }

    @Step
    fun createSerenityMyAppointmentSessionVariable() {
        val timeZone = TimeZone.getTimeZone("Europe/London")
        val dateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        dateTimeFormat.timeZone = timeZone
        val fromDate = dateTimeFormat.format(Calendar.getInstance().time)
        try {
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.getMyAppointments(fromDate)
            Serenity.setSessionVariable(MyAppointmentsResponse::class.java).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }
    }

    @Step
    fun checkMyAppointmentsAreAllUpcomingOnes() {
        val dateTimeFormat = SimpleDateFormat(backendDateTimeFormatWithoutTimezone)
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                MyAppointmentsResponse::class.java
        )
        val now = Date().time
        myAppointmentsResponse.appointments.forEach { appointment ->
            val startTime = dateTimeFormat.parse(appointment.startTime).time
            assertTrue("appointment with slot id ${appointment.id} is a past appointment",
                    now <= startTime)
        }
    }

    @Step
    fun checkCancellationReasonExistForApplicableGPService() {
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                MyAppointmentsResponse::class.java
        )
        val cancellationReasons = myAppointmentsResponse.cancellationReasons

        val expectedCancellationReasons = Serenity.sessionVariableCalled<List<AppointmentCancellationReason>>(
                AppointmentCancellationReason::class
        )
        assertEquals("Cancellation options count doesn't match",
                expectedCancellationReasons?.size ?: 0,
                cancellationReasons.size
        )
        expectedCancellationReasons?.forEach { expectedReason ->
            val actualReason = cancellationReasons.firstOrNull { expectedReason.displayName == it.displayName }
            assertNotNull("Expected reason ${expectedReason.displayName} not found",
                    actualReason)
        }
    }

    @Step
    fun clickFirstCancelLink() {
        myAppointmentsPage.clickFirstCancelAppointmentLink()
    }

    @Step
    fun verifyCancellationConfirmationMessage() {
        val message = myAppointmentsPage.getSuccessMessage()
        assertEquals(cancellationSuccessMessage, message)
    }

    @Step
    fun verifyThatThereIsACancelLinkForEachUpcomingAppointment() {
        assertEquals(
                "Missing at least one cancel link. ",
                myAppointmentsPage.getWebAppointmentSlotDivs().size,
                myAppointmentsPage.getNumberOfCancelLinks()
        )
    }
}
