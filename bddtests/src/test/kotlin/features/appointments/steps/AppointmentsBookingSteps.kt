package features.appointments.steps

import models.Slot
import net.thucydides.core.annotations.Step
import org.hamcrest.Matcher
import org.junit.Assert
import org.junit.Assert.*
import pages.ErrorPage
import pages.appointments.AppointmentsBookingPage
import pages.navigation.Header

open class AppointmentsBookingSteps {

    private val pageHeader by lazy { "Book an appointment" }
    private val bookThisButtonText by lazy { "Book this appointment" }

    lateinit var appointmentsBooking: AppointmentsBookingPage
    lateinit var header: Header
    lateinit var error: ErrorPage

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
    fun checkTimeoutErrorMessage() {
        error.waitForSpinnerToDisappear(15)
        assertEquals("Sorry, there's been a problem loading this page", error.subHeading.element.text)
        assertEquals("Please try again", error.detailOne.element.text)
        assertEquals("If the problem persists and you need to book an appointment now, contact your GP surgery directly.", error.detailTwo.element.text)
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
        error.waitForSpinnerToDisappear()
        assertEquals("Sorry, there's been a problem loading this page", error.subHeading.element.text)
        assertEquals("Please try again later. If the problem persists and you need to book an appointment now, contact your GP surgery directly.", error.detailTwo.element.text)
    }

    @Step
    fun clickOnTryAgainButton() {
        appointmentsBooking.clickOnButton("Try again")
    }
}
