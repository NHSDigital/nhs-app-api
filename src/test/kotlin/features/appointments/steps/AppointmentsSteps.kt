package features.appointments.steps

import mocking.MockingClient
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertTrue
import pages.AppointmentsPage

open class AppointmentsSteps {

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    lateinit var appointmentsPage: AppointmentsPage

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
        val message = appointmentsPage.getSuccessMessage()
        assertTrue(message.contains("Appointment Booked"))
    }

    @Step
    fun clickOnButton(button: String) {
        appointmentsPage.clickOnButton(button)
    }

    @Step
    fun checkHeader(expectedHeader: String)
    {
        val actualHeader = appointmentsPage.getHeader()
        Assert.assertEquals(expectedHeader, actualHeader);
    }
}
