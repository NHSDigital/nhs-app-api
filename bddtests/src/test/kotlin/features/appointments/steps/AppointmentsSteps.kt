package features.appointments.steps

import com.google.common.collect.Ordering
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.data.AppointmentData
import models.Patient
import models.Slot
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert.*
import pages.appointments.MyAppointmentsPage
import worker.WorkerClient
import worker.models.appointments.MyAppointmentsResponse
import java.text.SimpleDateFormat
import java.util.*

open class AppointmentsSteps {
    @Steps
    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    lateinit var myAppointmentsPage: MyAppointmentsPage

    val pageHeader = "My appointments"
    val expectedNoUpcomingText = "You don't currently have any appointments booked\n" +
            "Once you've booked an appointment here, you'll be able to view details, cancel it and see your appointment history.\n" +
            "If you have an upcoming appointment that isn't shown here, contact your GP surgery for more information."
    val bookingSuccessMessage = "Your appointment has been booked. You can view details or cancel it here."
    val cancellationSuccessMessage = "Your appointment has been cancelled."
    val bookAppotintmentButtonText = "Book an appointment"

    @Step
    fun checkBookingWasRequested() {
        val wiremockRequests = mockingClient.getRequests().split("\n")
        var validBookingBody = false
        val expectedBookRequestBody = "\\\"UserPatientLinkToken\\\":\\\"" +
                patient.userPatientLinkToken +
                "\\\",\\\"SlotId\\\":301,\\\"BookingReason\\\":\\\"" +
                sessionVariableCalled<String>("Symptoms").take(150) +
                "\\\""
        var bookingCreated = false
        for (requestLine in wiremockRequests) {
            if (requestLine.contains(expectedBookRequestBody)) {
                validBookingBody = true
            }
            if (requestLine.contains("\\\"BookingCreated\\\":true")) {
                bookingCreated = true
                break
            }
        }
        assertTrue("Incorrect booking was requested. ", validBookingBody)
        assertTrue("No booking was created. ", bookingCreated)
    }

    @Step
    fun checkBookingSuccessMessage() {
        val message = myAppointmentsPage.getSuccessMessage()
        assertEquals(bookingSuccessMessage, message)
    }

    @Step
    fun clickOnBookAppointmentButton() {
        clickOnButtonByText(bookAppotintmentButtonText)
    }

    @Step
    fun clickOnButtonByText(buttonText: String) {
        myAppointmentsPage.clickOnButton(buttonText)
    }

    @Step
    fun checkHeaderTextIsCorrect() {
        assertTrue("Expected Header text is not found: $pageHeader",
                myAppointmentsPage.waitForPageHeaderText(pageHeader))
    }

    @Step
    fun checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated() {
        val slots = myAppointmentsPage.getAllSlots()
        val expectedSlots = AppointmentData.instance.generateExpectedMyAppointments("Europe/London")
        assertEquals("Expected upcoming appointments size doesn't match with the actual size",
                expectedSlots.size, slots.size)
        assertEquals("Exact expected Appointments list not found. ", expectedSlots, slots)
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
    fun mockEMISMyAppointmentResponse(noUpcomingAppointments: Boolean = false) {
        val appointmentData = AppointmentData.instance
        val getResponse = when {
            noUpcomingAppointments -> appointmentData.createGetAppointmentsResponseForNoUpcomingAppointments()
            else -> appointmentData.createGetAppointmentsResponse()
        }

        mockingClient.forEmis {
            appointmentGetRequest(Patient.montelFrye)
                    .respondWithSuccess(getResponse)
        }
    }

    @Step
    fun createSerenityEmisMyAppointmentSessionVariable() {
        val dateTimeFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")
        val fromDate = dateTimeFormat.format(Calendar.getInstance().time)
        val result = Serenity
                .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .getMyAppointments(fromDate)
        Serenity.setSessionVariable(MyAppointmentsResponse::class.java).to(result)
    }

    @Step
    fun checkEmisMyAppointmentsAreAllUpcomingOnes() {
        val dateTimeFormat = SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss")
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(MyAppointmentsResponse::class.java)
        val now = Date().time
        myAppointmentsResponse.appointments.forEach { appointment ->
            val startTime = dateTimeFormat.parse(appointment.startTime).time
            assertTrue("appointment with slot id ${appointment.id} is a past appointment",
                    now <= startTime)
        }
    }

    @Step
    fun checkEmisCancellationReasonExist() {
        val myAppointmentsResponse = Serenity.sessionVariableCalled<MyAppointmentsResponse>(MyAppointmentsResponse::class.java)
        val cancellationReasons = myAppointmentsResponse.cancellationReasons
        val expectedCancellationReasons = AppointmentData.instance.getEmisAppointmentCancellationReasons()
        assertTrue("EMIS cancellation options count doesn't match",
                cancellationReasons.size == expectedCancellationReasons.size)
        expectedCancellationReasons.forEach { expectedReason ->
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
    fun storeDetailsOfFirstAppointment() {
        Serenity.setSessionVariable(Slot::class.java).to(myAppointmentsPage.getSlotAtIndex(0))
    }

    @Step
    fun verifyCancellationConfirmationMessage() {
        val message = myAppointmentsPage.getSuccessMessage()
        assertEquals(cancellationSuccessMessage, message)
    }

    fun verifyThatThereIsACancelLinkForEachUpcomingAppointment() {
        assertEquals("Missing at least one cancel link. ", myAppointmentsPage.getAllSlots().size, myAppointmentsPage.getNumberOfCancelLinks())
    }
}
