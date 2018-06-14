package features.appointments.steps

import models.Slot
import net.thucydides.core.annotations.Step
import org.hamcrest.Matcher
import org.junit.Assert.*
import pages.AppointmentsBookingPage

open class AppointmentsBookingSteps {

    lateinit var appointmentsBooking: AppointmentsBookingPage

    @Step
    fun slots(matches: Matcher<ArrayList<Slot>>) {
        assertThat(appointmentsBooking.getAllSlots(), matches)
    }

    @Step
    fun checkIfSlotsAreDisplayed() {
        assertTrue(appointmentsBooking.countSlots() > 0)
    }

    @Step
    fun checkIfSlotsAreNotDisplayed() {
        assertTrue(appointmentsBooking.countSlots() == 0)
    }

    @Step
    fun selectSlot() {
        appointmentsBooking.selectFirstSlot()
    }

    @Step
    fun clickOnBookAppointmentButton() {
        appointmentsBooking.clickOnBookAppointmentButton()
    }

    @Step
    fun checkTimeoutErrorMessage(presence: Boolean = true) {
        val message = appointmentsBooking.getServerErrorMessage()
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedBody = "Please try again\n" +
                "If the problem persists and you need to book an appointment now, contact your GP surgery directly."
        assertTrue(String.format("Actual text: %s. Expected text: %s? %b", message, "$expectedHeader\n$expectedBody", presence),
                presence == message.contains("$expectedHeader\n$expectedBody"))
    }

    @Step
    fun checkIfTyAgainButtonDisplayed() {
        val button = appointmentsBooking.getTryAgainButton()
        assertEquals("Try again", button.text)
        assertTrue(button.isDisplayed)
    }

    @Step
    fun checkIfTyAgainButtonIsNotDisplayed() {
        val hasButton = appointmentsBooking.hasTryAgainButton()
        assertFalse(hasButton)
    }

    @Step
    fun checkUnavailableErrorMessage() {
        val message = appointmentsBooking.getServerErrorMessage()
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedBody = "Please try again later. If the problem persists and you need to book an appointment now, contact your GP surgery directly."
        assertTrue(String.format("Actual text: %s. Expected text: %s. ", message, "$expectedHeader\n$expectedBody"), message.contains("$expectedHeader\n$expectedBody"))
    }

    @Step
    fun clickOnTryAgainButton() {
        val button = appointmentsBooking.getTryAgainButton()
        button.click()
    }
}
