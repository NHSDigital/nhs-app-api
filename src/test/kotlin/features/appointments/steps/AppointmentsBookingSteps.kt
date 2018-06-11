package features.appointments.steps

import models.Slot
import net.thucydides.core.annotations.Step
import org.hamcrest.Matcher
import org.junit.Assert
import org.junit.Assert.*
import pages.appointments.AppointmentsBookingPage

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
    fun checkIfPageHeaderIsCorrect() {
        val actualHeader = appointmentsBooking.getPageHeaderText()
        assertEquals("Expected Header text ${appointmentsBooking.pageHeader} of the page is not found",
                appointmentsBooking.pageHeader, actualHeader)
    }

    @Step
    fun clickOnBookAppointmentButton(bookButtonText:String = appointmentsBooking.bookThisButtonText) {
        appointmentsBooking.clickOnButton(bookButtonText)
    }

    @Step
    fun checkTimeoutErrorMessage(presence: Boolean = true) {
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedBody = "Please try again\n" +
                "If the problem persists and you need to book an appointment now, contact your GP surgery directly."

        val message: String? = appointmentsBooking.getServerErrorMessage()

        if (presence && message != null) {
            assertTrue("Expected text:\n$expectedHeader\n$expectedBody\nbut was:\n$message",
                    message.contains("$expectedHeader\n$expectedBody"))
        } else {
            Assert.assertNull(message)
        }

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
        assertNotNull("No error message displayed, expecting $expectedHeader\n$expectedBody", message)
        assertTrue("Actual text: $message. Expected text: $expectedHeader\n$expectedBody", message!!.contains("$expectedHeader\n$expectedBody"))
    }

    @Step
    fun clickOnTryAgainButton() {
        val button = appointmentsBooking.getTryAgainButton()
        button.click()
    }
}
