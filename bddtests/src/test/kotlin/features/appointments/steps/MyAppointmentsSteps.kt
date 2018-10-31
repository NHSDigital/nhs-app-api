package features.appointments.steps

import com.google.common.collect.Ordering
import constants.DateTimeFormats.Companion.backendDateTimeFormatWithoutTimezone
import features.appointments.factories.UpcomingAppointmentsFactory
import mocking.MockingClient
import mocking.emis.models.AppointmentCancellationReason
import models.Slot
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.*
import pages.ErrorPage
import pages.appointments.MyAppointmentsPage
import pages.navigation.HeaderNative
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.text.SimpleDateFormat
import java.util.*

open class MyAppointmentsSteps {

    val mockingClient = MockingClient.instance

    lateinit var myAppointmentsPage: MyAppointmentsPage
    lateinit var errorPage: ErrorPage

    lateinit var headerNative: HeaderNative

    val pageHeader = "My appointments"
    val expectedNoUpcomingText = "You don't currently have any appointments booked\n" +
            "Once you've booked an appointment here, you'll be able to view details and cancel it.\n" +
            "If you have an upcoming appointment that isn't shown here, contact your GP surgery for more information."
    private val bookingSuccessMessage = "Your appointment has been booked. You can view details or cancel it here."
    private val cancellationSuccessMessage = "Your appointment has been cancelled."

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
        headerNative.waitForPageHeaderText(pageHeader)
    }

    @Step
    fun checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated() {
        val expectedSlots = Serenity.sessionVariableCalled<List<Slot>>(
                UpcomingAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        ).map { slot ->
            slot.copy(id = null)
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
            myAppointmentsPage.bookButton.element.waitUntilVisible<WebElementFacade>()

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
    fun generateTimeoutStubForMyAppointment(provider: String) {
        val currentViewAppointmentFactory = UpcomingAppointmentsFactory.getForSupplier(provider)
        currentViewAppointmentFactory.createTimeoutUpcomingAppointmentsResponse()
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
    fun checkMyAppointments() {
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                MyAppointmentsResponse::class.java
        )
        val expectedResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(
                UpcomingAppointmentsFactory.Expectations.EXPECTED_API_RESPONSE_OF_MY_UPCOMING_APPOINTMENTS
        )
        assertEquals(
                "Incorrect number of appointments returned. ",
                expectedResponse.appointments.count(),
                myAppointmentsResponse.appointments.size
        )
        assertArrayEquals(
                "List of appointments returned is incorrect. ",
                expectedResponse.appointments.toTypedArray(),
                myAppointmentsResponse.appointments.toTypedArray()
        )
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

    fun verifyAppointmentDataErrorHeaderIsDisplayed() {
        headerNative.waitForPageHeaderText("Appointment data error")
    }

    fun checkAppointmentDataErrorMessagesAreCorrect() {
        val expectedHeader = "There's been a problem getting your appointment history"
        val expectedBody = "Try again later. If the problem continues and you need this information now, contact your GP surgery directly. For urgent medical advice, call 111."
        errorPage.waitForSpinnerToDisappear()
        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
        assertEquals("expected error text $expectedBody but found ${errorPage.errorText1.element.text}",
                expectedBody, errorPage.errorText1.element.text)
    }
}
