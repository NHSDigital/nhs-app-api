package features.appointments.steps

import com.google.common.collect.Ordering
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.data.AppointmentData
import models.Patient
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
    fun checkSuccessMessage() {
        val message = myAppointmentsPage.getSuccessMessage()
        assertTrue(message.contains(myAppointmentsPage.bookingSuccessMessage))
    }

    @Step
    fun clickOnBookAppointmentButton(buttonText: String = myAppointmentsPage.bookAnButtonText) {
        myAppointmentsPage.clickOnButton(buttonText)
    }

    @Step
    fun checkHeaderTextIsCorrect() {
        val actualHeader = myAppointmentsPage.getPageHeaderText()
        assertEquals("Expected Header text ${myAppointmentsPage.pageHeader} of the page is not found",
                myAppointmentsPage.pageHeader, actualHeader)
    }

    @Step
    fun checkAppointmentsExistAndAppointmentDataAreCorrectlyPopulated() {
        val slots = myAppointmentsPage.getAllSlots()
        val expectedSlots = AppointmentData.instance.expectedTempMyAppointmets
        assertEquals("Expected upcoming appointments size doesn't match with the actual size",
                expectedSlots.size, slots.size)
        assertEquals("Exact expected Appointments list not found", expectedSlots, slots)
    }

    @Step
    fun checkIfSlotsAreInCorrectOrder(): Boolean {
        val slotDate = myAppointmentsPage.getDateTimestampsOfSlots()
        return Ordering.natural<Long>().isOrdered(slotDate)
    }

    @Step
    fun checkNoUpcomingAppointmentsHeaderIsDisplaying() {
        val actualNoUpcomingHeader = myAppointmentsPage.getNoUpcomingHeaderText()
        assertEquals("Can't find expected no upcoming appointment header is found",
                myAppointmentsPage.noUpcomingHeader, actualNoUpcomingHeader)
    }

    @Step
    fun checkIfBookAnAppointmentButtonExistAndEnabled() {
        try {
            myAppointmentsPage.bookButton.isVisible
            assertTrue("Book an appointment is not displaying",
                    myAppointmentsPage.bookButton.isDisplayed)

            assertTrue("Book an appointment is not enabled",
                    myAppointmentsPage.bookButton.isCurrentlyEnabled)
        } catch (e: Exception) {
            fail("Book an appointment is not found")
        }
    }


    @Step
    fun mockEMISMyAppointmentResponse(noUpcomingAppointments: Boolean = false) {
        val appointmentData = AppointmentData.instance
        val getResponse = when {
            noUpcomingAppointments -> appointmentData.createGetAppointmentsResponseForNoUpcomingAppoinments()
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
                expectedCancellationReasons.size == expectedCancellationReasons.size)
        expectedCancellationReasons.forEach { expectedReason ->
            val acutalReason = cancellationReasons.firstOrNull { expectedReason.id == it.id }
            assertNotNull("Expected reason ${expectedReason.displayName} not found",
                    acutalReason)
        }
    }
}
