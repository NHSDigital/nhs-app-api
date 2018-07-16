package features.appointments.steps

import models.Slot
import net.thucydides.core.annotations.Step
import org.hamcrest.Matcher
import org.junit.Assert
import org.junit.Assert.*
import pages.appointments.AppointmentsBookingPage
import pages.navigation.Header

open class AppointmentsBookingSteps {

    private val pageHeader by lazy { "Book an appointment" }
    private val bookThisButtonText by lazy { "Book this appointment" }

    lateinit var appointmentsBooking: AppointmentsBookingPage
    lateinit var header: Header

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
        val actualHeader = header.getPageHeaderText()
        assertEquals("Expected Header text ${pageHeader} of the page is not found",
                pageHeader, actualHeader)
    }

    @Step
    fun clickOnBookAppointmentButton() {
        appointmentsBooking.clickOnButton(bookThisButtonText)
    }

    @Step
    fun checkTimeoutErrorMessage(presence: Boolean = true) {
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedBody = "Please try again\n" +
                "If the problem persists and you need to book an appointment now, contact your GP surgery directly."

        val message: String? = appointmentsBooking.getErrorText()

        if (presence && message != null) {
            assertEquals("$expectedHeader\n$expectedBody", message)
        } else {
            assertNull(message)
        }

    }

    @Step
    fun checkIfTryAgainButtonDisplayed() {
        val buttonExists = doesTryAgainButtonExist()
        assertTrue(buttonExists)
    }

    @Step
    fun checkIfTryAgainButtonIsNotDisplayed() {
        val buttonExists = doesTryAgainButtonExist()
        assertFalse(buttonExists)
    }

    private fun doesTryAgainButtonExist(): Boolean {
        val buttonExists = appointmentsBooking.doesButtonExistBasedOnVisibleText("Try again")
        return buttonExists
    }

    @Step
    fun checkUnavailableErrorMessage() {
        val message = appointmentsBooking.getErrorText()
        val expectedHeader = "Sorry, there's been a problem loading this page"
        val expectedBody = "Please try again later. If the problem persists and you need to book an appointment now, contact your GP surgery directly."
        assertNotNull("No error message displayed, expecting $expectedHeader\n$expectedBody", message)
        assertTrue("Actual text: $message. Expected text: $expectedHeader\n$expectedBody", message!!.contains("$expectedHeader\n$expectedBody"))
    }

    @Step
    fun clickOnTryAgainButton() {
        appointmentsBooking.clickOnButton("Try again")
    }
}
